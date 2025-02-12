 using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;
using TomorrowsVoices.Utilities;

namespace TomorrowsVoices.Controllers
{
    public class SingerController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public SingerController(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        // GET: Singer
        public async Task<IActionResult> Index(/*bool? searchIsAvailable,*/ int? page, int? pageSizeID, string? actionButton, string? SearchString, string? SearchCity, string sortDirection = "asc", string sortField = "Name")
        {
            var singers = _context.Singers
                .Include(s => s.Location) // Include Location for each Singer
                .AsNoTracking();

            string[] sortOptions = new[] { "FullName", "Location", "IsAvailable" };
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
                singers = singers.Where(s => s.FirstName != null && s.FirstName.ToLower().Contains(SearchString.ToLower())
                                            || s.LastName != null && s.LastName.ToLower().Contains(SearchString.ToLower()));

                numberFilters++;
            }

            if (!string.IsNullOrEmpty(SearchCity))
            {
                // If City is an Enum:
                if (Enum.TryParse<City>(SearchCity, true, out var searchCityEnum))
                {
                    singers = singers
                        .Where(p => p.Location != null && p.Location.City == searchCityEnum);
                    numberFilters++;
                }
                // If City is a string:
                else
                {
                    singers = (IQueryable<Singer>)singers
                        .AsEnumerable()
                        .Where(p => p.Location != null && p.Location.City.ToString().Contains(SearchCity));

                    numberFilters++;
                }
            }

            //if (searchIsAvailable != null)
            //{
            //    singers = singers.Where(s => s.IsAvailable == searchIsAvailable);
            //    numberFilters++;
            //}

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

            else if (sortField == "IsAvailable")
            {
                if (sortDirection == "asc")
                {
                    singers = singers
                        .OrderBy(s => s.IsAvailable);
                }
                else
                {
                    singers = singers
                        .OrderByDescending(s => s.IsAvailable);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["numberFilters"] = numberFilters;

            var cityList = singers
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
        public async Task<IActionResult> Create([Bind("FirstName,LastName,LocationID,IsAvailable")] Singer singer)
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
            return RedirectToAction(nameof(Index));
        }

        // GET: Singer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var singer = await _context.Singers.FindAsync(id);
            if (singer == null) return NotFound();

            // Build the filtered city list from Directors.
            var cityList = _context.Directors
                .Where(d => d.Location != null)
                .Select(d => d.Location.City.ToString())
                .Distinct()
                .ToList();

            ViewBag.CityList = new SelectList(cityList, singer.Location?.City.ToString());

            return View(singer);
        }
        // POST: Singer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Location,IsAvailable")] Singer singer)
        {
            if (id != singer.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSinger = await _context.Singers.FindAsync(id);
                    if (existingSinger == null) return NotFound();

                    existingSinger.FirstName = singer.FirstName;
                    existingSinger.LastName = singer.LastName;
                    existingSinger.Location = singer.Location;
                    existingSinger.IsAvailable = singer.IsAvailable;
                    existingSinger.UpdatedOn = DateTime.Now;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"The singer {existingSinger.FullName} has been edited and saved";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SingerExists(singer.ID)) return NotFound();
                    throw;
                }
            }
            
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
                ModelState.AddModelError("", "An error occurred while trying to delete the Singer. Please try again.");
                return View(singer);
            }
        }
        // POST: Singer/ToggleAvailability/
        [HttpPost]
        public IActionResult ToggleAvailability(int id)
        {
            var singer = _context.Singers.Find(id);  // Find the singer by ID
            if (singer == null)
            {
                return Json(new { success = false, message = "Singer not found" });
            }

            // Toggle the availability status
            singer.IsAvailable = !singer.IsAvailable;
            singer.UpdatedOn = DateTime.Now;
            _context.SaveChanges();  // Save the updated status

            // Return the updated availability status as JSON
            return Json(new { success = true, isAvailable = singer.IsAvailable });
        }

        //added the autocomplete 
        public JsonResult CitySuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<object>());
            }
            var suggestions = Enum.GetValues(typeof(City))
                .Cast<City>()
                .Select(city => DisplayNameEnum(city))
                .Where(cityName => cityName.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Json(suggestions);
        }

        public static string DisplayNameEnum(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
            return attribute != null ? attribute.Name : value.ToString();
        }

        private bool SingerExists(int id)
        {
            return _context.Singers.Any(e => e.ID == id);
        }
    }
}
