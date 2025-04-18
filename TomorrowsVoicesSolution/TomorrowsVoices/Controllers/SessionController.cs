using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;
using TomorrowsVoices.ViewModels;
using TomorrowsVoices.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace TomorrowsVoices.Controllers
{
    [Authorize]
    public class SessionController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public SessionController(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        // GET: Session
        public async Task<IActionResult> Index(string? SearchString, int? minPresentSinger, int? maxPresentSinger, DateTime StartDate, DateTime EndDate, string? SearchCity, int? page, string? actionButton, bool archived = false, string sortDirection = "asc", string sortField = "Session", int? pageSizeID = 10)
        {

            string[] sortOptions = new[] { "City", "Date", "Attendance", "Director" };
            int numberFilters = 0;

          

            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start

                if (sortOptions.Contains(actionButton))
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; //Sort by the button clicked
                }
            }


            //Always Filter by date range
            //If first time loading the page, set the date range filter based on the values in the database
            if (EndDate == DateTime.MinValue)
            {
                if (!User.IsInRole("Admin"))
                {
                    var currentDirector = await GetCurrentDirectorAsync();
                    if (currentDirector != null)
                    {
                        // For Directors, only get date range from their assigned cities
                        var assignedLocationIds = currentDirector.DirectorLocations.Select(dl => dl.LocationID).ToList();
                        var directorSessions = _context.Sessions
                            .Where(s => assignedLocationIds.Contains(s.LocationID.Value) && s.IsArchived == archived);

                        if (directorSessions.Any())
                        {
                            StartDate = directorSessions.Min(o => o.Date).Value;
                            EndDate = directorSessions.Max(o => o.Date).Value;
                        }
                        else
                        {
                            // If no sessions exist for the director, set default dates
                            StartDate = DateTime.Today.AddMonths(-1);
                            EndDate = DateTime.Today;
                        }
                    }
                    else
                    {
                        // If no director record exists, set default dates
                        StartDate = DateTime.Today.AddMonths(-1);
                        EndDate = DateTime.Today;
                    }
                }
                else
                {
                    // For Admins, get date range from all sessions
                    var adminSessions = _context.Sessions.Where(s => s.IsArchived == archived);
                    if (adminSessions.Any())
                    {
                        StartDate = adminSessions.Min(o => o.Date).Value;
                        EndDate = adminSessions.Max(o => o.Date).Value;
                    }
                    else
                    {
                        StartDate = DateTime.Today.AddMonths(-1);
                        EndDate = DateTime.Today;
                    }
                }
            }
            else
            {
                // If dates are provided in the request, increment the filter count
                if (StartDate != DateTime.MinValue)
                {
                    numberFilters++;
                }
                if (EndDate != DateTime.MinValue)
                {
                    numberFilters++;
                }
            }

            //Check the order of the dates and swap them if required
            if (EndDate < StartDate)
            {
                DateTime temp = EndDate;
                EndDate = StartDate;
                StartDate = temp;
            }

            //Save to View Data
            ViewData["StartDate"] = StartDate.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = EndDate.ToString("yyyy-MM-dd");

            // First apply the Director's city filter if needed
            var sessions = _context.Sessions
                .Include(s => s.Location)
                .ThenInclude(l => l.DirectorLocations)
                .ThenInclude(dl => dl.Director)
                .Include(s => s.Attendance)
                .ThenInclude(a => a.Singer)
                .Where(s => s.IsArchived == archived)
                .AsNoTracking();

            if (!User.IsInRole("Admin"))
            {
                var currentDirector = await GetCurrentDirectorAsync();
                if (currentDirector != null)
                {
                    var assignedCityIds = currentDirector.DirectorLocations.Select(dl => dl.LocationID).ToList();
                    sessions = sessions.Where(s => assignedCityIds.Contains(s.LocationID.Value));
                }
                else
                {
                    sessions = sessions.Where(v => false);
                }
            }

            // Then apply the date range filter
            sessions = sessions.Where(a => a.Date >= StartDate && a.Date <= EndDate.AddDays(1));

            ViewData["IsArchived"] = archived;
            ViewData["ActiveTab"] = archived ? "archived" : "active";


            if (!String.IsNullOrEmpty(SearchString))
            {
                if (!User.IsInRole("Admin"))
                {
                    var currentDirector = await GetCurrentDirectorAsync();
                    if (currentDirector != null)
                    {
                        // For Directors, only search within their assigned cities
                        sessions = sessions.Where(p => 
                            p.Location.DirectorLocations.Any(dl => dl.DirectorID == currentDirector.ID) &&
                            (p.Location.DirectorLocations.FirstOrDefault().Director.LastName != null && 
                             p.Location.DirectorLocations.FirstOrDefault().Director.LastName.ToLower().Contains(SearchString.ToLower()) ||
                             p.Location.DirectorLocations.FirstOrDefault().Director.FirstName != null && 
                             p.Location.DirectorLocations.FirstOrDefault().Director.FirstName.ToLower().Contains(SearchString.ToLower()) ||
                             ((p.Location.DirectorLocations.FirstOrDefault().Director.FirstName + " " + 
                               p.Location.DirectorLocations.FirstOrDefault().Director.LastName).ToLower().Contains(SearchString.ToLower()))));
                    }
                }
                else
                {
                    // For Admins, search across all directors
                    sessions = sessions.Where(p => 
                        p.Location.DirectorLocations.FirstOrDefault().Director.LastName != null && 
                        p.Location.DirectorLocations.FirstOrDefault().Director.LastName.ToLower().Contains(SearchString.ToLower()) ||
                        p.Location.DirectorLocations.FirstOrDefault().Director.FirstName != null && 
                        p.Location.DirectorLocations.FirstOrDefault().Director.FirstName.ToLower().Contains(SearchString.ToLower()) ||
                        ((p.Location.DirectorLocations.FirstOrDefault().Director.FirstName + " " + 
                          p.Location.DirectorLocations.FirstOrDefault().Director.LastName).ToLower().Contains(SearchString.ToLower())));
                }
                numberFilters++;
            }

            if (minPresentSinger.HasValue || maxPresentSinger.HasValue)
            {
                if (!User.IsInRole("Admin"))
                {
                    var currentDirector = await GetCurrentDirectorAsync();
                    if (currentDirector != null)
                    {
                        // For Directors, only filter attendance within their assigned cities
                        sessions = sessions.Where(p =>
                            p.Location.DirectorLocations.Any(dl => dl.DirectorID == currentDirector.ID) &&
                            (!minPresentSinger.HasValue || p.Attendance.Count(a => a.Status) >= minPresentSinger.Value) &&
                            (!maxPresentSinger.HasValue || p.Attendance.Count(a => a.Status) <= maxPresentSinger.Value)
                        );
                    }
                }
                else
                {
                    // For Admins, filter attendance across all sessions
                    sessions = sessions.Where(p =>
                        (!minPresentSinger.HasValue || p.Attendance.Count(a => a.Status) >= minPresentSinger.Value) &&
                        (!maxPresentSinger.HasValue || p.Attendance.Count(a => a.Status) <= maxPresentSinger.Value)
                    );
                }
                numberFilters++;
            }


            if (!string.IsNullOrEmpty(SearchCity))
            {


                sessions = sessions
             .Where(p => p.Location.City != null && p.Location.City == SearchCity);
                numberFilters++;
            }





            // sorting functionality
            if (sortField == "Director")
            {
                if (sortDirection == "asc")
                {
                    sessions = sessions
                        .OrderBy(p => p.Location.DirectorLocations.FirstOrDefault().Director.FirstName)
                        .ThenBy(p => p.Location.DirectorLocations.FirstOrDefault().Director.LastName);
                }
                else
                {
                    sessions = sessions
                        .OrderByDescending(p => p.Location.DirectorLocations.FirstOrDefault().Director.FirstName)
                        .ThenBy(p => p.Location.DirectorLocations.FirstOrDefault().Director.LastName);
                }
            }
            else if (sortField == "Date")
            {
                if (sortDirection == "asc")
                {
                    sessions = sessions
                        .OrderBy(p => p.Date);
                }
                else
                {
                    sessions = sessions
                        .OrderByDescending(p => p.Date);
                }
            }
            else if (sortField == "Attendance")
            {
                if (sortDirection == "asc")
                {
                    sessions = sessions
                        .OrderBy(p => p.Attendance.Count(a => a.Status == true));
                }
                else
                {
                    sessions = sessions
                        .OrderByDescending(p => p.Attendance.Count(a => a.Status == true));
                }
            }
            else if (sortField == "City")
            {
                if (sortDirection == "asc")
                {
                    sessions = sessions
                        .OrderBy(p => p.Location.City)
                           .ThenBy(p => p.Location.DirectorLocations.FirstOrDefault().Director.FirstName)
                        .ThenBy(p => p.Location.DirectorLocations.FirstOrDefault().Director.LastName);
                }
                else
                {
                    sessions = sessions
                        .OrderByDescending(p => p.Location.City)
                              .ThenBy(p => p.Location.DirectorLocations.FirstOrDefault().Director.FirstName)
                        .ThenBy(p => p.Location.DirectorLocations.FirstOrDefault().Director.LastName);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["numberFilters"] = numberFilters;
            int archivedCount = await _context.Sessions.CountAsync(d => d.IsArchived == true);
            ViewData["numberofArchive"] = archivedCount;
            int activeCount = await _context.Sessions.CountAsync(d => d.IsArchived == false);
            ViewData["numberofActive"] = activeCount;

            var cityList = sessions.AsEnumerable()
                .Select(d => d.Location?.City.ToString())
                .Where(city => city != null)
                .Distinct()
                .Select(city => new SelectListItem
                {
                    Value = city,
                    Text = city
                })
                .ToList();

            cityList.Insert(0, new SelectListItem { Value = "", Text = "All Cities" });

            // If user is a Director, filter cities to only show their assigned cities
            if (!User.IsInRole("Admin"))
            {
                var currentDirector = await GetCurrentDirectorAsync();
                if (currentDirector != null)
                {
                    var assignedCities = currentDirector.DirectorLocations
                        .Select(dl => dl.Location.City)
                        .Distinct()
                        .ToList();
                    
                    cityList = cityList.Where(c => string.IsNullOrEmpty(c.Value) || assignedCities.Contains(c.Value))
                        .ToList();
                }
            }

            ViewData["Cities"] = cityList;


            var availableSingersPerCity = _context.Singers
                .GroupBy(s => s.LocationID)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var session in sessions)
            {
                int locationId = session.Location?.ID ?? 0;
                ViewData["AvailableSingers_" + locationId] = availableSingersPerCity.ContainsKey(locationId)
                    ? availableSingersPerCity[locationId]
                    : 0;
            }

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Session>.CreateAsync(sessions.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Session/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Location)
                    .ThenInclude(l => l.DirectorLocations)
                        .ThenInclude(dl => dl.Director)
                .Include(s => s.Attendance)
                    .ThenInclude(a => a.Singer)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (session == null)
            {
                return NotFound();
            }

            // Get all singers for this location
            var allSingers = await _context.Singers
                .Where(s => s.LocationID == session.LocationID)
                .ToListAsync();

            // Create attendance records for singers that don't have one
            foreach (var singer in allSingers)
            {
                if (!session.Attendance.Any(a => a.SingerID == singer.ID))
                {
                    session.Attendance.Add(new Attendance
                    {
                        SingerID = singer.ID,
                        SessionID = session.ID,
                        Status = false,
                        Singer = singer
                    });
                }
            }

            return View(session);
        }

        // GET: Session/Create
        public async Task<IActionResult> Create()
        {
            var session = new Session();
            var currentDirector = await GetCurrentDirectorAsync();

            // Default to director's first assigned city (if not admin)
            if (currentDirector != null && !User.IsInRole("Admin"))
            {
                // Get director's first location (assuming they have at least one)
                var directorLocation = currentDirector.DirectorLocations.FirstOrDefault();
                if (directorLocation != null)
                {
                    session.LocationID = directorLocation.LocationID;

                    // Pre-set the director (hidden in view)
                    session.Location = new Location
                    {
                        DirectorLocations = new List<DirectorLocation> {
                    new DirectorLocation { DirectorID = currentDirector.ID }
                }
                    };
                }

                ViewBag.DirectorName = currentDirector.DirectorFullName;
                ViewBag.DirectorId = currentDirector.ID;
                ViewBag.IsDirectorReadonly = true;
                ViewBag.IsLocationReadonly = true;
                ViewData["IsDirector"] = true;
            }
            else
            {
                ViewBag.IsDirectorReadonly = false;
                ViewBag.IsLocationReadonly = false;
            }

            // Filter locations (same as before)
            List<Location> locations;
            if (currentDirector != null && !User.IsInRole("Admin"))
            {
                var assignedLocationIds = currentDirector.DirectorLocations.Select(dl => dl.LocationID).ToList();
                locations = _context.Locations
                    .Where(l => assignedLocationIds.Contains(l.ID))
                    .OrderBy(l => l.City)
                    .ToList();
            }
            else
            {
                locations = _context.Locations.OrderBy(l => l.City).ToList();
            }

            ViewData["LocationID"] = new SelectList(locations, "ID", "City", session.LocationID);
            PopulateAssignedSingerData(session);
            return View(session);
        }


        // POST: Session/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,Notes,LocationID")] Session session,
            string[] selectedOptions)
        {
            try
            {
                UpdateSessionSingers(selectedOptions, session);
                if (ModelState.IsValid)
                {
                    _context.Add(session);
                    await _context.SaveChangesAsync();


                    var attendances = _context.Attendances
                        .Where(async => async.SessionID == session.ID)
                        .Include(a => a.Singer)
                        .ToList();

                    var presentSingersCount = session.Attendance.Count(a => a.Status == true);
                    var absentSingersCount = session.Attendance.Count(a => a.Status == false);
                    var totalSingersCount = session.Attendance.Count();


                    TempData["SuccessMessage"] = $"{presentSingersCount} singers attended ";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
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

            // Load the session with related data
            var session = await _context.Sessions
                .Include(s => s.Location)
                    .ThenInclude(l => l.DirectorLocations)
                        .ThenInclude(dl => dl.Director)
                .Include(s => s.Attendance)
                    .ThenInclude(a => a.Singer)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (session == null)
            {
                return NotFound();
            }

            // Get the current director
            var currentDirector = await GetCurrentDirectorAsync();

            // Set view flags
            ViewBag.IsDirectorReadonly = currentDirector != null && !User.IsInRole("Admin");
            if (ViewBag.IsDirectorReadonly)
            {
                ViewBag.DirectorName = currentDirector.DirectorFullName;
                ViewBag.DirectorId = currentDirector.ID;
                ViewBag.AutoSelectedCityId = session.LocationID;
            }

            // Filter locations
            List<Location> locations;
            if (currentDirector != null && !User.IsInRole("Admin"))
            {
                // Only show assigned locations
                var assignedLocationIds = currentDirector.DirectorLocations.Select(dl => dl.LocationID).ToList();
                locations = _context.Locations
                    .Where(l => assignedLocationIds.Contains(l.ID))
                    .OrderBy(l => l.City)
                    .ToList();
            }
            else if (User.IsInRole("Admin"))
            {
                // Admins see all locations
                locations = _context.Locations.OrderBy(l => l.City).ToList();
            }
            else
            {
                locations = new List<Location>();
            }

            ViewData["LocationID"] = new SelectList(locations, "ID", "City", session.LocationID);
            PopulateAssignedSingerData(session);

            return View(session);
        }

        // POST: Session/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( [Bind("Date,Notes,LocationID")] Session session, int id, string[] selectedOptions, string[] removedOptions)
        {
            try
            {
                var sessionToUpdate = await _context.Sessions
                    .Include(s => s.Location)
                    .Include(s => s.Attendance)
                        .ThenInclude(a => a.Singer)
                    .FirstOrDefaultAsync(m => m.ID == id);

                if (sessionToUpdate == null)
                {
                    return NotFound("Session not found");
                }

                if (sessionToUpdate.Location == null)
                {
                    return NotFound("Session location not found");
                }

                // Initialize collections if null
                sessionToUpdate.Attendance ??= new List<Attendance>();
                selectedOptions ??= Array.Empty<string>();
                removedOptions ??= Array.Empty<string>();

                // Convert selected and removed options to integer sets
                var selectedIds = selectedOptions
                    .Where(o => !string.IsNullOrEmpty(o))
                    .Select(int.Parse)
                    .ToHashSet();

                var removedIds = removedOptions
                    .Where(o => !string.IsNullOrEmpty(o))
                    .Select(int.Parse)
                    .ToHashSet();

                // Get all existing attendance records
                var existingAttendance = sessionToUpdate.Attendance.ToList();

                // First, handle removed singers
                foreach (var attendance in existingAttendance)
                {
                    if (removedIds.Contains(attendance.SingerID))
                    {
                        attendance.Status = false;
                        removedIds.Remove(attendance.SingerID);
                    }
                }

                // Then, handle selected singers
                foreach (var attendance in existingAttendance)
                {
                    if (selectedIds.Contains(attendance.SingerID))
                    {
                        attendance.Status = true;
                        selectedIds.Remove(attendance.SingerID);
                    }
                }

                // Add new attendance records for remaining selected singers
                foreach (var singerId in selectedIds)
                {
                    sessionToUpdate.Attendance.Add(new Attendance
                    {
                        SingerID = singerId,
                        SessionID = sessionToUpdate.ID,
                        Status = true
                    });
                }

                // Add attendance records for removed singers that don't exist yet
                foreach (var singerId in removedIds)
                {
                    if (!existingAttendance.Any(a => a.SingerID == singerId))
                    {
                        sessionToUpdate.Attendance.Add(new Attendance
                        {
                            SingerID = singerId,
                            SessionID = sessionToUpdate.ID,
                            Status = false
                        });
                    }
                }
                sessionToUpdate.Date = session.Date;
                sessionToUpdate.Notes = session.Notes; 
                if (ModelState.IsValid)
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Session attendance updated successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SessionExists(id))
                        {
                            return NotFound();
                        }
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while updating the session: {ex.Message}");
            }

            // If we got this far, something failed, redisplay form
            return await ReloadEditView(id);
        }

        private async Task<IActionResult> ReloadEditView(int id)
        {
            var session = await _context.Sessions
                .Include(s => s.Location)
                .Include(s => s.Attendance)
                    .ThenInclude(a => a.Singer)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (session == null)
            {
                return NotFound();
            }

            PopulateAssignedSingerData(session);
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
                .Include(s => s.Location).ThenInclude(l => l.DirectorLocations)
                    .ThenInclude(dl => dl.Director)
                .Include(s => s.Attendance).ThenInclude(a => a.Singer)
                .AsNoTracking()
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
            var session = await _context.Sessions
                .Include(s => s.Location).ThenInclude(l => l.DirectorLocations).ThenInclude(dl => dl.Director)
                .Include(s => s.Attendance).ThenInclude(a => a.Singer)
                .FirstOrDefaultAsync(m => m.ID == id);

            try
            {
                if (session != null)
                {
                    _context.Sessions.Remove(session);
                }

                await _context.SaveChangesAsync();
                var returnUrl = ViewData["returnURL"]?.ToString();
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction(nameof(Index));
                }
                return Redirect(returnUrl);
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Session. Remember, you cannot Delete a Session that has singers assigned.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }

            }
            await _context.SaveChangesAsync();
            return View(session);
        }
        private SelectList LocationSelectList()
        {
            return new SelectList(
                _context.Locations
                    .GroupBy(l => l.City)
                    .OrderBy(g => g.Key)
                    .Select(g => g.FirstOrDefault())
                    .ToList(),
                "ID",
                "City");
        }
        
        [HttpGet]
        public JsonResult GetDirectorAndSingersByLocation(int locationId)
        {
            try
            {
                var location = _context.Locations
                    .Include(l => l.DirectorLocations)
                        .ThenInclude(dl => dl.Director)
                    .FirstOrDefault(l => l.ID == locationId);

                if (location == null)
                {
                    return Json(new { directors = new List<object>(), singers = new List<object>() });
                }

                var directors = location.DirectorLocations
                    .Select(dl => new
                    {
                        id = dl.Director.ID,
                        name = dl.Director.DirectorFullName
                    })
                    .ToList();

                var singers = _context.Singers
                    .Where(s => s.LocationID == locationId)
                    .Select(s => new
                    {
                        id = s.ID,
                        name = $"{s.FullName} ({s.Location.City})"
                    })
                    .ToList();

                return Json(new { directors, singers });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDirectorAndSingersByLocation: {ex.Message}");
                return Json(new { directors = new List<object>(), singers = new List<object>() });
            }
        }
        private void PopulateAssignedSingerData(Session session)
        {
            // Get all singers for this location
            var allSingers = _context.Singers
                .Include(s => s.Location)
                .Where(s => s.LocationID == session.LocationID)
                .Select(s => new {
                    s.ID,
                    s.FirstName,
                    s.LastName,
                    City = s.Location != null ? s.Location.City : null
                })
                .ToList();

            // Get attendance records for this session
            var attendanceRecords = session.Attendance.ToList();

            // Separate into attended, absent, and unassigned singers
            var attendedSingers = attendanceRecords
                .Where(a => a.Status)
                .Select(a => a.SingerID)
                .ToHashSet();

            var absentSingers = attendanceRecords
                .Where(a => !a.Status)
                .Select(a => a.SingerID)
                .ToHashSet();

            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();

            foreach (var singer in allSingers)
            {
                string fullName = $"{singer.FirstName} {singer.LastName}".Trim();
                string displayText = string.IsNullOrEmpty(singer.City)
                    ? fullName
                    : $"{fullName} ({singer.City})";

                var option = new ListOptionVM
                {
                    ID = singer.ID,
                    DisplayText = displayText
                };

                if (attendedSingers.Contains(singer.ID))
                {
                    selected.Add(option); // Goes to RIGHT list (attended)
                }
                else
                {
                    available.Add(option); // Goes to LEFT list (absent or unassigned)
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }

        private void UpdateSessionSingers(string[] selectedOptions, Session sessionToUpdate)
        {
            if (selectedOptions == null)
            {
                selectedOptions = new string[0];
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentAttendances = new HashSet<int>(
                sessionToUpdate.Attendance.Select(a => a.SingerID));

            foreach (var attendance in sessionToUpdate.Attendance)
            {
                // Update status for existing attendance records
                attendance.Status = selectedOptionsHS.Contains(attendance.SingerID.ToString());
            }

            // Add new attendance records for selected singers not already in the session
            foreach (var singerIdStr in selectedOptions)
            {
                if (int.TryParse(singerIdStr, out int singerId))
                {
                    if (!currentAttendances.Contains(singerId))
                    {
                        sessionToUpdate.Attendance.Add(new Attendance
                        {
                            SingerID = singerId,
                            SessionID = sessionToUpdate.ID,
                            Status = true
                        });
                    }
                }
            }
        }

        public IActionResult AttendanceReportExport()
        {
            var sessAtts = _context.Sessions
                .Include(s => s.Location).ThenInclude(l => l.DirectorLocations).ThenInclude(dl => dl.Director)
                .Include(s => s.Attendance).ThenInclude(a => a.Singer)
                .OrderBy(s => s.Date)
                .Select(x => new
                {
                    x.Date,
                    AttendancePresent = x.Attendance.Count(a => a.Status),
                    AttendanceTotal = x.Attendance.Count,
                    x.Location.City,
                    Director = x.Location.DirectorLocations.FirstOrDefault().Director.DirectorFullName
                    ,
                    Notes = x.Notes
                })
                .ToList();
            ;



            int numRows = sessAtts.Count();

            if (numRows > 0)
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var workSheet = excel.Workbook.Worksheets.Add("Session Attendance Report");

                    workSheet.Cells[1, 1].Value = "Attendance Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 6])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        Rng.Style.Fill.BackgroundColor.SetColor(Color.LightPink);
                    }

                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 6])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightSalmon);
                    }

                    workSheet.Cells[3, 1].LoadFromCollection(sessAtts, true);


                    workSheet.Cells[3, 1].Value = "Date";
                    workSheet.Cells[3, 2].Value = "Attended Singers";
                    workSheet.Cells[3, 3].Value = "Total Singers";
                    workSheet.Cells[3, 4].Value = "City";
                    workSheet.Cells[3, 5].Value = "Director";
                    workSheet.Cells[3, 6].Value = "Notes";





                    var range = workSheet.Cells[4, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    workSheet.Column(1).Style.Numberformat.Format = "mmm d, yyyy";

                    workSheet.Cells.AutoFitColumns();

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "Attendance Report.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No data.");

        }


        /* Trying Filtered Export */
        [HttpGet]
        public IActionResult FilteredAttendanceReportExport(string SearchString, string SearchCity, string StartDate, string EndDate, string minPresentSinger, string maxPresentSinger, bool archived = false)
        {
            // Check if any filter is applied
            bool isFilterApplied = !string.IsNullOrEmpty(SearchString) ||
                                  !string.IsNullOrEmpty(SearchCity) ||
                                  !string.IsNullOrEmpty(StartDate) ||
                                  !string.IsNullOrEmpty(EndDate) ||
                                  !string.IsNullOrEmpty(minPresentSinger) ||
                                  !string.IsNullOrEmpty(maxPresentSinger);

            if (!isFilterApplied)
            {
                return Content("Please apply at least one filter before exporting.");
            }

            // Parse date values
            var startDateTime = !string.IsNullOrEmpty(StartDate) ? DateTime.Parse(StartDate) : DateTime.MinValue;
            var endDateTime = !string.IsNullOrEmpty(EndDate) ? DateTime.Parse(EndDate) : DateTime.MaxValue;

            // Parse numeric values
            int? minPresent = !string.IsNullOrEmpty(minPresentSinger) ? int.Parse(minPresentSinger) : (int?)null;
            int? maxPresent = !string.IsNullOrEmpty(maxPresentSinger) ? int.Parse(maxPresentSinger) : (int?)null;

            // Query the database with filters
            var sessions = _context.Sessions
                .Include(s => s.Location).ThenInclude(l => l.DirectorLocations).ThenInclude(dl => dl.Director)
                .Include(s => s.Attendance).ThenInclude(a => a.Singer)
                .Where(a => a.Date >= startDateTime && a.Date <= (endDateTime != DateTime.MaxValue ? endDateTime.AddDays(1) : endDateTime))
                .Where(s => s.IsArchived == archived)
                .AsQueryable();

            if (!String.IsNullOrEmpty(SearchString))
            {
                sessions = sessions.Where(p => p.Location.DirectorLocations.FirstOrDefault().Director.LastName != null && p.Location.DirectorLocations.FirstOrDefault().Director.LastName.ToLower().Contains(SearchString.ToLower())
                                            || p.Location.DirectorLocations.FirstOrDefault().Director.FirstName != null && p.Location.DirectorLocations.FirstOrDefault().Director.FirstName.ToLower().Contains(SearchString.ToLower())
                                             || ((p.Location.DirectorLocations.FirstOrDefault().Director.FirstName + " " + p.Location.DirectorLocations.FirstOrDefault().Director.LastName).ToLower().Contains(SearchString.ToLower())));
            }

            if (minPresent.HasValue || maxPresent.HasValue)
            {
                sessions = sessions.Where(p =>
                    (!minPresent.HasValue || p.Attendance.Count(a => a.Status) >= minPresent.Value) &&
                    (!maxPresent.HasValue || p.Attendance.Count(a => a.Status) <= maxPresent.Value)
                );
            }

            if (!string.IsNullOrEmpty(SearchCity))
            {
                sessions = sessions.Where(p => p.Location.City != null && p.Location.City == SearchCity);
            }

            // Get the data for the report
            var sessAtts = sessions
                .OrderBy(s => s.Date)
                .Select(x => new
                {
                    x.Date,
                    AttendancePresent = x.Attendance.Count(a => a.Status),
                    AttendanceTotal = x.Attendance.Count,
                    x.Location.City,
                    Director = x.Location.DirectorLocations.FirstOrDefault().Director.DirectorFullName,
                    Notes = x.Notes,
                    AttendedSingers = x.Attendance.Where(a => a.Status).Select(a => a.Singer.FullName).ToList(),
                    TotalSingers = x.Attendance.Select(a => a.Singer.FullName).ToList()
                })
                .ToList();

            if (!sessAtts.Any())
            {
                return Content("No data available for the selected filters.");
            }

            // Generate Excel file
            using (ExcelPackage excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.Add("Filtered Session Attendance Report");

                // Title row
                workSheet.Cells[1, 1].Value = "Filtered Attendance Report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 6])
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
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 6])
                {
                    headings.Style.Font.Bold = true;
                    headings.Style.Font.Color.SetColor(Color.White);
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5b1fc7"));
                }

                // Set column headers
                workSheet.Cells[3, 1].Value = "Date";
                workSheet.Cells[3, 2].Value = "Attended Singers";
                workSheet.Cells[3, 3].Value = "Total Singers";
                workSheet.Cells[3, 4].Value = "City";
                workSheet.Cells[3, 5].Value = "Director";
                workSheet.Cells[3, 6].Value = "Notes";

                // Fill data rows
                int row = 4;
                foreach (var session in sessAtts)
                {
                    workSheet.Cells[row, 1].Value = session.Date;
                    workSheet.Cells[row, 2].Value = session.AttendancePresent;
                    workSheet.Cells[row, 3].Value = session.AttendanceTotal;
                    workSheet.Cells[row, 4].Value = session.City;
                    workSheet.Cells[row, 5].Value = session.Director;
                    workSheet.Cells[row, 6].Value = session.Notes;

                    // Add comments with singer lists
                    if (session.AttendedSingers.Any())
                    {
                        var attendedSingers = string.Join(", ", session.AttendedSingers);
                        var comment1 = workSheet.Cells[row, 2].AddComment("Attended Singers:\n" + attendedSingers, "Singer List");
                        comment1.AutoFit = true;

                        // Adjust comment size based on content
                        int width = Math.Min(300, Math.Max(150, attendedSingers.Length / 2));
                        int height = Math.Min(200, Math.Max(50, session.AttendedSingers.Count * 15));
                        comment1.From.Column = comment1.From.Column;
                        comment1.From.Row = comment1.From.Row;
                        comment1.To.Column = comment1.From.Column + width / 7;  // Adjust divisor as needed
                        comment1.To.Row = comment1.From.Row + height / 15;      // Adjust divisor as needed
                    }

                    if (session.TotalSingers.Any())
                    {
                        var totalSingers = string.Join(", ", session.TotalSingers);
                        var comment2 = workSheet.Cells[row, 3].AddComment("Total Singers:\n" + totalSingers, "Singer List");
                        comment2.AutoFit = true;

                        // Adjust comment size based on content
                        int width = Math.Min(300, Math.Max(150, totalSingers.Length / 2));
                        int height = Math.Min(200, Math.Max(50, session.TotalSingers.Count * 15));
                        comment2.From.Column = comment2.From.Column;
                        comment2.From.Row = comment2.From.Row;
                        comment2.To.Column = comment2.From.Column + width / 7;  // Adjust divisor as needed
                        comment2.To.Row = comment2.From.Row + height / 15;      // Adjust divisor as needed
                    }

                    row++;
                }

                // Style and format
                var range = workSheet.Cells[4, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Column(1).Style.Numberformat.Format = "mmm d, yyyy";
                workSheet.Cells.AutoFitColumns();

                // Return the Excel file
                try
                {
                    byte[] fileData = excel.GetAsByteArray();
                    string filename = "Filtered Attendance Report.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(fileData, mimeType, filename);
                }
                catch (Exception ex)
                {
                    return Content("Could not build and download the file: " + ex.Message);
                }
            }
        }









        //archive and unarchiving
        [HttpPost]
        public async Task<IActionResult> Archive(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            session.IsArchived = true;
            await _context.SaveChangesAsync();



            TempData["SuccessMessage"] = "The Data has been archived successfully!";
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> UnArchive(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            session.IsArchived = false;
            _context.Update(session);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "This archive has been activated successfully!";
            return RedirectToAction(nameof(Index));
        }

        //ViewData["AvailableSingers_" + s.Location.ID] as int? ?? 0;

        // Calendar Fetching Action Method for Sessions
        [HttpGet]
        public async Task<IActionResult> GetCalendarSessions()
        {
            // Fetch all sessions with Location and Attendance
            var sessions = await _context.Sessions
                .Where(s => !s.IsArchived) // Exclude archived sessions
                .Include(s => s.Location) // Include Location
                .Include(s => s.Attendance) // Include Attendance to count attendees
                .ToListAsync();

            // Calculate the total singers available for each city
            var availableSingersPerCity = await _context.Singers
                .GroupBy(s => s.LocationID)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            // Prepare the response
            var sessionData = sessions.Select(s => new
            {
                id = s.ID,
                title = $"{s.Location?.City ?? "No Location"} ({s.Attendance.Count(s => s.Status)}/{availableSingersPerCity.GetValueOrDefault(s.Location?.ID ?? 0, 0)})", // Include Location and Attendance count over total singers
                start = s.Date?.ToString("yyyy-MM-dd"), // Use the session date
                allDay = true, // Sessions are all-day events
                notes = s.Notes, // Include notes if needed
                location = s.Location != null ? s.Location.City : "No Location" // Include location name
            }).ToList();

            return Json(sessionData);
        }

        // Get Session Details for Pop-up
        [HttpGet]
        public async Task<IActionResult> GetSessionDetails(int id)
        {
            var session = await _context.Sessions
                .Include(s => s.Location) // Include location details
                .FirstOrDefaultAsync(s => s.ID == id);

            if (session == null)
            {
                return NotFound();
            }

            // Return session details as JSON
            return Json(new
            {
                id = session.ID,
                date = session.Date?.ToString("yyyy-MM-dd"), // Format date
                notes = session.Notes,
                location = session.Location?.City, // Include location name
                isArchived = session.IsArchived
            });
        }

        private async Task<Director?> GetCurrentDirectorAsync()
        {
            var userEmail = User.Identity?.Name; 
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }

            // Check if the user is an Admin
            if (User.IsInRole("Admin"))
            {
                return null; 
            }

           
            return await _context.Directors
                .Include(d => d.DirectorLocations)
                .ThenInclude(dl => dl.Location)
                .FirstOrDefaultAsync(d => d.Email == userEmail);
        }
        // For Calendar View
        public IActionResult Calendar()
        {
            return View();
        }

        // Chart methods
        // Doughnut Chart - Sessions by City
        [HttpGet]
        public async Task<IActionResult> GetSessionsByCityData()
        {
            var sessionsByCity = await _context.Sessions
                .Include(s => s.Location)
                .GroupBy(s => s.Location.City)
                .Select(g => new
                {
                    City = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var labels = sessionsByCity.Select(s => s.City).ToArray();
            var data = sessionsByCity.Select(s => s.Count).ToArray();

            return Json(new { labels, data });
        }

        // Active vs. Archived Session
        [HttpGet]
        public async Task<IActionResult> GetActiveVsArchivedSessionsData()
        {
            var activeSessionsCount = await _context.Sessions.CountAsync(s => !s.IsArchived);
            var archivedSessionsCount = await _context.Sessions.CountAsync(s => s.IsArchived);

            var labels = new[] { "Active", "Archived" };
            var data = new[] { activeSessionsCount, archivedSessionsCount };

            return Json(new { labels, data });
        }

        // Upcoming Session Widget
        [HttpGet]
        public async Task<IActionResult> GetUpcomingSessions()
        {
            var upcomingSessions = await _context.Sessions
                .Where(s => s.Date <= DateTime.Today && !s.IsArchived)
                .Include(s => s.Location)
                .OrderByDescending(s => s.Date) // Changed to descending order
                .Take(10) // Take only 10 most recent
                .Select(s => new
                {
                    id = s.ID,
                    title = s.Location.City, // City name
                    date = s.Date.Value.ToString("yyyy-MM-dd") // Session date
                })
                .ToListAsync();

            return Json(upcomingSessions);
        }

        // Sessions Count
        [HttpGet]
        public async Task<JsonResult> GetTotalSessionCount()
        {
            try
            {
                // Count all sessions, regardless of their archived status
                var totalCount = await _context.Sessions.CountAsync();
                return Json(new { TotalCount = totalCount });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog or NLog)
                Console.Error.WriteLine($"Error in GetTotalSessionCount: {ex.Message}");
                return Json(new { TotalCount = 0 }); // Return a default value in case of error
            }
        }

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.ID == id);
        }
    }
}
