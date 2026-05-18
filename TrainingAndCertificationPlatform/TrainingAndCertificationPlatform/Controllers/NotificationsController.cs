using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.Services;

namespace TrainingAndCertificationPlatform.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly TrainAndCertContext _context;
        private readonly NotificationService _notificationService;

        public NotificationsController(TrainAndCertContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // shows all notifications for the logged in user
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            // Mark all as read when they open the page
            foreach (var n in notifications.Where(n => !n.IsRead))
                n.IsRead = true;

            await _context.SaveChangesAsync();

            return View(notifications);
        }

        // Marks a single notification as read through link
        [HttpPost]
        public async Task<IActionResult> MarkRead(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == userId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}