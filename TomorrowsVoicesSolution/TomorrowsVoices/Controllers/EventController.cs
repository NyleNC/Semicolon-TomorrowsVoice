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
using OfficeOpenXml.Style;
using System.Drawing;
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
        public async Task<IActionResult> Index(string? SearchString, string? SearchCity, int? page, int? pageSizeID, string? actionButton, bool archived = false, string sortDirection = "asc", string sortField = "Event")
        {
            string[] sortOptions = new[] { "Title", "City", "StartTime", "EndTime" };
            int numberFilters = 0;

            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1; //Reset page to start

                if (sortOptions.Contains(actionButton))
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; //Sort by the button clicked
                }
            }

            // Initialize the base query
            var eventsQuery = _context.Events
                .Include(e => e.VolLocation)
                .Where(s => s.IsArchived == archived)
                .AsNoTracking();

            // Default dates if not provided
            //if (!StartDate.HasValue || !EndDate.HasValue)
            //{
            //    StartDate = _context.Events.Min(o => o.Start.Date);
            //    EndDate = _context.Events.Max(o => o.End.Date);
            //}
            //else
            //{
            //    // Only count dates as filters if they're not default values
            //    // Check if StartDate is from a user filter (not default)
            //    if (StartDate.HasValue && StartDate != _context.Events.Min(o => o.Start.Date))
            //    {
            //        numberFilters++;
            //    }

            //    // Check if EndDate is from a user filter (not default)
            //    if (EndDate.HasValue && EndDate != _context.Events.Max(o => o.End.Date))
            //    {
            //        numberFilters++;
            //    }
            //}

            //// Check the order of the dates and swap them if required
            //if (EndDate < StartDate)
            //{
            //    DateTime? temp = EndDate;
            //    EndDate = StartDate;
            //    StartDate = temp;
            //}

            //// Save to View Data
            //ViewData["StartDate"] = StartDate?.ToString("yyyy-MM-dd");
            //ViewData["EndDate"] = EndDate?.ToString("yyyy-MM-dd");

            //// Filter by date range (date part only)
            //eventsQuery = eventsQuery.Where(a => a.Start.Date >= StartDate.Value.Date && a.End.Date <= EndDate.Value.Date);

            ViewData["IsArchived"] = archived;
            ViewData["ActiveTab"] = archived ? "archived" : "active";

            if (!String.IsNullOrEmpty(SearchString))
            {
                eventsQuery = eventsQuery.Where(p => p.Name != null && p.Name.ToLower().Contains(SearchString.ToLower()));
                numberFilters++;
            }

            if (!string.IsNullOrEmpty(SearchCity))
            {
                eventsQuery = eventsQuery.Where(p => p.VolLocation.City != null && p.VolLocation.City == SearchCity);
                numberFilters++;
            }

            // Execute the database query before applying time filters
            var events = await eventsQuery.ToListAsync();

            // Now filter by time range if provided (using in-memory filtering)
            //if (StartTime.HasValue || EndTime.HasValue)
            //{
            //    if (StartTime.HasValue)
            //    {
            //        events = events.Where(e => e.Start.TimeOfDay >= StartTime.Value).ToList();
            //        numberFilters++;
            //        ViewData["StartTime"] = StartTime?.ToString(@"hh\:mm");
            //    }

            //    if (EndTime.HasValue)
            //    {
            //        events = events.Where(e => e.End.TimeOfDay <= EndTime.Value).ToList();
            //        numberFilters++;
            //        ViewData["EndTime"] = EndTime?.ToString(@"hh\:mm");
            //    }
            //}

            // Apply sorting to the in-memory collection
            events = ApplySorting(events, sortField, sortDirection).ToList();

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["numberFilters"] = numberFilters;

            int archivedCount = await _context.Events.CountAsync(d => d.IsArchived == true);
            ViewData["numberofArchive"] = archivedCount;
            int activeCount = await _context.Events.CountAsync(d => d.IsArchived == false);
            ViewData["numberofActive"] = activeCount;

            var cityList = eventsQuery.AsEnumerable()
                .Select(v => v.VolLocation?.City.ToString())
                .Where(city => city != null)
                .Distinct()
                .Select(city => new SelectListItem
                {
                    Value = city,
                    Text = city
                })
                .ToList();

            cityList.Insert(0, new SelectListItem { Value = "", Text = "All Cities" });

            ViewData["Cities"] = cityList;

            // Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            // Use PaginatedList with IEnumerable instead of IQueryable
            var pagedData = PaginatedList<Event>.CreateFromList(events, page ?? 1, pageSize);

            return View(pagedData);
        }

        // Helper method to apply sorting to an in-memory collection
        private IEnumerable<Event> ApplySorting(IEnumerable<Event> events, string sortField, string sortDirection)
        {
            switch (sortField)
            {
                case "Title":
                    events = sortDirection == "asc" ? events.OrderBy(p => p.Name) : events.OrderByDescending(p => p.Name);
                    break;
                case "StartTime":
                    events = sortDirection == "asc" ? events.OrderBy(p => p.Start) : events.OrderByDescending(p => p.Start);
                    break;
                case "EndTime":
                    events = sortDirection == "asc" ? events.OrderBy(p => p.End) : events.OrderByDescending(p => p.End);
                    break;
                case "City":
                    events = sortDirection == "asc" ? events.OrderBy(p => p.VolLocation?.City) : events.OrderByDescending(p => p.VolLocation?.City);
                    break;
                default:
                    events = events.OrderBy(p => p.Name);
                    break;
            }
            
            return events;
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
                
              
            };

            // Load locations for dropdown
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City");
            return View(model);
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateVM model)
        {
            // Check if the model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Begin transaction for database operations
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Check if we have multi-date events
                            bool hasMultiDateEvents = model.MultiDateEvents != null && model.MultiDateEvents.Any();

                            // If we have multi-date events, don't create a default event
                            // Otherwise, create the single event as normal
                            if (!hasMultiDateEvents)
                            {
                                // Ensure the model has proper date/time values
                                if (model.Event.Date == default)
                                {
                                    model.Event.Date = DateTime.Today;
                                }

                                // Save the single event
                                _context.Events.Add(model.Event);
                                await _context.SaveChangesAsync();

                                int eventId = model.Event.ID;

                                // Save schedules for the single event
                                if (model.ExistingSchedules != null && model.ExistingSchedules.Any())
                                {
                                    foreach (var scheduleVM in model.ExistingSchedules)
                                    {
                                        var schedule = new VolSchedule
                                        {
                                            ShiftDate = model.Event.Date,
                                            ScheduledStart = scheduleVM.ScheduledStart,
                                            ScheduledEnd = scheduleVM.ScheduledEnd,
                                            Capacity = scheduleVM.Capacity > 0 ? scheduleVM.Capacity : 1,
                                            EventID = eventId
                                        };
                                        _context.VolSchedules.Add(schedule);
                                    }
                                    await _context.SaveChangesAsync();
                                }
                            }

                            // Handle multi-date events
                            if (hasMultiDateEvents)
                            {
                                foreach (var multiDateEvent in model.MultiDateEvents)
                                {
                                    // Skip if date is invalid
                                    if (string.IsNullOrEmpty(multiDateEvent.Date))
                                    {
                                        continue;
                                    }

                                    // Parse the date and times
                                    if (!DateTime.TryParse(multiDateEvent.Date, out DateTime eventDate))
                                    {
                                        continue; // Skip invalid dates
                                    }

                                    DateTime eventStartTime;
                                    DateTime eventEndTime;

                                    try
                                    {
                                        eventStartTime = DateTime.Parse($"{multiDateEvent.Date} {multiDateEvent.StartTime}");
                                        eventEndTime = DateTime.Parse($"{multiDateEvent.Date} {multiDateEvent.EndTime}");
                                    }
                                    catch
                                    {
                                        // If time parsing fails, skip this entry
                                        continue;
                                    }

                                    // Create the event
                                    var multiDateEventModel = new Event
                                    {
                                        Name = model.Event.Name,
                                        Location = model.Event.Location,
                                        Address = model.Event.Address,
                                        VolLocationID = model.Event.VolLocationID,
                                        Notes = model.Event.Notes,
                                        Date = eventDate,
                                        Start = eventStartTime,
                                        End = eventEndTime,
                                        IsArchived = false // Ensure new events are not archived
                                    };

                                    _context.Events.Add(multiDateEventModel);
                                    await _context.SaveChangesAsync();

                                    int multiDateEventId = multiDateEventModel.ID;

                                    // Add shifts for this event if any
                                    if (multiDateEvent.Shifts != null && multiDateEvent.Shifts.Any())
                                    {
                                        foreach (var shift in multiDateEvent.Shifts)
                                        {
                                            try
                                            {
                                                // Parse the shift times
                                                var shiftStartStr = $"{multiDateEvent.Date} {shift.ScheduledStart:HH:mm}";
                                                var shiftEndStr = $"{multiDateEvent.Date} {shift.ScheduledEnd:HH:mm}";

                                                DateTime shiftStart = DateTime.Parse(shiftStartStr);
                                                DateTime shiftEnd = DateTime.Parse(shiftEndStr);

                                                var schedule = new VolSchedule
                                                {
                                                    ShiftDate = eventDate,
                                                    ScheduledStart = shiftStart,
                                                    ScheduledEnd = shiftEnd,
                                                    Capacity = shift.Capacity > 0 ? shift.Capacity : 1,
                                                    EventID = multiDateEventId
                                                };

                                                _context.VolSchedules.Add(schedule);
                                            }
                                            catch (Exception ex)
                                            {
                                                // Log the error but continue with other shifts
                                                Console.WriteLine($"Error adding shift: {ex.Message}");
                                            }
                                        }

                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }

                            // Commit the transaction
                            await transaction.CommitAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        catch (Exception ex)
                        {
                            // Rollback on error
                            await transaction.RollbackAsync();
                            ModelState.AddModelError("", $"Error saving event: {ex.Message}");

                            if (ex.InnerException != null)
                            {
                                ModelState.AddModelError("", $"Details: {ex.InnerException.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while creating the event: " + ex.Message);
                }
            }
            else
            {
                // Log validation errors
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Validation error for {state.Key}: {error.ErrorMessage}");
                    }
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

            var model = new EventEditVM
            {
                Event = @event,
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
                    Capacity = schedule.Capacity
                };
                model.ExistingSchedules.Add(scheduleVM);
            }

            // Load locations for dropdown
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", @event.VolLocationID);
            return View(model);
        }

        // POST: Event/Edit/5
        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventEditVM model)
        {
            if (id != model.Event.ID)
            {
                return NotFound();
            }

            // For debugging
            Console.WriteLine($"Edit POST received. Model.MultiDateEvents count: {model.MultiDateEvents?.Count ?? 0}");
            Console.WriteLine($"ExistingSchedules count: {model.ExistingSchedules?.Count ?? 0}");

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
                            eventToUpdate.Address = model.Event.Address;
                            eventToUpdate.Notes = model.Event.Notes;
                            eventToUpdate.VolLocationID = model.Event.VolLocationID;

                            // Check and log which update path we're taking
                            bool isMultiDateUpdate = model.MultiDateEvents != null && model.MultiDateEvents.Any();
                            Console.WriteLine($"Update path: {(isMultiDateUpdate ? "Multi-date" : "Single-date")}");

                            // Handle multi-date events if present
                            if (isMultiDateUpdate)
                            {
                                // Multi-date handling code (unchanged)
                                // ...existing implementation...

                                // First, update the current event with the first date's information
                                var firstDate = model.MultiDateEvents.First();

                                if (!string.IsNullOrEmpty(firstDate.Date) &&
                                    DateTime.TryParse(firstDate.Date, out DateTime eventDate))
                                {
                                    eventToUpdate.Date = eventDate;

                                    // Parse start and end times
                                    if (!string.IsNullOrEmpty(firstDate.StartTime) &&
                                        !string.IsNullOrEmpty(firstDate.EndTime))
                                    {
                                        try
                                        {
                                            var startDateTime = DateTime.Parse($"{firstDate.Date} {firstDate.StartTime}");
                                            var endDateTime = DateTime.Parse($"{firstDate.Date} {firstDate.EndTime}");
                                            eventToUpdate.Start = startDateTime;
                                            eventToUpdate.End = endDateTime;
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error parsing time: {ex.Message}");
                                        }
                                    }
                                }

                                // Update the current event
                                _context.Update(eventToUpdate);
                                await _context.SaveChangesAsync();

                                // Handle existing schedules for the current event
                                var currentScheduleIds = eventToUpdate.VolSchedules.Select(s => s.ID).ToList();
                                foreach (var scheduleId in currentScheduleIds)
                                {
                                    var scheduleToRemove = await _context.VolSchedules
                                        .Include(s => s.VolAttendances)
                                        .FirstOrDefaultAsync(s => s.ID == scheduleId);

                                    if (scheduleToRemove != null)
                                    {
                                        // Remove associated attendances
                                        _context.VolAttendances.RemoveRange(scheduleToRemove.VolAttendances);
                                        _context.VolSchedules.Remove(scheduleToRemove);
                                    }
                                }
                                await _context.SaveChangesAsync();

                                // Add shifts for the first date (current event)
                                if (firstDate.Shifts != null && firstDate.Shifts.Any())
                                {
                                    Console.WriteLine($"Adding {firstDate.Shifts.Count} shifts for first date");
                                    foreach (var shift in firstDate.Shifts)
                                    {
                                        try
                                        {
                                            // Parse shift times
                                            var shiftStartStr = $"{firstDate.Date} {shift.ScheduledStart}";
                                            var shiftEndStr = $"{firstDate.Date} {shift.ScheduledEnd}";

                                            DateTime shiftStart = DateTime.Parse(shiftStartStr);
                                            DateTime shiftEnd = DateTime.Parse(shiftEndStr);

                                            var schedule = new VolSchedule
                                            {
                                                ShiftDate = eventToUpdate.Date,
                                                ScheduledStart = shiftStart,
                                                ScheduledEnd = shiftEnd,
                                                Capacity = shift.Capacity > 0 ? shift.Capacity : 1,
                                                EventID = id
                                            };

                                            _context.VolSchedules.Add(schedule);
                                        }
                                        catch (Exception ex)
                                        {
                                            // Log error but continue with other shifts
                                            Console.WriteLine($"Error adding shift: {ex.Message}");
                                        }
                                    }
                                    await _context.SaveChangesAsync();
                                }

                                // Rest of multi-date code (unchanged)
                                // ...
                            }
                            else // No multi-date events, handle single event update
                            {
                                // Update event with single date details
                                eventToUpdate.Date = model.Event.Date;
                                eventToUpdate.Start = model.Event.Start;
                                eventToUpdate.End = model.Event.End;
                                _context.Update(eventToUpdate);
                                await _context.SaveChangesAsync();

                                // IMPORTANT: Check if the form submitted any schedule data
                                Console.WriteLine($"Form submitted schedule count: {model.ExistingSchedules?.Count ?? 0}");

                                // Get existing schedule IDs from database
                                var currentScheduleIds = eventToUpdate.VolSchedules.Select(s => s.ID).ToList();
                                Console.WriteLine($"Current schedules in DB: {string.Join(", ", currentScheduleIds)}");

                                // Get IDs from submitted schedules
                                var submittedScheduleIds = model.ExistingSchedules?
                                    .Where(s => s.ScheduleID.HasValue && s.ScheduleID.Value > 0)
                                    .Select(s => s.ScheduleID.Value)
                                    .ToList() ?? new List<int>();
                                Console.WriteLine($"Submitted schedule IDs: {string.Join(", ", submittedScheduleIds)}");

                                // Remove schedules that are no longer present
                                var schedulesToRemove = currentScheduleIds.Except(submittedScheduleIds).ToList();
                                Console.WriteLine($"Schedules to remove: {string.Join(", ", schedulesToRemove)}");

                                foreach (var scheduleId in schedulesToRemove)
                                {
                                    var scheduleToRemove = await _context.VolSchedules
                                        .Include(s => s.VolAttendances)
                                        .FirstOrDefaultAsync(s => s.ID == scheduleId);

                                    if (scheduleToRemove != null)
                                    {
                                        // Remove associated attendances
                                        _context.VolAttendances.RemoveRange(scheduleToRemove.VolAttendances);
                                        _context.VolSchedules.Remove(scheduleToRemove);
                                    }
                                }
                                await _context.SaveChangesAsync();

                                // Now update existing schedules and add new ones
                                if (model.ExistingSchedules != null && model.ExistingSchedules.Any())
                                {
                                    foreach (var scheduleVM in model.ExistingSchedules)
                                    {
                                        VolSchedule schedule;

                                        if (scheduleVM.ScheduleID.HasValue && scheduleVM.ScheduleID.Value > 0)
                                        {
                                            // Existing schedule - update it
                                            schedule = await _context.VolSchedules
                                                .Include(s => s.VolAttendances)
                                                .FirstOrDefaultAsync(s => s.ID == scheduleVM.ScheduleID.Value);

                                            if (schedule != null)
                                            {
                                                schedule.ScheduledStart = scheduleVM.ScheduledStart;
                                                schedule.ScheduledEnd = scheduleVM.ScheduledEnd;
                                                schedule.Capacity = scheduleVM.Capacity > 0 ? scheduleVM.Capacity : 1;
                                                schedule.ShiftDate = model.Event.Date;

                                                _context.Update(schedule);
                                                Console.WriteLine($"Updated schedule ID {schedule.ID}");
                                            }
                                            else
                                            {
                                                // Schedule ID provided but not found - create new
                                                schedule = new VolSchedule
                                                {
                                                    ShiftDate = model.Event.Date,
                                                    ScheduledStart = scheduleVM.ScheduledStart,
                                                    ScheduledEnd = scheduleVM.ScheduledEnd,
                                                    Capacity = scheduleVM.Capacity > 0 ? scheduleVM.Capacity : 1,
                                                    EventID = id
                                                };
                                                _context.VolSchedules.Add(schedule);
                                                Console.WriteLine("Created new schedule (ID not found)");
                                            }
                                        }
                                        else
                                        {
                                            // New schedule - create it
                                            schedule = new VolSchedule
                                            {
                                                ShiftDate = model.Event.Date,
                                                ScheduledStart = scheduleVM.ScheduledStart,
                                                ScheduledEnd = scheduleVM.ScheduledEnd,
                                                Capacity = scheduleVM.Capacity > 0 ? scheduleVM.Capacity : 1,
                                                EventID = id
                                            };
                                            _context.VolSchedules.Add(schedule);
                                            Console.WriteLine("Created new schedule");
                                        }
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }

                            await transaction.CommitAsync();
                            TempData["SuccessMessage"] = "Event updated successfully!";
                            return RedirectToAction(nameof(Index));
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            ModelState.AddModelError("", "An error occurred while updating the event: " + ex.Message);
                            if (ex.InnerException != null)
                            {
                                ModelState.AddModelError("", "Details: " + ex.InnerException.Message);
                            }
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
                        ModelState.AddModelError("", "The record was modified by another user. Please try again.");
                    }
                }
            }

            // If we get here, something failed, redisplay form
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", model.Event.VolLocationID);
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
                .Include(e => e.VolSchedules)
                    .ThenInclude(s => s.VolAttendances)
                        .ThenInclude(a => a.Volunteer)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Events");

                // Add headers
                worksheet.Cells[1, 1].Value = "Event Name";
                worksheet.Cells[1, 2].Value = "Address";
                worksheet.Cells[1, 3].Value = "Start";
                worksheet.Cells[1, 4].Value = "End";
                worksheet.Cells[1, 5].Value = "Location";
                worksheet.Cells[1, 6].Value = "Volunteer Count";
                worksheet.Cells[1, 7].Value = "Total Hours";
                worksheet.Cells[1, 8].Value = "Notes";

                // Make headers bold
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5b1fc7"));
                    range.Style.Font.Color.SetColor(Color.White);
                }

                int row = 2;
                foreach (var eventItem in events)
                {
                    int volunteerCount = eventItem.VolSchedules.SelectMany(s => s.VolAttendances).Count(a => a.Status);
                    double totalHours = eventItem.VolSchedules.Sum(s => (s.ScheduledEnd - s.ScheduledStart).TotalHours);
                    var volunteers = eventItem.VolSchedules
                        .SelectMany(s => s.VolAttendances)
                        .Where(a => a.Status)
                        .Select(a => a.Volunteer.FullName)
                        .Distinct()
                        .ToList();
                        
                    worksheet.Cells[row, 1].Value = eventItem.Name;
                    worksheet.Cells[row, 2].Value = eventItem.Location;
                    worksheet.Cells[row, 3].Value = eventItem.Start;
                    worksheet.Cells[row, 4].Value = eventItem.End;
                    worksheet.Cells[row, 5].Value = eventItem.VolLocation?.City;
                    worksheet.Cells[row, 6].Value = volunteerCount;
                    worksheet.Cells[row, 7].Value = totalHours;
                    worksheet.Cells[row, 8].Value = eventItem.Notes;
                    
                    // Add comment with volunteer names
                    if (volunteers.Any())
                    {
                        var volunteersList = string.Join(", ", volunteers);
                        var comment = worksheet.Cells[row, 6].AddComment("Volunteers:\n" + volunteersList, "Volunteer List");
                        comment.AutoFit = true;
                        
                        // Adjust comment size based on content length
                        int width = Math.Min(300, Math.Max(150, volunteersList.Length / 2));
                        int height = Math.Min(200, Math.Max(50, volunteers.Count * 15));
                        comment.From.Column = comment.From.Column;
                        comment.From.Row = comment.From.Row;
                        comment.To.Column = comment.From.Column + width / 7;
                        comment.To.Row = comment.From.Row + height / 15;
                    }
                    
                    row++;
                }

                // Format dates and numbers
                worksheet.Column(3).Style.Numberformat.Format = "mmm d, yyyy hh:mm tt";
                worksheet.Column(4).Style.Numberformat.Format = "mmm d, yyyy hh:mm tt";
                worksheet.Column(7).Style.Numberformat.Format = "#,##0.0";
                
                // Auto-fit columns for better readability
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"Event_Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpGet]
        public IActionResult FilteredEventReportExport(string SearchString, string SearchCity, string StartDate, string EndDate, string StartTime, string EndTime, bool archived = false)
        {
            // Check if any filter is applied
            bool isFilterApplied = !string.IsNullOrEmpty(SearchString) ||
                                  !string.IsNullOrEmpty(SearchCity) ||
                                  !string.IsNullOrEmpty(StartDate) ||
                                  !string.IsNullOrEmpty(EndDate) ||
                                  !string.IsNullOrEmpty(StartTime) ||
                                  !string.IsNullOrEmpty(EndTime);

            if (!isFilterApplied)
            {
                return Content("Please apply at least one filter before exporting.");
            }

            // Parse date values
            var startDateTime = !string.IsNullOrEmpty(StartDate) ? DateTime.Parse(StartDate) : DateTime.MinValue;
            var endDateTime = !string.IsNullOrEmpty(EndDate) ? DateTime.Parse(EndDate) : DateTime.MaxValue;

            // Parse time values
            var startTimeSpan = !string.IsNullOrEmpty(StartTime) ? TimeSpan.Parse(StartTime) : TimeSpan.MinValue;
            var endTimeSpan = !string.IsNullOrEmpty(EndTime) ? TimeSpan.Parse(EndTime) : TimeSpan.MaxValue;

            // Query the database with filters
            var events = _context.Events
                .Include(e => e.VolLocation)
                .Include(e => e.VolSchedules).ThenInclude(v => v.VolAttendances).ThenInclude(a => a.Volunteer)
                .Where(e => e.Start >= startDateTime && e.End <= (endDateTime != DateTime.MaxValue ? endDateTime.AddDays(1) : endDateTime))
                .Where(e => e.IsArchived == archived)
                .AsEnumerable() // Switch to client-side evaluation
                .Where(e => e.Start.TimeOfDay >= startTimeSpan && e.End.TimeOfDay <= endTimeSpan)
                .AsQueryable();


            if (!String.IsNullOrEmpty(SearchString))
            {
                events = events.Where(e => e.Name != null && e.Name.ToLower().Contains(SearchString.ToLower()));
            }

            if (!string.IsNullOrEmpty(SearchCity))
            {
                events = events.Where(e => e.VolLocation.City != null && e.VolLocation.City == SearchCity);
            }

            // Process the data for the report including volunteer information
            var eventList = events
                .OrderBy(e => e.Start)
                .Select(e => new
                {
                    e.Name,
                    e.Start,
                    e.End,
                    e.VolLocation.City,
                    e.Location,
                    e.Notes,
                    VolunteerCount = e.VolSchedules.SelectMany(s => s.VolAttendances).Count(a => a.Status),
                    TotalHours = e.VolSchedules.Sum(s => (s.ScheduledEnd - s.ScheduledStart).TotalHours),
                    Volunteers = e.VolSchedules
                        .SelectMany(s => s.VolAttendances)
                        .Where(a => a.Status)
                        .Select(a => a.Volunteer.FullName)
                        .Distinct()
                        .ToList()
                })
                .ToList();

            if (!eventList.Any())
            {
                return Content("No data available for the selected filters.");
            }

            // Generate Excel file
            using (ExcelPackage excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.Add("Filtered Event Report");

                // Title row
                workSheet.Cells[1, 1].Value = "Filtered Event Report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 8])
                {
                    Rng.Merge = true;
                    Rng.Style.Font.Bold = true;
                    Rng.Style.Font.Size = 18;
                    Rng.Style.Font.Color.SetColor(Color.White);
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    Rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    Rng.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#100527"));
                }

                // Header row
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 8])
                {
                    headings.Style.Font.Bold = true;
                    headings.Style.Font.Color.SetColor(Color.White);
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5b1fc7"));
                }

                // Set column headers
                workSheet.Cells[3, 1].Value = "Event Name";
                workSheet.Cells[3, 2].Value = "Start";
                workSheet.Cells[3, 3].Value = "End";
                workSheet.Cells[3, 4].Value = "City";
                workSheet.Cells[3, 5].Value = "Location";
                workSheet.Cells[3, 6].Value = "Volunteer Count";
                workSheet.Cells[3, 7].Value = "Total Hours";
                workSheet.Cells[3, 8].Value = "Notes";

                // Fill data rows
                int row = 4;
                foreach (var eventItem in eventList)
                {
                    workSheet.Cells[row, 1].Value = eventItem.Name;
                    workSheet.Cells[row, 2].Value = eventItem.Start;
                    workSheet.Cells[row, 3].Value = eventItem.End;
                    workSheet.Cells[row, 4].Value = eventItem.City;
                    workSheet.Cells[row, 5].Value = eventItem.Location;
                    workSheet.Cells[row, 6].Value = eventItem.VolunteerCount;
                    workSheet.Cells[row, 7].Value = eventItem.TotalHours;
                    workSheet.Cells[row, 8].Value = eventItem.Notes;
                    
                    // Add comment with volunteer names
                    if (eventItem.Volunteers.Any())
                    {
                        var volunteersList = string.Join(", ", eventItem.Volunteers);
                        var comment = workSheet.Cells[row, 6].AddComment("Volunteers:\n" + volunteersList, "Volunteer List");
                        comment.AutoFit = true;
                        
                        // Adjust comment size based on content length
                        int width = Math.Min(300, Math.Max(150, volunteersList.Length / 2));
                        int height = Math.Min(200, Math.Max(50, eventItem.Volunteers.Count * 15));
                        comment.From.Column = comment.From.Column;
                        comment.From.Row = comment.From.Row;
                        comment.To.Column = comment.From.Column + width / 7;
                        comment.To.Row = comment.From.Row + height / 15;
                    }
                    
                    row++;
                }

                // Style and format
                var range = workSheet.Cells[4, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Column(2).Style.Numberformat.Format = "mmm d, yyyy hh:mm tt";
                workSheet.Column(3).Style.Numberformat.Format = "mmm d, yyyy hh:mm tt";
                workSheet.Column(7).Style.Numberformat.Format = "#,##0.0";
                workSheet.Cells.AutoFitColumns();

                // Return the Excel file
                try
                {
                    byte[] fileData = excel.GetAsByteArray();
                    string filename = "Filtered Event Report.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(fileData, mimeType, filename);
                }
                catch (Exception ex)
                {
                    return Content("Could not build and download the file: " + ex.Message);
                }
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
                .Where(e => !e.IsArchived) // Ensure only non-archived events are fetched
                .Select(e => new
                {
                    id = e.ID,
                    title = e.Name,
                    description = e.Location,
                    start = e.Start.ToString("o"), // ISO 8601 format
                    end = e.End.ToString("o"),     // ISO 8601 format
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

        // Chart Methods
        // Doughnut Chart - Events by City
        [HttpGet]
        public async Task<IActionResult> GetEventsByCityData()
        {
            var eventsByCity = await _context.Events
                .Include(e => e.VolLocation) // Include the location to access the city
                .GroupBy(e => e.VolLocation.City)
                .Select(g => new
                {
                    City = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var labels = eventsByCity.Select(e => e.City).ToArray();
            var data = eventsByCity.Select(e => e.Count).ToArray();

            return Json(new { labels, data });
        }

        // Active vs. Archived Events
        [HttpGet]
        public async Task<IActionResult> GetActiveVsArchivedEventsData()
        {
            var activeEventsCount = await _context.Events.CountAsync(e => !e.IsArchived);
            var archivedEventsCount = await _context.Events.CountAsync(e => e.IsArchived);

            var labels = new[] { "Active", "Archived" };
            var data = new[] { activeEventsCount, archivedEventsCount };

            return Json(new { labels, data });
        }

        // Get Upcoming Events Widget
        [HttpGet]
        public async Task<IActionResult> GetUpcomingEvents()
        {
            var upcomingEvents = await _context.Events
                .Where(e => e.Start >= DateTime.Today && !e.IsArchived) // Filter upcoming and non-archived events
                .Include(e => e.VolLocation) // Include location details
                .OrderByDescending(s => s.Start) // Changed to descending order
                .Take(10) // Take only 10 most recent
                .Select(e => new
                {
                    id = e.ID,
                    title = e.Name, // Event name
                    date = e.Start.ToString("yyyy-MM-dd"), // Event start date
                    location = e.VolLocation.City // Event location (city)
                })
                .ToListAsync();

            return Json(upcomingEvents);
        }

        // Volunteers Count
        [HttpGet]
        public async Task<JsonResult> GetTotalEventCount()
        {
            try
            {
                // Count all events, regardless of their archived status
                var totalCount = await _context.Events.CountAsync();
                return Json(new { TotalCount = totalCount });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog or NLog)
                Console.Error.WriteLine($"Error in GetTotalEventCount: {ex.Message}");
                return Json(new { TotalCount = 0 }); // Return a default value in case of error
            }
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.ID == id);
        }
    }
}
