using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;
using TomorrowsVoices.Utilities;
using TomorrowsVoices.ViewModels;



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
        public async Task<IActionResult> Index(string? SearchString, string? SearchCity, DateTime StartDate, DateTime EndDate, int? page, int? pageSizeID, string? actionButton, bool archived = false, string sortDirection = "asc", string sortField = "Event")
        {
            //string[] sortOptions = new[] { "Title", "City", "Date", "StartTime", "EndTime" };
            //int numberFilters = 0;



            //if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            //{
            //    page = 1;//Reset page to start

            //    if (sortOptions.Contains(actionButton))
            //    {
            //        if (actionButton == sortField) //Reverse order on same field
            //        {
            //            sortDirection = sortDirection == "asc" ? "desc" : "asc";
            //        }
            //        sortField = actionButton; //Sort by the button clicked
            //    }
            //}


            //Always Filter by date range
            //If first time loading the page, set the date range filter based on the values in the database
            //if (EndDate == DateTime.MinValue)
            //{
            //    StartDate = _context.Events.Min(o => o.Date);
            //    EndDate = _context.Events.Max(o => o.Date);
            //}
            ////Check the order of the dates and swap them if required
            //if (EndDate < StartDate)
            //{
            //    DateOnly temp = EndDate;
            //    EndDate = StartDate;
            //    StartDate = temp;
            //}
            //Save to View Data
            //ViewData["StartDate"] = StartDate.ToString("yyyy-MM-dd");
            //ViewData["EndDate"] = EndDate.ToString("yyyy-MM-dd");

            //var @events = _context.Events
            //    .Include(e => e.VolLocation)
            //   .Include(e => e.VolSchedules).ThenInclude(v => v.Volunteer)
            //   .Where(a => a.Date >= StartDate && a.Date <= EndDate.AddDays(1))
            //   .Where(s => s.IsArchived == archived)
            //  .AsNoTracking();

            //ViewData["IsArchived"] = archived;
            //ViewData["ActiveTab"] = archived ? "archived" : "active";


            //if (!String.IsNullOrEmpty(SearchString))
            //{
            //    @events = @events.Where(p => p.Name != null && p.Name.ToLower().Contains(SearchString.ToLower())
            //                                 );

            //    numberFilters++;
            //}



            //if (!string.IsNullOrEmpty(SearchCity))
            //{


            //    @events = @events
            // .Where(p => p.VolLocation.City != null && p.VolLocation.City == SearchCity);
            //    numberFilters++;
            //}



            //if (sortField == "Title")
            //{
            //    if (sortDirection == "asc")
            //    {
            //        @events = @events
            //                                .OrderBy(p => p.Name);
            //    }
            //    else
            //    {
            //        @events = @events
            //            .OrderByDescending(p => p.Name);
            //    }
            //}

            //else if (sortField == "StartTime")
            //{
            //    if (sortDirection == "asc")
            //    {
            //        @events = @events
            //                                .OrderBy(p => p.StartTime);
            //    }
            //    else
            //    {
            //        @events = @events
            //            .OrderByDescending(p => p.StartTime);
            //    }
            //}
            //else if (sortField == "EndTime")
            //{
            //    if (sortDirection == "asc")
            //    {
            //        @events = @events
            //                                .OrderBy(p => p.EndTime);
            //    }
            //    else
            //    {
            //        @events = @events
            //            .OrderByDescending(p => p.EndTime);
            //    }
            //}

            //else if (sortField == "Date")
            //{
            //    if (sortDirection == "asc")
            //    {
            //        @events = @events
            //            .OrderBy(p => p.Date);
            //    }
            //    else
            //    {
            //        @events = @events
            //            .OrderByDescending(p => p.Date);
            //    }
            //}
            //else if (sortField == "City")
            //{
            //    if (sortDirection == "asc")
            //    {
            //        @events = @events
            //            .OrderBy(p => p.VolLocation.City);

            //    }
            //    else
            //    {
            //        @events = @events
            //            .OrderByDescending(p => p.VolLocation.City);

            //    }
            //}

            //ViewData["sortField"] = sortField;
            //ViewData["sortDirection"] = sortDirection;
            //ViewData["numberFilters"] = numberFilters;


            //ViewData["IsArchived"] = archived;
            //ViewData["ActiveTab"] = archived ? "archived" : "active";
            //int archivedCount = await _context.Events.CountAsync(d => d.IsArchived == true);
            //ViewData["numberofArchive"] = archivedCount;
            //int activeCount = await _context.Events.CountAsync(d => d.IsArchived == false);
            //ViewData["numberofActive"] = activeCount;

            //        var cityList = events.AsEnumerable()
            //.Select(v => v.VolLocation?.City.ToString())
            //.Where(city => city != null)
            //.Distinct()
            //.Select(city => new SelectListItem
            //{
            //    Value = city,
            //    Text = city
            //})
            //.ToList();

            //cityList.Insert(0, new SelectListItem { Value = "", Text = "All Cities" });

            //ViewData["Cities"] = cityList;

            //// Handle Paging
            //int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            //ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            //var pagedData = await PaginatedList<Event>.CreateAsync(events.AsNoTracking(), page ?? 1, pageSize);

            //return View(pagedData);



            var tomorrowsVoicesContext = _context.Events.Where(d => d.IsArchived == archived).Include(a => a.VolLocation);
            ViewData["IsArchived"] = archived;
            ViewData["ActiveTab"] = archived ? "archived" : "active";
            int archivedCount = await _context.Events.CountAsync(d => d.IsArchived == true);
            ViewData["numberofArchive"] = archivedCount;
            int activeCount = await _context.Events.CountAsync(d => d.IsArchived == false);
            ViewData["numberofActive"] = activeCount;

            // Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Event>.CreateAsync(tomorrowsVoicesContext.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Event/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var @event = await _context.Events
            //    .Include(e => e.VolLocation)
            //    .Include(e => e.Schedules)
            //        .ThenInclude(a => a.Volunteer)
            //    .FirstOrDefaultAsync(m => m.ID == id);

            //if (@event == null)
            //{
            //    return NotFound();
            //}


            //return View(@event);

            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(Index => Index.VolLocation)
                .Include(Index => Index.VolSchedules).ThenInclude(Index => Index.VolAttendances).ThenInclude(Index => Index.Volunteer)
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
            var model = new EventCreateVM
            {
                Event = new Event(),
                NewSchedule = new ScheduleVM(),
                ExistingSchedules = new List<ScheduleVM>()
            };
            PopulateAssignedVolunteerData(model.Event);

            // Load locations for dropdown
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", model.Event.VolLocationID);
            return View(model);
        }
        // Update your Create POST method to handle volunteer assignments
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateVM model, string[] selectedOptions)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    // Begin transaction
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {

                        try
                        {

                            // Save Event
                            _context.Events.Add(model.Event);
                            await _context.SaveChangesAsync();

                            // Get the newly created event ID
                            int eventId = model.Event.ID;

                            // Save Schedules and Volunteer Assignments
                            if (model.ExistingSchedules != null && model.ExistingSchedules.Any())
                            {
                                foreach (var scheduleVM in model.ExistingSchedules)
                                {
                                    VolSchedule schedule;

                                    if (scheduleVM.ScheduleID.HasValue && scheduleVM.ScheduleID.Value > 0)
                                    {
                                        // Existing schedule - update it
                                        schedule = await _context.VolSchedules.FindAsync(scheduleVM.ScheduleID.Value);
                                        if (schedule != null)
                                        {
                                            schedule.ScheduledStart = scheduleVM.ScheduledStart;
                                            schedule.ScheduledEnd = scheduleVM.ScheduledEnd;

                                            // Remove existing attendance records
                                            var existingAttendances = _context.VolAttendances.Where(a => a.VolScheduleID == schedule.ID);
                                            _context.VolAttendances.RemoveRange(existingAttendances);
                                        }
                                        else
                                        {
                                            // Schedule ID provided but not found - create new
                                            schedule = new VolSchedule
                                            {
                                                ScheduledStart = scheduleVM.ScheduledStart,
                                                ScheduledEnd = scheduleVM.ScheduledEnd,
                                                EventID = eventId
                                            };
                                            _context.VolSchedules.Add(schedule);
                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                    else
                                    {
                                        // New schedule - create it
                                        schedule = new VolSchedule
                                        {
                                            ScheduledStart = scheduleVM.ScheduledStart,
                                            ScheduledEnd = scheduleVM.ScheduledEnd,
                                            EventID = eventId
                                        };
                                        _context.VolSchedules.Add(schedule);
                                        await _context.SaveChangesAsync();
                                    }

                                    // Add volunteer attendance records
                                    if (scheduleVM.VolunteerIds != null && scheduleVM.VolunteerIds.Any())
                                    {
                                        foreach (var volunteerId in scheduleVM.VolunteerIds)
                                        {
                                            var attendance = new VolAttendance
                                            {
                                                VolunteerID = volunteerId,
                                                VolScheduleID = schedule.ID,
                                                Status = true,
                                                ActualStart = schedule.ScheduledStart,
                                                ActualEnd = schedule.ScheduledEnd
                                            };

                                            _context.VolAttendances.Add(attendance);
                                        }
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }

                            // Commit transaction
                            await transaction.CommitAsync();

                            return RedirectToAction(nameof(Index));
                        }
                        catch (Exception)
                        {
                            // Rollback transaction
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error
                    ModelState.AddModelError("", "An error occurred while creating the event: " + ex.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", model.Event.VolLocationID);
            return View(model);
        }

        // Populate assigned 
        private void PopulateAssignedVolunteerData(Event @event)
        {
            var allOptions = _context.Volunteers.Where(v => v.IsArchived == false);
            var currentOptionsHS = new HashSet<int>(
        @event.VolSchedules
            .SelectMany(s => s.VolAttendances)  // Flatten VolAttendances from all VolSchedules
            .Where(a => !a.IsArchived)          // Filter non-archived attendances
            .Select(a => a.VolunteerID)
             );

            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();

            foreach (var s in allOptions)
            {
                string displayText = $"{s.FullName}";

                if (currentOptionsHS.Contains(s.ID))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = displayText
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = displayText
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }


        private void UpdateSessionVolunteers(string[] selectedOptions, VolSchedule scheduleToUpdate)
        {
            var allVolunteerIDs = _context.Volunteers.Where(v => v.IsArchived == false).Select(s => s.ID).ToHashSet(); // Get all singers
            var selectedOptionsHS = new HashSet<int>(selectedOptions.Select(int.Parse));

            // Get all current attendance records for this session
            var currentAttendance = scheduleToUpdate.VolAttendances.ToList();

            foreach (var volunteerID in allVolunteerIDs)
            {
                var existingAttendance = currentAttendance.FirstOrDefault(a => a.VolunteerID == volunteerID);

                if (selectedOptionsHS.Contains(volunteerID))
                {
                    if (existingAttendance == null) // If not already in attendance, add it with Status = true
                    {
                        var schedule = scheduleToUpdate; // Assuming you want to add to the first schedule
                        if (schedule != null)
                        {
                            schedule.VolAttendances.Add(new VolAttendance
                            {
                                VolunteerID = volunteerID,
                                VolScheduleID = schedule.ID,
                                Status = true,
                                ActualStart = schedule.ScheduledStart, // Default to scheduled times
                                ActualEnd = schedule.ScheduledEnd
                            });
                        }
                    }
                    else // If already exists, ensure Status is true
                    {
                        existingAttendance.Status = true;
                    }
                }
                else // Singer was NOT selected
                {
                    if (existingAttendance != null) // If already exists, set Status = false
                    {
                        existingAttendance.Status = false;
                    }
                    else // If not in attendance, add it with Status = false
                    {
                        var schedule = scheduleToUpdate; // Assuming you want to add to the first schedule
                        if (schedule != null)
                        {
                            schedule.VolAttendances.Add(new VolAttendance
                            {
                                VolunteerID = volunteerID,
                                VolScheduleID = schedule.ID,
                                Status = false,
                                ActualStart = null,
                                ActualEnd = null
                            });
                        }
                    }
                }
            }
        }



        // Optional: AJAX method for volunteer availability
        [HttpGet]
        public IActionResult GetAvailableVolunteers(DateTime start, DateTime end)
        {
            // Query available volunteers (those not already scheduled during this time)
            var scheduledVolunteers = _context.VolAttendances
                .Where(a =>
                    (a.ActualStart <= end && a.ActualEnd >= start) &&
                    a.Status == true)
                .Select(a => a.VolunteerID)
                .Distinct();

            var availableVolunteers = _context.Volunteers
                .Where(v => !scheduledVolunteers.Contains(v.ID))
                .Select(v => new { id = v.ID, name = v.FirstName + " " + v.LastName })
                .ToList();

            return Json(availableVolunteers);
        }
        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.VolLocation)
                .Include(e => e.VolSchedules)
                    .ThenInclude(s => s.VolAttendances)
                        .ThenInclude(a => a.Volunteer)
                .FirstOrDefaultAsync(e => e.ID == id);

            if (@event == null)
            {
                return NotFound();
            }

            var model = new EventCreateVM
            {
                Event = @event,
                NewSchedule = new ScheduleVM(),
                ExistingSchedules = new List<ScheduleVM>()
            };

            // Convert existing schedules to ScheduleVM objects
            foreach (var schedule in @event.VolSchedules)
            {
                var scheduleVM = new ScheduleVM
                {
                    ScheduleID = schedule.ID,
                    ScheduledStart = schedule.ScheduledStart,
                    ScheduledEnd = schedule.ScheduledEnd,
                    VolunteerIds = schedule.VolAttendances
                        .Where(a => a.Status)
                        .Select(a => a.VolunteerID)
                        .ToList()
                };
                model.ExistingSchedules.Add(scheduleVM);
            }

            PopulateAssignedVolunteerData(@event);

            // Load locations for dropdown
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", @event.VolLocationID);
            return View(model);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventCreateVM model, string[] selectedOptions)
        {
            if (id != model.Event.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Begin transaction
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Get the existing event with all related data
                            var eventToUpdate = await _context.Events
                                .Include(e => e.VolSchedules)
                                    .ThenInclude(s => s.VolAttendances)
                                .FirstOrDefaultAsync(e => e.ID == id);

                            if (eventToUpdate == null)
                            {
                                return NotFound();
                            }

                            // Update event basic properties
                            eventToUpdate.Name = model.Event.Name;
                            eventToUpdate.Location = model.Event.Location;
                            eventToUpdate.Notes = model.Event.Notes;
                            eventToUpdate.Start = model.Event.Start;
                            eventToUpdate.End = model.Event.End;
                            eventToUpdate.VolLocationID = model.Event.VolLocationID;

                            // Update existing schedules and add new ones
                            if (model.ExistingSchedules != null && model.ExistingSchedules.Any())
                            {
                                foreach (var scheduleVM in model.ExistingSchedules)
                                {
                                    VolSchedule schedule;

                                    if (scheduleVM.ScheduleID.HasValue && scheduleVM.ScheduleID.Value > 0)
                                    {
                                        // Existing schedule - update it
                                        schedule = await _context.VolSchedules.FindAsync(scheduleVM.ScheduleID.Value);
                                        if (schedule != null)
                                        {
                                            schedule.ScheduledStart = scheduleVM.ScheduledStart;
                                            schedule.ScheduledEnd = scheduleVM.ScheduledEnd;

                                            // Remove existing attendance records
                                            var existingAttendances = _context.VolAttendances.Where(a => a.VolScheduleID == schedule.ID);
                                            _context.VolAttendances.RemoveRange(existingAttendances);
                                        }
                                        else
                                        {
                                            // Schedule ID provided but not found - create new
                                            schedule = new VolSchedule
                                            {
                                                ScheduledStart = scheduleVM.ScheduledStart,
                                                ScheduledEnd = scheduleVM.ScheduledEnd,
                                                EventID = id
                                            };
                                            _context.VolSchedules.Add(schedule);
                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                    else
                                    {
                                        // New schedule - create it
                                        schedule = new VolSchedule
                                        {
                                            ScheduledStart = scheduleVM.ScheduledStart,
                                            ScheduledEnd = scheduleVM.ScheduledEnd,
                                            EventID = id
                                        };
                                        _context.VolSchedules.Add(schedule);
                                        await _context.SaveChangesAsync();
                                    }

                                    // Add volunteer attendance records
                                    if (scheduleVM.VolunteerIds != null && scheduleVM.VolunteerIds.Any())
                                    {
                                        foreach (var volunteerId in scheduleVM.VolunteerIds)
                                        {
                                            var attendance = new VolAttendance
                                            {
                                                VolunteerID = volunteerId,
                                                VolScheduleID = schedule.ID,
                                                Status = true,
                                                ActualStart = schedule.ScheduledStart,
                                                ActualEnd = schedule.ScheduledEnd
                                            };

                                            _context.VolAttendances.Add(attendance);
                                        }
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }

                            // Handle schedules that were removed (in the current model but not in the submitted model)
                            var currentScheduleIds = eventToUpdate.VolSchedules.Select(s => s.ID).ToList();
                            var submittedScheduleIds = model.ExistingSchedules
                                .Where(s => s.ScheduleID.HasValue)
                                .Select(s => s.ScheduleID.Value)
                                .ToList();

                            var schedulesToRemove = currentScheduleIds.Except(submittedScheduleIds).ToList();

                            foreach (var scheduleId in schedulesToRemove)
                            {
                                var scheduleToRemove = await _context.VolSchedules
                                    .Include(s => s.VolAttendances)
                                    .FirstOrDefaultAsync(s => s.ID == scheduleId);

                                if (scheduleToRemove != null)
                                {
                                    // Remove associated attendances first
                                    _context.VolAttendances.RemoveRange(scheduleToRemove.VolAttendances);

                                    // Then remove the schedule
                                    _context.VolSchedules.Remove(scheduleToRemove);
                                }
                            }

                            await _context.SaveChangesAsync();

                            // Update the event entity
                            _context.Update(eventToUpdate);
                            await _context.SaveChangesAsync();

                            // Commit transaction
                            await transaction.CommitAsync();

                            return RedirectToAction(nameof(Index));
                        }
                        catch (Exception ex)
                        {
                            // Rollback transaction
                            await transaction.RollbackAsync();

                            // Add more detailed error information
                            ModelState.AddModelError("", "An error occurred while updating the event: " + ex.Message);
                            if (ex.InnerException != null)
                            {
                                ModelState.AddModelError("", "Details: " + ex.InnerException.Message);
                            }

                            throw;
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(model.Event.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    // Log error
                    ModelState.AddModelError("", "An error occurred while updating the event: " + ex.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", model.Event.VolLocationID);
            PopulateAssignedVolunteerData(model.Event);
            return View(model);
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateMultiple(List<Schedule> schedules)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Schedules.AddRange(schedules); // Add multiple schedules
        //            await _context.SaveChangesAsync();
        //            return Json(new { success = true });
        //        }
        //        catch (Exception ex)
        //        {
        //            // Log the exception (optional)
        //            return Json(new { success = false, message = ex.Message });
        //        }
        //    }

        //    // If the model state is invalid, return validation errors
        //    var errors = ModelState.Values
        //        .SelectMany(v => v.Errors)
        //        .Select(e => e.ErrorMessage)
        //        .ToList();
        //    return Json(new { success = false, message = "Validation errors: " + string.Join(", ", errors) });
        //}


        ////ImportExcel 
        //[HttpPost]
        //public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        //{
        //    var response = new { success = false, message = "" };

        //    if (theExcel == null || theExcel.Length == 0)
        //    {
        //        response = new { success = false, message = "❌ No file uploaded. Please select an Excel file." };
        //        return Json(response);
        //    }

        //    string feedbackMessage = "";
        //    int successCount = 0, errorCount = 0;

        //    try
        //    {
        //        string mimeType = theExcel.ContentType;
        //        if (!mimeType.Contains("excel") && !mimeType.Contains("spreadsheet"))
        //        {
        //            response = new { success = false, message = "⚠️ Invalid file format. Please upload a valid Excel file." };
        //            return Json(response);
        //        }

        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await theExcel.CopyToAsync(memoryStream);
        //            using (var package = new ExcelPackage(memoryStream))
        //            {
        //                var workSheet = package.Workbook.Worksheets[0];
        //                var start = workSheet.Dimension.Start;
        //                var end = workSheet.Dimension.End;

        //                // Validate headers
        //                if (workSheet.Cells[1, 1].Text != "Name" ||
        //                    workSheet.Cells[1, 2].Text != "Location" ||
        //                    workSheet.Cells[1, 3].Text != "Date" ||
        //                    workSheet.Cells[1, 4].Text != "Start Time" ||
        //                    workSheet.Cells[1, 5].Text != "End Time")
        //                {
        //                    response = new { success = false, message = "❌ Invalid Excel format. Please ensure the file has 'Name', 'Location', 'Date', 'Start Time', and 'End Time' headers." };
        //                    return Json(response);
        //                }

        //                for (int row = start.Row + 1; row <= end.Row; row++)
        //                {
        //                    Event events = new Event();
        //                    try
        //                    {
        //                        events.Name = workSheet.Cells[row, 1].Text.Trim();
        //                        string cityName = workSheet.Cells[row, 2].Text.Trim();

        //                        // Parse Date
        //                        if (DateOnly.TryParse(workSheet.Cells[row, 3].Text.Trim(), out DateOnly date))
        //                        {
        //                            events.Date = date;
        //                        }
        //                        else
        //                        {
        //                            errorCount++;
        //                            feedbackMessage += $"⚠️ Error: Invalid date format in row {row}.<br>";
        //                            continue;
        //                        }

        //                        // Parse Start Time
        //                        if (TimeOnly.TryParse(workSheet.Cells[row, 4].Text.Trim(), out TimeOnly startTime))
        //                        {
        //                            events.StartTime = startTime;
        //                        }
        //                        else
        //                        {
        //                            errorCount++;
        //                            feedbackMessage += $"⚠️ Error: Invalid start time format in row {row}.<br>";
        //                            continue;
        //                        }

        //                        // Parse End Time
        //                        if (TimeOnly.TryParse(workSheet.Cells[row, 5].Text.Trim(), out TimeOnly endTime))
        //                        {
        //                            events.EndTime = endTime;
        //                        }
        //                        else
        //                        {
        //                            errorCount++;
        //                            feedbackMessage += $"⚠️ Error: Invalid end time format in row {row}.<br>";
        //                            continue;
        //                        }

        //                        // Validate data before adding
        //                        if (string.IsNullOrEmpty(events.Name) ||
        //                            string.IsNullOrEmpty(cityName))
        //                        {
        //                            errorCount++;
        //                            feedbackMessage += $"⚠️ Error: Row {row} has missing fields.<br>";
        //                            continue; // Skip invalid row
        //                        }

        //                        // Check if event with the same name, date, and location already exists
        //                        var location = _context.VolLocations.FirstOrDefault(l => l.City == cityName);

        //                        if (location == null)
        //                        {
        //                            // If location doesn't exist, create a new one
        //                            location = new VolLocation { City = cityName };
        //                            _context.VolLocations.Add(location);
        //                            await _context.SaveChangesAsync(); // Save the new location to get its ID
        //                        }

        //                        if (_context.Events.Any(e => e.Name == events.Name && e.Date == events.Date && e.VolLocationID == location.ID))
        //                        {
        //                            errorCount++;
        //                            feedbackMessage += $"⚠️ Error: The event {events.Name} on {events.Date.ToShortDateString()} at {cityName} already exists.<br>";
        //                            continue;
        //                        }

        //                        events.VolLocationID = location.ID;
        //                        _context.Events.Add(events);
        //                        successCount++;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        errorCount++;
        //                        feedbackMessage += $"⚠️ Error: Exception in row {row} - {ex.Message}<br>";
        //                    }
        //                }

        //                // Save changes to the database
        //                await _context.SaveChangesAsync();

        //                // Prepare response
        //                if (successCount > 0)
        //                {
        //                    response = new { success = true, message = $"✅ {successCount} events added successfully.<br>{feedbackMessage}" };
        //                }
        //                else
        //                {
        //                    response = new { success = false, message = $"❌ No events were added.<br>{feedbackMessage}" };
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new { success = false, message = $"❌ An error occurred: {ex.Message}" };
        //    }

        //    return Json(response);
        //}



        // GET: Event/ExportEventsToExcel
        [HttpGet]
        public async Task<IActionResult> ExportEventsToExcel()
        {
            var events = await _context.Events
                .OrderBy(e => e.Name)
                .Include(e => e.VolLocation)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Events");

                // Add headers
                worksheet.Cells[1, 1].Value = "Event Name";
                worksheet.Cells[1, 2].Value = "Address";
                worksheet.Cells[1, 3].Value = "Date";
                worksheet.Cells[1, 4].Value = "Start Time";
                worksheet.Cells[1, 5].Value = "End Time";
                worksheet.Cells[1, 6].Value = "Location";
                worksheet.Cells[1, 7].Value = "Notes";

                // Make headers bold
                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                }

                int row = 2;
                foreach (var eventItem in events)
                {
                    worksheet.Cells[row, 1].Value = eventItem.Name;
                    worksheet.Cells[row, 2].Value = eventItem.Location;
               
                    worksheet.Cells[row, 4].Value = eventItem.Start.ToString("hh\\:mm tt");
                    worksheet.Cells[row, 5].Value = eventItem.End.ToString("hh\\:mm tt");
                    worksheet.Cells[row, 6].Value = eventItem.VolLocation?.City;
                    worksheet.Cells[row, 7].Value = eventItem.Notes;
                    row++;
                }

                // Auto-fit columns for better readability
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"Event_Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }








        // Excel Template Server
        public IActionResult DownloadSampleExcel()
        {
            // Path to the sample Excel file in your project
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExcelTemplates", "EventTemplate.xlsx");

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Serve the file for download
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EventTemplate.xlsx");
        }

        // Calendar Fetching Action Method
        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents()
        {
            var events = await _context.Events
                .Where(e => !e.IsArchived) // Exclude archived events
                .Select(e => new
                {
                    id = e.ID,
                    title = e.Name,
                    description = e.Location,
                    location = e.VolLocation.City // Include location if needed
                })
                .ToListAsync();

            return Json(events);
        }

        // Get Event Details for Pop up
        [HttpGet]
        public async Task<IActionResult> GetEventDetails(int id)
        {
            var @event = await _context.Events
                .Include(e => e.VolLocation) // Include location details
                .FirstOrDefaultAsync(e => e.ID == id);

            if (@event == null)
            {
                return NotFound();
            }

            // Return event details as JSON
            return Json(new
            {
                id = @event.ID,
                name = @event.Name,
                description = @event.Location,
               
                startTime = @event.Start.ToString(),
                endTime = @event.End.ToString(),
                location = @event.VolLocation?.City // Include location name
            });
        }

        // For Calender View
        public IActionResult Calendar()
        {
            return View();
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.ID == id);
        }


  
    }


}
