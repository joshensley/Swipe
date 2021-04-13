using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Swipe.Data;
using Swipe.Models.Admin;
using Swipe.Utility;

namespace Swipe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminEndUser)]
    public class HeightsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HeightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Heights
        public async Task<IActionResult> Index()
        {
            return View(await _context.Heights.ToListAsync());
        }

        // GET: Admin/Heights/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var height = await _context.Heights
                .FirstOrDefaultAsync(m => m.ID == id);
            if (height == null)
            {
                return NotFound();
            }

            return View(height);
        }

        // GET: Admin/Heights/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Heights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,DisplayHeight,HeightInInches")] Height height)
        {
            if (ModelState.IsValid)
            {
                height.ID = Guid.NewGuid();
                _context.Add(height);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(height);
        }

        // GET: Admin/Heights/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var height = await _context.Heights.FindAsync(id);
            if (height == null)
            {
                return NotFound();
            }
            return View(height);
        }

        // POST: Admin/Heights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ID,DisplayHeight,HeightInInches")] Height height)
        {
            if (id != height.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(height);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HeightExists(height.ID))
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
            return View(height);
        }

        // GET: Admin/Heights/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var height = await _context.Heights
                .FirstOrDefaultAsync(m => m.ID == id);
            if (height == null)
            {
                return NotFound();
            }

            return View(height);
        }

        // POST: Admin/Heights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var height = await _context.Heights.FindAsync(id);
            _context.Heights.Remove(height);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HeightExists(Guid id)
        {
            return _context.Heights.Any(e => e.ID == id);
        }
    }
}
