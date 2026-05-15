using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.Models;

namespace TrainingAndCertificationPlatform.Controllers
{
    //[Authorize(Roles = "Instructor,Training Coordinator")]
    public class AssessmentsController : Controller
    {
        private readonly TrainAndCertContext _context;

        public AssessmentsController(TrainAndCertContext context)
        {
            _context = context;
        }

        // GET: Assessments
        public async Task<IActionResult> Index()
        {
            var trainAndCertContext = _context.Assessments.Include(a => a.Enrollment);
            return View(await trainAndCertContext.ToListAsync());
        }

        // GET: Assessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessments
                .Include(a => a.Enrollment)
                .FirstOrDefaultAsync(m => m.AssessmentId == id);
            if (assessment == null)
            {
                return NotFound();
            }

            return View(assessment);
        }

        // GET: Assessments/Create
        public IActionResult Create()
        {
            ViewData["EnrollmentId"] = new SelectList(
            _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Select(e => new
                {
                    e.EnrollmentId,
                    Display = e.Trainee.FullName + " - " + e.Session.Course.Title
                }),
            "EnrollmentId",
            "Display"
            );
            ViewData["Result"] = new SelectList(new[] { "Pass", "Fail" });
            return View();
        }

        // POST: Assessments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssessmentId,EnrollmentId,Result,Notes")] Assessment assessment)
        {
            //Validate that the selected enrollment does not already have an assessment
            if (_context.Assessments.Any(a => a.EnrollmentId == assessment.EnrollmentId))
            {
                ModelState.AddModelError("EnrollmentId", "This enrollment already has an assessment.");
            }

            if (ModelState.IsValid)
            {
                assessment.RecordedAt = DateTime.Now;
                _context.Add(assessment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EnrollmentId"] = new SelectList(
            _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Select(e => new
                {
                    e.EnrollmentId,
                    Display = e.Trainee.FullName + " - " + e.Session.Course.Title
                }),
            "EnrollmentId", "Display", assessment.EnrollmentId
            );
            ViewData["Result"] = new SelectList(new[] { "Pass", "Fail" }, assessment?.Result);
            return View(assessment);
        }

        // GET: Assessments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
            {
                return NotFound();
            }
            ViewData["EnrollmentId"] = new SelectList(
            _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Select(e => new
                {
                    e.EnrollmentId,
                    Display = e.Trainee.FullName + " - " + e.Session.Course.Title
                }),
            "EnrollmentId", "Display", assessment.EnrollmentId
            );
            ViewData["Result"] = new SelectList(new[] { "Pass", "Fail" }, assessment?.Result);
            return View(assessment);
        }

        // POST: Assessments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssessmentId,EnrollmentId,Result,Notes")] Assessment assessment)
        {
            if (id != assessment.AssessmentId)
            {
                return NotFound();
            }

            // Validate that the selected enrollment does not already have an assessment (excluding the current one)
            if (_context.Assessments.Any(a =>
                a.AssessmentId != assessment.AssessmentId &&
                a.EnrollmentId == assessment.EnrollmentId))
            {
                ModelState.AddModelError("EnrollmentId", "This enrollment already has an assessment.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    assessment.RecordedAt = DateTime.Now;
                    _context.Update(assessment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssessmentExists(assessment.AssessmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EnrollmentId"] = new SelectList(
            _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Select(e => new
                {
                    e.EnrollmentId,
                    Display = e.Trainee.FullName + " - " + e.Session.Course.Title
                }),
            "EnrollmentId", "Display", assessment.EnrollmentId
            );
            ViewData["Result"] = new SelectList(new[] { "Pass", "Fail" }, assessment?.Result);
            return View(assessment);
        }

        // GET: Assessments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessments
                .Include(a => a.Enrollment)
                .FirstOrDefaultAsync(m => m.AssessmentId == id);
            if (assessment == null)
            {
                return NotFound();
            }

            return View(assessment);
        }

        // POST: Assessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment != null)
            {
                _context.Assessments.Remove(assessment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssessmentExists(int id)
        {
            return _context.Assessments.Any(e => e.AssessmentId == id);
        }
    }
}
