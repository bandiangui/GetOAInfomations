using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CommonClass.IO;
using NNWebFlow.DBContexts;
using NNWebFlow.DBContexts.BPM;
using NNWebFlow.DBContexts.OA;

namespace GetOAInfomations
{
    public class OaInfoHandle
    {
        DBContext db;
        private string enter;
        private string path;
        private string storePath;

        public OaInfoHandle()
        {
            db = DB.DBFactory<UOWOffice>.GetInstance().DBContext;
            enter = ((char)10).ToString();
            storePath = ConfigurationSettings.AppSettings["OAStore"];
            path = ConfigurationSettings.AppSettings["DestFolder"];
        }

        public List<OA_Receive> GetReceiveses(int beginYear, int type)
        {
            DateTime beginDate = new DateTime(beginYear, 1, 1);
            DateTime endDate = new DateTime(beginYear + 1, 1, 1);

            var receives =
                db.OA_Receives.Get(p =>
                        p.Project.IsDeleted == false && p.GetDate > beginDate && p.GetDate < endDate &&
                        p.Project.ProcessId == type, "Staff,PrimaryStaff");

            return receives.ToList();
        }

        public List<ReceiveData> GetReceiveses(List<OA_Receive> receives)
        {
            List<ReceiveData> ret = new List<ReceiveData>();

            foreach (var r in receives)
            {
                var item = new ReceiveData() { Receive = r };
                item.LeaderAttitude.AddRange(db.OA_Signatures.Get(p => p.Receive_ID == r.ID && p.IsRead == true, "Staff")
                    .ToList()
                    .Select(
                        p =>
                            string.Format("{0}[{1}]：{2}", p.Staff.Name,
                                p.Time.HasValue ? p.Time.Value.ToString("yyyy年MM月dd日") : "", p.Content)));

                item.DealAttitudes.AddRange(db.S_ExpandValues.Get(
                    p => p.TableName == "OA_Receives" && p.KeyId == r.ID && p.FieldName == "DealAttitude", "Staff,Dept")
                    .ToList()
                    .Select(
                        p =>
                            string.Format("{0} {1}[{2}]：{3}", p.Dept.Name, p.Staff.Name,
                                p.EndTime.HasValue ? p.EndTime.Value.ToString("yyyy年MM月dd日") : "", p.Content)));

                db.OA_Cooperates.Get(p => p.Project_ID == r.ID && p.IsDeleted == false && p.IsRead == true, "Dept")
                    .ToList().ForEach(p =>
                    {
                        if (item.CoopAttitudes.All(s => s.StartsWith(p.Dept.Name)))
                        {
                            if (string.IsNullOrEmpty(p.Content))
                            {
                                item.CoopAttitudes.AddRange(db.S_ExpandValues.Get(
                                    s =>
                                        s.TableName == "OA_Receives" && s.FieldName == "CoopAttitude" && s.KeyId == r.ID &&
                                        s.DeptId == p.Dept_ID, "Staff,Dept")
                                    .ToList()
                                    .Select(
                                        s =>
                                            string.Format("{0} {1}[{2}]：{3}", s.Dept.Name, s.Staff.Name,
                                                s.EndTime.HasValue ? s.EndTime.Value.ToString("yyyy年MM月dd日") : "", s.Content)));
                            }
                            else
                            {
                                item.CoopAttitudes.Add(string.Format("{0}[{1}]：{2}", p.Dept.Name,
                                    p.EndTime.HasValue ? p.EndTime.Value.ToString("yyyy年MM月dd日") : "", p.Content));
                            }
                        }
                    });
                ret.Add(item);
            }
            return ret;
        }

        public List<OA_Send> GetSends(int beginYear)
        {
            string serialNumber = string.Format("[{0}]", beginYear);
            var sends = db.OA_Sends.Get(p => p.Project.IsDeleted == false && p.SerialNumber.Contains(serialNumber), "Staff,StaffDept");
            return sends.ToList();
        }

        public List<SendData> GetSends(List<OA_Send> sends)
        {
            List<SendData> ret = new List<SendData>();
            foreach (var s in sends.OrderBy(p => p.ID))
            {
                var item = new SendData() { Send = s };
                item.CoopAttitudes.AddRange(db.S_ExpandValues.Get(
                    p => p.TableName == "OA_Sends" && p.KeyId == s.ID && p.FieldName == "SignAttitude", "Staff,Dept")
                    .ToList()
                    .Select(
                        p =>
                            string.Format("{0} {1}[{2}]：{3}", p.Dept.Name, p.Staff.Name,
                                p.EndTime.HasValue ? p.EndTime.Value.ToString("yyyy年MM月dd日") : "", p.Content)));
                ret.Add(item);
            }
            return ret;
        }

        public void ExcelImport(int beginYear, int type, List<SendData> sends = null, List<ReceiveData> receiveses = null)
        {
            const string filter = "HIDDEN";
            string title = beginYear + "年";
            title += "收文列表";
            List<object> excelList = new List<object>();
            if (sends != null)
            {
                #region 发文
                title = title.Replace("收文列表", "发文列表");
                excelList.Add(new
                {
                    ID = "序列号",
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
                sends.ForEach(p => excelList.Add(new
                {
                    ID = string.Format("{0:D5}", p.Send.ID),
                    p.Send.Title,
                    p.Send.SerialNumber,
                    SignDate = p.Send.SignDate.HasValue ? p.Send.SignDate.Value.ToString("yyyy年MM月dd日") : "",
                    Attituder = p.Send.AttituderName,
                    p.Send.Attitude,
                    p.Send.Department,
                    Staff = p.Send.Staff != null ? p.Send.Staff.Name : "",
                    Dept = p.Send.StaffDept != null ? p.Send.StaffDept.Name : "",
                    p.Send.DeptStaffName,
                    SignAttitudes = string.Join(enter, p.CoopAttitudes),
                    CheckStaffName = string.Format("{0} {1}", p.Send.CheckStaffName, p.Send.AuditorStaffName),
                    p.Send.CollateStaffName,
                    p.Send.PrintStaffName
                }));
                #endregion
            }
            else if (receiveses != null)
            {
                #region 收文与信访
                if (type != 1)
                {
                    title = title.Replace("收文列表", "信访列表");
                }

                excelList.Add(new
                {
                    ID = "序列号",
                    Title = "标题",
                    SerialNumber = "公文文号",
                    Department = "来文单位",
                    Type = "公文类型",
                    GetDate = "收件日期",
                    Staff = "收件人",
                    PrimaryAttitude = "拟办意见",
                    PrimaryStaff = "拟办人",
                    LeaderAttitude = "局领导批示",
                    DealAttitude = "处理意见",
                    CoopAttitude = "协办意见"
                });
                receiveses.ForEach(p => excelList.Add(new
                {
                    ID = string.Format("{0:D5}", p.Receive.ID),
                    p.Receive.Title,
                    p.Receive.SerialNumber,
                    p.Receive.Department,
                    p.Receive.Type,
                    GetDate = p.Receive.GetDate.HasValue ? p.Receive.GetDate.Value.ToString("yyyy年MM月dd日") : "",
                    Staff = p.Receive.Staff != null ? p.Receive.Staff.Name : "",
                    p.Receive.PrimaryAttitude,
                    PrimaryStaff = p.Receive.PrimaryStaff != null ? p.Receive.PrimaryStaff.Name : "",
                    LeaderAttitude = string.Join(enter, p.LeaderAttitude),
                    DealAttitude = string.Join(enter, p.DealAttitudes),
                    CoopAttitude = string.Join(enter, p.CoopAttitudes),
                }));
                #endregion
            }

            excelList.ToExcel("admin", title, filter, path + title + ".xls");
        }

        public string GetAnnex(AnnexItem data)
        {
            List<S_Annex> annexes = new List<S_Annex>();
            string ret = null;

            annexes.AddRange(GetAnnexList(data));
            var doc = GetDocument(data);

            if (!annexes.Any() && (doc == null || !doc.HasRevision)) return ret;

            string destPath = string.Format("{0}{1}\\{2}\\", path, data.FolderName, data.Index);
            FileExt.CheckDirectoryExist(destPath);

            foreach (var annex in annexes)
            {
                try
                {
                    string destFullPath = string.Format("{0}{1}-{2}.{3}", destPath, annex.Name, annex.Order, annex.Extended);

                    if (string.IsNullOrEmpty(annex.Url))
                    {
                        // 文件方式存储的,读取Path字段
                        string originalPath = string.Format("{0}{1}{2}.{3}", storePath, annex.Path, annex.ID, annex.Extended);
                        string r = FileExt.GetFileStoreToServer(originalPath, destFullPath);

                        if (!string.IsNullOrEmpty(r))
                            ret += string.Format("{4}{0}({1}{2})：{3}{4}{5}", data.ID, data.SerialNumber, data.Title, annex.ID, Environment.NewLine, r);
                    }
                    else
                    {
                        // FileCloud存储的,读取Url字段
                        FileExt.GetFileUrlToServer(annex.Url, destPath);
                    }
                }
                catch (Exception ex)
                {
                    ret += string.Format("{4}{0}({1}{2})：{3}{4}{5}", data.ID, data.SerialNumber, data.Title, annex.ID, Environment.NewLine, ex.Message);
                }
            }

            try
            {
                string destFullPath = string.Format("{0}{1}：{2}.{3}", destPath, data.SerialNumber, doc.Name, doc.Extended);
                string originalPath = string.Format("{0}{1}{2}_FinalVersion.{3}", storePath, doc.Path, doc.ID, doc.Extended);

                string r = FileExt.GetFileStoreToServer(originalPath, destFullPath);

                if (!string.IsNullOrEmpty(r))
                    ret += string.Format("{4}{0}({1}{2})：{3}{4}{5}", data.ID, data.SerialNumber, data.Title, doc.ID, Environment.NewLine, r);
            }
            catch (Exception ex)
            {
                ret += string.Format("{4}{0}({1}{2})：{3}{4}{5}", data.ID, data.SerialNumber, data.Title, doc.ID, Environment.NewLine, ex.Message);
            }
            return null;
        }

        public List<S_Annex> GetAnnexList(AnnexItem data)
        {
            return db.S_Annexs.Get(p => p.S_Project_ID == data.ID && p.IsDeleted == false && p.Type != 4).OrderBy(p => p.Order).ToList();
        }

        public S_Annex GetDocument(AnnexItem data)
        {
            return db.S_Annexs.Get(p => p.S_Project_ID == data.ID && p.Type == 4 && p.IsDeleted == false).FirstOrDefault();
        }
    }

    public class ReceiveData
    {
        public OA_Receive Receive { get; set; }
        /// <summary>
        /// 局领导意见
        /// </summary>
        public List<string> LeaderAttitude { get; set; }
        /// <summary>
        /// 科室处理意见
        /// </summary>
        public List<string> DealAttitudes { get; set; }

        public List<string> CoopAttitudes { get; set; }

        public ReceiveData()
        {
            this.LeaderAttitude = new List<string>();
            this.DealAttitudes = new List<string>();
            this.CoopAttitudes = new List<string>();
        }

    }

    public class SendData
    {
        public OA_Send Send { get; set; }
        /// <summary>
        /// 会签意见
        /// </summary>
        public List<string> CoopAttitudes { get; set; }

        public SendData()
        {
            this.CoopAttitudes = new List<string>();
        }

    }

    public class AnnexItem
    {
        public string Index { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
        public string SerialNumber { get; set; }
        public string FolderName { get; set; }
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
