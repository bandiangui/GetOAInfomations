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
    
    public partial class OA_Reads
    {
        public int ID { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> Receive_ID { get; set; }
        public Nullable<int> Staff_ID { get; set; }
        public Nullable<int> Dept_ID { get; set; }
        public string OldSystemId { get; set; }
    
        public virtual OA_Receives OA_Receives { get; set; }
        public virtual OG_Depts OG_Depts { get; set; }
        public virtual OG_Staffs OG_Staffs { get; set; }
    }
}
