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
    //[Authorize(Roles = "Training Coordinator")]
    public class CertificationTracksController : Controller
    {
        private readonly TrainAndCertContext _context;

        public CertificationTracksController(TrainAndCertContext context)
        {
            _context = context;
        }

        // GET: CertificationTracks
        public async Task<IActionResult> Index()
        {
            return View(await _context.CertificationTracks.ToListAsync());
        }

        // GET: CertificationTracks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificationTrack = await _context.CertificationTracks
                .FirstOrDefaultAsync(m => m.TrackId == id);
            if (certificationTrack == null)
            {
                return NotFound();
            }

            return View(certificationTrack);
        }

        // GET: CertificationTracks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CertificationTracks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrackId,Name,Description")] CertificationTrack certificationTrack)
        {
            if (ModelState.IsValid)
            {
                _context.Add(certificationTrack);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(certificationTrack);
        }

        // GET: CertificationTracks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificationTrack = await _context.CertificationTracks.FindAsync(id);
            if (certificationTrack == null)
            {
                return NotFound();
            }
            return View(certificationTrack);
        }

        // POST: CertificationTracks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrackId,Name,Description")] CertificationTrack certificationTrack)
        {
            if (id != certificationTrack.TrackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(certificationTrack);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CertificationTrackExists(certificationTrack.TrackId))
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
            return View(certificationTrack);
        }

        // GET: CertificationTracks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificationTrack = await _context.CertificationTracks
                .FirstOrDefaultAsync(m => m.TrackId == id);
            if (certificationTrack == null)
            {
                return NotFound();
            }

            return View(certificationTrack);
        }

        // POST: CertificationTracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var certificationTrack = await _context.CertificationTracks.FindAsync(id);
            if (certificationTrack != null)
            {
                _context.CertificationTracks.Remove(certificationTrack);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CertificationTrackExists(int id)
        {
            return _context.CertificationTracks.Any(e => e.TrackId == id);
        }
    }
}
