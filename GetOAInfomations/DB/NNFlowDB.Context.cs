﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class NNWebDBContextEntities : DbContext
    {
        public NNWebDBContextEntities()
            : base("name=NNWebDBContextEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<OA_Cooperates> OA_Cooperates { get; set; }
        public virtual DbSet<OA_Reads> OA_Reads { get; set; }
        public virtual DbSet<OA_Receives> OA_Receives { get; set; }
        public virtual DbSet<OA_Sends> OA_Sends { get; set; }
        public virtual DbSet<OG_Depts> OG_Depts { get; set; }
        public virtual DbSet<OG_DeptStaffs> OG_DeptStaffs { get; set; }
        public virtual DbSet<OG_Roles> OG_Roles { get; set; }
        public virtual DbSet<OG_RoleStaffs> OG_RoleStaffs { get; set; }
        public virtual DbSet<OG_Staffs> OG_Staffs { get; set; }
        public virtual DbSet<OG_Usrs> OG_Usrs { get; set; }
        public virtual DbSet<S_ExpandValues> S_ExpandValues { get; set; }
        public virtual DbSet<S_Processes> S_Processes { get; set; }
        public virtual DbSet<S_ProjectBases> S_ProjectBases { get; set; }
        public virtual DbSet<S_ProjectItems> S_ProjectItems { get; set; }
        public virtual DbSet<S_Projects> S_Projects { get; set; }
        public virtual DbSet<OA_Signatures> OA_Signatures { get; set; }
        public virtual DbSet<S_Annexs> S_Annexs { get; set; }
    }
}
