using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingAndCertificationAPI.DTOs;
using TrainingAndCertificationPlatform.Data;

namespace TrainingAndCertificationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "TrainingCoordinator")]
    public class ReportsController : ControllerBase
    {
        private readonly TrainAndCertContext _context;
        public ReportsController(TrainAndCertContext context) => _context = context;

        [HttpGet("enrollment-stats")]
        public async Task<ActionResult<IEnumerable<EnrollmentStatDto>>> EnrollmentStats()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Session).ThenInclude(s => s.Course).ThenInclude(c => c.Subject)
                .ToListAsync();

            var stats = enrollments
                .GroupBy(e => new { e.Session.Course.Title, e.Session.Course.Subject.Name })
                .Select(g => new EnrollmentStatDto
                {
                    Course = g.Key.Title,
                    Subject = g.Key.Name,
                    TotalEnrollments = g.Count(),
                    Completed = g.Count(e => e.Status == "Completed"),
                    Dropped = g.Count(e => e.Status == "Dropped")
                })
                .ToList();

            return Ok(stats);
        }

        [HttpGet("instructor-workload")]
        public async Task<ActionResult<IEnumerable<InstructorWorkloadDto>>> InstructorWorkload()
        {
            var sessions = await _context.CourseSessions
                .Include(s => s.Instructor)
                .Include(s => s.Enrollments)
                .ToListAsync();

            var workload = sessions
                .GroupBy(s => s.Instructor.FullName)
                .Select(g => new InstructorWorkloadDto
                {
                    Instructor = g.Key,
                    SessionsTaught = g.Count(),
                    TotalTraineesTaught = g.Sum(s => s.Enrollments.Count)
                })
                .ToList();

            return Ok(workload);
        }

        [HttpGet("certification-rates")]
        public async Task<ActionResult<IEnumerable<CertificationRateDto>>> CertificationRates()
        {
            var tracks = await _context.CertificationTracks
                .Include(t => t.TraineeCertifications)
                .ToListAsync();

            var rates = tracks.Select(t =>
            {
                int total = t.TraineeCertifications.Count;
                int eligible = t.TraineeCertifications.Count(tc => tc.Status == "Eligible");
                return new CertificationRateDto
                {
                    Track = t.Name,
                    TotalTrainees = total,
                    Eligible = eligible,
                    CompletionRatePercent = total == 0 ? 0 : Math.Round(100.0 * eligible / total, 1)
                };
            }).ToList();

            return Ok(rates);
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueReportDto>> Revenue()
        {
            var payments = await _context.Payments.ToListAsync();

            decimal collected = 0m;
            decimal outstanding = 0m;
            int overdue = 0;

            foreach (var p in payments)
            {
                if (p.PaidAt != null)
                    collected += p.Amount;
                else
                    outstanding += p.Amount;

                if (p.IsOverdue)
                    overdue++;
            }

            return Ok(new RevenueReportDto
            {
                TotalCollected = collected,
                TotalOutstanding = outstanding,
                OverduePayments = overdue
            });
        }

        [HttpGet("course-popularity")]
        public async Task<ActionResult<IEnumerable<CoursePopularityDto>>> CoursePopularity()
        {
            var courses = await _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.CourseSessions).ThenInclude(s => s.Enrollments)
                .ToListAsync();

            var popularity = courses.Select(c => new CoursePopularityDto
            {
                Course = c.Title,
                Subject = c.Subject.Name,
                TotalEnrollments = c.CourseSessions.Sum(s => s.Enrollments.Count),
                AvailableCapacity = c.Capacity
            })
            .OrderByDescending(x => x.TotalEnrollments)
            .ToList();

            return Ok(popularity);
        }

        [HttpGet("room-utilization")]
        public async Task<ActionResult<IEnumerable<RoomUtilizationDto>>> RoomUtilization()
        {
            var rooms = await _context.Rooms
                .Include(r => r.CourseSessions)
                .ToListAsync();

            var utilization = rooms.Select(r => new RoomUtilizationDto
            {
                RoomName = r.RoomName,
                Capacity = r.Capacity,
                TotalSessionsHosted = r.CourseSessions.Count
            })
            .OrderByDescending(x => x.TotalSessionsHosted)
            .ToList();

            return Ok(utilization);
        }

        [HttpGet("trainee-payments")]
        public async Task<ActionResult<IEnumerable<TraineePaymentDto>>> TraineePayments()
        {
            var payments = await _context.Payments
                .Include(p => p.Enrollment).ThenInclude(e => e.Trainee)
                .ToListAsync();

            var perTrainee = payments
                .GroupBy(p => p.Enrollment.Trainee.FullName)
                .Select(g => new TraineePaymentDto
                {
                    TraineeName = g.Key,
                    TotalPaid = g.Where(p => p.PaidAt != null).Sum(p => p.Amount),
                    TotalOutstanding = g.Where(p => p.PaidAt == null).Sum(p => p.Amount),
                    OverdueCount = g.Count(p => p.IsOverdue)
                })
                .ToList();

            return Ok(perTrainee);
        }
    }
}