using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingAndCertificationAPI.DTOs;
using TrainingAndCertificationPlatform.Data;

namespace TrainingAndCertificationAPI.Controllers
{
    [ApiController]
    [Route("api/public")]
    [AllowAnonymous]
    public class PublicController : ControllerBase
    {
        private readonly TrainAndCertContext _context;
        public PublicController(TrainAndCertContext context) => _context = context;

        [HttpGet("certifications/verify")]
        public async Task<ActionResult<CertificationVerificationDto>> Verify([FromQuery] int traineeId,[FromQuery] string certificateRef)
        {
            if (traineeId <= 0 || string.IsNullOrWhiteSpace(certificateRef))
                return BadRequest("Both traineeId and certificateRef are required.");

            var cert = await _context.TraineeCertifications
                .Include(c => c.Trainee)
                .Include(c => c.Track)
                .FirstOrDefaultAsync(c => c.TraineeId == traineeId && c.CertificateRef == certificateRef);

            if (cert == null)
                return Ok(new CertificationVerificationDto { Found = false });

            var completedCourses = await _context.Assessments
                .Include(a => a.Enrollment).ThenInclude(e => e.Session).ThenInclude(s => s.Course)
                .Where(a => a.Enrollment.TraineeId == traineeId && a.Result == "Pass")
                .Select(a => a.Enrollment.Session.Course.Title)
                .Distinct()
                .ToListAsync();

            return Ok(new CertificationVerificationDto
            {
                Found = true,
                TraineeName = cert.Trainee.FullName,
                TrackName = cert.Track.Name,
                Status = cert.Status,
                IssuedAt = cert.IssuedAt,
                CompletedCourses = completedCourses
            });
        }
    }
}