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
    
    public partial class OA_Signatures
    {
        public int ID { get; set; }
        public bool IsRead { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
        public string Content { get; set; }
        public int Receive_ID { get; set; }
        public int Staff_ID { get; set; }
        public string OldSystemId { get; set; }
    
        public virtual OA_Receives OA_Receives { get; set; }
        public virtual OG_Staffs OG_Staffs { get; set; }
    }
}
