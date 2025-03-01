using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;

namespace TomorrowsVoices.Controllers
{
    public class EventController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public EventController(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        // GET: Event
        public async Task<IActionResult> Index(bool archived = false)
        {
            var tomorrowsVoicesContext = _context.Events.Where(d => d.IsArchived == archived).Include(a => a.VolLocation);
            ViewData["IsArchived"] = archived;
            ViewData["ActiveTab"] = archived ? "archived" : "active";
            int archivedCount = await _context.Events.CountAsync(d => d.IsArchived == true);
            ViewData["numberofArchive"] = archivedCount;
            return View(await tomorrowsVoicesContext.ToListAsync());
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(Index => Index.VolLocation)
                .Include(Index => Index.VolAttendance).ThenInclude(Index => Index.Volunteer)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Event/Create
        public IActionResult Create()
        {
            Event @event = new Event();

             

            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City");
            return View(@event);
        }

        // POST: Event/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,Notes,StartTime,EndTime,VolLocationID")] Event @event)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(@event);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", @event.VolLocationID);
                return RedirectToAction(nameof(Index));
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", @event.VolLocationID);
            return View(@event);
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(Index => Index.VolLocation)
                .Include(Index => Index.VolAttendance).ThenInclude(Index => Index.Volunteer)
                .FirstOrDefaultAsync(Index => Index.ID == id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", @event.VolLocationID);
            return View(@event);
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,Notes,StartTime,EndTime,VolLocationID")] Event @event)
        {
            if (id != @event.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var eventToUpdate = await _context.Events
                        .Include(Index => Index.VolLocation)
                        .Include(Index => Index.VolAttendance)
                        .ThenInclude(Index => Index.Volunteer)
                        .FirstOrDefaultAsync(Index => Index.ID == id);

                    if (eventToUpdate == null)
                    {
                        return NotFound();
                    }

                    if (await TryUpdateModelAsync<Event>(
                     eventToUpdate, "",
                   Index => Index.Name, Index => Index.Description, Index => Index.Notes, Index => Index.StartTime, Index => Index.EndTime ,Index => Index.VolLocationID))
                    {
                        var attendance = await _context.VolAttendances
                            .Include(Index => Index.Volunteer)
                            .Where(Index => Index.EventID == id)
                            .ToListAsync(); 
                        eventToUpdate.VolAttendance = attendance;
                        _context.Update(eventToUpdate);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", @event.VolLocationID);
            return View(@event);
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(a => a.VolLocation)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //archive and unarchiving
        [HttpPost]
        public async Task<IActionResult> Archive(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            @event.IsArchived = true;
            await _context.SaveChangesAsync();



            TempData["SuccessMessage"] = "The Data has been archived successfully!";
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> UnArchive(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            @event.IsArchived = false;
            _context.Update(@event);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "This archive has been activated successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.ID == id);
        }
    }
}
