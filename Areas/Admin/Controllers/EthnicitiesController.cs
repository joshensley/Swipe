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
    public class EthnicitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EthnicitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Ethnicities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ethnicity.ToListAsync());
        }

        // GET: Admin/Ethnicities/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ethnicity = await _context.Ethnicity
                .FirstOrDefaultAsync(m => m.ID == id);
            if (ethnicity == null)
            {
                return NotFound();
            }

            return View(ethnicity);
        }

        // GET: Admin/Ethnicities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Ethnicities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Type")] Ethnicity ethnicity)
        {
            if (ModelState.IsValid)
            {
                ethnicity.ID = Guid.NewGuid();
                _context.Add(ethnicity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ethnicity);
        }

        // GET: Admin/Ethnicities/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ethnicity = await _context.Ethnicity.FindAsync(id);
            if (ethnicity == null)
            {
                return NotFound();
            }
            return View(ethnicity);
        }

        // POST: Admin/Ethnicities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ID,Type")] Ethnicity ethnicity)
        {
            if (id != ethnicity.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ethnicity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EthnicityExists(ethnicity.ID))
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
            return View(ethnicity);
        }

        // GET: Admin/Ethnicities/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ethnicity = await _context.Ethnicity
                .FirstOrDefaultAsync(m => m.ID == id);
            if (ethnicity == null)
            {
                return NotFound();
            }

            return View(ethnicity);
        }

        // POST: Admin/Ethnicities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var ethnicity = await _context.Ethnicity.FindAsync(id);
            _context.Ethnicity.Remove(ethnicity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EthnicityExists(Guid id)
        {
            return _context.Ethnicity.Any(e => e.ID == id);
        }
    }
}
