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
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,Location")] Director director)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    if (director.Location != null && director.Location.City != null)
                    {
                     
                        var existingDirector = await _context.Directors.Include(d => d.Location)
                            .FirstOrDefaultAsync(d => d.Location.City == director.Location.City);

                        if (existingDirector != null)
                        {
                          
                            ModelState.AddModelError("Location.City", "A director is already assigned to this city.");
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

                // Ensure dropdown lists are populated even if ModelState is not valid
                PopulateDropDownLists(director);
                return View(director);
            }

            // Ensure dropdown lists are populated even if ModelState is not valid
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
