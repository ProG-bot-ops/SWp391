using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Data;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }

    public DbSet<Appointment> Appointments { get; set; }
    
    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Disease> Diseases { get; set; }
    public DbSet<DiseaseDetail> DiseaseDetails { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Doctor_Appointment> Doctor_Appointments { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Insurance> Insurances { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    public DbSet<Medical_Record> Medical_Records { get; set; }
    public DbSet<Medicine> Medicines { get; set; }
    public DbSet<Medicine_Inventory> Medicine_Inventories { get; set; }
    public DbSet<MedicineCategory> MedicineCategories { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    public DbSet<Nurse_Appointment> Nurse_Appointments { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Payment_Invoice> Payment_Invoices { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionDetail> PrescriptionDetails { get; set; }
    public DbSet<Reception> Receptions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Supply> Supplies { get; set; }
    public DbSet<Supply_Inventory> Supply_Inventories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<User_Role> User_Roles { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<MedicineDetail> MedicineDetails { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<MedicineImport> MedicineImports { get; set; }
    public DbSet<MedicineImportDetail> MedicineImportDetails { get; set; }
    public DbSet<Doctor_Shift> Doctor_Shifts { get; set; }
    public DbSet<ShiftRequest> ShiftRequests { get; set; }
    public IEnumerable<object> UserRoles { get; internal set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointments");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedNever();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.AppointmentDate).IsRequired();
            entity.Property(a => a.StartTime).IsRequired();
            entity.Property(a => a.EndTime).IsRequired(false);
            entity.Property(a => a.Note).IsRequired().HasMaxLength(100);


            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (AppointmentStatus)value);

            entity.HasMany(d => d.Doctor_Appointments)
                .WithOne(a => a.Appointment)
                .HasForeignKey(a => a.AppointmentId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Nurse_Appointments)
                    .WithOne(a => a.Appointment)
                    .HasForeignKey(a => a.AppointmentId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Supplies)
                .WithOne(a => a.Appointment)
                .HasForeignKey(a => a.AppointmentId).OnDelete(DeleteBehavior.Cascade);


            entity.HasOne(i => i.Invoice)
            .WithOne(a => a.Appointment)
                .HasForeignKey<Invoice>(i => i.Id).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(i => i.Medical_Record)
            .WithOne(a => a.Appointment)
                .HasForeignKey<Medical_Record>(i => i.Id).OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Patient)
                .WithMany(cv => cv.Appointments)
                .HasForeignKey(cus => cus.PatientId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Clinic)
                .WithMany(cv => cv.Appointments)
                .HasForeignKey(cus => cus.ClinicId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Reception)
                .WithMany(cv => cv.Appointments)
                .HasForeignKey(cus => cus.ReceptionId).OnDelete(DeleteBehavior.Cascade);


        });

        modelBuilder.Entity<Doctor_Shift>(entity =>
        {
            entity.ToTable("doctor_shifts");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.ShiftType)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.ShiftDate).IsRequired();
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime).IsRequired();

            entity.Property(e => e.Notes).HasMaxLength(255);

            entity.Property(e => e.CreateDate);
            entity.Property(e => e.UpdateDate);
            entity.Property(e => e.CreateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateBy).HasMaxLength(100);

            entity.HasOne(e => e.Doctor)
                  .WithMany(d => d.Doctor_Shifts)
                  .HasForeignKey(e => e.DoctorId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ShiftRequest>(entity =>
        {
            entity.ToTable("shift_request");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.RequestType)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Reason)
                  .IsRequired()
                  .HasColumnType("TEXT");

            entity.Property(e => e.Status)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.ApprovedDate);

            entity.HasOne(e => e.Doctor)
                  .WithMany()
                  .HasForeignKey(e => e.DoctorId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Shift)
                  .WithMany()
                  .HasForeignKey(e => e.ShiftId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MedicineDetail>(entity =>
        {
            entity.ToTable("medicine_detail");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Ingredients);
            entity.Property(e => e.ExpiryDate);
            entity.Property(e => e.Manufacturer);
            entity.Property(e => e.Warning);
            entity.Property(e => e.StorageInstructions);
            entity.Property(e => e.Status);
            entity.Property(e => e.CreateDate).IsRequired();
            entity.Property(e => e.UpdateDate).IsRequired();
            entity.Property(e => e.CreateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnType("TEXT");

            entity.HasOne(e => e.Medicine)
                  .WithOne() 
                  .HasForeignKey<MedicineDetail>(e => e.MedicineId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Supplier
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("suppliers");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Address).HasColumnType("TEXT");
            entity.Property(e => e.CreateDate).IsRequired();
            entity.Property(e => e.UpdateDate);
            entity.Property(e => e.CreateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateBy).HasMaxLength(100);
        });

        // MedicineImport
        modelBuilder.Entity<MedicineImport>(entity =>
        {
            entity.ToTable("medicine_imports");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Notes).HasColumnType("TEXT");
            entity.Property(e => e.CreateDate).IsRequired();
            entity.Property(e => e.UpdateDate);
            entity.Property(e => e.CreateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateBy).HasMaxLength(100);

            entity.HasOne(e => e.Supplier)
                  .WithMany(s => s.MedicineImports)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // MedicineImportDetail
        modelBuilder.Entity<MedicineImportDetail>(entity =>
        {
            entity.ToTable("medicine_import_details");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(65,30)");
            entity.Property(e => e.ManufactureDate).IsRequired();
            entity.Property(e => e.ExpiryDate).IsRequired();
            entity.Property(e => e.CreateDate).IsRequired();
            entity.Property(e => e.UpdateDate);
            entity.Property(e => e.CreateBy).HasMaxLength(100);
            entity.Property(e => e.UpdateBy).HasMaxLength(100);

            entity.HasOne(e => e.Medicine)
                  .WithMany(m => m.MedicineImportDetails)
                  .HasForeignKey(e => e.MedicineId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Import)
                  .WithMany(i => i.MedicineImportDetails)
                  .HasForeignKey(e => e.ImportId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Unit)
                  .WithMany()
                  .HasForeignKey(e => e.UnitId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Supplier)
                    .WithMany()
                    .HasForeignKey(e => e.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.MedicineId, e.BatchNumber, e.SupplierId }).IsUnique();
        });

        modelBuilder.Entity<Clinic>(entity =>
        {
            entity.ToTable("Clinics");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Address).HasMaxLength(255);
            entity.Property(a => a.Email).HasMaxLength(100);
            entity.Property(a => a.ImageUrl).HasMaxLength(255);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);

            entity.Property(emp => emp.Status).IsRequired().HasMaxLength(100)
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (ClinicStatus)value);

            entity.Property(e => e.Type).IsRequired()
                .HasConversion(type => (int)type
                , value => (ClinicType)value);

            entity.HasMany(d => d.Appointments)
                .WithOne(a => a.Clinic)
                .HasForeignKey(a => a.ClinicId).OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.Doctors)
                .WithOne(a => a.Clinic)
                .HasForeignKey(a => a.ClinicId).OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(d => d.Departments)
                .WithOne(a => a.Clinic)
                .HasForeignKey(a => a.ClinicId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Description).IsRequired().HasMaxLength(100);
            entity.Property(a => a.TotalAmountOfPeople).IsRequired();


            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (DepartmentStatus)value);

            entity.HasMany(d => d.Doctors)
                .WithOne(a => a.Department)
                .HasForeignKey(a => a.DepartmentId).OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.Nurses)
                .WithOne(a => a.Department)
                .HasForeignKey(a => a.DepartmentId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Services)
                .WithOne(a => a.Department)
                .HasForeignKey(a => a.DepartmentId).OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(p => p.Clinic)
                 .WithMany(cv => cv.Departments)
                 .HasForeignKey(cus => cus.ClinicId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SWP391_SE1914_ManageHospital.Models.Entities.Service>(entity =>
        {
            entity.ToTable("services");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Description).HasColumnType("TEXT");
            entity.Property(a => a.ImageUrl).HasMaxLength(255);
            entity.Property(a => a.Price).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,
                value => (ServiceStatus)value);

            entity.HasOne(p => p.Department)
                .WithMany(d => d.Services)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(d => d.InvoiceDetails)
                .WithOne(a => a.Service)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.ToTable("Units");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.Status)
                .IsRequired();

            entity.HasMany(u => u.Medicines)
                .WithOne(m => m.Unit)
                .HasForeignKey(m => m.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<Disease>(entity =>
        {
            entity.ToTable("Diseases");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Description).IsRequired().HasMaxLength(100);
     

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (DiseaseStatus)value);

            entity.HasMany(d => d.Medical_Records)
                .WithOne(a => a.Disease)
                .HasForeignKey(a => a.DiseaseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.DiseaseDetails)
                .WithOne(a => a.Disease)
                .HasForeignKey(a => a.DiseaseId).OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<DiseaseDetail>(entity =>
        {
            entity.ToTable("DiseaseDetail");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Description).IsRequired().HasMaxLength(100);


            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (DiseaseDetailStatus)value);

            entity.HasOne(p => p.Disease)
                 .WithMany(cv => cv.DiseaseDetails)
                 .HasForeignKey(cus => cus.DiseaseId).OnDelete(DeleteBehavior.Cascade);


        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctors");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CCCD).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Phone).IsRequired().HasMaxLength(100);
            entity.Property(a => a.ImageURL).IsRequired();
            entity.Property(a => a.LicenseNumber).IsRequired().HasMaxLength(100);
            entity.Property(a => a.YearOfExperience).IsRequired();
            entity.Property(a => a.WorkingHours).IsRequired();
            entity.Property(a => a.Dob).IsRequired();
            entity.Property(dr => dr.Gender).IsRequired().HasMaxLength(100)
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (Gender)value);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (DoctorStatus)value);

            entity.HasOne(p => p.User)
                 .WithMany(cv => cv.Doctors)
                 .HasForeignKey(cus => cus.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Department)
                 .WithMany(cv => cv.Doctors)
                 .HasForeignKey(cus => cus.DepartmentId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Clinic)
                 .WithMany(cv => cv.Doctors)
                 .HasForeignKey(cus => cus.ClinicId).OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(d => d.Doctor_Appointments)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Medical_Records)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Feedbacks)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Doctor_Appointment>(entity =>
        {
            entity.ToTable("Doctor_Appointment");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (DoctorAppointmentStatus)value);

            entity.HasOne(p => p.Doctor)
                 .WithMany(cv => cv.Doctor_Appointments)
                 .HasForeignKey(cus => cus.DoctorId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Appointment)
                 .WithMany(cv => cv.Doctor_Appointments)
                 .HasForeignKey(cus => cus.AppointmentId).OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.ToTable("Feedbacks");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Content).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();


            entity.HasOne(p => p.Patient)
                 .WithMany(cv => cv.Feedbacks)
                 .HasForeignKey(cus => cus.PatientId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Doctor)
                 .WithMany(cv => cv.Feedbacks)
                 .HasForeignKey(cus => cus.DoctorId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Appointment)
                 .WithMany(cv => cv.Feedbacks)
                 .HasForeignKey(cus => cus.AppointmentId).OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Insurance>(entity =>
        {
            entity.ToTable("Insurances");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Description).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.StartDate).IsRequired();
            entity.Property(a => a.EndDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (InsuranceStatus)value);

            entity.Property(a => a.CoveragePercent).IsRequired();
            
            entity.HasOne(p => p.Patient)
                 .WithMany(cv => cv.Insurances)
                 .HasForeignKey(cus => cus.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.Invoices)
                .WithOne(ins => ins.Insurance)
                .HasForeignKey(ins => ins.InsuranceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoices");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.InitialAmount).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.DiscountAmount).IsRequired();
            entity.Property(a => a.TotalAmount).IsRequired();
            entity.Property(a => a.Notes).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (InvoiceStatus)value);

            entity.HasOne(p => p.Patient)
                 .WithMany(cv => cv.Invoices)
                 .HasForeignKey(cus => cus.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Insurance)
                 .WithMany(cv => cv.Invoices)
                 .HasForeignKey(cus => cus.InsuranceId)
                 .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Appointment)
                 .WithOne(cv => cv.Invoice)
                 .HasForeignKey<Appointment>(cus => cus.Id)
                 .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.InvoiceDetails)
                .WithOne(ins => ins.Invoice)
                .HasForeignKey(ins => ins.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(a => a.Payment_Invoices)
                .WithOne(ins => ins.Invoice)
                .HasForeignKey(ins => ins.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.ToTable("InvoiceDetail");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Discount).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.TotalAmount).IsRequired();
            entity.Property(a => a.Notes).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (InvoiceDetailStatus)value);


            entity.HasOne(p => p.Invoice)
                 .WithMany(cv => cv.InvoiceDetails)
                 .HasForeignKey(cus => cus.InvoiceId)
                 .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Medicine)
                 .WithMany(cv => cv.InvoiceDetails)
                 .HasForeignKey(cus => cus.MedicineId)
                 .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Service)
                 .WithMany(cv => cv.InvoiceDetails)
                 .HasForeignKey(cus => cus.ServiceId)
                 .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Medical_Record>(entity =>
        {
            entity.ToTable("Medical_Records");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Diagnosis).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.TestResults).IsRequired();
            entity.Property(a => a.Notes).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (MedicalRecordStatus)value);


            entity.HasOne(p => p.Patient)
                 .WithMany(cv => cv.Medical_Records)
                 .HasForeignKey(cus => cus.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Doctor)
                 .WithMany(cv => cv.Medical_Records)
                 .HasForeignKey(cus => cus.DoctorId)
                 .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Prescription)
                 .WithMany(cv => cv.Medical_Records)
                 .HasForeignKey(cus => cus.PrescriptionId)
                 .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Disease)
                 .WithMany(cv => cv.Medical_Records)
                 .HasForeignKey(cus => cus.DiseaseId)
                 .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Appointment)
                 .WithOne(cv => cv.Medical_Record)
                 .HasForeignKey<Appointment>(cus => cus.Id)
                 .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.ToTable("Medicines");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Description).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.Dosage).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UnitPrice)
           .IsRequired()
           .HasColumnType("decimal(65,30)");
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.ImageUrl)
                .HasMaxLength(255);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (MedicineStatus)value);
            entity.Property(e => e.Prescribed).IsRequired()
                .HasConversion(v => (int)v, 
                v => (PrescribedMedication)v);


            entity.HasOne(p => p.MedicineCategory)
                 .WithMany(cv => cv.Medicines)
                 .HasForeignKey(cus => cus.MedicineCategoryId)
                 .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.PrescriptionDetails)
                .WithOne(a => a.Medicine)
                .HasForeignKey(a => a.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(a => a.Medicine_Inventories)
                .WithOne(a => a.Medicine)
                .HasForeignKey(a => a.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(a => a.InvoiceDetails)
                .WithOne(a => a.Medicine)
                .HasForeignKey(a => a.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(a => a.Unit)
                .WithMany(u => u.Medicines)
                .HasForeignKey(a => a.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Medicine>()
                .HasOne(m => m.MedicineDetail)
                .WithOne(d => d.Medicine)
                .HasForeignKey<MedicineDetail>(d => d.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);


        });

        modelBuilder.Entity<Medicine_Inventory>(entity =>
        {
            entity.ToTable("Medicine_Inventories");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Quantity).IsRequired().HasMaxLength(100);
            entity.Property(a => a.BatchNumber).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UnitPrice)
           .IsRequired()
           .HasColumnType("decimal(65,30)");
            entity.Property(a => a.ImportDate).IsRequired();
            entity.Property(a => a.ExpiryDate).IsRequired();

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (MedicineInventoryStatus)value);


            entity.HasOne(p => p.Medicine)
                 .WithMany(cv => cv.Medicine_Inventories)
                 .HasForeignKey(cus => cus.MedicineId)
                 .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(i => i.ImportDetail)
                .WithMany(d => d.Inventories)
                .HasForeignKey(i => i.ImportDetailId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<MedicineCategory>(entity =>
        {
            entity.ToTable("MedicineCategories");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Description).IsRequired().HasMaxLength(100);
            entity.Property(a => a.ImageUrl).HasMaxLength(255);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (MedicineCategoryStatus)value);
            entity.Property(a => a.Code)
                  .IsRequired()
                  .HasColumnType("longtext");

            entity.HasMany(a => a.Medicines)
            .WithOne(a => a.MedicineCategory)
            .HasForeignKey(a=>a.MedicineCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Title).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Content).IsRequired().HasMaxLength(100);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (NotificationStatus)value);


            entity.HasOne(a=> a.User)
            .WithMany(a=>a.Notifications)
            .HasForeignKey(a=>a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Nurse>(entity =>
        {
            entity.ToTable("Nurses");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CCCD).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Phone).IsRequired().HasMaxLength(100);
            entity.Property(a => a.ImageURL).IsRequired();
            entity.Property(a => a.Dob).IsRequired();
            
            entity.Property(dr => dr.Gender).IsRequired().HasMaxLength(100)
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (Gender)value);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (NurseStatus)value);

            entity.HasOne(p => p.User)
                 .WithMany(cv => cv.Nurses)
                 .HasForeignKey(cus => cus.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Department)
                 .WithMany(cv => cv.Nurses)
                 .HasForeignKey(cus => cus.DepartmentId).OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.Nurse_Appointments)
                .WithOne(a => a.Nurse)
                .HasForeignKey(a => a.NurseId).OnDelete(DeleteBehavior.Cascade);
            
        });

        modelBuilder.Entity<Nurse_Appointment>(entity =>
        {
            entity.ToTable("Nurse_Appointments");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();


            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (NurseAppointmentStatus)value);

            entity.HasOne(p => p.Nurse)
                 .WithMany(cv => cv.Nurse_Appointments)
                 .HasForeignKey(cus => cus.NurseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Appointment)
                 .WithMany(cv => cv.Nurse_Appointments)
                 .HasForeignKey(cus => cus.AppointmentId).OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patients");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CCCD).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Phone).IsRequired().HasMaxLength(100);
            entity.Property(a => a.ImageURL).IsRequired();
            entity.Property(a => a.Dob).IsRequired();
            entity.Property(a => a.Allergies).IsRequired().HasMaxLength(100);
            entity.Property(a => a.InsuranceNumber).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Address).IsRequired().HasMaxLength(100);
            entity.Property(p => p.EmergencyContact).IsRequired(false);
            entity.Property(a => a.BloodType).IsRequired().HasMaxLength(100);

            entity.Property(dr => dr.Gender).IsRequired().HasMaxLength(100)
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (Gender)value);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (PatientStatus)value);

            entity.HasOne(p => p.User)
                 .WithMany(cv => cv.Patients)
                 .HasForeignKey(cus => cus.UserId).OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(d => d.Insurances)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Medical_Records)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Appointments)
                            .WithOne(a => a.Patient)
                            .HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Feedbacks)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Invoices)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.PaymentMethod).IsRequired().HasMaxLength(100);
            entity.Property(a => a.PaymentDate).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Amount).IsRequired();
            entity.Property(a => a.Payer).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Notes).IsRequired().HasMaxLength(100);
            
            entity.HasMany(d => d.Payment_Invoices)
                .WithOne(a => a.Payment)
                .HasForeignKey(a => a.PaymentId).OnDelete(DeleteBehavior.Cascade);
            
        });

        modelBuilder.Entity<Payment_Invoice>(entity =>
        {
            entity.ToTable("Payment_Invoices");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.AmountPaid).IsRequired().HasMaxLength(100);

            entity.HasOne(p => p.Payment)
                 .WithMany(cv => cv.Payment_Invoices)
                 .HasForeignKey(cus => cus.PaymentId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Invoice)
                 .WithMany(cv => cv.Payment_Invoices)
                 .HasForeignKey(cus => cus.InvoiceId).OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.ToTable("Prescriptions");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Note).IsRequired().HasMaxLength(100);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (PrescriptionStatus)value);

            entity.HasOne(p => p.Patient)
                 .WithMany(cv => cv.Prescriptions)
                 .HasForeignKey(cus => cus.PatientId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Doctor)
                 .WithMany(cv => cv.Prescriptions)
                 .HasForeignKey(cus => cus.DoctorId).OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.Medical_Records)
                .WithOne(a => a.Prescription)
                .HasForeignKey(a => a.PrescriptionId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.PrescriptionDetails)
                .WithOne(a => a.Prescription)
                .HasForeignKey(a => a.PrescriptionId).OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<PrescriptionDetail>(entity =>
        {
            entity.ToTable("PrescriptionDetails");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Quantity).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Usage).IsRequired().HasMaxLength(100);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (PrescriptionDetailStatus)value);

            entity.HasOne(p => p.Medicine)
                 .WithMany(cv => cv.PrescriptionDetails)
                 .HasForeignKey(cus => cus.MedicineId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Prescription)
                 .WithMany(cv => cv.PrescriptionDetails)
                 .HasForeignKey(cus => cus.PrescriptionId).OnDelete(DeleteBehavior.Cascade);

            
        });

        modelBuilder.Entity<Reception>(entity =>
        {
            entity.ToTable("Receptions");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Dob).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CCCD).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Phone).IsRequired().HasMaxLength(100);
            entity.Property(a => a.ImageURL).IsRequired().HasMaxLength(100);

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (ReceptionStatus)value);

            entity.Property(emp => emp.Gender).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (Gender)value);

            entity.HasOne(p => p.User)
                 .WithMany(cv => cv.Receptions)
                 .HasForeignKey(cus => cus.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Appointments)
                .WithOne(a => a.Reception)
                .HasForeignKey(a => a.ReceptionId).OnDelete(DeleteBehavior.Cascade);


        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);


             entity.HasMany(d => d.User_Roles)
                .WithOne(a => a.Role)
                .HasForeignKey(a => a.RoleId).OnDelete(DeleteBehavior.Cascade);


        });

        modelBuilder.Entity<Supply>(entity =>
        {
            entity.ToTable("Supplies");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreateDate).IsRequired();
            entity.Property(a => a.CreateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.UpdateDate).IsRequired();
            entity.Property(a => a.UpdateBy).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Description).IsRequired().HasMaxLength(100);
           

            entity.Property(emp => emp.Status).IsRequired()
                .HasConversion(status => (int)status,  // Lưu số nguyên vào database
                value => (SupplyStatus)value);

            entity.HasMany(d => d.Supply_Inventories)
               .WithOne(a => a.Supply)
               .HasForeignKey(a => a.SupplyId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Appointment)
                 .WithMany(cv => cv.Supplies)
                 .HasForeignKey(cus => cus.AppointmentId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Supply_Inventory>(entity =>
        {
            entity.ToTable("Supply_Inventories");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Quantity).IsRequired().HasMaxLength(100);
            entity.Property(a => a.ImportDate).IsRequired().HasMaxLength(100);
            entity.Property(a => a.ExpiryDate).IsRequired();
            entity.Property(a => a.SupplierName).IsRequired().HasMaxLength(100);

            entity.HasOne(p => p.Supply)
                 .WithMany(cv => cv.Supply_Inventories)
                 .HasForeignKey(cus => cus.SupplyId).OnDelete(DeleteBehavior.Cascade);


        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
            entity.Property(a => a.Password).IsRequired().HasMaxLength(100);
            entity.Property(a => a.RefreshToken).HasMaxLength(100);
            entity.Property(a => a.RefreshTokenExpiryTime);
            entity.Property(a => a.ResetPasswordToken).HasMaxLength(100);
            entity.Property(a => a.ResetPasswordTokenExpiryTime).HasMaxLength(100);

            entity.HasMany(d => d.User_Roles)
               .WithOne(a => a.User)
               .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Patients)
              .WithOne(a => a.User)
              .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Doctors)
              .WithOne(a => a.User)
              .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Nurses)
              .WithOne(a => a.User)
              .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Receptions)
              .WithOne(a => a.User)
              .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.Notifications)
              .WithOne(a => a.User)
              .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);


        });

        modelBuilder.Entity<User_Role>(entity =>
        {
            entity.ToTable("User_Roles");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();

            entity.HasOne(p => p.User)
                 .WithMany(cv => cv.User_Roles)
                 .HasForeignKey(cus => cus.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Role)
                 .WithMany(cv => cv.User_Roles)
                 .HasForeignKey(cus => cus.RoleId).OnDelete(DeleteBehavior.Cascade);

        });
    }

}
