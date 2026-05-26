using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.Models;

namespace TrainingAndCertificationPlatform.Controllers
{
    [Authorize(Roles = "TrainingCoordinator")]
    public class PaymentsController : Controller
    {
        private readonly TrainAndCertContext _context;

        public PaymentsController(TrainAndCertContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Flag overdue payments
            var overduePayments = await _context.Payments
                .Where(p => p.PaidAt == null && p.DueDate < DateOnly.FromDateTime(DateTime.Today))
                .ToListAsync();

            foreach (var p in overduePayments)
                p.IsOverdue = true;

            await _context.SaveChangesAsync();

            var payments = await _context.Payments
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Trainee)
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .ToListAsync();

            return View(payments);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Trainee)
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null) return NotFound();

            return View(payment);
        }

        public async Task<IActionResult> TraineeHistory(int traineeId)
        {
            var trainee = await _context.Users.FindAsync(traineeId);
            if (trainee == null) return NotFound();

            var payments = await _context.Payments
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Trainee)
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .Where(p => p.Enrollment.TraineeId == traineeId)
                .ToListAsync();

            var enrollments = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Payments)
                .Where(e => e.TraineeId == traineeId)
                .ToListAsync();

            decimal totalFees = enrollments.Sum(e => e.Session.Course.Fee);
            decimal totalPaid = payments.Where(p => p.PaidAt != null).Sum(p => p.Amount);
            decimal outstanding = totalFees - totalPaid;

            ViewBag.Trainee = trainee;
            ViewBag.TotalFees = totalFees;
            ViewBag.TotalPaid = totalPaid;
            ViewBag.Outstanding = outstanding;

            return View(payments);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Payments)
                .FirstOrDefaultAsync(e => e.EnrollmentId == payment.EnrollmentId);

            if (enrollment == null) return NotFound();

            decimal totalPaid = enrollment.Payments
                .Where(p => p.PaidAt != null)
                .Sum(p => p.Amount);

            decimal remaining = enrollment.Session.Course.Fee - totalPaid;

            if (payment.Amount > remaining)
            {
                ModelState.AddModelError("Amount",
                    $"Amount exceeds remaining balance of {remaining:C}.");
                await PopulateDropdowns();
                return View(payment);
            }

            payment.PaidAt = DateTime.Now;
            payment.IsOverdue = payment.IsOverdue;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment recorded!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Trainee)
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null) return NotFound();

            return View(payment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Payment deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns()
        {
            ViewData["EnrollmentId"] = new SelectList(
                await _context.Enrollments
                    .Include(e => e.Trainee)
                    .Include(e => e.Session)
                        .ThenInclude(s => s.Course)
                    .Select(e => new {
                        e.EnrollmentId,
                        Display = e.Trainee.FullName + " - " + e.Session.Course.Title
                    }).ToListAsync(),
                "EnrollmentId", "Display");
        }
    }
}