using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class DirectorController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public DirectorController(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        // GET: Director

        public async Task<IActionResult> Index(string? SearchString, string? SearchEmail, string? SearchCity, string? SearchPhone, int? page, int? pageSizeID, string? actionButton, bool archived = false, string sortDirection = "asc", string sortField = "Director")
        {
            var currentDirector = await GetCurrentDirectorAsync();

            // Fetch all directors if the user is an Admin
            var directors = _context.Directors
                .Where(d => d.IsArchived == archived)
                .Include(d => d.DirectorLocations)
                .ThenInclude(d => d.Location)
                .AsNoTracking();

            // Apply city-based filtering only for Directors
            if (currentDirector != null)
            {
                var assignedCityIds = currentDirector.DirectorLocations.Select(dl => dl.LocationID).ToList();
                directors = directors.Where(d => d.DirectorLocations.Any(dl => assignedCityIds.Contains(dl.LocationID)));
            }

            // Rest of the filtering, sorting, and paging logic remains the same
            ViewData["IsArchived"] = archived;
            ViewData["ActiveTab"] = archived ? "archived" : "active";

            string[] sortOptions = new[] { "Director", "City", "Email" };
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            if (!String.IsNullOrEmpty(actionButton)) // Form Submitted!
            {
                page = 1; // Reset page to start
                if (sortOptions.Contains(actionButton))
                {
                    if (actionButton == sortField) // Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; // Sort by the button clicked
                }
            }

            if (!String.IsNullOrEmpty(SearchString))
            {
                directors = directors.Where(p => (p.LastName != null && p.LastName.ToLower().Contains(SearchString.ToLower()))
                                         || (p.FirstName != null && p.FirstName.ToLower().Contains(SearchString.ToLower()))
                                         || ((p.FirstName + " " + p.LastName).ToLower().Contains(SearchString.ToLower())));
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
                    .Where(p => p.DirectorLocations.Any(l => l.Location.City != null && l.Location.City == SearchCity));
                numberFilters++;
            }

            // Sorting functionality
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
            else if (sortField == "Phone")
            {
                if (sortDirection == "asc")
                {
                    directors = directors
                        .OrderBy(p => p.dirPhoneNumber)
                        .ThenBy(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
                else
                {
                    directors = directors
                        .OrderByDescending(p => p.dirPhoneNumber)
                        .ThenBy(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
            }
            else if (sortField == "City")
            {
                if (sortDirection == "asc")
                {
                    directors = directors
                        .OrderBy(p => p.DirectorLocations.FirstOrDefault().Location.City)
                        .ThenBy(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
                else
                {
                    directors = directors
                        .OrderByDescending(p => p.DirectorLocations.FirstOrDefault().Location.City)
                        .ThenBy(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["numberFilters"] = numberFilters;

            int archivedCount = await _context.Directors.CountAsync(d => d.IsArchived == true);
            ViewData["numberofArchive"] = archivedCount;

            int activeCount = await _context.Directors.CountAsync(d => d.IsArchived == false);
            ViewData["numberofActive"] = activeCount;

            var cityList = directors.AsEnumerable()
                .Where(d => d.DirectorLocations.FirstOrDefault()?.Location?.City != null)
                .Select(d => d.DirectorLocations.FirstOrDefault()?.Location?.City)
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

            var pagedData = await PaginatedList<Director>.CreateAsync(directors.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        // GET: Director/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Directors
                .Include(d => d.DirectorLocations)
                .ThenInclude(d => d.Location)


                .FirstOrDefaultAsync(m => m.ID == id);
            if (director == null)
            {
                return NotFound();
            }

            return View(director);
        }

        // GET: Director/Create
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email")] Director director, int? locationId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the selected location from the database
                    if (locationId.HasValue)
                    {
                        var location = await _context.Locations.FindAsync(locationId.Value);
                        if (location == null)
                        {
                            ModelState.AddModelError("LocationID", "Invalid city selected.");
                            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City");
                            return View(director);
                        }

                        // Initialize the DirectorLocations collection and add the selected location
                        director.DirectorLocations = new List<DirectorLocation>
                {
                    new DirectorLocation
                    {
                        Location = location
                    }
                };
                    }

                    _context.Add(director);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"{director.DirectorFullName} successfully added to the city of {director.DirectorLocations.FirstOrDefault().Location.City}.";
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

            // Repopulate the locations dropdown in case of validation errors
            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City");
            return View(director);
        }
        // GET: Director/Edit/5
        [Authorize(Roles = "Admin,Director")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Directors
                .Include(d => d.DirectorLocations)
                      .ThenInclude(d => d.Location)

                .FirstOrDefaultAsync(d => d.ID == id);
            if (director == null)
            {
                return NotFound();
            }

            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City", director.DirectorLocations.FirstOrDefault()?.LocationID);
            return View(director);
        }
        // POST: Director/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Director")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Email")] Director director, int? locationId)
        {
            if (id != director.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the director to update, including the related DirectorLocations and Location
                    var directorToUpdate = await _context.Directors
                        .Include(d => d.DirectorLocations)
                        .ThenInclude(dl => dl.Location)
                        .FirstOrDefaultAsync(d => d.ID == id);

                    if (directorToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Update scalar properties
                    if (await TryUpdateModelAsync(directorToUpdate, "",
                        d => d.FirstName, d => d.LastName, d => d.Email))
                    {
                        // Fetch the new location from the database
                        if (locationId.HasValue)
                        {
                            var location = await _context.Locations.FindAsync(locationId.Value);
                            if (location == null)
                            {
                                ModelState.AddModelError("LocationID", "Invalid city selected.");
                                ViewBag.CityList = new SelectList(_context.Locations, "ID", "City", locationId);
                                return View(directorToUpdate);
                            }

                            // Remove the existing DirectorLocation (if any)
                            var existingDirectorLocation = directorToUpdate.DirectorLocations.FirstOrDefault();
                            if (existingDirectorLocation != null)
                            {
                                _context.DirectorLocations.Remove(existingDirectorLocation);
                                await _context.SaveChangesAsync(); // Save changes to delete the existing entity
                            }

                            // Add a new DirectorLocation with the updated Location
                            directorToUpdate.DirectorLocations.Add(new DirectorLocation
                            {
                                Location = location
                            });

                            await _context.SaveChangesAsync(); // Save changes to add the new entity
                        }

                        TempData["SuccessMessage"] = $"{directorToUpdate.FirstName} {directorToUpdate.LastName} has been edited and saved.";
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
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact support.");
                }
            }

            // Repopulate the locations dropdown in case of validation errors
            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City", locationId);
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

        //ImportExcel - Updated for Modal View
        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        {
            var response = new { success = false, message = "" };

            if (theExcel == null || theExcel.Length == 0)
            {
                response = new { success = false, message = "❌ No file uploaded. Please select an Excel file." };
                return Json(response);
            }

            string feedbackMessage = "";
            int successCount = 0, errorCount = 0;

            try
            {
                string mimeType = theExcel.ContentType;
                if (!mimeType.Contains("excel") && !mimeType.Contains("spreadsheet"))
                {
                    response = new { success = false, message = "⚠️ Invalid file format. Please upload a valid Excel file." };
                    return Json(response);
                }

                using (var memoryStream = new MemoryStream())
                {
                    await theExcel.CopyToAsync(memoryStream);
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var workSheet = package.Workbook.Worksheets[0];
                        var start = workSheet.Dimension.Start;
                        var end = workSheet.Dimension.End;

                        // Validate headers
                        if (workSheet.Cells[1, 1].Text != "FirstName" ||
                            workSheet.Cells[1, 2].Text != "LastName" ||
                            workSheet.Cells[1, 3].Text != "City" ||
                            workSheet.Cells[1, 4].Text != "Email")
                        {
                            response = new { success = false, message = "❌ Invalid Excel format. Please ensure the file has 'FirstName', 'LastName', 'City', and 'Email' headers." };
                            return Json(response);
                        }

                        // Track duplicate emails within the file
                        var emailSet = new HashSet<string>();

                        for (int row = start.Row + 1; row <= end.Row; row++)
                        {
                            Director director = new Director();
                            try
                            {
                                director.FirstName = workSheet.Cells[row, 1].Text;
                                director.LastName = workSheet.Cells[row, 2].Text;
                                string cityName = workSheet.Cells[row, 3].Text;
                                director.Email = workSheet.Cells[row, 4].Text;

                                // Validate required fields
                                if (string.IsNullOrEmpty(director.FirstName) || string.IsNullOrEmpty(director.LastName) || string.IsNullOrEmpty(director.Email))
                                {
                                    errorCount++;
                                    feedbackMessage += $"⚠️ Error: Row {row} has missing fields.<br>";
                                    continue;
                                }

                                // Check for duplicate emails within the file
                                if (emailSet.Contains(director.Email))
                                {
                                    errorCount++;
                                    feedbackMessage += $"⚠️ Error: Row {row} - Duplicate email '{director.Email}' found within the file.<br>";
                                    continue;
                                }

                                // Check for duplicate emails in the database
                                if (_context.Directors.Any(d => d.Email == director.Email))
                                {
                                    errorCount++;
                                    feedbackMessage += $"⚠️ Error: Row {row} - Director with email '{director.Email}' already exists in the database.<br>";
                                    continue;
                                }

                                // Add email to the set to track duplicates within the file
                                emailSet.Add(director.Email);

                                // Handle location
                                var location = _context.Locations.FirstOrDefault(l => l.City == cityName);
                                if (location == null)
                                {
                                    location = new Location { City = cityName };
                                    _context.Locations.Add(location);
                                    await _context.SaveChangesAsync();
                                }

                                // Initialize the DirectorLocations collection
                                director.DirectorLocations = new List<DirectorLocation>
                        {
                            new DirectorLocation
                            {
                                Location = location
                            }
                        };

                                // Add director to the database
                                _context.Directors.Add(director);
                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                errorCount++;
                                feedbackMessage += $"⚠️ Error: Exception in row {row} - {ex.Message}<br>";
                            }
                        }

                        // Save changes to the database
                        await _context.SaveChangesAsync();

                        // Prepare response
                        if (successCount > 0)
                        {
                            response = new { success = true, message = $"✅ {successCount} directors added successfully.<br>{feedbackMessage}" };
                        }
                        else
                        {
                            response = new { success = false, message = $"❌ No directors were added.<br>{feedbackMessage}" };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = new { success = false, message = $"❌ An error occurred: {ex.Message}" };
            }

            return Json(response);
        }

        // Excel Template Server
        public IActionResult DownloadSampleExcel()
        {
            // Path to the sample Excel file in your project
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExcelTemplates", "DirectorTemplate.xlsx");

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Serve the file for download
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DirectorTemplate.xlsx");
        }

        //Autocomplete for City
        public JsonResult CitySuggestions(string term)

        {

            if (string.IsNullOrWhiteSpace(term))

            {

                var allCities = _context.Locations

                    .Select(c => new { id = c.ID, text = c.City })

                    .ToList();



                return Json(allCities);

            }

            else

            {

                var filteredCities = _context.Locations

                    .Where(c => c.City.ToLower().Contains(term.ToLower()))

                    .Select(c => new { id = c.ID, text = c.City })

                    .ToList();



                return Json(filteredCities);

            }

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Archive(int id)
        {
            var director = await _context.Directors.FindAsync(id);
            if (director == null)
            {
                return NotFound();
            }

            director.IsArchived = true;
            await _context.SaveChangesAsync();



            TempData["SuccessMessage"] = "The Data has been archived successfully! ";
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        private async Task<Director?> GetCurrentDirectorAsync()
        {
            var userEmail = User.Identity?.Name; // Assuming the email is stored in the claims
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }
            // Check if the user is an Admin
            if (User.IsInRole("Admin"))
            {
                return null; // Admins bypass city restrictions
            }

            // Fetch the Director for non-Admin users
            return await _context.Directors
                .Include(d => d.DirectorLocations)
                .ThenInclude(dl => dl.Location)
                .FirstOrDefaultAsync(d => d.Email == userEmail);
        }

        // Chart Methods
        // Pie Chart - Director by City
        public JsonResult GetDirectorsByCityForDoughnut()
        {
            var directorsByCity = _context.Directors
                .Include(d => d.DirectorLocations)
                .ThenInclude(dl => dl.Location)
                .GroupBy(d => d.DirectorLocations.FirstOrDefault().Location.City)
                .Select(g => new
                {
                    City = g.Key,
                    Count = g.Count()
                })
                .ToList();

            return Json(directorsByCity);
        }

        // Active vs. Archived Directors
        public JsonResult GetActiveVsArchivedDirectors()
        {
            var activeCount = _context.Directors.Count(d => d.IsArchived == false);
            var archivedCount = _context.Directors.Count(d => d.IsArchived == true);

            var data = new
            {
                Labels = new[] { "Active", "Archived" },
                Counts = new[] { activeCount, archivedCount }
            };

            return Json(data);
        }

        // Director Count
        [HttpGet]
        public async Task<JsonResult> GetTotalDirectorCount()
        {
            try
            {
                var totalCount = await _context.Directors.CountAsync();
                return Json(new { TotalCount = totalCount });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog or NLog)
                Console.Error.WriteLine($"Error in GetTotalDirectorCount: {ex.Message}");
                return Json(new { TotalCount = 0 }); // Return a default value in case of error
            }
        }

        private bool DirectorExists(int id)
        {
            return _context.Directors.Any(e => e.ID == id);
        }
    }
}
