using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;

namespace TomorrowsVoices.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public VolunteerController(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        // GET: Volunteer
        public async Task<IActionResult> Index()
        {
            var tomorrowsVoicesContext = _context.Volunteers.Include(v => v.VolLocation);
            return View(await tomorrowsVoicesContext.ToListAsync());
        }

        // GET: Volunteer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers
                .Include(v => v.VolLocation)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // GET: Volunteer/Create
        public IActionResult Create()
        {
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City");
            return View();
        }

        // POST: Volunteer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Phone,VolLocationID")] Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(volunteer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", volunteer.VolLocationID);
            return View(volunteer);
        }

        // GET: Volunteer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", volunteer.VolLocationID);
            return View(volunteer);
        }

        // POST: Volunteer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Phone,VolLocationID")] Volunteer volunteer)
        {
            if (id != volunteer.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(volunteer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerExists(volunteer.ID))
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
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", volunteer.VolLocationID);
            return View(volunteer);
        }

        // GET: Volunteer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers
                .Include(v => v.VolLocation)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }
        [HttpPost]
        public async Task<IActionResult> InsertSingersFromExcel(IFormFile theExcel)
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
                        if (workSheet.Cells[1, 1].Text.Trim() == "FirstName" &&
                            workSheet.Cells[1, 2].Text.Trim() == "LastName" &&
                            workSheet.Cells[1, 3].Text.Trim() == "City" &&
                            workSheet.Cells[1, 4].Text.Trim() == "Phone" &&
                            workSheet.Cells[1, 5].Text.Trim() == "Email")
                        {
                            for (int row = start.Row + 1; row <= end.Row; row++)
                            {
                                Singer singer = new Singer();
                                try
                                {
                                    singer.FirstName = workSheet.Cells[row, 1].Text.Trim();
                                    singer.LastName = workSheet.Cells[row, 2].Text.Trim();
                                    string cityName = workSheet.Cells[row, 3].Text.Trim();
                                    singer.EmergencyContactName = workSheet.Cells[row, 4].Text.Trim();
                                    singer.EmergencyContactNumber = workSheet.Cells[row, 5].Text.Trim();

                                    // Validate data before adding
                                    if (string.IsNullOrEmpty(singer.FirstName) ||
                                        string.IsNullOrEmpty(singer.LastName) ||
                                        string.IsNullOrEmpty(cityName) ||
                                        string.IsNullOrEmpty(singer.EmergencyContactName) ||
                                        string.IsNullOrEmpty(singer.EmergencyContactNumber))
                                    {
                                        errorCount++;
                                        feedback += $"Error: Row {row} has missing fields.<br />";
                                        continue; // Skip invalid row
                                    }

                                    if (!Regex.IsMatch(singer.EmergencyContactNumber, @"^\d{10}$"))
                                    {
                                        errorCount++;
                                        feedback += $"Error: Invalid phone number in row {row}.<br />";
                                        continue;
                                    }

                                    if (!_context.Singers.Any(s => s.FirstName == singer.FirstName && s.LastName == singer.LastName))
                                    {
                                        var location = _context.Locations.FirstOrDefault(l => l.City == cityName && l.DirectorID != null);
                                        if (location == null)
                                        {
                                            errorCount++;
                                            feedback += $"Error:<b>{singer.FullName}</b> is from <b>{cityName}</b>, but there is no <b>Director</b> assigned for this city.<br/>";
                                            continue;
                                        }
                                        singer.Location = location;
                                        _context.Singers.Add(singer);
                                        successCount++;
                                    }
                                    else
                                    {
                                        errorCount++;
                                        feedback += $"Error: Singer {singer.FullName} already exists.<br />";
                                    }
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

                        TempData["Success"] = $"<b>{successCount}</b> singers successfully added.";
                    }

                    TempData["Feedback"] = feedback;
                }
            }

            return RedirectToAction("Index");
        }


        // POST: Volunteer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer != null)
            {
                _context.Volunteers.Remove(volunteer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VolunteerExists(int id)
        {
            return _context.Volunteers.Any(e => e.ID == id);
        }
    }
}
