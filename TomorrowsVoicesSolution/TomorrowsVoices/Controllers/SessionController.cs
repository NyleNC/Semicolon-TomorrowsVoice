using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;

namespace TomorrowsVoices.Controllers
{
    public class SessionController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public SessionController(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        // GET: Session
        public async Task<IActionResult> Index()
        {
            var tomorrowsVoicesContext = _context.Sessions
                .Include(s => s.Location).ThenInclude(l => l.Director)
                .Include(s => s.Attendance)
                .AsNoTracking();
            return View(await tomorrowsVoicesContext.ToListAsync());
        }

        // GET: Session/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Location).ThenInclude(l => l.Director)
                .Include(s => s.Attendance)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (session == null)
            {
                return NotFound();
            }

            var presentSingersCount = session.Attendance.Count(a => a.Status == true);
            var absentSingersCount = session.Attendance.Count(a => a.Status == false);
            var totalSingersCount = session.Attendance.Count();

            ViewBag.PresentSingersCount = $"{presentSingersCount}/{totalSingersCount}";
            ViewBag.AbsentSingersCount = $"{absentSingersCount}/{totalSingersCount}";



            return View(session);
        }

        // GET: Session/Create
        public IActionResult Create()
        {
            Session session = new Session { LocationID = null };
            return View(session);
        }



        // POST: Session/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,Notes,LocationID")] Session session)
        {
            if (ModelState.IsValid)
            {
                _context.Add(session);
                await _context.SaveChangesAsync();


                var attendances = _context.Attendances
                    .Where( async => async.SessionID == session.ID)
                    .Include(a => a.Singer)
                    .ToList();

                var presentSingersCount = session.Attendance.Count(a => a.Status == true);
                var absentSingersCount = session.Attendance.Count(a => a.Status == false);
                var totalSingersCount = session.Attendance.Count();

                ViewBag.PresentSingersCount = $"{presentSingersCount}/{totalSingersCount}";
                ViewBag.AbsentSingersCount = $"{absentSingersCount}/{totalSingersCount}";



                return RedirectToAction(nameof(Index));
            }
            LocationSelectList();
            return View(session);
        }

        // GET: Session/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "ID", session.LocationID);
            return View(session);
        }

        // POST: Session/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Notes,Date,LocationID")] Session session)
        {
            if (id != session.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(session.ID))
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
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "ID", session.LocationID);
            return View(session);
        }

        // GET: Session/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Location)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: Session/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session != null)
            {
                _context.Sessions.Remove(session);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.ID == id);
        }



        private SelectList LocationSelectList()
        {
            return new SelectList(_context.Locations
                .OrderBy(d => d.City)
                , "ID", "City");
        }

        [HttpGet]
        public JsonResult GetDirectorByLocation(int locationId)
        {
            var director = _context.Locations
                                   .Where(l => l.ID == locationId)
                                   .Select(l => l.Director.DirectorFullName) // Make sure `Director` exists and has a Name
                                   .FirstOrDefault();

            return Json(new { directorName = director });
        }


    }
}
