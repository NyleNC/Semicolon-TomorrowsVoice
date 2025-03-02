using System;
using System.Collections.Generic;
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
            ViewData["ActiveTab"] = archived ? "archived" : "active";
            string[] sortOptions = new[] { "FullName", "Location", "Email" };
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
                                            || s.LastName != null && s.LastName.ToLower().Contains(SearchString.ToLower()));

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

            ViewData["Cities"] = cityList;
            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Volunteer>.CreateAsync(volunteers.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
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
                .Include(v => v.VolAttendances)
                .ThenInclude(va => va.Event)
                .AsNoTracking()
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
            Volunteer volunteer = new Volunteer();
            ViewData["VolLocationID"] = new SelectList(_context.VolLocations, "ID", "City");
            return View();
        }

        // POST: Volunteer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Phone,Email,VolLocationID")] Volunteer volunteer)
        {
            try
            { if (ModelState.IsValid)
                {
                    _context.Add(volunteer);
                    await _context.SaveChangesAsync();
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
                v => v.FirstName, v => v.LastName, v => v.Phone, v => v.VolLocationID))
            {
                try
                {
                    await _context.SaveChangesAsync();
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
        //archive and unarchiving
        [HttpPost]
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

        private bool VolunteerExists(int id)
        {
            return _context.Volunteers.Any(e => e.ID == id);
        }
    }
}
