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
    
    public partial class S_ProjectBases
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Sponsor { get; set; }
        public string ProjectNo { get; set; }
        public string DepartmentName { get; set; }
        public string ContextPerson { get; set; }
        public string ContextMobile { get; set; }
        public string ContextPersonalID { get; set; }
        public string ProjectAddress { get; set; }
        public string ArtificialPerson { get; set; }
        public string ArtificialPersonID { get; set; }
        public string DeputyPerson { get; set; }
        public string DeputyPersonID { get; set; }
        public string GetAddress { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<int> ArchiveId { get; set; }
        public string ProcessName { get; set; }
        public string DeputyPhone { get; set; }
    
        public virtual S_Projects S_Projects { get; set; }
    }
}
