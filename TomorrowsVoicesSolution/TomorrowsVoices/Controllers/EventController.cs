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

using System.Globalization;
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
        public async Task<IActionResult> Index(int? page, int? pageSizeID, bool archived = false)
        {
            var tomorrowsVoicesContext = _context.Events.Where(d => d.IsArchived == archived).Include(a => a.VolLocation);
            ViewData["IsArchived"] = archived;
            ViewData["ActiveTab"] = archived ? "archived" : "active";
            int archivedCount = await _context.Events.CountAsync(d => d.IsArchived == true);
            ViewData["numberofArchive"] = archivedCount;

            // Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Event>.CreateAsync(tomorrowsVoicesContext.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
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

        //ImportExcel 
        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        {
            string feedback = string.Empty;
            string successMessage = string.Empty;

            if (theExcel == null || theExcel.Length == 0)
            {
                TempData["Feedback"] = "Error: No file uploaded. Please select an Excel file.";
                return RedirectToAction("Index");
            }

            if (theExcel != null)
            {
                string mimeType = theExcel.ContentType;
                long fileLength = theExcel.Length;

                if (!(mimeType == "" || fileLength == 0))
                {
                    if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet"))
                    {
                        ExcelPackage excel;
                        using (var memoryStream = new MemoryStream())
                        {
                            await theExcel.CopyToAsync(memoryStream);
                            excel = new ExcelPackage(memoryStream);
                        }

                        var workSheet = excel.Workbook.Worksheets[0];
                        var start = workSheet.Dimension.Start;
                        var end = workSheet.Dimension.End;

                        int successCount = 0;
                        int errorCount = 0;

                        // Validate column headers
                        if (workSheet.Cells[1, 1].Text.Trim() == "Name" &&
                            workSheet.Cells[1, 2].Text.Trim() == "Location" &&
                            workSheet.Cells[1, 3].Text.Trim() == "Date" &&
                            workSheet.Cells[1, 4].Text.Trim() == "Start Time" &&
                            workSheet.Cells[1, 5].Text.Trim() == "End Time")
                        {
                            for (int row = start.Row + 1; row <= end.Row; row++)
                            {
                                Event events = new Event();
                                try
                                {
                                    events.Name = workSheet.Cells[row, 1].Text.Trim();
                                    string cityName = workSheet.Cells[row, 2].Text.Trim();

                                    // Parse Date
                                    if (DateOnly.TryParse(workSheet.Cells[row, 3].Text.Trim(), out DateOnly date))
                                    {
                                        events.Date = date;
                                    }
                                    else
                                    {
                                        errorCount++;
                                        feedback += $"Error: Invalid date format in row {row}.<br />";
                                        continue;
                                    }

                                    // Parse Start Time
                                    if (TimeOnly.TryParse(workSheet.Cells[row, 4].Text.Trim(), out TimeOnly startTime))
                                    {
                                        events.StartTime = startTime;
                                    }
                                    else
                                    {
                                        errorCount++;
                                        feedback += $"Error: Invalid start time format in row {row}.<br />";
                                        continue;
                                    }

                                    // Parse End Time
                                    if (TimeOnly.TryParse(workSheet.Cells[row, 5].Text.Trim(), out TimeOnly endTime))
                                    {
                                        events.EndTime = endTime;
                                    }
                                    else
                                    {
                                        errorCount++;
                                        feedback += $"Error: Invalid end time format in row {row}.<br />";
                                        continue;
                                    }

                                    // Validate data before adding
                                    if (string.IsNullOrEmpty(events.Name) ||
                                        string.IsNullOrEmpty(cityName))
                                    {
                                        errorCount++;
                                        feedback += $"Error: Row {row} has missing fields.<br />";
                                        continue; // Skip invalid row
                                    }

                                    // Check if event with the same name, date, and location already exists
                                    var location = _context.VolLocations.FirstOrDefault(l => l.City == cityName);

                                    if (location == null)
                                    {
                                        // If location doesn't exist, create a new one
                                        location = new VolLocation { City = cityName };
                                        _context.VolLocations.Add(location);
                                        await _context.SaveChangesAsync(); // Save the new location to get its ID
                                    }

                                    if (_context.Events.Any(e => e.Name == events.Name && e.Date == events.Date && e.VolLocationID == location.ID))
                                    {
                                        errorCount++;
                                        feedback += $"Error: The event {events.Name} on {events.Date.ToShortDateString()} at {cityName} already exists.<br />";
                                        continue;
                                    }

                                    events.VolLocationID = location.ID;
                                    _context.Events.Add(events);
                                    successCount++;
                                }
                                catch (Exception ex)
                                {
                                    errorCount++;
                                    feedback += $"Error: Exception in row {row} - {ex.Message}<br />";
                                }
                            }

                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            feedback += "Error: Invalid Excel file format.<br />";
                        }

                        TempData["Success"] = $"<b>{successCount}</b> events successfully added.";
                    }

                    TempData["Feedback"] = feedback;
                }
            }

            return RedirectToAction("Index");
        }
        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.ID == id);
        }
    }
}
