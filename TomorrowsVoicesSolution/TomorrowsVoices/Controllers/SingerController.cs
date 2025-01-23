using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;

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
        public async Task<IActionResult> Index()
        {
            var singers = _context.Singers
                .AsNoTracking()  
                .Include(s => s.Location);
            return View(await singers.ToListAsync());
        }

        // GET: Singer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var singer = await _context.Singers
                .AsNoTracking()  
                .Include(s => s.Location)
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

            ViewData["Title"] = "Create";
            ViewData["LocationID"] = new SelectList(locations, "ID", "City");
            return View(new TomorrowsVoices.Models.Singer());
        }


        // POST: Singer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,LocationID")] Singer singer)
        {
            if (ModelState.IsValid)
            {
                singer.CreatedAt = DateTime.Now;
                singer.UpdatedAt = DateTime.Now;

                _context.Add(singer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "City", singer.LocationID);
            return View(singer);
        }







        // GET: Singer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var singer = await _context.Singers.FindAsync(id);
            if (singer == null) return NotFound();

            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "City", singer.LocationID);
            return View(singer);
        }

        // POST: Singer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,LocationID")] Singer singer)
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
                    existingSinger.LocationID = singer.LocationID;
                    existingSinger.UpdatedAt = DateTime.Now; 

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SingerExists(singer.ID)) return NotFound();
                    throw;
                }
            }
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "City", singer.LocationID);
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
            if (singer == null) return NotFound();

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

            _context.Singers.Remove(singer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SingerExists(int id)
        {
            return _context.Singers.Any(e => e.ID == id);
        }
    }
}
