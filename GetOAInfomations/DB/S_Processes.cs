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
    
    public partial class S_Processes
    {
        public S_Processes()
        {
            this.S_Processes1 = new HashSet<S_Processes>();
            this.S_Projects = new HashSet<S_Projects>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Content2 { get; set; }
        public string Version { get; set; }
        public bool Activate { get; set; }
        public bool Deleted { get; set; }
        public int CreatorId { get; set; }
        public int LastEditorId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime Update { get; set; }
        public Nullable<int> StartFormId { get; set; }
        public Nullable<int> ParentId { get; set; }
        public int ProcessTypeId { get; set; }
        public bool IsMustRelation { get; set; }
        public bool IsOARelation { get; set; }
        public bool IsSimpleRelation { get; set; }
        public Nullable<int> PrivateMainDept_ID { get; set; }
        public int TimeLimitType { get; set; }
        public int TimeLimit { get; set; }
        public bool IsFullForm { get; set; }
        public string SerialNumber { get; set; }
        public bool IsPermission { get; set; }
        public bool IsSpecial { get; set; }
        public int SpecialType { get; set; }
        public bool IsBatch { get; set; }
        public string RelFindField { get; set; }
    
        public virtual OG_Depts OG_Depts { get; set; }
        public virtual OG_Staffs OG_Staffs { get; set; }
        public virtual OG_Staffs OG_Staffs1 { get; set; }
        public virtual ICollection<S_Processes> S_Processes1 { get; set; }
        public virtual S_Processes S_Processes2 { get; set; }
        public virtual ICollection<S_Projects> S_Projects { get; set; }
    }
}
