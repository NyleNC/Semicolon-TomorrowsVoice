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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace TomorrowsVoices.Controllers
{
    public class VolunteerPortalController : Controller
    {
        private readonly TomorrowsVoicesContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public VolunteerPortalController(TomorrowsVoicesContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Helper method to get the current volunteer based on logged-in user's email
        private async Task<Volunteer> GetCurrentVolunteerAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return null;
            }

            // If the user is an admin, they don't need to be restricted
            if (User.IsInRole("Admin"))
            {
                // For admins, we'll still try to find their volunteer record if it exists
                var adminVolunteer = await _context.Volunteers
                    .FirstOrDefaultAsync(v => v.Email == user.Email);

                return adminVolunteer;
            }

            // For regular volunteers, find their record
            return await _context.Volunteers
                .FirstOrDefaultAsync(v => v.Email == user.Email);
        }

        // GET: VolunteerPortal/AvailableEvents
        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> AvailableEvents()
        {
            var currentVolunteer = await GetCurrentVolunteerAsync();

            // If volunteer not found and not an admin, redirect to an error page
            if (currentVolunteer == null && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Your account is not linked to a volunteer profile. Please contact an administrator.";
                return RedirectToAction("Index", "Home");
            }

            int volunteerId = currentVolunteer?.ID ?? 0;

            var events = await _context.Events
                .Include(e => e.VolLocation)
                .Include(e => e.VolSchedules)
                    .ThenInclude(s => s.VolAttendances)
                .Where(e => e.End > DateTime.Now && !e.IsArchived)
                .OrderBy(e => e.Start)
                .ToListAsync();

            var eventCards = events.Select(e => new EventCardViewModel
            {
                EventId = e.ID,
                Title = e.Name,
                Address = e.Address,
                //City = e.VolLocation?.City,
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

            ViewData["VolunteerName"] = currentVolunteer?.FullName ?? User.Identity.Name;
            ViewData["VolunteerId"] = volunteerId;

            return View("~/Views/VolPortal/AvailableEvents.cshtml", eventCards);
        }

        // GET: VolunteerPortal/MyEvents
        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> MyEvents()
        {
            var currentVolunteer = await GetCurrentVolunteerAsync();

            // If volunteer not found and not an admin, redirect to an error page
            if (currentVolunteer == null && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Your account is not linked to a volunteer profile. Please contact an administrator.";
                return RedirectToAction("Index", "Home");
            }

            int volunteerId = currentVolunteer?.ID ?? 0;

            var myAttendances = await _context.VolAttendances
                .Include(a => a.VolSchedule)
                    .ThenInclude(s => s.Event)
                        .ThenInclude(e => e.VolLocation)
                .Include(a => a.Volunteer)
                .Where(a => a.VolunteerID == volunteerId && a.Status)
                .ToListAsync();

            var myEvents = myAttendances
                .GroupBy(a => a.VolSchedule.Event)
                .Select(g => new MyEventViewModel
                {
                    EventId = g.Key.ID,
                    Title = g.Key.Name,
                    Address = g.Key.Address,
                    //City = Key.VolLocation?.City,
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

            ViewData["VolunteerName"] = currentVolunteer?.FullName ?? User.Identity.Name;

            ViewData["UpcomingEvents"] = myEvents.Where(e => e.EventStart > DateTime.Now).ToList();
            ViewData["PastEvents"] = myEvents.Where(e => e.EventEnd < DateTime.Now).ToList();
            ViewData["CurrentEvents"] = myEvents.Where(e => e.EventStart <= DateTime.Now && e.EventEnd >= DateTime.Now).ToList();

            return View("~/Views/VolPortal/MyEvents.cshtml", myEvents);
        }

        // GET: VolunteerPortal/CheckIn/5
        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> CheckIn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentVolunteer = await GetCurrentVolunteerAsync();

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

            // Only allow volunteers to see their own attendances (unless they're an admin)
            if (!User.IsInRole("Admin") && attendance.VolunteerID != currentVolunteer?.ID)
            {
                return Forbid();
            }

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

            return View("~/Views/VolPortal/CheckIn.cshtml", checkInViewModel);
        }

        // POST: VolunteerPortal/PerformCheckIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> PerformCheckIn(int id)
        {
            var attendance = await _context.VolAttendances.FindAsync(id);

            if (attendance == null)
            {
                return NotFound();
            }

            var currentVolunteer = await GetCurrentVolunteerAsync();

            // Only allow volunteers to check in for themselves (unless they're an admin)
            if (!User.IsInRole("Admin") && attendance.VolunteerID != currentVolunteer?.ID)
            {
                return Forbid();
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
        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> PerformCheckOut(int id)
        {
            var attendance = await _context.VolAttendances.FindAsync(id);

            if (attendance == null)
            {
                return NotFound();
            }

            var currentVolunteer = await GetCurrentVolunteerAsync();

            // Only allow volunteers to check out for themselves (unless they're an admin)
            if (!User.IsInRole("Admin") && attendance.VolunteerID != currentVolunteer?.ID)
            {
                return Forbid();
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
        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> SignUpForShift(int scheduleId)
        {
            var currentVolunteer = await GetCurrentVolunteerAsync();

            if (currentVolunteer == null && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Your account is not linked to a volunteer profile. Please contact an administrator.";
                return RedirectToAction("Index", "Home");
            }

            int volunteerId = currentVolunteer?.ID ?? 0;

            var schedule = await _context.VolSchedules.FindAsync(scheduleId);
            if (schedule == null)
            {
                return NotFound();
            }

            var existingAttendance = await _context.VolAttendances
                .FirstOrDefaultAsync(a => a.VolScheduleID == scheduleId && a.VolunteerID == volunteerId);

            if (existingAttendance != null)
            {
                existingAttendance.Status = true;
                _context.Update(existingAttendance);
            }
            else
            {
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
        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> CancelShift(int attendanceId)
        {
            var attendance = await _context.VolAttendances
                .Include(a => a.Volunteer)
                .FirstOrDefaultAsync(a => a.ID == attendanceId);

            if (attendance == null)
            {
                return NotFound();
            }

            var currentVolunteer = await GetCurrentVolunteerAsync();

            // Only allow volunteers to cancel their own shifts (unless they're an admin)
            if (!User.IsInRole("Admin") && attendance.VolunteerID != currentVolunteer?.ID)
            {
                return Forbid();
            }

            var schedule = await _context.VolSchedules.FindAsync(attendance.VolScheduleID);
            if (schedule != null && schedule.ScheduledStart <= DateTime.Now.AddHours(1))
            {
                TempData["ErrorMessage"] = "Cannot cancel shifts that start within an hour.";
                return RedirectToAction(nameof(MyEvents));
            }

            attendance.Status = false;
            _context.Update(attendance);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your shift has been cancelled.";
            return RedirectToAction(nameof(MyEvents));
        }

        // For development/demo only: Switch between volunteers (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SwitchVolunteer(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = $"Admin view: Acting as volunteer {volunteer.FullName}";
            return RedirectToAction(nameof(AvailableEvents));
        }
    }
}