using Arch.CoreLibrary.Entities.Log;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.Data
{
    public partial class LogManagementContext : DbContext
    {
        public LogManagementContext()
        {
        }

        public LogManagementContext(DbContextOptions<LogManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<ExceptionLog> ExceptionLog { get; set; }
        public virtual DbSet<LoginLog> LoginLog { get; set; }
        public virtual DbSet<PerformanceLog> PerformanceLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DatabaseConfig.GetConnectionString("AlonetLogManagement"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ExceptionLog>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<LoginLog>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<PerformanceLog>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });
        }
    }
}
