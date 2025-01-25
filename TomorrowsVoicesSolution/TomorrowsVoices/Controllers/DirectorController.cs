using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MedicalOffice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;

namespace TomorrowsVoices.Controllers
{
    public class DirectorController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public DirectorController(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        // GET: Director
        public async Task<IActionResult> Index(string? SearchString, string? SearchEmail, string? SearchCity,string? actionButton, string sortDirection = "asc", string sortField = "Director")
        {
            var directors = await _context.Directors
                .Include(d => d.Location) // Include Location for each Director
                .AsNoTracking()
                .ToListAsync();

            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            if (!String.IsNullOrEmpty(SearchString))
            {
                directors = directors.Where(p => p.LastName != null && p.LastName.Contains(SearchString)
                                            || p.FirstName != null && p.FirstName.Contains(SearchString))
                                     .ToList();
                numberFilters++;
            }
            if (!String.IsNullOrEmpty(SearchEmail))
            {
                directors = directors.Where(p => p.Email != null && p.Email.Contains(SearchEmail))
                                     .ToList();
                numberFilters++;
            }
            if (!string.IsNullOrEmpty(SearchCity))
            {
                directors = directors.Where(p => p.Location != null && p.Location.City.ToString().Contains(SearchCity, StringComparison.OrdinalIgnoreCase))
                                     .ToList();
                numberFilters++;
            }
            string[] sortOptions = new[] { "Director", "City", "Email" };
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                if (sortOptions.Contains(actionButton))
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; //Sort by the button clicked
                }
            }
            // sorting functionality
            if (sortField == "Director")
            {
                if (sortDirection == "asc")
                {
                    directors = directors
                        .OrderBy(p => p.LastName)
                        .ThenBy(p => p.FirstName)
                        .ToList();
                }
                else
                {
                    directors = directors
                        .OrderByDescending(p => p.LastName)
                        .ThenByDescending(p => p.FirstName)
                        .ToList();
                }
            }
            else if (sortField == "Email")
            {
                if (sortDirection == "asc")
                {
                    directors = directors
                        .OrderBy(p => p.Email)
                        .ThenBy(p => p.LastName)
                        .ThenBy(p => p.FirstName)
                        .ToList();
                }
                else
                {
                    directors = directors
                        .OrderByDescending(p => p.Email)
                        .ThenByDescending(p => p.LastName)
                        .ThenByDescending(p => p.FirstName)
                        .ToList();
                }
            }
            else if (sortField == "City")
            {
                if (sortDirection == "asc")
                {
                    directors = directors
                        .OrderBy(p => p.Location?.City)
                        .ThenBy(p => p.LastName)
                        .ThenBy(p => p.FirstName)
                        .ToList();
                }
                else
                {
                    directors = directors
                        .OrderByDescending(p => p.Location?.City)
                        .ThenByDescending(p => p.LastName)
                        .ThenByDescending(p => p.FirstName)
                        .ToList();
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["numberFilters"] = numberFilters;
            var cityList = directors.Select(d => d.Location?.City.ToString())
                                    .Distinct()
                                    .Where(city => city != null)
                                    .Select(city => new SelectListItem
                                    {
                                        Value = city,
                                        Text = city
                                    }).ToList();

            // Add a default option for "All Conditions"
            cityList.Insert(0, new SelectListItem { Value = "", Text = "All Cities" });

            // Set the ViewData for Cities dropdown
            ViewData["Cities"] = cityList;
            return View(directors);
        }

        // GET: Director/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Directors
                .Include(d=>d.Location)
           
                .FirstOrDefaultAsync(m => m.ID == id);
            if (director == null)
            {
                return NotFound();
            }

            return View(director);
        }

        // GET: Director/Create
        public IActionResult Create()
        {
            Director director = new Director();
            PopulateDropDownLists();
            return View();
        }

        // POST: Director/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,Location.City")] Director director)
        {
            try {
                if (ModelState.IsValid)
                {
                    // Ensure the Location object is properly initialized
                    if (director.Location == null)
                    {
                        director.Location = new Location();
                    }

                    _context.Add(director);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Director.Email"))
                {
                    ModelState.AddModelError("Email", "Unable to save changes. Remember, " +
                        "You cant have the same email ");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            PopulateDropDownLists(director);
            return View(director);

        }

        // GET: Director/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Directors.FindAsync(id);
            if (director == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(director);
            return View(director);
        }
        // POST: Director/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var directorToUpdate = await _context.Directors
        .Include(d => d.Location) 
        .FirstOrDefaultAsync(d => d.ID == id);
            if (directorToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Director>(directorToUpdate, "",
                  p => p.FirstName, p => p.LastName, p => p.Email, p => p.Location))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DirectorExists(directorToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Email"))
                    {
                        ModelState.AddModelError("Email", "Unable to save changes.Remember,cannot duplicate the same email");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }
            PopulateDropDownLists(directorToUpdate);
            return View(directorToUpdate);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Directors == null)
            {
                return NotFound();
            }

            var appointmentReason = await _context.Directors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (appointmentReason == null)
            {
                return NotFound();
            }

            return View(appointmentReason);
        }

        // POST: AppointmentReason/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Directors == null)
            {
                return Problem("Entity set 'MedicalOfficeContext.AppointmentReasons'  is null.");
            }
            var director= await _context.Directors
                  .FirstOrDefaultAsync(m => m.ID == id);
            try
            {
                if (director != null)
                {
                    _context.Directors.Remove(director);
                }
                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                ExceptionMessageVM msg = new();
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    msg.ErrProperty = "";
                    msg.ErrMessage = "Unable to Delete " + ViewData["ControllerFriendlyName"] +
                        ". Remember, you cannot delete a " + ViewData["ControllerFriendlyName"] +
                        " that has related records.";
                }
                ModelState.AddModelError(msg.ErrProperty, msg.ErrMessage);
            }
            return View(director);
        }


        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        {
            string feedBack = string.Empty;

            if (theExcel != null)
            {
                string mimeType = theExcel.ContentType;
                long fileLength = theExcel.Length;

                if (!(mimeType == "" || fileLength == 0)) // Looks like we have a file!!!
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

        
                        if (workSheet.Cells[1, 1].Text == "FirstName" &&
                            workSheet.Cells[1, 2].Text == "LastName" &&
                            workSheet.Cells[1, 3].Text == "City" &&
                            workSheet.Cells[1, 4].Text == "Email")
                        {
                            for (int row = start.Row + 1; row <= end.Row; row++)
                            {
                                Director director = new Director();
                                try
                                {
                               
                                    director.FirstName = workSheet.Cells[row, 1].Text;
                                    director.LastName = workSheet.Cells[row, 2].Text;
                                    string cityName = workSheet.Cells[row, 3].Text;
                                    director.Email = workSheet.Cells[row, 4].Text;

                                    // Validate data before adding
                                    if (string.IsNullOrEmpty(director.FirstName) || string.IsNullOrEmpty(director.LastName) || string.IsNullOrEmpty(director.Email))
                                    {
                                        errorCount++;
                                        feedBack += $"Error: Row {row} has missing required fields.<br />";
                                        continue;
                                    }

                                    if (!_context.Directors.Any(d => d.FirstName == director.FirstName && d.LastName == director.LastName && d.Email == director.Email))
                                    {
                                        if (Enum.TryParse(cityName, true, out City parsedCity))
                                        {
                                            var location = _context.Locations.FirstOrDefault(l => l.City == parsedCity);
                                            if (location == null)
                                            {
                                                location = new Location { City = parsedCity };
                                                _context.Locations.Add(location);
                                                await _context.SaveChangesAsync();
                                            }

                                            director.Location = location;
                                            _context.Directors.Add(director);
                                            await _context.SaveChangesAsync();
                                            successCount++;
                                        }
                                        else
                                        {
                                            errorCount++;
                                            feedBack += $"Error: Invalid city '{cityName}' in row {row}.<br />";
                                        }
                                    }
                                    else
                                    {
                                        errorCount++;
                                        feedBack += $"Error: Director {director.FirstName} {director.LastName} with email {director.Email} is a duplicate.<br />";
                                    }
                                }
                                catch (DbUpdateException dex)
                                {
                                    errorCount++;
                                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                                    {
                                        feedBack += $"Error: Record {director.FirstName} {director.LastName} was rejected as a duplicate.<br />";
                                    }
                                    else
                                    {
                                        feedBack += $"Error: Record {director.FirstName} {director.LastName} caused a database error.<br />";
                                    }
                                    _context.Remove(director);
                                }
                                catch (Exception ex)
                                {
                                    errorCount++;
                                    feedBack += $"Error: An error occurred while processing row {row}. Details: {ex.Message}<br />";
                                }
                            }

                            feedBack += $"Finished Importing {successCount + errorCount} Records with {successCount} inserted and {errorCount} rejected.";
                        }
                        else
                        {
                            feedBack = "Error: Invalid file format. Ensure the first row contains 'FirstName', 'LastName', 'City', and 'Email'.";
                        }
                    }
                    else
                    {
                        feedBack = "Error: That file is not an Excel spreadsheet.";
                    }
                }
                else
                {
                    feedBack = "Error: File appears to be empty.";
                }
            }
            else
            {
                feedBack = "Error: No file uploaded.";
            }

            TempData["Feedback"] = feedBack;
            return RedirectToAction("Index");
        }
        private void PopulateDropDownLists(Director? director = null)
        {
            var dQuery = from d in _context.Directors
                         orderby d.LastName, d.FirstName
                         select d;
            ViewData["DirectorID"] = new SelectList(dQuery, "ID", "DirectorFullName", director?.ID);
        }

        private bool DirectorExists(int id)
        {
            return _context.Directors.Any(e => e.ID == id);
        }
    }
}
