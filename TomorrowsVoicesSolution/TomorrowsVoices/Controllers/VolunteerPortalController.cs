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
    public class VolunteerPortalController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public VolunteerPortalController(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        // GET: VolunteerPortal/AvailableEvents
        public async Task<IActionResult> AvailableEvents()
        {
            // In a real application, get the current volunteer ID from authentication
            // For demo purposes, we'll get the first volunteer or allow selection
            var volunteerId = TempData["VolunteerId"] as int? ?? 1;
            TempData["VolunteerId"] = volunteerId; // Persist between requests
            
            // Get all active events that haven't ended yet
            var events = await _context.Events
                .Include(e => e.VolLocation)
                .Include(e => e.VolSchedules)
                    .ThenInclude(s => s.VolAttendances)
                .Where(e => e.End > DateTime.Now && !e.IsArchived)
                .OrderBy(e => e.Start)
                .ToListAsync();

            // Transform to view models
            var eventCards = events.Select(e => new EventCardViewModel
            {
                EventId = e.ID,
                Title = e.Name,
                Address = e.Location,
                City = e.VolLocation?.City,
                Start = e.Start,
                End = e.End,
                Notes = e.Notes,
                Schedules = e.VolSchedules
                    .OrderBy(s => s.ScheduledStart)
                    .Select(s => new ScheduleViewModel
                    {
                        ScheduleId = s.ID,
                        Start = s.ScheduledStart,
                        End = s.ScheduledEnd,
                        IsRegistered = s.VolAttendances.Any(a => a.VolunteerID == volunteerId && a.Status)
                    })
                    .ToList(),
                IsRegistered = e.VolSchedules.Any(s => s.VolAttendances.Any(a => a.VolunteerID == volunteerId && a.Status))
            }).ToList();

            // Get the volunteer for the header display
            var volunteer = await _context.Volunteers.FindAsync(volunteerId);
            ViewData["VolunteerName"] = volunteer?.FullName ?? "Volunteer";
            ViewData["VolunteerId"] = volunteerId;

            return View(eventCards);
        }

        // GET: VolunteerPortal/MyEvents
        public async Task<IActionResult> MyEvents()
        {
            // Get volunteer ID (normally from auth)
            var volunteerId = TempData["VolunteerId"] as int? ?? 1;
            TempData["VolunteerId"] = volunteerId; // Persist between requests

            // Get all events where the volunteer is registered
            var myAttendances = await _context.VolAttendances
                .Include(a => a.VolSchedule)
                    .ThenInclude(s => s.Event)
                        .ThenInclude(e => e.VolLocation)
                .Where(a => a.VolunteerID == volunteerId && a.Status)
                .ToListAsync();

            // Group by event for display
            var myEvents = myAttendances
                .GroupBy(a => a.VolSchedule.Event)
                .Select(g => new MyEventViewModel
                {
                    EventId = g.Key.ID,
                    Title = g.Key.Name,
                    Address = g.Key.Location,
                    City = g.Key.VolLocation?.City,
                    EventStart = g.Key.Start,
                    EventEnd = g.Key.End,
                    MyShifts = g.Select(a => new MyShiftViewModel
                    {
                        AttendanceId = a.ID,
                        ScheduleId = a.VolScheduleID,
                        ShiftStart = a.VolSchedule.ScheduledStart,
                        ShiftEnd = a.VolSchedule.ScheduledEnd,
                        ActualStart = a.ActualStart,
                        ActualEnd = a.ActualEnd,
                        TimeUntilShift = a.VolSchedule.ScheduledStart - DateTime.Now,
                        CanCheckIn = a.VolSchedule.ScheduledStart.AddMinutes(-15) <= DateTime.Now && 
                                    a.VolSchedule.ScheduledEnd >= DateTime.Now &&
                                    a.ActualStart == null
                    }).ToList()
                })
                .OrderBy(e => e.EventStart)
                .ToList();

            // Get the volunteer for the header display
            var volunteer = await _context.Volunteers.FindAsync(volunteerId);
            ViewData["VolunteerName"] = volunteer?.FullName ?? "Volunteer";

            // Categorize events
            ViewData["UpcomingEvents"] = myEvents.Where(e => e.EventStart > DateTime.Now).ToList();
            ViewData["PastEvents"] = myEvents.Where(e => e.EventEnd < DateTime.Now).ToList();
            ViewData["CurrentEvents"] = myEvents.Where(e => e.EventStart <= DateTime.Now && e.EventEnd >= DateTime.Now).ToList();

            return View(myEvents);
        }

        // GET: VolunteerPortal/CheckIn/5
        public async Task<IActionResult> CheckIn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the attendance record
            var attendance = await _context.VolAttendances
                .Include(a => a.VolSchedule)
                    .ThenInclude(s => s.Event)
                        .ThenInclude(e => e.VolLocation)
                .Include(a => a.Volunteer)
                .FirstOrDefaultAsync(a => a.ID == id);

            if (attendance == null)
            {
                return NotFound();
            }

            // Create view model
            var checkInViewModel = new CheckInViewModel
            {
                AttendanceId = attendance.ID,
                VolunteerId = attendance.VolunteerID,
                VolunteerName = attendance.Volunteer?.FullName,
                EventId = attendance.VolSchedule.EventID,
                EventName = attendance.VolSchedule.Event.Name,
                EventLocation = attendance.VolSchedule.Event.Location,
                EventCity = attendance.VolSchedule.Event.VolLocation?.City,
                ScheduleId = attendance.VolScheduleID,
                ShiftStart = attendance.VolSchedule.ScheduledStart,
                ShiftEnd = attendance.VolSchedule.ScheduledEnd,
                ActualStart = attendance.ActualStart,
                ActualEnd = attendance.ActualEnd,
                CanCheckIn = attendance.VolSchedule.ScheduledStart.AddMinutes(-15) <= DateTime.Now && 
                             attendance.VolSchedule.ScheduledEnd >= DateTime.Now &&
                             attendance.ActualStart == null,
                CanCheckOut = attendance.ActualStart.HasValue && 
                              !attendance.ActualEnd.HasValue && 
                              attendance.VolSchedule.ScheduledEnd.AddMinutes(30) >= DateTime.Now
            };

            return View(checkInViewModel);
        }

        // POST: VolunteerPortal/PerformCheckIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerformCheckIn(int id)
        {
            var attendance = await _context.VolAttendances.FindAsync(id);
            
            if (attendance == null)
            {
                return NotFound();
            }
            
            attendance.ActualStart = DateTime.Now;
            _context.Update(attendance);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Successfully checked in!";
            return RedirectToAction(nameof(MyEvents));
        }

        // POST: VolunteerPortal/PerformCheckOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerformCheckOut(int id)
        {
            var attendance = await _context.VolAttendances.FindAsync(id);
            
            if (attendance == null)
            {
                return NotFound();
            }
            
            attendance.ActualEnd = DateTime.Now;
            _context.Update(attendance);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Successfully checked out!";
            return RedirectToAction(nameof(MyEvents));
        }

        // POST: VolunteerPortal/SignUpForShift
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpForShift(int scheduleId)
        {
            // Get volunteer ID (normally from auth)
            var volunteerId = TempData["VolunteerId"] as int? ?? 1;
            TempData["VolunteerId"] = volunteerId; // Persist between requests
            
            // Find the schedule
            var schedule = await _context.VolSchedules.FindAsync(scheduleId);
            if (schedule == null)
            {
                return NotFound();
            }
            
            // Check if already registered
            var existingAttendance = await _context.VolAttendances
                .FirstOrDefaultAsync(a => a.VolScheduleID == scheduleId && a.VolunteerID == volunteerId);
            
            if (existingAttendance != null)
            {
                // Update existing attendance
                existingAttendance.Status = true;
                _context.Update(existingAttendance);
            }
            else
            {
                // Create new attendance
                var newAttendance = new VolAttendance
                {
                    VolunteerID = volunteerId,
                    VolScheduleID = scheduleId,
                    Status = true
                };
                _context.VolAttendances.Add(newAttendance);
            }
            
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Successfully signed up for the event!";
            
            return RedirectToAction(nameof(MyEvents));
        }

        // POST: VolunteerPortal/CancelShift
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelShift(int attendanceId)
        {
            var attendance = await _context.VolAttendances.FindAsync(attendanceId);
            
            if (attendance == null)
            {
                return NotFound();
            }
            
            // Only allow cancellation if the shift hasn't started
            var schedule = await _context.VolSchedules.FindAsync(attendance.VolScheduleID);
            if (schedule != null && schedule.ScheduledStart <= DateTime.Now.AddHours(1))
            {
                TempData["ErrorMessage"] = "Cannot cancel shifts that start within an hour.";
                return RedirectToAction(nameof(MyEvents));
            }
            
            // Set status to false instead of deleting
            attendance.Status = false;
            _context.Update(attendance);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Your shift has been cancelled.";
            return RedirectToAction(nameof(MyEvents));
        }

        // For development/demo only: Switch between volunteers
        public async Task<IActionResult> SwitchVolunteer(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }
            
            TempData["VolunteerId"] = id;
            TempData["SuccessMessage"] = $"Switched to volunteer: {volunteer.FullName}";
            return RedirectToAction(nameof(AvailableEvents));
        }
    }
}
