using FortniteNotifier.Admin.Helpers;
using FortniteNotifier.Admin.ViewModels;
using FortniteNotifier.Helpers;
using FortniteNotifier.Shared.Data;
using FortniteNotifier.Shared.Data.Models;
using FortniteNotifier.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Serilog;
using Serilog.Core;

namespace FortniteNotifier.Admin.Controllers
{
    public class UnsubscribeController : Controller
    {
        private readonly ConfigHelper _configHelper;
        private readonly UnitOfWork _unitOfWork;

        public UnsubscribeController(IConfiguration configuration, UnitOfWork unitOfWork)
        {
            _configHelper = new ConfigHelper(configuration);
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public async Task<IActionResult> Index(bool loadAll = false)
        {
            try
            {
                // Get all the unsubscribe requests
                List<UnsubscribeRequest> unsubscribeRequests = await _unitOfWork.UnsubscribeRequestRepository.GetAllUnsubscribeRequestAsync();

                // Order then by date time inserted
                unsubscribeRequests = unsubscribeRequests.OrderByDescending(x => x.InsertTimestamp).ToList();

                // If load all is not set only take the top 10
                if (!loadAll)
                {
                    unsubscribeRequests = unsubscribeRequests.Take(10).ToList();
                }

                // Create the view model
                UnsubscribeViewModel vm = new()
                {
                    UnsubscribeRequests = unsubscribeRequests
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while navigating to the index page.");
                return RedirectToAction("Index", "Error");
            }
        }

        public IActionResult Requesting()
        {
            try
            {
                return View(new RequestingUnsubscibeViewModel());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while navigating to the requesting page.");
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Requesting(RequestingUnsubscibeViewModel vm)
        {
            try
            {
                Log.Information("START: Requesting for {Email}", vm.Email);

                if (!ModelState.IsValid)
                {
                    return View(vm);
                }

                // Get the recipient for this email address if they exist
                Recipient? recipient = await _unitOfWork.RecipientRepository.GetRecipientByEmailAsync(vm.Email);

                // If the recipient is not null then email them a unsubscription email
                if (recipient != null)
                {
                    // Generate a new unsubscription request
                    UnsubscribeRequest unsubRequest = await _unitOfWork.UnsubscribeRequestRepository.InsertUnsubsribeRequestAsync(vm.Email);

                    // Save the request
                    await _unitOfWork.SaveAsync();

                    // Create the email to send to this recipient
                    EmailHelper email = new(_configHelper.EmailUnsubscribeTemplatePath, _configHelper.EmailEmailImagePath, _configHelper.EmailGitHubImagePath);

                    // Prepare the unsubscription link
                    string unsubscribeLink = $"{_configHelper.UnsubscribeUrl}{unsubRequest.UnsubscribeRequestUrlId}";

                    MimeEntity mineEntityBody = email.GetUnsubscribeEmailBody(unsubscribeLink, _configHelper.RequestUnsubscribeUrl);

                    // Create the smtp helper
                    SMTPHelper smtp = new(_configHelper.SMTPURL, _configHelper.SMTPPort, _configHelper.SMTPUsername, _configHelper.SMTPPassword, _configHelper.SMTPFromName, _configHelper.SMTPFromAddress);

                    // Send the email
                    await smtp.SendEmailAsync("Unsubscription Request", mineEntityBody, new List<InternetAddress>() { new MailboxAddress(recipient.Name, vm.Email) }, new List<InternetAddress>(), new List<InternetAddress>());
                }

                Log.Information("END: Requesting for {Email}", vm.Email);

                return View("Requested");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while requesting an unsubscribe request.");
                return RedirectToAction("Index", "Error");
            }
        }

        public IActionResult Requested()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while navigating to the requested page.");
                return RedirectToAction("Index", "Error");
            }
        }

        public async Task<IActionResult> Completing(string requestId)
        {
            try
            {
                Log.Information("START: Completing for: {requestId}", requestId);

                // Get the request for this id
                UnsubscribeRequest? unsubRequest = await _unitOfWork.UnsubscribeRequestRepository.GetUnsubscribeRequestByUnsubscribeRequestUrlIdAsync(requestId);

                // If the request is not null then we can complete it and update the recipient
                if (unsubRequest != null)
                {
                    // Get the recipient
                    Recipient? recipient = await _unitOfWork.RecipientRepository.GetRecipientByIdAsync(unsubRequest.RecipientId);

                    // If the recipient is not null then we can complete the request and update them
                    if (recipient != null)
                    {
                        // Update the recipient
                        recipient.Enabled = false;
                        recipient.UpdateTimestamp = DateTime.UtcNow;
                        _unitOfWork.RecipientRepository.UpdateRecipient(recipient);

                        // Update the request
                        await _unitOfWork.UnsubscribeRequestRepository.CompleteUnsubsribeRequestAsync(unsubRequest.UnsubscribeRequestUrlId);

                        // Save the changes
                        await _unitOfWork.SaveAsync();
                    }
                }

                Log.Information("END: Completing for: {requestId}", requestId);

                return View("Confirmed");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while confirming the subscribe request.");
                return RedirectToAction("Index", "Error");
            }
        }

        public IActionResult Confirmed()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while navigating to the confirmed page.");
                return RedirectToAction("Index", "Error");
            }
        }
    }
}