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
    
    public partial class S_ExpandValues
    {
        public int ID { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public int KeyId { get; set; }
        public int StaffId { get; set; }
        public int DeptId { get; set; }
        public System.DateTime Time { get; set; }
        public string Content { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
    
        public virtual OG_Depts OG_Depts { get; set; }
        public virtual OG_Staffs OG_Staffs { get; set; }
    }
}