using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;
using TomorrowsVoices.Utilities;

namespace TomorrowsVoices.Controllers
{
    [Authorize]
    public class VolunteerController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public VolunteerController(TomorrowsVoicesContext context)
        {
            _context = context;
        }
        // GET: Volunteer
        public async Task<IActionResult> Index(string? SearchString, string? SearchEmail, string? SearchCity, int? page, int? pageSizeID, string? actionButton, string sortDirection = "asc", string sortField = "Volunteer", bool archived = false)
        {
            var volunteers = _context.Volunteers
                .Where(v => v.IsArchived == archived)
                .Include(v => v.VolLocation)
                .AsNoTracking();
            if (!User.IsInRole("Admin"))
            {
                var currentVolunteer = await GetCurrentVolunteerAsync();
                if (currentVolunteer != null)
                {
                    volunteers = volunteers.Where(v => v.Email == currentVolunteer.Email);
                }
                else
                {
                    // If no volunteer record exists, show nothing to non-admins
                    volunteers = volunteers.Where(v => false);
                }
            }
            if (!User.IsInRole("Admin") /*&& !User.IsInRole("Director")*/)
            {
                volunteers = volunteers.Where(v => v.Status == ApprovalStatus.Approved);
            }
            ViewData["ActiveTab"] = archived ? "archived" : "active";
            string[] sortOptions = new[] { "FullName", "City", "Email" };
            ViewData["Filtering"] = "btn-outline-secondary";
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

            if (!String.IsNullOrEmpty(SearchString))
            {
                volunteers = volunteers.Where(s => s.FirstName != null && s.FirstName.ToLower().Contains(SearchString.ToLower())
                                            || s.LastName != null && s.LastName.ToLower().Contains(SearchString.ToLower())
                                             || ((s.FirstName + " " + s.LastName).ToLower().Contains(SearchString.ToLower())));

                numberFilters++;
            }
            if (!String.IsNullOrEmpty(SearchEmail))
            {
                volunteers = volunteers.Where(v => v.Email != null && v.Email.ToLower().Contains(SearchEmail.ToLower()));
                numberFilters++;
            }

            if (!string.IsNullOrEmpty(SearchCity))
            {
                volunteers = volunteers.Where(v => v.VolLocation.City != null && v.VolLocation.City == SearchCity);
                numberFilters++;
            }

            // Sorting logic based on selected field and direction
            if (sortField == "FullName")
            {
                if (sortDirection == "asc")
                {
                    volunteers = volunteers
                        .OrderBy(s => s.FirstName)
                        .ThenBy(s => s.LastName);
                }
                else
                {
                    volunteers = volunteers
                        .OrderByDescending(s => s.FirstName)
                        .ThenByDescending(s => s.LastName);
                }
            }
            else if (sortField == "Email")
            {
                if (sortDirection == "asc")
                {
                    volunteers = volunteers
                        .OrderBy(v => v.Email)
                        .ThenBy(v => v.FirstName)
                        .ThenBy(v => v.LastName);
                }
                else
                {
                    volunteers = volunteers
                        .OrderByDescending(v => v.Email)
                        .ThenByDescending(v => v.FirstName)
                        .ThenByDescending(v => v.LastName);
                }
            }
            else if (sortField == "City")
            {
                if (sortDirection == "asc")
                {
                    volunteers = volunteers
                        .OrderBy(v => v.VolLocation.City)
                        .ThenBy(v => v.FirstName)
                        .ThenBy(v => v.LastName);
                }
                else
                {
                    volunteers = volunteers
                        .OrderByDescending(v => v.VolLocation.City)
                        .ThenByDescending(v => v.FirstName)
                        .ThenByDescending(v => v.LastName);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["numberFilters"] = numberFilters;
            int archivedCount = await _context.Volunteers.CountAsync(d => d.IsArchived == true);
            ViewData["numberofArchive"] = archivedCount;
            int activeCount = await _context.Volunteers.CountAsync(d => d.IsArchived == false);
            ViewData["numberofActive"] = activeCount;

            var cityList = volunteers.AsEnumerable()
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

            ViewData["SearchCity"] = cityList;
            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Volunteer>.CreateAsync(volunteers.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Volunteer/Details/5
        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers
                .Include(v => v.VolLocation)
                .Include(v => v.VolAttendances)
                .ThenInclude(v => v.VolSchedule)
                .ThenInclude(s => s.Event)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }
        // GET: Volunteer/Create
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            Volunteer volunteer = new Volunteer();
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City");
            return View();
        }

        // POST: Volunteer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Phone,Email,VolLocationID")] Volunteer volunteer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(volunteer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"a new Volunteer has been added {volunteer.FullName}";
                    return RedirectToAction(nameof(Index));
                }



            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                {
                    ModelState.AddModelError("Email", "Unable to save changes. Remember, you cannot have duplicate email addresses.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", volunteer.VolLocationID);
            return View(volunteer);
        }


        // GET: Volunteer/Edit/5
        [Authorize(Roles = "Admin,Volunteer")]
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
        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Phone,Email,VolLocationID")] Volunteer volunteer)
        {
            if (id != volunteer.ID)
            {
                return NotFound();
            }

            var volunteerToUpdate = await _context.Volunteers.FirstOrDefaultAsync(v => v.ID == id);

            if (volunteerToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Volunteer>(volunteerToUpdate, "",
                   v => v.FirstName, v => v.LastName, v => v.Phone, v => v.Email, v => v.VolLocationID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"{volunteer.FullName} and its details has been edited and saved";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerExists(volunteerToUpdate.ID))
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
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("Email", "Unable to save changes. Remember, you cannot have duplicate email addresses.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }

            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City", volunteer.VolLocationID);
            return View(volunteerToUpdate);
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


        // GET: Volunteer/InsertVolunteersFromExcel
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InsertVolunteersFromExcel(IFormFile theExcel)
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
                            workSheet.Cells[1, 3].Text != "Phone" ||
                            workSheet.Cells[1, 4].Text != "Email" ||
                            workSheet.Cells[1, 5].Text != "City")
                        {
                            response = new { success = false, message = "❌ Invalid Excel format. Please ensure the file has 'FirstName', 'LastName', 'Phone', 'Email', and 'City' headers." };
                            return Json(response);
                        }

                        for (int row = start.Row + 1; row <= end.Row; row++)
                        {
                            Volunteer volunteer = new Volunteer();
                            try
                            {
                                volunteer.FirstName = workSheet.Cells[row, 1].Text.Trim();
                                volunteer.LastName = workSheet.Cells[row, 2].Text.Trim();
                                volunteer.Phone = workSheet.Cells[row, 3].Text.Trim();
                                volunteer.Email = workSheet.Cells[row, 4].Text.Trim();
                                string cityName = workSheet.Cells[row, 5].Text.Trim();

                                // Validate data before adding
                                if (string.IsNullOrEmpty(volunteer.FirstName) ||
                                    string.IsNullOrEmpty(volunteer.LastName) ||
                                    string.IsNullOrEmpty(volunteer.Phone) ||
                                    string.IsNullOrEmpty(volunteer.Email) ||
                                    string.IsNullOrEmpty(cityName))
                                {
                                    errorCount++;
                                    feedbackMessage += $"⚠️ Error: Row {row} has missing fields.<br>";
                                    continue; // Skip invalid row
                                }

                                if (!Regex.IsMatch(volunteer.Phone, @"^\d{10}$"))
                                {
                                    errorCount++;
                                    feedbackMessage += $"⚠️ Error: Invalid phone number in row {row}.<br>";
                                    continue;
                                }

                                // Check if volunteer with the same email already exists
                                if (_context.Volunteers.Any(v => v.Email == volunteer.Email))
                                {
                                    errorCount++;
                                    feedbackMessage += $"⚠️ Error: Volunteer with email {volunteer.Email} already exists.<br>";
                                    continue;
                                }

                                // Check if the location exists, otherwise create it
                                var location = _context.VolLocations.FirstOrDefault(l => l.City == cityName);
                                if (location == null)
                                {
                                    location = new VolLocation { City = cityName };
                                    _context.VolLocations.Add(location);
                                    await _context.SaveChangesAsync(); // Save the new location to get its ID
                                }

                                volunteer.VolLocationID = location.ID;
                                _context.Volunteers.Add(volunteer);
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
                            response = new { success = true, message = $"✅ {successCount} volunteers added successfully.<br>{feedbackMessage}" };
                        }
                        else
                        {
                            response = new { success = false, message = $"❌ No volunteers were added.<br>{feedbackMessage}" };
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
        /**/

        // Excel Template Server

        public IActionResult DownloadSampleExcel()
        {
            // Path to the sample Excel file in your project
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExcelTemplates", "VolunteerTemplate.xlsx");

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Serve the file for download
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "VolunteerTemplate.xlsx");
        }

        // GET: Volunteer/ExportVolunteersToExcel
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportVolunteersToExcel()
        {
            var volunteers = await _context.Volunteers
                .Include(v => v.VolLocation)
                .Include(v => v.VolAttendances)
                .ThenInclude(va => va.VolSchedule)
                .ThenInclude(va => va.Event)
                .ToListAsync();

  
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Volunteers");

                // Add headers
                worksheet.Cells[1, 1].Value = "First Name";
                worksheet.Cells[1, 2].Value = "Last Name";
                worksheet.Cells[1, 3].Value = "Phone";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "City";
                worksheet.Cells[1, 6].Value = "Event Name";
                worksheet.Cells[1, 7].Value = "Event Date";
                worksheet.Cells[1, 8].Value = "Scheduled Start Time";
                worksheet.Cells[1, 9].Value = "Scheduled End Time";
                worksheet.Cells[1, 10].Value = "Actual Start Time";
                worksheet.Cells[1, 11].Value = "Actual End Time";
                worksheet.Cells[1, 12].Value = "Hours Spent";
                worksheet.Cells[1, 13].Value = "Status";

                // Make headers bold
                for (int i = 1; i <= 13; i++)
                {
                    worksheet.Cells[1, i].Style.Font.Bold = true;
                }

                int row = 2;
                //foreach (var volunteer in sortedVolunteers)
                //{
                //    // If no schedules, still export volunteer details
                //    if (volunteer.Schedules == null || !volunteer.Schedules.Any())
                //    {
                //        worksheet.Cells[row, 1].Value = volunteer.FirstName;
                //        worksheet.Cells[row, 2].Value = volunteer.LastName;
                //        worksheet.Cells[row, 3].Value = volunteer.Phone;
                //        worksheet.Cells[row, 4].Value = volunteer.Email;
                //        worksheet.Cells[row, 5].Value = volunteer.VolLocation?.City;

                //        // Highlight the lack of attendance
                //        worksheet.Cells[row, 6].Value = "No Attendance Records";
                //        worksheet.Cells[row, 6].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                //        worksheet.Cells[row, 6].Style.Font.Italic = true;

                //        row++;
                //    }
                //    else
                //    {
                //        foreach (var schedule in volunteer.Schedules)
                //        {
                //            worksheet.Cells[row, 1].Value = volunteer.FirstName;
                //            worksheet.Cells[row, 2].Value = volunteer.LastName;
                //            worksheet.Cells[row, 3].Value = volunteer.Phone;
                //            worksheet.Cells[row, 4].Value = volunteer.Email;
                //            worksheet.Cells[row, 5].Value = volunteer.VolLocation?.City;
                //            worksheet.Cells[row, 6].Value = schedule.Event?.Name;
                //            worksheet.Cells[row, 7].Value = schedule.Date.ToString("yyyy-MM-dd");
                //            worksheet.Cells[row, 8].Value = schedule.ScheduledStartTime.ToString();
                //            worksheet.Cells[row, 9].Value = schedule.ScheduledEndTime.ToString();
                //            worksheet.Cells[row, 10].Value = schedule.ActualStartTime?.ToString();
                //            worksheet.Cells[row, 11].Value = schedule.ActualEndTime?.ToString();
                //            worksheet.Cells[row, 12].Value = schedule.ActualEndTime.HasValue && schedule.ActualStartTime.HasValue
                //                ? (schedule.ActualEndTime.Value - schedule.ActualStartTime.Value).TotalHours
                //                : 0;
                //            worksheet.Cells[row, 13].Value = schedule.Status ? "Attended" : "Absent";
                //            row++;
                //        }
                //    }
                //}

                // Auto-fit columns for better readability
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"Volunteer_Attendance_{DateTime.Now:MMMM_yyyy}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }


        /**/





        //archive and unarchiving
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Archive(int id)
        {
            var director = await _context.Volunteers.FindAsync(id);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnArchive(int id)
        {
            var director = await _context.Volunteers.FindAsync(id);
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
        private async Task<Volunteer?> GetCurrentVolunteerAsync()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }

            // Admins see all records
            if (User.IsInRole("Admin"))
            {
                return null;
            }

            return await _context.Volunteers
                .FirstOrDefaultAsync(v => v.Email == userEmail);
        }

        // Chart Methods
        // Doughnut Chart - Volunteers by City
        [HttpGet]
        public async Task<IActionResult> GetVolunteersByCityData()
        {
            var volunteersByCity = await _context.Volunteers
                .Include(v => v.VolLocation) // Include the location to access the city
                .GroupBy(v => v.VolLocation.City)
                .Select(g => new
                {
                    City = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var labels = volunteersByCity.Select(v => v.City).ToArray();
            var data = volunteersByCity.Select(v => v.Count).ToArray();

            return Json(new { labels, data });
        }

        // Active vs Archived - Volunteers
        [HttpGet]
        public async Task<IActionResult> GetActiveVsArchivedVolunteersData()
        {
            var activeVolunteersCount = await _context.Volunteers.CountAsync(v => !v.IsArchived);
            var archivedVolunteersCount = await _context.Volunteers.CountAsync(v => v.IsArchived);

            var labels = new[] { "Active", "Archived" };
            var data = new[] { activeVolunteersCount, archivedVolunteersCount };

            return Json(new { labels, data });
        }

        // Volunteers Count
        [HttpGet]
        public async Task<JsonResult> GetTotalVolunteerCount()
        {
            try
            {
                // Count all volunteers, regardless of their archived status
                var totalCount = await _context.Volunteers.CountAsync();
                return Json(new { TotalCount = totalCount });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog or NLog)
                Console.Error.WriteLine($"Error in GetTotalVolunteerCount: {ex.Message}");
                return Json(new { TotalCount = 0 }); // Return a default value in case of error
            }
        }

        private bool VolunteerExists(int id)
        {
            return _context.Volunteers.Any(e => e.ID == id);
        }
    }
}
