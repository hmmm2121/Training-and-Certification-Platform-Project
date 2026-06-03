using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TrainingAndCertificationPlatform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TrainingAndCertificationPlatform.Data;

public partial class TrainAndCertContext : IdentityDbContext
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

    public virtual DbSet<InstructorSubject> InstructorSubjects { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomEquipment> RoomEquipments { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<TrackCourse> TrackCourses { get; set; }

    public virtual DbSet<TraineeCertification> TraineeCertifications { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.HasKey(e => e.AssessmentId).HasName("PK__Assessme__3D2BF81E48067DB5");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.Assessments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Assessmen__Enrol__6754599E");
        });

        modelBuilder.Entity<CertificationTrack>(entity =>
        {
            entity.HasKey(e => e.TrackId).HasName("PK__Certific__7A74F8E0CB0F170A");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D71A7291EB40C");

            entity.HasOne(d => d.PrerequisiteCourse).WithMany(p => p.InversePrerequisiteCourse).HasConstraintName("FK__Courses__Prerequ__5070F446");

            entity.HasOne(d => d.Subject).WithMany(p => p.Courses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Courses__Subject__4F7CD00D");
        });

        modelBuilder.Entity<CourseSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__CourseSe__C9F492905E610E06");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseSes__Cours__5EBF139D");

            entity.HasOne(d => d.Instructor).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseSes__Instr__5FB337D6");

            entity.HasOne(d => d.Room).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseSes__RoomI__60A75C0F");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__7F68771B5B779DC3");

            entity.HasOne(d => d.Session).WithMany(p => p.Enrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__Sessi__6477ECF3");

            entity.HasOne(d => d.Trainee).WithMany(p => p.Enrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__Train__6383C8BA");
        });

        modelBuilder.Entity<InstructorAvailability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId).HasName("PK__Instruct__DA3979B1E1FC3887");

            entity.HasOne(d => d.Instructor).WithMany(p => p.InstructorAvailabilities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Instructo__Instr__571DF1D5");
        });

        modelBuilder.Entity<InstructorSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Instruct__3214EC07B678ECCD");

            entity.HasOne(d => d.Instructor).WithMany(p => p.InstructorSubjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Instructo__Instr__534D60F1");

            entity.HasOne(d => d.Subject).WithMany(p => p.InstructorSubjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Instructo__Subje__5441852A");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12CDB32DE1");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__787EE5A0");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A38090E3B2B");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Enroll__74AE54BC");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Rooms__32863939BCDE0D19");
        });

        modelBuilder.Entity<RoomEquipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("PK__RoomEqui__344744799BEAF242");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomEquipments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomEquip__RoomI__5BE2A6F2");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__AC1BA3A8C7ED05A5");
        });

        modelBuilder.Entity<TrackCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TrackCou__3214EC070C715E67");

            entity.HasOne(d => d.Course).WithMany(p => p.TrackCourses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TrackCour__Cours__6D0D32F4");

            entity.HasOne(d => d.Track).WithMany(p => p.TrackCourses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TrackCour__Track__6C190EBB");
        });

        modelBuilder.Entity<TraineeCertification>(entity =>
        {
            entity.HasKey(e => e.CertificationId).HasName("PK__TraineeC__1237E58A58412299");

            entity.HasOne(d => d.Track).WithMany(p => p.TraineeCertifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TraineeCe__Track__70DDC3D8");

            entity.HasOne(d => d.Trainee).WithMany(p => p.TraineeCertifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TraineeCe__Train__6FE99F9F");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C987AFF7F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
