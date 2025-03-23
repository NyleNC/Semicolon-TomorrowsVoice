using System;
using System.Collections.Generic;
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
    public class VolLocationController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public VolLocationController(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        // GET: VolLocation
        public async Task<IActionResult> Index(int? page, int? pageSizeID, bool archived = false)
        {
            ViewData["IsArchived"] = archived;
            ViewData["ActiveTab"] = archived ? "archived" : "active";
            int archivedCount = await _context.VolLocations.CountAsync(d => d.IsArchived == true);
            ViewData["numberofArchive"] = archivedCount;
            int activeCount = await _context.VolLocations.CountAsync(d => d.IsArchived == false);
            ViewData["numberofActive"] = activeCount;

            // Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<VolLocation>.CreateAsync(
                _context.VolLocations.Where(d => d.IsArchived == archived).AsNoTracking(),
                page ?? 1,
                pageSize
            );

            return View(pagedData);
        }

        // GET: VolLocation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volLocation = await _context.VolLocations
                .FirstOrDefaultAsync(m => m.ID == id);
            if (volLocation == null)
            {
                return NotFound();
            }

            return View(volLocation);
        }

        // GET: VolLocation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VolLocation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,City")] VolLocation volLocation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(volLocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(volLocation);
        }

        // GET: VolLocation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volLocation = await _context.VolLocations.FindAsync(id);
            if (volLocation == null)
            {
                return NotFound();
            }
            return View(volLocation);
        }

        // POST: VolLocation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,City")] VolLocation volLocation)
        {
            if (id != volLocation.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(volLocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolLocationExists(volLocation.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(volLocation);
        }

        // GET: VolLocation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volLocation = await _context.VolLocations
                .FirstOrDefaultAsync(m => m.ID == id);
            if (volLocation == null)
            {
                return NotFound();
            }

            return View(volLocation);
        }

        // POST: VolLocation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volLocation = await _context.VolLocations.FindAsync(id);
            if (volLocation != null)
            {
                _context.VolLocations.Remove(volLocation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //archive and unarchiving
        [HttpPost]
        public async Task<IActionResult> Archive(int id)
        {
            var volLocation = await _context.VolLocations.FindAsync(id);
            if (volLocation == null)
            {
                return NotFound();
            }

            volLocation.IsArchived = true;
            await _context.SaveChangesAsync();



            TempData["SuccessMessage"] = "The Data has been archived successfully! Pre-existing data wont be affected";
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> UnArchive(int id)
        {
            var volLocation = await _context.VolLocations.FindAsync(id);
            if (volLocation == null)
            {
                return NotFound();
            }

            volLocation.IsArchived = false;
            _context.Update(volLocation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "This archive has been activated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Chart Methods
        // Doughnut Chart - Locations by City
        [HttpGet]
        public async Task<IActionResult> GetLocationsByCityData()
        {
            var locationsByCity = await _context.VolLocations
                .GroupBy(l => l.City)
                .Select(g => new
                {
                    City = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var labels = locationsByCity.Select(l => l.City).ToArray();
            var data = locationsByCity.Select(l => l.Count).ToArray();

            return Json(new { labels, data });
        }

        // Active vs. Archived Locations
        [HttpGet]
        public async Task<IActionResult> GetActiveVsArchivedLocationsData()
        {
            var activeLocationsCount = await _context.VolLocations.CountAsync(l => !l.IsArchived);
            var archivedLocationsCount = await _context.VolLocations.CountAsync(l => l.IsArchived);

            var labels = new[] { "Active", "Archived" };
            var data = new[] { activeLocationsCount, archivedLocationsCount };

            return Json(new { labels, data });
        }


        // Locations Count
        [HttpGet]
        public async Task<JsonResult> GetTotalLocationCount()
        {
            try
            {
                // Count all locations, regardless of their archived status
                var totalCount = await _context.VolLocations.CountAsync();
                return Json(new { TotalCount = totalCount });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog or NLog)
                Console.Error.WriteLine($"Error in GetTotalLocationCount: {ex.Message}");
                return Json(new { TotalCount = 0 }); // Return a default value in case of error
            }
        }

        private bool VolLocationExists(int id)
        {
            return _context.VolLocations.Any(e => e.ID == id);
        }
    }
}
