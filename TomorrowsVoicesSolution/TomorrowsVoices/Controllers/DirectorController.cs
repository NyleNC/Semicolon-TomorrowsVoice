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
                .Include(d => d.Location) 
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
             
                if (Enum.TryParse<City>(SearchCity, true, out var searchCityEnum))
                {
                    directors = directors
                        .Where(p => p.Location != null && p.Location.City == searchCityEnum); 
                    numberFilters++;
                }
          
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
            PopulateDropDownLists();
            return View();
        }

        // POST: Director/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,Location")] Director director)
        {
           

            if (ModelState.IsValid)
            {
                try
                {

                    if (director.Location != null && !string.IsNullOrEmpty(director.Location.City.ToString()))
                    {
                        var existingDirector = await _context.Directors.Include(d => d.Location)
                            .FirstOrDefaultAsync(d => d.Location.City == director.Location.City);

                        if (existingDirector != null)
                        {
                            ModelState.AddModelError("Location.City", $"Someone is already assigned to this City: {director.Location.City}");
                            PopulateDropDownLists(director);
                            return View(director);
                        }
               
                    }



                    _context.Add(director);
                    await _context.SaveChangesAsync();
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

                    if (directorToUpdate.Location != null && directorToUpdate.Location.City != null)
                    {
                        var existingDirector = await _context.Directors.Include(d => d.Location)
                            .FirstOrDefaultAsync(d => d.Location.City == directorToUpdate.Location.City);
                        if (existingDirector != null)
                        {

                            ModelState.AddModelError("Location.City", "A director is already assigned to this city.");
                            PopulateDropDownLists(directorToUpdate);
                            return View(directorToUpdate);
                        }
                    }
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
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
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
                  .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (director == null)
            {
                return NotFound();
            }

            return View(director);
        }

        // delete is finally working , we can finally delete director without an error
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
                ModelState.AddModelError("", "An error occurred while trying to delete the Director. Please try again.");
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

        //Autocomplete for City
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

        private bool DirectorExists(int id)
        {
            return _context.Directors.Any(e => e.ID == id);
        }
    }
}
