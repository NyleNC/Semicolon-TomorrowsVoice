using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;
using TomorrowsVoices.ViewModels;

namespace TomorrowsVoices.Controllers
{
    public class PeopleController : Controller
    {
        private readonly TomorrowsVoicesContext _context;

        public PeopleController(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        // GET: People
        public async Task<IActionResult> Index()
        {
            var model = new PeopleViewModel
            {
                Directors = await _context.Directors.ToListAsync(),
                Singers = await _context.Singers.Include(s => s.Location).ToListAsync()
            };

            return View(model);
        }


        // GET: People/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var singer = await _context.Singers
                .Include(s => s.Location)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (singer == null)
            {
                return NotFound();
            }

            return View(singer);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,LocationID")] Singer singer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(singer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropDownLists(singer);
            return View(singer);
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var singer = await _context.Singers.Include(s=>s.Location).FirstOrDefaultAsync(s => s.ID == id); ;
            if (singer == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(singer);
            return View(singer);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,City")] Singer singer)
        {
            if (id != singer.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(singer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SingerExists(singer.ID))
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
            PopulateDropDownLists(singer);
            return View(singer);
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var singer = await _context.Singers
                .Include(s => s.Location)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (singer == null)
            {
                return NotFound();
            }

            return View(singer);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var singer = await _context.Singers.FindAsync(id);
            if (singer != null)
            {
                _context.Singers.Remove(singer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // director controllers

        // GET: People/Details/5
        public async Task<IActionResult> DetailsDirector(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var singer = await _context.Singers
                .Include(s => s.Location)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (singer == null)
            {
                return NotFound();
            }

            return View(singer);
        }

        // GET: People/Create
        public IActionResult CreateDirector()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDirector([Bind("ID,FirstName,LastName,LocationID")] Director director)
        {
            if (ModelState.IsValid)
            {
                _context.Add(singer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropDownLists(singer);
            return View(singer);
        }

        // GET: People/Edit/5
        public async Task<IActionResult> EditDirector(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var singer = await _context.Singers.Include(s => s.Location).FirstOrDefaultAsync(s => s.ID == id); ;
            if (singer == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(singer);
            return View(singer);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDirector(int id, [Bind("ID,FirstName,LastName,City")] Director director)
        {
            if (id != singer.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(director);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SingerExists(singer.ID))
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
            PopulateDropDownLists(singer);
            return View(singer);
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var singer = await _context.Singers
                .Include(s => s.Location)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (singer == null)
            {
                return NotFound();
            }

            return View(singer);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var singer = await _context.Singers.FindAsync(id);
            if (singer != null)
            {
                _context.Singers.Remove(singer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private SelectList LocationList(int? selectedId)
        {
            return new SelectList(_context.Locations
                .OrderBy(m => m.City), "ID", "City", selectedId);
        }
        private void PopulateDropDownLists(Singer? singer = null)
        {
            ViewData["LocationID"] = LocationList(singer?.LocationID);
        }
        private bool SingerExists(int id)
        {
            return _context.Singers.Any(e => e.ID == id);
        }
    }
}
