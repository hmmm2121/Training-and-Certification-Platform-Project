using Microsoft.EntityFrameworkCore;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.Models;

namespace TrainingAndCertificationPlatform.Services
{
    public class NotificationService
    {
        private readonly TrainAndCertContext _context;

        public NotificationService(TrainAndCertContext context)
        {
            _context = context;
        }

        // creates a notification for a user
        public async Task SendAsync(int userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        // trainee enrolled in a session
        public async Task NotifyEnrollmentConfirmed(int traineeId, string courseName)
        {
            await SendAsync(traineeId,
                $"Your enrollment in \"{courseName}\" has been confirmed.");
        }

        // instructor assigned to a session
        public async Task NotifyInstructorAssigned(int instructorId, string courseName, string sessionDate)
        {
            await SendAsync(instructorId,
                $"You have been assigned to teach \"{courseName}\" on {sessionDate}.");
        }

        // trainee gets their assessment result
        public async Task NotifyAssessmentRecorded(int traineeId, string courseName, string result)
        {
            await SendAsync(traineeId,
                $"Your assessment result for \"{courseName}\" has been recorded: {result}.");
        }

        // instructor gets notified when a new trainee joins their session
        public async Task NotifyInstructorNewEnrollment(int instructorId, string traineeName, string courseName)
        {
            await SendAsync(instructorId,
                $"{traineeName} has enrolled in your \"{courseName}\" session.");
        }

        // trainee gets notified when they earn a certificate
        public async Task NotifyCertificationIssued(int traineeId, string trackName)
        {
            await SendAsync(traineeId,
                $"Congratulations! Your certificate for \"{trackName}\" has been issued.");
        }

        // returns unread count for the navbar badge
        public int GetUnreadCount(int userId)
        {
            return _context.Notifications
                .Count(n => n.UserId == userId && !n.IsRead);
        }
    }
}