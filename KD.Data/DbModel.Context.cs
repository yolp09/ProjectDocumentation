﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KD.Data
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class KDv1Context : DbContext
    {
        private static string CreateNewConnectionString(string connectionName, string password)
        {
            var config = ConfigurationManager.ConnectionStrings[connectionName];
            var split = config.ConnectionString.Split(Convert.ToChar(";"));
            var sb = new System.Text.StringBuilder();

            for (var i = 0; i <= (split.Length - 1); i++)
            {
                if (split[i].ToLower().Contains("user id"))
                {
                    //split[i] = "user id=" + userName;
                    split[i] += ";Password=" + password;
                }

                if (i < (split.Length - 1))
                {
                    sb.AppendFormat("{0};", split[i]);
                }
                else
                {
                    sb.Append(split[i]);
                }
            }
            return sb.ToString();
        }

        //public KDv1Context()
        //    : base("name=KDv1Context")
        //{
        //}
        public KDv1Context()
            : base(CreateNewConnectionString("KDv1Context", "12345678"))
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AddressNotice> AddressNotices { get; set; }
        public virtual DbSet<Applicability> Applicabilities { get; set; }
        public virtual DbSet<ApplicabilityFile> ApplicabilityFiles { get; set; }
        public virtual DbSet<ApplicabilityNotice> ApplicabilityNotices { get; set; }
        public virtual DbSet<ApplicabilitySB> ApplicabilitySB { get; set; }
        public virtual DbSet<Detail> Details { get; set; }
        public virtual DbSet<FileTable> FileTables { get; set; }
        public virtual DbSet<Notice> Notices { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<History> History { get; set; }
    }
}