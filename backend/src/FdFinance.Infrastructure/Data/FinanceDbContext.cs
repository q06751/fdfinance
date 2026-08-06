using FdFinance.Application.Services;
using FdFinance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FdFinance.Infrastructure.Data;

/// <summary>
/// 映射规则：原表名/列名 1:1；仅允许可空附加列与新辅助表。
/// 禁止对老字段做 rename / required / 改类型。
/// </summary>
public class FinanceDbContext : DbContext, IFinanceDbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

    public DbSet<T_Reimbursement> Reimbursements => Set<T_Reimbursement>();
    public DbSet<T_Abstract> Abstracts => Set<T_Abstract>();
    public DbSet<T_Sign> Signs => Set<T_Sign>();
    public DbSet<T_Admin> Admins => Set<T_Admin>();
    public DbSet<T_User> Users => Set<T_User>();
    public DbSet<T_Category> Categories => Set<T_Category>();
    public DbSet<T_Apply> Applies => Set<T_Apply>();
    public DbSet<T_Autograph> Autographs => Set<T_Autograph>();
    public DbSet<T_Relation> Relations => Set<T_Relation>();
    public DbSet<T_Report> Reports => Set<T_Report>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<SerialCounter> SerialCounters => Set<SerialCounter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<T_Reimbursement>(e =>
        {
            e.ToTable("T_Reimbursement");
            e.HasKey(x => x.F_ReimbursementId);
            e.Property(x => x.F_Money).HasMaxLength(64);
            e.Property(x => x.F_Code).HasMaxLength(64);
            e.Property(x => x.F_Name).HasMaxLength(128);
            e.Property(x => x.F_DepartmentName).HasMaxLength(256);
            e.Property(x => x.F_DepartmentLeader).HasMaxLength(128);
            e.Property(x => x.F_Producer).HasMaxLength(128);
            e.Property(x => x.F_ClassTypt).HasMaxLength(16);
            e.Property(x => x.F_MergeId).HasMaxLength(512);
            // 附加可空列 — 旧程序可不写
            e.Property(x => x.ExtensionJson).IsRequired(false);
            e.HasIndex(x => x.F_Code);
            e.HasIndex(x => x.F_Depid);
            e.HasIndex(x => x.F_AddDate);
        });

        modelBuilder.Entity<T_Abstract>(e =>
        {
            e.ToTable("T_Abstract");
            e.HasKey(x => x.F_AbstractId);
            e.Property(x => x.F_Abstract).HasMaxLength(512);
            e.Property(x => x.F_Money).HasPrecision(18, 2);
            e.Property(x => x.ExtensionJson).IsRequired(false);
            // 导航关系仅应用层使用；不强制库内 FK，避免改动老库约束
            e.HasOne(x => x.Reimbursement)
                .WithMany(r => r.Abstracts)
                .HasForeignKey(x => x.F_ReimbursementId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .IsRequired(false);
        });

        modelBuilder.Entity<T_Sign>(e =>
        {
            e.ToTable("T_Sign");
            e.HasKey(x => x.F_SignId);
            e.Property(x => x.F_Name).HasMaxLength(128);
            e.Property(x => x.F_Position).HasMaxLength(64);
            e.Property(x => x.F_ImageUrl).HasMaxLength(512);
            e.Property(x => x.F_Status).HasMaxLength(32);
            // TodoId 为老模型已有字段（钉钉待办），保持
            e.Property(x => x.TodoId).HasMaxLength(128);
            e.Property(x => x.ExtensionJson).IsRequired(false);
            e.HasOne(x => x.Reimbursement)
                .WithMany(r => r.Signs)
                .HasForeignKey(x => x.F_ReimbursementId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .IsRequired(false);
            e.HasOne(x => x.Abstract)
                .WithMany(a => a.Signs)
                .HasForeignKey(x => x.F_AbstractId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .IsRequired(false);
        });

        modelBuilder.Entity<T_Admin>(e =>
        {
            e.ToTable("T_Admin");
            e.HasKey(x => x.F_AdminId);
            e.Property(x => x.F_AdminId).HasMaxLength(64);
            e.Property(x => x.F_LoginNmae).HasMaxLength(128);
            e.Property(x => x.F_PassWord).HasMaxLength(256);
            e.Property(x => x.F_DName).HasMaxLength(128);
            e.Property(x => x.F_Role).HasMaxLength(32).IsRequired(false);
            e.Property(x => x.ExtensionJson).IsRequired(false);
            e.HasIndex(x => x.F_LoginNmae);
        });

        modelBuilder.Entity<T_User>(e =>
        {
            e.ToTable("T_User");
            e.HasKey(x => x.F_UserId);
            e.Property(x => x.F_Name).HasMaxLength(128);
            e.Property(x => x.F_Phone).HasMaxLength(32);
            e.Property(x => x.F_Code).HasMaxLength(64);
            e.Property(x => x.ExtensionJson).IsRequired(false);
        });

        modelBuilder.Entity<T_Category>(e =>
        {
            e.ToTable("T_Category");
            e.HasKey(x => x.F_Id);
            e.Property(x => x.F_Id).ValueGeneratedOnAdd();
            e.Property(x => x.F_Code).HasMaxLength(16);
            e.Property(x => x.F_Name).HasMaxLength(128);
            e.Property(x => x.ExtensionJson).IsRequired(false);
        });

        modelBuilder.Entity<T_Apply>(e =>
        {
            e.ToTable("T_Apply");
            e.HasKey(x => x.F_ApplyId);
            e.Property(x => x.F_Id).ValueGeneratedOnAdd();
            e.Property(x => x.F_Name).HasMaxLength(128);
            e.Property(x => x.F_Department).HasMaxLength(256);
            e.Property(x => x.F_Money).HasMaxLength(64);
            e.Property(x => x.F_Descripion).HasMaxLength(1024);
            e.Property(x => x.ExtensionJson).IsRequired(false);
        });

        modelBuilder.Entity<T_Autograph>(e =>
        {
            e.ToTable("T_Autograph");
            e.HasKey(x => x.F_AutographId);
            e.Property(x => x.F_Url).HasMaxLength(512);
            e.Property(x => x.ExtensionJson).IsRequired(false);
        });

        modelBuilder.Entity<T_Relation>(e =>
        {
            e.ToTable("T_Relation");
            e.HasKey(x => x.F_RelationId);
            e.Property(x => x.ExtensionJson).IsRequired(false);
        });

        // 原表：打印次数，新老共用
        modelBuilder.Entity<T_Report>(e =>
        {
            e.ToTable("T_Report");
            e.HasKey(x => x.ReportId);
            e.Property(x => x.ReportId).HasMaxLength(64);
            e.Property(x => x.F_ReimbursementId).HasMaxLength(64);
            e.HasIndex(x => x.F_ReimbursementId);
        });

        // 辅助：组织（演示/自建库）；对接老环境可指向钉钉部门表或仅用代码映射
        modelBuilder.Entity<Department>(e =>
        {
            e.ToTable("Department");
            e.HasKey(x => x.DepId);
            e.Property(x => x.DepId).ValueGeneratedNever();
            e.Property(x => x.DName).HasMaxLength(256);
            e.Property(x => x.ClassCode).HasMaxLength(16);
            e.Property(x => x.ExtensionJson).IsRequired(false);
        });

        // 新辅助表：流水号（老库可用存储过程 GetSerialNo，此表仅新系统兜底）
        modelBuilder.Entity<SerialCounter>(e =>
        {
            e.ToTable("SerialCounter");
            e.HasKey(x => new { x.Code, x.Year, x.Month });
            e.Property(x => x.Code).HasMaxLength(16);
        });
    }
}
