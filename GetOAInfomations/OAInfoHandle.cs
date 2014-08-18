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
        NNWebDBContextEntities db = new NNWebDBContextEntities();
        public List<OA_Receives> GetReceiveses(int beginYear, int endYear)
        {
            endYear += 1;
            DateTime beginDate = new DateTime(beginYear, 1, 1);
            DateTime endDate = new DateTime(endYear, 1, 1);
            var list = from receive in db.OA_Receives
                       join project in db.S_Projects on receive.ID equals project.ID
                       join cooperate in db.OA_Cooperates on receive.ID equals cooperate.Project_ID
                       join expandValue in db.S_ExpandValues on receive.ID equals expandValue.KeyId
                       join signature in db.OA_Signatures on receive.ID equals signature.Receive_ID
                       where receive.GetDate > beginDate && receive.GetDate < endDate &&
                             project.IsDeleted == false && cooperate.IsDeleted == false &&
                             expandValue.TableName == "OA_Receives" && !expandValue.FieldName.Contains("_Del")
                       select new
                       {
                           receive,
                           cooperate,
                           expandValue,
                           signature
                       };
            return null;

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
        public List<AttitudeData> CoopAttitude { get; set; }

    }

    public class AttitudeData
    {
        public string Content { get; set; }
        public string Staff { get; set; }
        public string Dept { get; set; }
        public string Date { get; set; }
    }
}
