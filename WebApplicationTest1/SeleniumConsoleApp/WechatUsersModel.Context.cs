﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SeleniumConsoleApp
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Travelzoo1Entities : DbContext
    {
        public Travelzoo1Entities()
            : base("name=Travelzoo1Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<members_g_wechat> members_g_wechat { get; set; }
        public virtual DbSet<members_g_wechat_staging> members_g_wechat_staging { get; set; }
    }
}