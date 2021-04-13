using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swipe.Areas.User.Models;
using Swipe.Areas.User.ViewModels;
using Swipe.Data;
using Swipe.Models;
using Swipe.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = SD.AdminEndUser + "," + SD.CustomerEndUser)]
    public class PreferencesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public PreferencesController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Edit(Guid? id, bool? savedChanges = false)
        {

            if (id == null)
            {
                return NotFound();
            }

            var preferences = await _db.Preferences
                .Include(x => x.EthnicityPreferences).ThenInclude(x => x.Ethnicity)
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (preferences == null)
            {
                return NotFound();
            }

            if (savedChanges == true)
            {
                ViewData["Message"] = "Preferences Saved";
            }
            
            ViewData["Gender"] = await _db.Gender.ToListAsync();
            ViewData["Height"] = await _db.Heights.ToListAsync();

            PopulateEthnicityPreferencesData(preferences);
            return View(preferences);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid? id, Guid[] selectedEthnicities)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preferencesToUpdate = await _db.Preferences
                .Include(x => x.EthnicityPreferences).ThenInclude(x => x.Ethnicity)
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (await TryUpdateModelAsync<Preferences>(
                preferencesToUpdate,
                "",
                x => x.MinAge,
                x => x.MaxAge,
                x => x.GenderID,
                x => x.HeightID))
            {
                UpdateEthnicityPreferences(selectedEthnicities, preferencesToUpdate);
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction("Edit", new { id = id, savedChanges = true });
            }
            UpdateEthnicityPreferences(selectedEthnicities, preferencesToUpdate);
            PopulateEthnicityPreferencesData(preferencesToUpdate);
            return View(preferencesToUpdate);
        }

        private void UpdateEthnicityPreferences(Guid[] selectedEthnicities, Preferences preferencesToUpdate)
        {
            if (selectedEthnicities == null)
            {
                preferencesToUpdate.EthnicityPreferences = new List<EthnicityPreferences>();
                return;
            }

            var selectedEthnicitiesHS = new HashSet<Guid>(selectedEthnicities);
            var ethnicityPreferences = new HashSet<Guid>
                (preferencesToUpdate.EthnicityPreferences.Select(x => x.Ethnicity.ID));

            foreach (var ethnicity in _db.Ethnicity)
            {
                if (selectedEthnicitiesHS.Contains(ethnicity.ID))
                {
                    if (!ethnicityPreferences.Contains(ethnicity.ID))
                    {
                        preferencesToUpdate.EthnicityPreferences.Add(new EthnicityPreferences { PreferencesID = preferencesToUpdate.ID, EthnicityID = ethnicity.ID });
                    }
                }
                else
                {
                    if (ethnicityPreferences.Contains(ethnicity.ID))
                    {
                        EthnicityPreferences ethnicityToRemove = preferencesToUpdate.EthnicityPreferences.FirstOrDefault(x => x.EthnicityID == ethnicity.ID);
                        _db.Remove(ethnicityToRemove);
                    }
                }
            }
        }

        private void PopulateEthnicityPreferencesData(Preferences preferences)
        {
            var allEthniticities = _db.Ethnicity;
            var ethniticityPreferences = new HashSet<Guid>(preferences.EthnicityPreferences.Select(x => x.EthnicityID));
            var viewModel = new List<EthnicityPreferencesData>();
            foreach (var ethnicity in allEthniticities)
            {
                viewModel.Add(new EthnicityPreferencesData
                {
                    EthnicityID = ethnicity.ID,
                    Type = ethnicity.Type,
                    Assigned = ethniticityPreferences.Contains(ethnicity.ID)
                });
            }
            ViewData["Ethnicity"] = viewModel;
        }



    }
}
