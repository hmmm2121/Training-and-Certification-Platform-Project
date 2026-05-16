using Microsoft.EntityFrameworkCore;
using TrainingPlatform.MVC.Models;

namespace TrainingPlatform.MVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<RoomEquipment> RoomEquipments { get; set; } = null!;
        public DbSet<CourseSession> CourseSessions { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Assessment> Assessments { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<CertificationTrack> CertificationTracks { get; set; } = null!;
        public DbSet<TrackCourse> TrackCourses { get; set; } = null!;
        public DbSet<TraineeCertification> TraineeCertifications { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fix primary keys (non-standard names)
            modelBuilder.Entity<CertificationTrack>()
                .HasKey(c => c.TrackId);

            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<Course>()
                .HasKey(c => c.CourseId);

            modelBuilder.Entity<Room>()
                .HasKey(r => r.RoomId);

            modelBuilder.Entity<RoomEquipment>()
                .HasKey(r => r.EquipmentId);

            modelBuilder.Entity<CourseSession>()
                .HasKey(c => c.SessionId);

            modelBuilder.Entity<Enrollment>()
                .HasKey(e => e.EnrollmentId);

            modelBuilder.Entity<Assessment>()
                .HasKey(a => a.AssessmentId);

            modelBuilder.Entity<Payment>()
                .HasKey(p => p.PaymentId);

            modelBuilder.Entity<TrackCourse>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<TraineeCertification>()
                .HasKey(t => t.CertificationId);

            modelBuilder.Entity<Notification>()
                .HasKey(n => n.NotificationId);

            modelBuilder.Entity<Subject>()
                .HasKey(s => s.SubjectId);

            // Prevent duplicate enrollments
            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.TraineeId, e.SessionId })
                .IsUnique();

            // Course self-referencing prerequisite
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Prerequisite)
                .WithMany()
                .HasForeignKey(c => c.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal precision
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Course>()
                .Property(c => c.Fee)
                .HasColumnType("decimal(10,2)");
        }
    }
}