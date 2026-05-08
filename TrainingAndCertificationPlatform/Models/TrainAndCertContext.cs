using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Models;

public partial class TrainAndCertContext : DbContext
{
    public TrainAndCertContext()
    {
    }

    public TrainAndCertContext(DbContextOptions<TrainAndCertContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assessment> Assessments { get; set; }

    public virtual DbSet<CertificationTrack> CertificationTracks { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseSession> CourseSessions { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<InstructorAvailability> InstructorAvailabilities { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomEquipment> RoomEquipments { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<TraineeCertification> TraineeCertifications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TrainingAndCertificationPlatform;Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.HasKey(e => e.AssessmentId).HasName("PK__Assessme__3D2BF81E432A7F55");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.Assessments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Assessmen__Enrol__6383C8BA");
        });

        modelBuilder.Entity<CertificationTrack>(entity =>
        {
            entity.HasKey(e => e.TrackId).HasName("PK__Certific__7A74F8E0548363B0");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D71A7758E8A7D");

            entity.HasOne(d => d.PrerequisiteCourse).WithMany(p => p.InversePrerequisiteCourse).HasConstraintName("FK__Courses__Prerequ__5070F446");

            entity.HasOne(d => d.Subject).WithMany(p => p.Courses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Courses__Subject__4F7CD00D");
        });

        modelBuilder.Entity<CourseSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__CourseSe__C9F492904A2D5685");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseSes__Cours__5AEE82B9");

            entity.HasOne(d => d.Instructor).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseSes__Instr__5BE2A6F2");

            entity.HasOne(d => d.Room).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseSes__RoomI__5CD6CB2B");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__7F68771BF598E020");

            entity.HasOne(d => d.Session).WithMany(p => p.Enrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__Sessi__60A75C0F");

            entity.HasOne(d => d.Trainee).WithMany(p => p.Enrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__Train__5FB337D6");
        });

        modelBuilder.Entity<InstructorAvailability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId).HasName("PK__Instruct__DA3979B132716F95");

            entity.HasOne(d => d.Instructor).WithMany(p => p.InstructorAvailabilities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Instructo__Instr__534D60F1");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12A4CD6FB8");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__6FE99F9F");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A38CCC64A06");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Enroll__6C190EBB");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Rooms__32863939677D6F5A");
        });

        modelBuilder.Entity<RoomEquipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("PK__RoomEqui__34474479ABB8F8A6");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomEquipments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomEquip__RoomI__5812160E");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__AC1BA3A8B2B7C080");
        });

        modelBuilder.Entity<TraineeCertification>(entity =>
        {
            entity.HasKey(e => e.CertificationId).HasName("PK__TraineeC__1237E58A39AF362E");

            entity.HasOne(d => d.Track).WithMany(p => p.TraineeCertifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TraineeCe__Track__693CA210");

            entity.HasOne(d => d.Trainee).WithMany(p => p.TraineeCertifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TraineeCe__Train__68487DD7");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CFAB0FA0D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
