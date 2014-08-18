//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace GetOAInfomations.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class S_Projects
    {
        public S_Projects()
        {
            this.S_ProjectItems = new HashSet<S_ProjectItems>();
            this.S_Annexs = new HashSet<S_Annexs>();
        }
    
        public int ID { get; set; }
        public int StatusValue { get; set; }
        public System.DateTime CreateDate { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatorId { get; set; }
        public bool IsUrge { get; set; }
        public Nullable<int> UrgeLimit { get; set; }
        public Nullable<System.DateTime> UrgeMaxDate { get; set; }
        public int ProcessId { get; set; }
        public string DeleteRemark { get; set; }
        public string SearchPassword { get; set; }
        public Nullable<int> PromisesDays { get; set; }
        public Nullable<System.DateTime> ExpectDate { get; set; }
        public bool IsHistory { get; set; }
        public string CreatorPhone { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public Nullable<int> BuildupId { get; set; }
        public bool IsLocked { get; set; }
        public string LockReason { get; set; }
        public bool IsRetract { get; set; }
        public Nullable<int> MeetingType { get; set; }
    
        public virtual OA_Receives OA_Receives { get; set; }
        public virtual OA_Sends OA_Sends { get; set; }
        public virtual OG_Usrs OG_Usrs { get; set; }
        public virtual S_Processes S_Processes { get; set; }
        public virtual S_ProjectBases S_ProjectBases { get; set; }
        public virtual ICollection<S_ProjectItems> S_ProjectItems { get; set; }
        public virtual ICollection<S_Annexs> S_Annexs { get; set; }
    }
}