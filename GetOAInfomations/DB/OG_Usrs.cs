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
    
    public partial class OG_Usrs
    {
        public OG_Usrs()
        {
            this.S_ProjectItems = new HashSet<S_ProjectItems>();
            this.S_Projects = new HashSet<S_Projects>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public string OldId { get; set; }
        public Nullable<int> District_ID { get; set; }
        public string AppName { get; set; }
        public Nullable<int> MenuId { get; set; }
        public System.Guid UserUniqueKey { get; set; }
    
        public virtual OG_Depts OG_Depts { get; set; }
        public virtual OG_Roles OG_Roles { get; set; }
        public virtual OG_Staffs OG_Staffs { get; set; }
        public virtual ICollection<S_ProjectItems> S_ProjectItems { get; set; }
        public virtual ICollection<S_Projects> S_Projects { get; set; }
    }
}
