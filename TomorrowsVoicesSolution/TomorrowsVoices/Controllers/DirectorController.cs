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
using TomorrowsVoices.Utilities;


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
        public async Task<IActionResult> Index(string? SearchString, string? SearchEmail, string? SearchCity, int? page, int? pageSizeID, string? actionButton, string sortDirection = "asc", string sortField = "Director")
        {
            var directors = _context.Directors
                .Include(d => d.Location) // Include Location for each Director
                .AsNoTracking();
            string[] sortOptions = new[] { "Director", "City", "Email" };
            ViewData["Filtering"] = "btn-outline-secondary";
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

            if (!String.IsNullOrEmpty(SearchString))
            {
                directors = directors.Where(p => p.LastName != null && p.LastName.Contains(SearchString)
                                                || p.FirstName != null && p.FirstName.Contains(SearchString));

                numberFilters++;
            }
            if (!String.IsNullOrEmpty(SearchEmail))
            {
                directors = directors.Where(p => p.Email != null && p.Email.Contains(SearchEmail));

                numberFilters++;
            }

            if (!string.IsNullOrEmpty(SearchCity))
            {
                // If City is an Enum:
                if (Enum.TryParse<City>(SearchCity, true, out var searchCityEnum))
                {
                    directors = directors
                        .Where(p => p.Location != null && p.Location.City == searchCityEnum); 
                    numberFilters++;
                }
                // If City is a string:
                else
                {
                    directors = (IQueryable<Director>)directors
                        .AsEnumerable() 
                        .Where(p => p.Location != null && p.Location.City.ToString().Contains(SearchCity));
                  
                    numberFilters++;
                }
            }

            // sorting functionality
            if (sortField == "Director")
            {
                if (sortDirection == "asc")
                {
                    directors = directors
                        .OrderBy(p => p.LastName)
                        .ThenBy(p => p.FirstName);
                }
                else
                {
                    directors = directors
                        .OrderByDescending(p => p.LastName)
                        .ThenBy(p => p.FirstName);
                }
            }
            else if (sortField == "Email")
            {
                if (sortDirection == "asc")
                {
                    directors = directors
                        .OrderBy(p => p.Email)
                        .ThenBy(p => p.LastName)
                        .ThenBy(p => p.FirstName);
                }
                else
                {
                    directors = directors
                        .OrderByDescending(p => p.Email)
                        .ThenBy(p => p.LastName)
                        .ThenBy(p => p.FirstName);
                }
            }
            else if (sortField == "City")
            {
                if (sortDirection == "asc")
                {
                    directors = directors
                        .OrderBy(p => p.Location.City)
                        .ThenBy(p => p.LastName)
                        .ThenBy(p => p.FirstName);
                }
                else
                {
                    directors = directors
                        .OrderByDescending(p => p.Location.City)
                        .ThenBy(p => p.LastName)
                        .ThenBy(p => p.FirstName);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["numberFilters"] = numberFilters;

            var cityList = directors
                .AsEnumerable()
                .Select(d => d.Location?.City.ToString())
                .Where(city => city != null)
                .Distinct()
                .Select(city => new SelectListItem
                {
                    Value = city,
                    Text = city
                })
                .ToList();

            // Add a default option for "All Cities"
            cityList.Insert(0, new SelectListItem { Value = "", Text = "All Cities" });

            // Set the ViewData for Cities dropdown
            ViewData["Cities"] = cityList;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Director>.CreateAsync(directors.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
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
                    // Check if the Location is null before saving
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

            var director = await _context.Directors
                .Include(d => d.Location)
                .FirstOrDefaultAsync(x => x.ID == id);
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
        // GET: Director/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Directors
                .Include(d => d.Location)
                  .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (director == null)
            {
                return NotFound();
            }

            return View(director);
        }

        // POST: Director/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var director = await _context.Directors
                   .Include(d => d.Location)
                   .FirstOrDefaultAsync(m => m.ID == id);
            try
            {
                if (director != null)
                {
                    _context.Directors.Remove(director);
                }
            }
            catch (DbUpdateException)
            {
                //Note: there is really no reason a delete should fail if you can "talk" to the database.
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //ImportExcel
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

                        if (workSheet.Cells[1, 1].Text == "FirstName" && workSheet.Cells[1, 2].Text == "LastName" && workSheet.Cells[1, 3].Text == "City" && workSheet.Cells[1, 4].Text == "Email")
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
                                        feedBack += $"Error: Row {row} has missing fields.<br />";
                                        continue; // Skip invalid row
                                    }

                                    if (!_context.Directors.Any(d => d.Email == director.Email))
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
                                        feedBack += $"Error: Director with email {director.Email} already exists.<br />";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorCount++;
                                    feedBack += $"Error: Exception in row {row} - {ex.Message}<br />";
                                }
                            }

                            await _context.SaveChangesAsync(); 
                        }
                        else
                        {
                            feedBack += "Error: Invalid Excel file format.<br />";
                        }
                    }
                    TempData["Feedback"] = feedBack;
                    
                }
            }
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
