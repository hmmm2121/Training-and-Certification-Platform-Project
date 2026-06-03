using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.Models;

namespace TrainingAndCertificationPlatform.Controllers
{
    [Authorize(Roles = "TrainingCoordinator")]
    public class RoomsController : Controller
    {
        private readonly TrainAndCertContext _context;

        public RoomsController(TrainAndCertContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomEquipments)
                .ToListAsync();

            return View(rooms);
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var room = await _context.Rooms
                .Include(r => r.RoomEquipments)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        // GET: Rooms/Create
        public IActionResult Create() => View();

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomName,Capacity")] Room room)
        {
            // Check for duplicate room name
            if (_context.Rooms.Any(r => r.RoomName == room.RoomName))
            {
                ModelState.AddModelError("RoomName", "A room with this name already exists.");
            }

            // Capacity must be greater than 0
            if (room.Capacity <= 0)
            {
                ModelState.AddModelError("Capacity", "Capacity must be greater than 0.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Room \"{room.RoomName}\" has been created.";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,RoomName,Capacity")] Room room)
        {
            if (id != room.RoomId)
                return NotFound();

            // check for duplicate room name excluding current room
            if (_context.Rooms.Any(r => r.RoomName == room.RoomName && r.RoomId != room.RoomId))
            {
                ModelState.AddModelError("RoomName", "A room with this name already exists.");
            }

            // capacity must be greater than 0
            if (room.Capacity <= 0)
            {
                ModelState.AddModelError("Capacity", "Capacity must be greater than 0.");
            }

            if (ModelState.IsValid)
            {
                _context.Update(room);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Room \"{room.RoomName}\" has been updated.";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var room = await _context.Rooms
                .Include(r => r.RoomEquipments)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomEquipments)
                .Include(r => r.CourseSessions)
                    .ThenInclude(s => s.Enrollments)
                        .ThenInclude(e => e.Assessments)
                .Include(r => r.CourseSessions)
                    .ThenInclude(s => s.Enrollments)
                        .ThenInclude(e => e.Payments)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room != null)
            {
                foreach (var session in room.CourseSessions)
                {
                    foreach (var enrollment in session.Enrollments)
                    {
                        _context.Assessments.RemoveRange(enrollment.Assessments);
                        _context.Payments.RemoveRange(enrollment.Payments);
                    }
                    _context.Enrollments.RemoveRange(session.Enrollments);
                }
                _context.CourseSessions.RemoveRange(room.CourseSessions);
                _context.RoomEquipments.RemoveRange(room.RoomEquipments);
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Room \"{room.RoomName}\" has been deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Rooms/AddEquipment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEquipment(int roomId, string equipmentName)
        {
            if (string.IsNullOrWhiteSpace(equipmentName))
            {
                TempData["ErrorMessage"] = "Equipment name cannot be empty.";
                return RedirectToAction(nameof(Details), new { id = roomId });
            }

            var equipment = new RoomEquipment
            {
                RoomId = roomId,
                EquipmentName = equipmentName
            };

            _context.RoomEquipments.Add(equipment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"\"{equipmentName}\" has been added.";
            return RedirectToAction(nameof(Details), new { id = roomId });
        }

        // POST: Rooms/RemoveEquipment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveEquipment(int equipmentId, int roomId)
        {
            var equipment = await _context.RoomEquipments.FindAsync(equipmentId);

            if (equipment != null)
            {
                _context.RoomEquipments.Remove(equipment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"\"{equipment.EquipmentName}\" has been removed.";
            }

            return RedirectToAction(nameof(Details), new { id = roomId });
        }
    }
}