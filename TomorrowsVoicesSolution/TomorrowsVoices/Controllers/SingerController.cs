using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;
using TomorrowsVoices.Utilities;

namespace TomorrowsVoices.Controllers
{
    [Authorize]
    public class SingerController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public SingerController(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        // GET: Singer

        public async Task<IActionResult> Index(int? page, int? pageSizeID, string? actionButton, string? SearchString, string? SearchCity, bool archived = false, string sortDirection = "asc", string sortField = "Name", string SingerEmergencyContactName = "EmergencyContactName", string SingerEmergencyContactNumber = "EmergencyContactNumber")
        {
           
       
            
            var singers = _context.Singers
                            .Where(d => d.IsArchived == archived)
                .Include(s => s.Location) // Include Location for each Singer
                .AsNoTracking();
            if (!User.IsInRole("Admin"))
            {
                var currentDirector = await GetCurrentDirectorAsync();
                if (currentDirector != null)
                {
                    var assignedCityIds = currentDirector.DirectorLocations.Select(dl => dl.LocationID).ToList();
                    singers = singers.Where(d => d.Location.DirectorLocations.Any(dl => assignedCityIds.Contains(dl.LocationID)));
                }
                else
                {

                    singers = singers.Where(v => false);
                }
            }
            ViewData["IsArchived"] = archived;
            ViewData["ActiveTab"] = archived ? "archived" : "active";
            string[] sortOptions = new[] { "FullName", "Location" };
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
                singers = singers.Where(p => (p.LastName != null && p.LastName.ToLower().Contains(SearchString.ToLower()))
                                     || (p.FirstName != null && p.FirstName.ToLower().Contains(SearchString.ToLower()))
                                     || ((p.FirstName + " " + p.LastName).ToLower().Contains(SearchString.ToLower())));
                numberFilters++;
            }

            if (!string.IsNullOrEmpty(SearchCity))
            {
                singers = singers
               .Where(p => p.Location.City != null && p.Location.City == SearchCity);
                numberFilters++;

            }


            // Sorting logic based on selected field and direction
            if (sortField == "FullName")
            {
                if (sortDirection == "asc")
                {
                    singers = singers
                        .OrderBy(s => s.FirstName)
                        .ThenBy(s => s.LastName);
                }
                else
                {
                    singers = singers
                        .OrderByDescending(s => s.FirstName)
                        .ThenByDescending(s => s.LastName);
                }
            }
            else if (sortField == "Location")
            {
                if (sortDirection == "asc")
                {
                    singers = singers
                        .OrderBy(s => s.Location.City);
                }
                else
                {
                    singers = singers
                        .OrderByDescending(s => s.Location.City);
                }
            }
            else if (sortField == "Location")
            {
                if (sortDirection == "asc")
                {
                    singers = singers
                        .OrderBy(s => s.Location.City)
                        .ThenBy(s => s.FirstName)
                        .ThenBy(s => s.LastName);
                }
                else
                {
                    singers = singers
                        .OrderByDescending(s => s.Location.City)
                        .ThenBy(s => s.FirstName)
                        .ThenBy(s => s.LastName);
                }
            }


            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["numberFilters"] = numberFilters;
            int archivedCount = await _context.Singers.CountAsync(d => d.IsArchived == true);
            ViewData["numberofArchive"] = archivedCount;
            int activeCount = await _context.Singers.CountAsync(d => d.IsArchived == false);
            ViewData["numberofActive"] = activeCount;

            var cityList = singers.AsEnumerable()
        .Where(d => d.Location?.City != null)
        .Select(d => d.Location.City)
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
            ViewData["Location"] = cityList;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Singer>.CreateAsync(singers.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }


        // GET: Singer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var singer = await _context.Singers
                .Include(s => s.Location)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (singer == null) return NotFound();

            return View(singer);
        }
        // GET: Singer/Create
        public IActionResult Create()
        {
            var locations = _context.Locations.ToList();
            if (!locations.Any())
            {
                ModelState.AddModelError("", "No locations found. Please add a location first.");
                return View();
            }

            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City");

            ViewData["Title"] = "Create";
            return View(new Singer());
        }

        // POST: Singer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,LocationID,IsAvailable,EmergencyContactName,EmergencyContactNumber")] Singer singer)
        {
            if (!ModelState.IsValid)
            {
                singer.CreatedOn = DateTime.Now;
                singer.UpdatedOn = DateTime.Now;

                _context.Add(singer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{singer.FullName} has been successfully added in the city of {singer.Location.City}";
                return RedirectToAction(nameof(Index));
            }

            var existingLocation = await _context.Locations.FindAsync(singer.LocationID);
            if (existingLocation == null)
            {
                ModelState.AddModelError("LocationID", "Invalid location selected.");
                ViewBag.CityList = new SelectList(_context.Locations, "ID", "City", singer.LocationID);
                return View(singer);
            }

            singer.Location = existingLocation;
            singer.CreatedOn = DateTime.Now;
            singer.UpdatedOn = DateTime.Now;

            _context.Add(singer);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"The singer {singer.FullName} has been added";
            return RedirectToAction(nameof(Index));
        }

        // GET: Singer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var singer = await _context.Singers
                .Include(s => s.Location) // Ensure the Location is included
                .FirstOrDefaultAsync(s => s.ID == id);

            if (singer == null) return NotFound();

            // Populate the city list
            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City", singer.LocationID);

            return View(singer);
        }
        // POST: Singer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,LocationID,IsAvailable,EmergencyContactName,EmergencyContactNumber")] Singer singer)
        {
            if (id != singer.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSinger = await _context.Singers.Include(s => s.Location).FirstOrDefaultAsync(s => s.ID == id);
                    if (existingSinger == null) return NotFound();

                    existingSinger.FirstName = singer.FirstName;
                    existingSinger.LastName = singer.LastName;
                    existingSinger.LocationID = singer.LocationID; // Ensure proper assignment
                    existingSinger.IsAvailable = singer.IsAvailable;
                    existingSinger.EmergencyContactName = singer.EmergencyContactName;
                    existingSinger.EmergencyContactNumber = singer.EmergencyContactNumber;
                    existingSinger.UpdatedOn = DateTime.Now;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"The singer {existingSinger.FullName} has been edited and saved";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Singers.Any(s => s.ID == singer.ID)) return NotFound();
                    throw;
                }
            }

            // Repopulate CityList in case of validation errors
            ViewBag.CityList = new SelectList(_context.Locations, "ID", "City", singer.LocationID);

            return View(singer);
        }
        // GET: Singer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var singer = await _context.Singers
                .AsNoTracking()
                .Include(s => s.Location)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (singer == null)
            {
                return NotFound();
            }

            return View(singer);
        }

        // POST: Singer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var singer = await _context.Singers.FindAsync(id);
            if (singer == null)
            {
                return NotFound();
            }

            try
            {
                _context.Singers.Remove(singer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while trying to Delete the Singer. Please try again.");
                return View(singer);
            }
        }


        [HttpPost]
        public async Task<IActionResult> InsertSingersFromExcel(IFormFile theExcel)
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
                        if (workSheet.Cells[1, 1].Text.Trim() != "FirstName" ||
                            workSheet.Cells[1, 2].Text.Trim() != "LastName" ||
                            workSheet.Cells[1, 3].Text.Trim() != "City" ||
                            workSheet.Cells[1, 4].Text.Trim() != "EmergencyContactName" ||
                            workSheet.Cells[1, 5].Text.Trim() != "EmergencyContactNumber")
                        {
                            response = new { success = false, message = "❌ Invalid Excel format. Please ensure the file has 'FirstName', 'LastName', 'City', 'EmergencyContactName', and 'EmergencyContactNumber' headers." };
                            return Json(response);
                        }

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

                                // Validate required fields
                                if (string.IsNullOrEmpty(singer.FirstName) ||
                                    string.IsNullOrEmpty(singer.LastName) ||
                                    string.IsNullOrEmpty(cityName) ||
                                    string.IsNullOrEmpty(singer.EmergencyContactName) ||
                                    string.IsNullOrEmpty(singer.EmergencyContactNumber))
                                {
                                    errorCount++;
                                    feedbackMessage += $"⚠️ Error: Row {row} has missing fields.<br>";
                                    continue;
                                }

                                // Validate phone number format
                                if (!Regex.IsMatch(singer.EmergencyContactNumber, @"^\d{10}$"))
                                {
                                    errorCount++;
                                    feedbackMessage += $"⚠️ Error: Invalid phone number in row {row}.<br>";
                                    continue;
                                }

                                // Check for duplicate singers in the database
                                if (_context.Singers.Any(s => s.FirstName == singer.FirstName && s.LastName == singer.LastName))
                                {
                                    errorCount++;
                                    feedbackMessage += $"⚠️ Error: Singer {singer.FullName} already exists.<br>";
                                    continue;
                                }

                                // Handle location
                                var location = _context.Locations.FirstOrDefault(l => l.City == cityName && l.DirectorLocations.FirstOrDefault().DirectorID != null);
                                if (location == null)
                                {
                                    errorCount++;
                                    feedbackMessage += $"⚠️ Error: <b>{singer.FullName}</b> is from <b>{cityName}</b>, but there is no <b>Director</b> assigned for this city.<br>";
                                    continue;
                                }
                                singer.Location = location;

                                // Add singer to the database
                                _context.Singers.Add(singer);
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
                            response = new { success = true, message = $"✅ {successCount} singers added successfully.<br>{feedbackMessage}" };
                        }
                        else
                        {
                            response = new { success = false, message = $"❌ No singers were added.<br>{feedbackMessage}" };
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
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExcelTemplates", "SingerTemplate.xlsx");

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Serve the file for download
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SingerTemplate.xlsx");
        }


        //added the autocomplete 
        public JsonResult CitySuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<object>());
            }

            var suggestions = _context.Locations
                .Where(c => c.City.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.City)
                .ToList();

            return Json(suggestions);
        }

        public static string DisplayNameEnum(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
            return attribute != null ? attribute.Name : value.ToString();
        }
        //Archiving
        [HttpPost]
        public async Task<IActionResult> Archive(int id)
        {
            var singer = await _context.Singers.FindAsync(id);
            if (singer == null)
            {
                return NotFound();
            }

            singer.IsArchived = true;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "This data has been archived successfully!";
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> UnArchive(int id)
        {
            var singer = await _context.Singers.FindAsync(id);
            if (singer == null)
            {
                return NotFound();
            }

            singer.IsArchived = false;
            _context.Update(singer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "This archive has been activated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Chart Methods
        // Pie Chart - Singers by City
        public JsonResult GetSingersByCityForDoughnut()
        {
            var singersByCity = _context.Singers
                .Include(s => s.Location) // Include Location for each Singer
                .GroupBy(s => s.Location.City)
                .Select(g => new
                {
                    City = g.Key,
                    Count = g.Count()
                })
                .ToList();

            return Json(singersByCity);
        }

        // Active vs. Archived Singers
        public JsonResult GetActiveVsArchivedSingers()
        {
            var activeCount = _context.Singers.Count(s => s.IsArchived == false);
            var archivedCount = _context.Singers.Count(s => s.IsArchived == true);

            var data = new
            {
                Labels = new[] { "Active", "Archived" },
                Counts = new[] { activeCount, archivedCount }
            };

            return Json(data);
        }

        // Singer Count
        [HttpGet]
        public async Task<JsonResult> GetTotalSingerCount()
        {
            // Count all singers, regardless of their archived status
            var totalCount = await _context.Singers.CountAsync();

            return Json(new { TotalCount = totalCount });
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
        private bool SingerExists(int id)
        {
            return _context.Singers.Any(e => e.ID == id);
        }
    }
}
