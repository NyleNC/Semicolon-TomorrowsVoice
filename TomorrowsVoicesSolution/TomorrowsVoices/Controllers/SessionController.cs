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
using TomorrowsVoices.Data.TVMigrations;

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
        public async Task<IActionResult> Index(int? page, int? pageSizeID)
        {
            var sessions = _context.Sessions
                .Include(s => s.Location).ThenInclude(l => l.Director)
                .Include(s => s.Attendance).ThenInclude(a => a.Singer)
                .AsNoTracking();


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
                .Include(s => s.Location).ThenInclude(l => l.Director)
                .Include(s => s.Attendance).ThenInclude(a => a.Singer)
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

                    ViewBag.PresentSingersCount = $"{presentSingersCount}/{totalSingersCount}";
                    ViewBag.AbsentSingersCount = $"{absentSingersCount}/{totalSingersCount}";



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

            var session = await _context.Sessions
                .Include(s => s.Location).ThenInclude(l => l.Director)
                .Include(s => s.Attendance).ThenInclude(a => a.Singer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (session == null)
            {
                return NotFound();
            }
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "ID", session.LocationID);
            PopulateAssignedSingerData(session);
            return View(session);
        }

        // POST: Session/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions)
        {
        
            var sessionToUpdate = await _context.Sessions
                .Include(s => s.Location).ThenInclude(l => l.Director)
                .Include(s => s.Attendance).ThenInclude(a => a.Singer)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (sessionToUpdate == null)
            {
               return NotFound();
            }
             
            UpdateSessionSingers(selectedOptions, sessionToUpdate);
            

            if (await TryUpdateModelAsync<Session>(sessionToUpdate, "",
                s => s.Date, s => s.Notes, s => s.Location))
            {
                try
                {
                    _context.Update(sessionToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { sessionToUpdate.ID});
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(sessionToUpdate.ID))
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
               
            }
    
        
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "ID", sessionToUpdate.LocationID);
            PopulateAssignedSingerData(sessionToUpdate);
            return View(sessionToUpdate);
        }

        // GET: Session/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Location).ThenInclude(l => l.Director)
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
                .Include(s => s.Location).ThenInclude(l => l.Director)
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
                    ModelState.AddModelError("", "Unable to Delete Session. Remember, you cannot delete a Session that has singers assigned.");
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
            return new SelectList(_context.Locations
                .OrderBy(d => d.City)
                , "ID", "City");
        }

        [HttpGet]
        public JsonResult GetDirectorByLocation(int locationId)
        {
            var director = _context.Locations
                .Include(l => l.Director) 
                .FirstOrDefault(l => l.ID == locationId)
                ?.Director?.DirectorFullName;

            return Json(new { directorName = director ?? "No director assigned" });
        }

        private void PopulateAssignedSingerData(Session session)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = _context.Singers;
            var currentOptionsHS = new HashSet<int>(session.Attendance
                .Where(a => a.Status == true)
                .Select(a => a.SingerID));
            //Instead of one list with a boolean, we will make two lists
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();
            foreach (var s in allOptions)
            {
                if (currentOptionsHS.Contains(s.ID))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.FullName
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.FullName
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }
        private void UpdateSessionSingers(string[] selectedOptions, Session sessionToUpdate)
      {
        var allSingerIDs = _context.Singers.Select(s => s.ID).ToHashSet(); // Get all singers
        var selectedOptionsHS = new HashSet<int>(selectedOptions.Select(int.Parse)); 

        // Get all current attendance records for this session
        var currentAttendance = sessionToUpdate.Attendance.ToList();

        foreach (var singerID in allSingerIDs)  
        {
            var existingAttendance = currentAttendance.FirstOrDefault(a => a.SingerID == singerID);

            if (selectedOptionsHS.Contains(singerID)) // Singer was selected
            {
                if (existingAttendance == null) // If not already in attendance, add it with Status = true
                {
                    sessionToUpdate.Attendance.Add(new Attendance
                    {
                        SingerID = singerID,
                        SessionID = sessionToUpdate.ID,
                        Status = true
                    });
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
                    sessionToUpdate.Attendance.Add(new Attendance
                    {
                        SingerID = singerID,
                        SessionID = sessionToUpdate.ID,
                        Status = false
                    });
                }
            }
        }
           
      }

        public IActionResult AttendanceReportExport()
        {
            var sessAtts = _context.Sessions
                .Include(s => s.Location).ThenInclude(l => l.Director)
                .Include(s => s.Attendance).ThenInclude(a => a.Singer)
                .OrderBy(s => s.Date)
                .Select(x => new
                {
                    x.Date,
                    Attendance = $"{x.Attendance.Count(a => a.Status)}/{x.Attendance.Count}", 
                    x.Location.City,
                    Director = x.Location.Director.DirectorFullName
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
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 5])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        Rng.Style.Fill.BackgroundColor.SetColor(Color.LightPink);
                    }

                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 5])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightSalmon);
                    }

                    workSheet.Cells[3, 1].Value = "Date";
                    workSheet.Cells[3, 2].Value = "Attendance";
                    workSheet.Cells[3, 3].Value = "City";
                    workSheet.Cells[3, 4].Value = "Director";
                    workSheet.Cells[3, 5].Value = "Notes";



                    workSheet.Cells[3,1].LoadFromCollection(sessAtts, true);
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

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.ID == id);
        }



      


    }
}
