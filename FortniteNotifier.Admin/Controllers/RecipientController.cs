using FortniteNotifier.Shared.Data;
using FortniteNotifier.Admin.Helpers;
using FortniteNotifier.Admin.ViewModels;
using FortniteNotifier.Shared.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Core;

namespace FortniteNotifier.Admin.Controllers
{
    [Authorize]
    public class RecipientController : Controller
    {
        private readonly ConfigHelper _config;
        private readonly UnitOfWork _unitOfWork;

        public RecipientController(IConfiguration configuration, UnitOfWork unitOfWork)
        {
            _config = new(configuration);
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IActionResult> IndexAsync()
        {
            try
            {
                // Get the list of recipients from the database
                List<Recipient> recipients = await _unitOfWork.RecipientRepository.GetAllRecipientsAsync();

                // Create the view model
                RecipientListViewModel vm = new()
                {
                    Recipients = recipients
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while navigating to the recipient page.");
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                // Create the view model
                RecipientAddEditViewModel vm = new();

                // Return the view
                return View("AddEdit", vm);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while navigating to the recipient add page.");
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAsync(int recipientId)
        {
            try
            {
                // Get the recipient from the database
                Recipient? recipient = await _unitOfWork.RecipientRepository.GetRecipientByIdAsync(recipientId);

                if (recipient == null)
                {
                    // Create a recipient
                    return RedirectToAction(nameof(Create));
                }
                else
                {
                    // Setup the view model
                    RecipientAddEditViewModel vm = new()
                    {
                        RecipientId = recipient.RecipientId,
                        Name = recipient.Name,
                        Email = recipient.Email,
                        Enabled = recipient.Enabled
                    };

                    // Return the view
                    return View("AddEdit", vm);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while navigating the the recipient edit page.");
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEditRecipientAsync(RecipientAddEditViewModel model)
        {
            try
            {
                // Validate the model
                if (!ModelState.IsValid)
                {
                    return View("AddEdit", model);
                }
                else
                {              
                    // Determine if we are adding or editing a recipient
                    if (model.RecipientId == null)
                    {
                        // Adding a recipient
                        await _unitOfWork.RecipientRepository.InsertRecipientAsync(model.Name, model.Email, model.Enabled);
                    }
                    else
                    {
                        // Editing a recipient
                        Recipient? recipient = await _unitOfWork.RecipientRepository.GetRecipientByIdAsync(model.RecipientId.Value);

                        if (recipient != null)
                        {
                            recipient.Name = model.Name;
                            recipient.Email = model.Email;
                            recipient.Enabled = model.Enabled;

                            _unitOfWork.RecipientRepository.UpdateRecipient(recipient);
                        }
                    }

                    // Save the changes made to the database
                    await _unitOfWork.SaveAsync();

                    // Return to list
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while {action} the recipient.", model.RecipientId == null ? "adding" : "editing");
                return RedirectToAction("Index", "Error");
            }
        }
    }
}