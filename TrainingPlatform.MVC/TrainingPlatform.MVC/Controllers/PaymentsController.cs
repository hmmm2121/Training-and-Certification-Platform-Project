using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainingPlatform.MVC.Data;
using TrainingPlatform.MVC.Models;

namespace TrainingPlatform.MVC.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly AppDbContext _context;

        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        // INDEX - Show all payments with outstanding balances
        public async Task<IActionResult> Index()
        {
            // Flag overdue payments automatically on load
            var overduePayments = await _context.Payments
                .Where(p => p.PaidAt == null && p.DueDate < DateOnly.FromDateTime(DateTime.Today))
                .ToListAsync();

            foreach (var payment in overduePayments)
                payment.IsOverdue = true;

            await _context.SaveChangesAsync();

            // Load all payments with enrollment and trainee info
            var payments = await _context.Payments
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Trainee)
                .Include(p => p.Enrollment)
                    .ThenInclude(e => e.Session)
                        .ThenInclude(s => s.Course)
                .ToListAsync();

            return View(payments);
        }

        // DETAILS
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

        // TRAINEE PAYMENT HISTORY
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

            // Calculate outstanding balance
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

        // CREATE GET - Record a new payment
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
        {
            // Get the enrollment to check course fee
            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Payments)
                .FirstOrDefaultAsync(e => e.EnrollmentId == payment.EnrollmentId);

            if (enrollment == null) return NotFound();

            // Check amount doesn't exceed remaining balance
            decimal totalPaid = enrollment.Payments
                .Where(p => p.PaidAt != null)
                .Sum(p => p.Amount);

            decimal remaining = enrollment.Session.Course.Fee - totalPaid;

            if (payment.Amount > remaining)
            {
                ModelState.AddModelError("Amount",
                    $"Payment amount exceeds remaining balance of {remaining:C}.");
                await PopulateDropdowns();
                return View(payment);
            }

            // Mark as paid now
            payment.PaidAt = DateTime.Now;
            payment.IsOverdue = false;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment recorded successfully!";
            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            await PopulateDropdowns();
            return View(payment);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Payment payment)
        {
            if (id != payment.PaymentId) return NotFound();

            try
            {
                _context.Update(payment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Payment updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Payments.Any(p => p.PaymentId == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // DELETE GET
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

        // DELETE POST
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

            TempData["Success"] = "Payment deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // Helper to populate dropdowns
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