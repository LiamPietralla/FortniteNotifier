using FortniteNotifier.Shared.Data;
using FortniteNotifier.Shared.Data.Models;
using FortniteNotifier.Admin.Helpers;
using FortniteNotifier.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Core;

namespace FortniteNotifier.Admin.Controllers
{
    [Authorize]
    public class VersionController : Controller
    {
        private readonly ConfigHelper _config;
        private readonly UnitOfWork _unitOfWork;

        public VersionController(IConfiguration configuration, UnitOfWork unitOfWork)
        {
            _config = new(configuration);
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> IndexAsync()
        {
            try
            {
                // Get a list of all versions from the database
                IEnumerable<VersionRecord> versions = await _unitOfWork.VersionRecordRepository.GetAllVersionRecordsAsync();

                // Order the versions by inserted date
                versions = versions.OrderByDescending(x => x.InsertTimestamp);

                // Create the view model
                VersionViewModel vm = new()
                {
                    Versions = versions.ToList()
                };

                // Return the view with data
                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while navigating the the version page.");
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllVersions()
        {
            try
            {
                // Get a list of all versions from the database
                await _unitOfWork.VersionRecordRepository.DeleteAllVersionsAsync();

                // Return the view with data
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting all versions.");
                return RedirectToAction("Index", "Error");
            }
        }
    }
}