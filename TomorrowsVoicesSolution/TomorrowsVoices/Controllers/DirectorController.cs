using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
        public async Task<IActionResult> Index( string ? SearchString, string? SearchEmail, string? SearchCity, int? page, int? pageSizeID, string? actionButton, bool archived = false, string sortDirection = "asc", string sortField = "Director")
        {
            var directors = _context.Directors
                 .Where(d => d.IsArchived== archived)
                .Include(d => d.Location) 
                .AsNoTracking();
            ViewData["IsArchived"] = archived;
            ViewData["ActiveTab"] = archived ? "archived" : "active";
      
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
                directors = directors.Where(p => p.LastName != null && p.LastName.ToLower().Contains(SearchString.ToLower())
                                                || p.FirstName!= null && p.FirstName.ToLower().Contains(SearchString.ToLower()));

                numberFilters++;
            }
            if (!String.IsNullOrEmpty(SearchEmail))
            {
                directors = directors.Where(p => p.Email != null && p.Email.ToLower().Contains(SearchEmail.ToLower()));

                numberFilters++;
            }

            if (!string.IsNullOrEmpty(SearchCity))
            {
             
                
                    directors = directors
                        .Where(p => p.Location.City != null && p.Location.City == SearchCity); 
                    numberFilters++;
               
            }

            // sorting functionality
            if (sortField == "Director")
            {
                if (sortDirection == "asc")
                {
                    directors = directors
                        .OrderBy(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
                else
                {
                    directors = directors
                        .OrderByDescending(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
            }
            else if (sortField == "Email")
            {
                if (sortDirection == "asc")
                {
                    directors = directors
                        .OrderBy(p => p.Email)
                        .ThenBy(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
                else
                {
                    directors = directors
                        .OrderByDescending(p => p.Email)
                        .ThenBy(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
            }
            else if (sortField == "City")
            {
                if (sortDirection == "asc")
                {
                    directors = directors
                        .OrderBy(p => p.Location.City)
                           .ThenBy(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
                else
                {
                    directors = directors
                        .OrderByDescending(p => p.Location.City)
                              .ThenBy(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["numberFilters"] = numberFilters;
            int archivedCount = await _context.Directors.CountAsync(d => d.IsArchived == true);
            ViewData["numberofArchive"] = archivedCount;

            var cityList = directors.AsEnumerable()
                .Where(d => d.Location?.City != null)
                .Select(d => d.Location.City)
                .Distinct()
                .Select(city => new SelectListItem
                {
                    Value = city,
                    Text = city
                })
                .ToList();

            cityList.Insert(0, new SelectListItem { Value = "", Text = "All Cities" });

     
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
            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City");
            PopulateDropDownLists();
            return View();
        }

        // POST: Director/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,LocationID")] Director director)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validate and add the city if it doesn't exist
                    if (director.LocationID > 0)
                    {
                        var location = await _context.Locations.FindAsync(director.LocationID);
                        if (location == null)
                        {
                            ModelState.AddModelError("LocationID", "Invalid city selected.");
                            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City");
                            return View(director);
                        }

                        // Check if a director is already assigned to this city
                        var existingDirector = await _context.Directors
                            .Include(d => d.Location)
                            .FirstOrDefaultAsync(d => d.LocationID == director.LocationID);

                        if (existingDirector != null)
                        {
                            ModelState.AddModelError("LocationID", $"Someone is already assigned to this City: {location.City}");
                            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City");
                            return View(director);
                        }

                        director.Location = location;
                    }

                    _context.Add(director);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"{director.DirectorFullName} successfully added in the city of {director.Location.City}";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dex)
                {
                    var baseMessage = dex.GetBaseException().Message;

                    if (baseMessage.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("Email", "Unable to save changes. Remember, you can't have the same email.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }

            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City");
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
                .FirstOrDefaultAsync(d => d.ID == id);
            if (director == null)
            {
                return NotFound();
            }

            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City", director.LocationID);
            return View(director);
        }
        // POST: Director/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Email,LocationID")] Director director)
        {
            if (id != director.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var directorToUpdate = await _context.Directors
                        .Include(d => d.Location)
                        .FirstOrDefaultAsync(d => d.ID == id);

                    if (directorToUpdate == null)
                    {
                        return NotFound();
                    }

                    if (await TryUpdateModelAsync<Director>(
                        directorToUpdate,
                        "",
                        d => d.FirstName, d => d.LastName, d => d.Email, d => d.LocationID))
                    {
                        var location = await _context.Locations.FindAsync(directorToUpdate.LocationID);
                        if (location == null)
                        {
                            ModelState.AddModelError("LocationID", "Invalid city selected.");
                            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City", directorToUpdate.LocationID);
                            return View(directorToUpdate);
                        }

                        directorToUpdate.Location = location;

                        _context.Update(directorToUpdate);
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = $"{directorToUpdate.DirectorFullName} has been edited and saved";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DirectorExists(director.ID))
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

            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City", director.LocationID);
            return View(director);
        }

        // GET: Director/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Directors
                  .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (director == null)
            {
                return NotFound();
            }

            return View(director);
        }

        // Delete is finally working , we can finally Delete director without an error
        // POST: Director/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var director = await _context.Directors.FindAsync(id);

            if (director == null)
            {
                return NotFound();
            }

            try
            {
                _context.Directors.Remove(director);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while trying to Delete the Director. Please try again.");
                return View(director);
            }
        }

        private void PopulateDropDownLists(Director? director = null)
        {
            var dQuery = from d in _context.Directors
                         orderby d.LastName, d.FirstName
                         select d;
            ViewData["DirectorID"] = new SelectList(dQuery, "ID", "DirectorFullName", director?.ID);
        }

        //ImportExcel 
        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        {
            string feedBack = string.Empty;
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
                                       
                                            var location = _context.Locations.FirstOrDefault(l => l.City == cityName);
                                            if (location == null)
                                            {
                                                location = new Location { City = cityName };
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
                        TempData["Success"] = $"{successCount} directors successfully added.";
                        
                    }
                 
                    TempData["Feedback"] = feedBack;

                }
            }
            return RedirectToAction("Index");
        }
        //Autocomplete for City
        public JsonResult CitySuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<object>());
            }

            var suggestions = _context.Locations
                .Where(c => c.City.ToLower().StartsWith(term.ToLower())) 
                .Select(c => new { id = c.ID, text = c.City })
                .ToList();

            return Json(suggestions);
         }
        public JsonResult GetInitialCities()
        {
            var cities = _context.Locations
                .Select(c => new { id = c.ID, text = c.City })
                .ToList();

            return Json(cities);
        }
        [HttpPost]
        public JsonResult AddCity(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
            {
                return Json(new { success = false, message = "City name cannot be empty." });
            }

            bool exists = _context.Locations.Any(c => c.City.ToLower() == cityName.ToLower());
            if (exists)
            {
                return Json(new { success = false, message = "City already exists." });
            }

            var newCity = new Location { City = cityName };
            _context.Locations.Add(newCity);
            _context.SaveChanges();

            // Return the new city's ID so it can be selected
            return Json(new { success = true, cityId = newCity.ID });
        }

        //archive and unarchiving
        [HttpPost]
        public async Task<IActionResult> Archive(int id)
        {
            var director = await _context.Directors.FindAsync(id);
            if (director == null)
            {
                return NotFound();
            }

            director.IsArchived = true;
            await _context.SaveChangesAsync();

       

        TempData["SuccessMessage"] = "The Data has been archived successfully!";
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> UnArchive(int id)
        {
            var director = await _context.Directors.FindAsync(id);
            if (director == null)
            {
                return NotFound();
            }

            director.IsArchived = false;
            _context.Update(director);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "This archive has been activated successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool DirectorExists(int id)
        {
            return _context.Directors.Any(e => e.ID == id);
        }
    }
}
