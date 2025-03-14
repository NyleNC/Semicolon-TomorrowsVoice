using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;
using TomorrowsVoices.ViewModels;

namespace TomorrowsVoices.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public ScheduleController(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        // GET: Schedule

        public async Task<IActionResult> Index()
        {
            // Fetch schedules with volunteer and event data
            var schedules = await _context.Schedules
                .Include(s => s.Volunteer)
                .Include(s => s.Event) // Include Event
                .ToListAsync();

            // Fetch volunteers and events for the dropdowns
            ViewBag.Volunteers = await _context.Volunteers.ToListAsync();
            ViewBag.Events = await _context.Events.ToListAsync();

            // Create the ViewModel
            var viewModel = new ScheduleViewModel();

            // Populate the ViewModel and calculate total hours
            foreach (var schedule in schedules)
            {
                // Calculate shift duration in hours
                if (schedule.ActualStartTime.HasValue && schedule.ActualEndTime.HasValue)
                {
                    var shiftDuration = (schedule.ActualEndTime.Value - schedule.ActualStartTime.Value).TotalHours;

                    // Add to the appropriate shift list
                    if (schedule.ActualStartTime >= TimeOnly.FromTimeSpan(TimeSpan.FromHours(8)) &&
                        schedule.ActualStartTime < TimeOnly.FromTimeSpan(TimeSpan.FromHours(12)))
                    {
                        viewModel.MorningShifts.Add(schedule);
                    }
                    else if (schedule.ActualStartTime >= TimeOnly.FromTimeSpan(TimeSpan.FromHours(12)) &&
                             schedule.ActualStartTime < TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)))
                    {
                        viewModel.AfternoonShifts.Add(schedule);
                    }
                    else if (schedule.ActualStartTime >= TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)))
                    {
                        viewModel.EveningShifts.Add(schedule);
                    }

                    // Update total hours for the volunteer
                    if (schedule.Volunteer != null)
                    {
                        var volunteerName = schedule.Volunteer.FullName;
                        if (viewModel.VolunteerTotalHours.ContainsKey(volunteerName))
                        {
                            viewModel.VolunteerTotalHours[volunteerName] += shiftDuration;
                        }
                        else
                        {
                            viewModel.VolunteerTotalHours[volunteerName] = shiftDuration;
                        }
                    }
                }
            }

            return View(viewModel);
        }

        // GET: Schedule/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Volunteer)
               
                .Include(s => s.Event)// Include VolLocation
                .FirstOrDefaultAsync(m => m.ID == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // GET: Schedule/Create
        public IActionResult Create()
        {
            ViewData["volunteerID"] = new SelectList(_context.Volunteers, "ID", "FullName");
            ViewData["eventID"] = new SelectList(_context.Events, "ID", "Name"); // Use Event instead of VolLocation
            return View();
        }
        // POST: Schedule/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,Notes,Date,StartTime,EndTime,ActualStartTime,ActualEndTime,VolLocationID")] Event @event, List<Schedule> Schedules)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Add the event to the database
                    _context.Add(@event);
                    await _context.SaveChangesAsync();

                    // Add schedules to the event
                    if (Schedules != null)
                    {
                        foreach (var schedule in Schedules)
                        {
                            schedule.eventID = @event.ID; // Link the schedule to the event
                            _context.Add(schedule);
                        }
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            // If we got this far, something failed; redisplay form
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", @event.VolLocationID);
            return View(@event);
        }

        // GET: Schedule/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Volunteer)
                .Include(s => s.Event)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (schedule == null)
            {
                return NotFound();
            }

            // Populate dropdowns for the view
            ViewData["volunteerID"] = new SelectList(_context.Volunteers, "ID", "FullName", schedule.volunteerID);
            ViewData["eventID"] = new SelectList(_context.Events, "ID", "Name", schedule.eventID);

            return View(schedule);
        }
        // POST: Schedule/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ShiftStart,ShiftEnd,ActualStartTime,ActualEndTime,eventID,volunteerID,IsArchived")] Schedule schedule)
        {
            if (id != schedule.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.ID))
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

            // Repopulate dropdowns if the model state is invalid
      
            ViewData["eventID"] = new SelectList(_context.Events, "ID", "Name", schedule.eventID); // Use Event instead of VolLocation

            return View(schedule);
        }
        // GET: Schedule/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Volunteer)
                .Include(s => s.Event) // Include Event
                .FirstOrDefaultAsync(m => m.ID == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }
        // POST: Schedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultiple(List<Schedule> schedules)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Schedules.AddRange(schedules); // Add multiple schedules
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    // Log the exception (optional)
                    return Json(new { success = false, message = ex.Message });
                }
            }

            // If the model state is invalid, return validation errors
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return Json(new { success = false, message = "Validation errors: " + string.Join(", ", errors) });
        }


        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.ID == id);
        }
    }
}