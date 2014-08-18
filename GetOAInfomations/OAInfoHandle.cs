using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetOAInfomations.DB;

namespace GetOAInfomations
{
    public class OaInfoHandle
    {
        NNWebDBContextEntities db;

        public OaInfoHandle()
        {
            db = new NNWebDBContextEntities();
        }

        public List<ReceiveData> GetReceiveses(int beginYear, int endYear, int type)
        {
            DateTime beginDate = new DateTime(beginYear, 1, 1);
            DateTime endDate = new DateTime(endYear + 1, 1, 1);
            List<ReceiveData> ret = new List<ReceiveData>();

            #region 获取数据

            var receives = (from receive in db.OA_Receives
                            join project in db.S_Projects on receive.ID equals project.ID
                            join process in db.S_Processes on project.ProcessId equals process.ID
                            where receive.GetDate > beginDate && receive.GetDate < endDate && process.SpecialType == type
                            orderby receive.ID
                            select receive).ToList();
            foreach (var r in receives)
            {
                var item = new ReceiveData() { Receive = r };
                item.LeaderAttitude.AddRange(db.OA_Signatures.Where(p => p.Receive_ID == r.ID)
                    .ToList()
                    .Select(p => new AttitudeData()
                    {
                        Content = p.Content,
                        Date = p.Time.HasValue ? p.Time.Value.ToString("yyyy年MM月dd日") : "",
                        Staff = db.OG_Usrs.FirstOrDefault(u => u.ID == p.Staff_ID).Name,
                        Dept = "局领导"
                    }));
                item.DealAttitudes.AddRange(db.S_ExpandValues.Where(
                    p => p.TableName == "OA_Receives" && p.KeyId == r.ID && p.FieldName == "DealAttitude")
                    .ToList()
                    .Select(p => new AttitudeData()
                    {
                        Content = p.Content,
                        Date = p.EndTime.HasValue ? p.EndTime.Value.ToString("yyyy年MM月dd日") : "",
                        Staff = db.OG_Usrs.FirstOrDefault(u => u.ID == p.StaffId).Name,
                        Dept = db.OG_Usrs.FirstOrDefault(u => u.ID == p.DeptId).Name
                    }));
                db.OA_Cooperates.Where(p => p.Project_ID == r.ID && p.IsDeleted == false && p.IsRead == true)
                    .ToList().ForEach(p =>
                    {
                        if (item.CoopAttitudes.All(s => s.DeptId != p.Dept_ID))
                        {
                            if (string.IsNullOrEmpty(p.Content))
                            {
                                item.CoopAttitudes.AddRange(db.S_ExpandValues.Where(
                                    s =>
                                        s.TableName == "OA_Receives" && s.FieldName == "CoopAttitude" && s.KeyId == r.ID &&
                                        s.DeptId == p.Dept_ID)
                                    .ToList()
                                    .Select(s => new AttitudeData()
                                    {
                                        Content = s.Content,
                                        Date = s.EndTime.HasValue ? s.EndTime.Value.ToString("yyyy年MM月dd日") : "",
                                        Staff = db.OG_Usrs.FirstOrDefault(u => u.ID == s.StaffId).Name,
                                        Dept = db.OG_Usrs.FirstOrDefault(u => u.ID == s.DeptId).Name
                                    }));
                            }
                            else
                            {
                                item.CoopAttitudes.Add(new AttitudeData()
                                {
                                    Content = p.Content,
                                    Date = p.EndTime.HasValue ? p.EndTime.Value.ToString("yyyy年MM月dd日") : "",
                                    Dept = db.OG_Usrs.FirstOrDefault(u => u.ID == p.Dept_ID).Name
                                });
                            }
                        }
                    });
                ret.Add(item);
            }

            #endregion
            return ret;
        }

        public List<SendData> GetSends(int beginYear, int endYear)
        {
            List<SendData> ret = new List<SendData>();
            var sends = from send in db.OA_Sends
                        join project in db.S_Projects on send.ID equals project.ID
                        where project.IsDeleted == false && send.SerialNumber.Contains(send.SerialNumber)
                        select new
                        {
                            send,
                            send.Staff,
                            send.Dept
                        };
            while (beginYear != endYear)
            {
                beginYear++;
                sends = sends.Union(from send in db.OA_Sends
                                    join project in db.S_Projects on send.ID equals project.ID
                                    where project.IsDeleted == false && send.SerialNumber.Contains(send.SerialNumber)
                                    select new
                                    {
                                        send,
                                        send.Staff,
                                        send.Dept
                                    });
            }

            foreach (var s in sends.OrderBy(p => p.send.ID))
            {
                var item = new SendData() { Send = s.send };
                item.Send.Staff = s.Staff;
                item.Send.Staff.OG_Usrs = db.OG_Usrs.FirstOrDefault(p => p.ID == s.Staff.ID);
                item.Send.Dept = s.Dept;
                item.Send.Dept.OG_Usrs = db.OG_Usrs.FirstOrDefault(p => p.ID == s.Dept.ID);
                item.CoopAttitudes.AddRange(db.S_ExpandValues.Where(
                    p => p.TableName == "OA_Sends" && p.KeyId == s.send.ID && p.FieldName == "SignAttitude")
                    .ToList()
                    .Select(p => new AttitudeData()
                    {
                        Content = p.Content,
                        Date = p.EndTime.HasValue ? p.EndTime.Value.ToString("yyyy年MM月dd日") : "",
                        Staff = db.OG_Usrs.FirstOrDefault(u => u.ID == p.StaffId).Name,
                        Dept = db.OG_Usrs.FirstOrDefault(u => u.ID == p.DeptId).Name
                    }));
                ret.Add(item);
            }
            return ret;
        }

        public void ExcelImport(int beginYear, int endYear, int type)
        {
            const string filter = "HIDDEN";
            int index = 1;
            string title = beginYear == endYear ? beginYear + "年" : beginYear + "-" + endYear + "年";
            title += "收文列表";
            List<object> excelList = new List<object>();
            switch (type)
            {
                case 2:
                    title = title.Replace("收文列表", "发文列表");

                    excelList.Add(new
                    {
                        Index = "序列号",
                        ID = "ID",
                        Title = "标题",
                        SerialNumber = "公文文号",
                        SignDate = "签发日期",
                        Attituder = "签发人",
                        Attitude = "签发意见",
                        Department = "主送",
                        Staff = "拟稿人",
                        Dept = "拟稿部门",
                        DeptStaffName = "部门审核人",
                        SignAttitudes = "会签部门意见",
                        CheckStaffName = "局办公室核稿人",
                        CollateStaffName = "二校",
                        PrintStaffName = "文印人"
                    });
                    var sendQuerist = GetSends(beginYear, endYear);
                    sendQuerist.ForEach(p =>
                    {
                        index++;
                        excelList.Add(new
                        {
                            Index = string.Format("{0:D5}", index),
                            p.Send.ID,
                            p.Send.Title,
                            p.Send.SerialNumber,
                            SignDate = p.Send.SignDate.HasValue ? p.Send.SignDate.Value.ToString("yyyyMMdd") : "",
                            Attituder = p.Send.AttituderName,
                            p.Send.Attitude,
                            p.Send.Department,
                            Staff = p.Send.Staff.OG_Usrs.Name,
                            Dept = p.Send.Dept.OG_Usrs.Name,
                            p.Send.DeptStaffName,
                            SignAttitudes = string.Join("\r\n", p.CoopAttitudes),
                            CheckStaffName = string.Format("{0} {1}", p.Send.CheckStaffName, p.Send.AuditorStaffName),
                            p.Send.CollateStaffName,
                            p.Send.PrintStaffName
                        });
                    });
                    break;
                case 1:
                default:
                    title = title.Replace("收文列表", "信访列表");
                    var receiveQuerist = GetReceiveses(beginYear, endYear, type);

                    excelList.Add(new
                    {
                        Index ="序列号",
                        ID = "ID",
                        Title = "标题",
                        SerialNumber = "公文文号",
                        Department = "来文单位",
                        Emergency = "紧急程度",
                        DealType = "处理类型",
                        Type = "公文类型",
                        GetDate = "收件日期",
                        Staff = "收件人",
                        PrimaryAttitude = "拟办意见",
                        PrimaryStaff = "拟办人",
                        LeaderAttitude = "局领导批示",
                        DealAttitude = "处理意见",
                        CoopAttitude = "协办意见"
                    });
                    receiveQuerist.ForEach(p =>
                    {
                        index++;
                        excelList.Add(new
                        {
                            Index = string.Format("{0:D5}", index),
                            p.Receive.ID,
                            p.Receive.Title,
                            p.Receive.SerialNumber,
                            p.Receive.Department,
                            p.Receive.Emergency,
                            p.Receive.DealType,
                            p.Receive.Type,
                            GetDate = p.Receive.GetDate.HasValue ? p.Receive.GetDate.Value.ToString("yyyyMMdd") : "",
                            Staff = p.Receive.Staff.OG_Usrs.Name,
                            p.Receive.PrimaryAttitude,
                            PrimaryStaff = p.Receive.PrimaryStaff.OG_Usrs.Name,
                            LeaderAttitude = string.Join("\r\n", p.LeaderAttitude),
                            DealAttitude = string.Join("\r\n", p.DealAttitudes),
                           CoopAttitude = string.Join("\r\n", p.CoopAttitudes),
                        });
                    });

                    excelList.AddRange(receiveQuerist);
                    break;
            }
            excelList.ToExcel("admin", title, filter, "");


        }
    }

    public class ReceiveData
    {
        public OA_Receives Receive { get; set; }
        /// <summary>
        /// 局领导意见
        /// </summary>
        public List<AttitudeData> LeaderAttitude { get; set; }
        /// <summary>
        /// 科室处理意见
        /// </summary>
        public List<AttitudeData> DealAttitudes { get; set; }
        /// <summary>
        /// 科室处理意见
        /// </summary>
        public AttitudeData DealAttitude { get; set; }

        public List<AttitudeData> CoopAttitudes { get; set; }

        public ReceiveData()
        {
            this.LeaderAttitude = new List<AttitudeData>();
            this.DealAttitudes = new List<AttitudeData>();
            this.CoopAttitudes = new List<AttitudeData>();
        }

    }

    public class SendData
    {
        public OA_Sends Send { get; set; }
        /// <summary>
        /// 会签意见
        /// </summary>
        public List<AttitudeData> CoopAttitudes { get; set; }

        public SendData()
        {
            this.CoopAttitudes = new List<AttitudeData>();
        }

    }

    public class AttitudeData
    {
        public string Content { get; set; }
        public string Staff { get; set; }
        public string Dept { get; set; }
        public int DeptId { get; set; }
        public string Date { get; set; }
    }
}
