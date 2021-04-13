using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Swipe.Areas.User.Models;
using Swipe.Data;
using Swipe.Models;
using Swipe.Utility;

namespace Swipe.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = SD.AdminEndUser + "," + SD.CustomerEndUser)]
    public class AboutController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AboutController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: User/About/Create
        public async Task<IActionResult> Create()
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;

            if (userId == null)
            {
                return NotFound();
            }

            var about = await _db.About.FindAsync(userId);

            if (about != null)
            {
                return RedirectToAction("Edit", "About", new { area = "User", id = userId } );
            }

            ViewData["ID"] = userId;

            return View();
        }

        // POST: User/About/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,AboutMe,WhatImLookingFor,IValue,LastShowBingeWatched,ICouldProbablyBeatYouAt")] About about)
        {
            if (ModelState.IsValid)
            {
                _db.Add(about);
                await _db.SaveChangesAsync();

                return RedirectToAction("Edit", "About", new { area = "User", id = about.ID, savedChanges = true});
            }
            return View(about);
        }

        // GET: User/About/Edit/5
        public async Task<IActionResult> Edit(string id, bool? savedChanges = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (savedChanges == true)
            {
                ViewData["Message"] = "About Saved";
            }

            var about = await _db.About.FindAsync(id);
            if (about == null)
            {
                return NotFound();
            }
            return View(about);
        }

        // POST: User/About/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,AboutMe,WhatImLookingFor,IValue,LastShowBingeWatched,ICouldProbablyBeatYouAt")] About about)
        {
            if (id != about.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(about);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AboutExists(about.ID))
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
            ViewData["ID"] = new SelectList(_db.ApplicationUser, "Id", "Id", about.ID);
            return View(about);
        }

        private bool AboutExists(string id)
        {
            return _db.About.Any(e => e.ID == id);
        }
    }
}
