using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using HeThongQuanLyTiemChung.ModelViews;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class db_VaccineContext : DbContext
    {
        public db_VaccineContext()
        {
        }

        public db_VaccineContext(DbContextOptions<db_VaccineContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Age> Ages { get; set; }
        public virtual DbSet<AppointDate> AppointDates { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Disease> Diseases { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<Injection> Injections { get; set; }
        public virtual DbSet<MonthAge> MonthAges { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Regimen> Regimens { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Vaccine> Vaccines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=TRANVUHAO;Database=db_Vaccine;Integrated Security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(150);

                entity.Property(e => e.Phone)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Salt)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_Accounts_Roles");
            });

            modelBuilder.Entity<Age>(entity =>
            {
                entity.Property(e => e.AgeId).HasColumnName("AgeID");

                entity.Property(e => e.AgeName).HasMaxLength(50);
            });

            modelBuilder.Entity<AppointDate>(entity =>
            {
                entity.Property(e => e.AppointDateId).HasColumnName("AppointDateID");

                entity.Property(e => e.AppointmentDate).HasColumnType("date");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.InjectionId).HasColumnName("InjectionID");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.AppointDates)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_AppointDates_Customers");

                entity.HasOne(d => d.Injection)
                    .WithMany(p => p.AppointDates)
                    .HasForeignKey(d => d.InjectionId)
                    .HasConstraintName("FK_AppointDates_Injections");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.Property(e => e.BrandId).HasColumnName("BrandID");

                entity.Property(e => e.BrandName).HasMaxLength(50);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.GenderId).HasColumnName("GenderID");

                entity.Property(e => e.Password).HasMaxLength(150);

                entity.Property(e => e.Phone).HasMaxLength(12);

                entity.Property(e => e.Salt)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.GenderId)
                    .HasConstraintName("FK_Customers_Genders");
            });

            modelBuilder.Entity<Disease>(entity =>
            {
                entity.Property(e => e.DiseaseId).HasColumnName("DiseaseID");

                entity.Property(e => e.DiseaseName).HasMaxLength(100);
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.Property(e => e.GenderId).HasColumnName("GenderID");

                entity.Property(e => e.GenderName).HasMaxLength(10);
            });

            modelBuilder.Entity<Injection>(entity =>
            {
                entity.Property(e => e.InjectionId).HasColumnName("InjectionID");

                entity.Property(e => e.DiseaseId).HasColumnName("DiseaseID");

                entity.Property(e => e.InjectionName).HasMaxLength(50);

                entity.HasOne(d => d.Disease)
                    .WithMany(p => p.Injections)
                    .HasForeignKey(d => d.DiseaseId)
                    .HasConstraintName("FK_Injections_Diseases");
            });

            modelBuilder.Entity<MonthAge>(entity =>
            {
                entity.Property(e => e.MonthAgeId).HasColumnName("MonthAgeID");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Orders_Customers");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.Property(e => e.Dosage).HasMaxLength(50);

                entity.Property(e => e.InjectionId).HasColumnName("InjectionID");

                entity.Property(e => e.LotNumber).HasMaxLength(50);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.VaccineId).HasColumnName("VaccineID");

                entity.HasOne(d => d.Injection)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.InjectionId)
                    .HasConstraintName("FK_OrderDetails_Injections");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasOne(d => d.Vaccine)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.VaccineId)
                    .HasConstraintName("FK_OrderDetails_Vaccines");
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.Property(e => e.PackageId).HasColumnName("PackageID");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.Property(e => e.PageId).HasColumnName("PageID");

                entity.Property(e => e.AuthorName).HasMaxLength(250);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Thumb).HasMaxLength(250);

                entity.Property(e => e.Title).HasMaxLength(250);
            });

            modelBuilder.Entity<Regimen>(entity =>
            {
                entity.Property(e => e.RegimenId).HasColumnName("RegimenID");

                entity.Property(e => e.InjectionId).HasColumnName("InjectionID");

                entity.Property(e => e.MonthAgeId).HasColumnName("MonthAgeID");

                entity.Property(e => e.VaccineId).HasColumnName("VaccineID");

                entity.HasOne(d => d.Injection)
                    .WithMany(p => p.Regimen)
                    .HasForeignKey(d => d.InjectionId)
                    .HasConstraintName("FK_Regimens_Injections");

                entity.HasOne(d => d.MonthAge)
                    .WithMany(p => p.Regimen)
                    .HasForeignKey(d => d.MonthAgeId)
                    .HasConstraintName("FK_Regimens_MonthAges");

                entity.HasOne(d => d.Vaccine)
                    .WithMany(p => p.Regimen)
                    .HasForeignKey(d => d.VaccineId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Regimens_Vaccines");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Description).HasMaxLength(150);

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<Vaccine>(entity =>
            {
                entity.Property(e => e.VaccineId).HasColumnName("VaccineID");

                entity.Property(e => e.AgeId).HasColumnName("AgeID");

                entity.Property(e => e.BrandId).HasColumnName("BrandID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DiseaseId).HasColumnName("DiseaseID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(250);

                entity.Property(e => e.VaccineName).HasMaxLength(150);

                entity.HasOne(d => d.Age)
                    .WithMany(p => p.Vaccines)
                    .HasForeignKey(d => d.AgeId)
                    .HasConstraintName("FK_Vaccines_Ages");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Vaccines)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Vaccines_Brands");

                entity.HasOne(d => d.Disease)
                    .WithMany(p => p.Vaccines)
                    .HasForeignKey(d => d.DiseaseId)
                    .HasConstraintName("FK_Vaccines_Categories");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<HeThongQuanLyTiemChung.ModelViews.RegisterViewModel> RegisterViewModel { get; set; }

        public DbSet<HeThongQuanLyTiemChung.ModelViews.LoginViewModel> LoginViewModel { get; set; }
    }
}
