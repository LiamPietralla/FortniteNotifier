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
using System.Text;

namespace FortniteNotifier.Admin.Controllers
{
    [Authorize]
    public class EmailController : Controller
    {
        private readonly ConfigHelper _configHelper;
        private readonly UnitOfWork _unitOfWork;

        public EmailController(IConfiguration configuration, UnitOfWork unitOfWork)
        {
            _configHelper = new ConfigHelper(configuration);
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            try
            {
                return View(new EmailViewModel());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while navigating to the email page.");
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendTestEmailAsync(EmailViewModel model)
        {
            try
            {
                Log.Information("START: SendTestEmailAsync");

                // Validate the email model
                if (!ModelState.IsValid)
                {
                    return View(nameof(Index), model);
                }

                // Create the smtp helper
                SMTPHelper smtp = new(_configHelper.SMTPURL, _configHelper.SMTPPort, _configHelper.SMTPUsername, _configHelper.SMTPPassword, _configHelper.SMTPFromName, _configHelper.SMTPFromAddress, Log.Logger);

                // Create the text body
                MimeKit.BodyBuilder bodyBuilder = new()
                {
                    TextBody = model.Body
                };

                // Send the email
                await smtp.SendEmailAsync(model.Subject, bodyBuilder.ToMessageBody(), model.ToList ?? "", model.CCList ?? "", model.BCCList ?? "");

                Log.Information("END: SendTestEmailAsync");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while sending the test email.");
                return RedirectToAction("Index", "Error");
            }
        }

        public IActionResult Notify()
        {
            try
            {
                return View(new NotifyEmailViewModel());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while navigating to the notify page.");
                return RedirectToAction("Index", "Error");
            }
        }

        public async Task<IActionResult> SendNotifyEmailAsync(NotifyEmailViewModel model)
        {
            try
            {
                Log.Information("START: SendNotifyEmailAsync");

                // Validate the email model
                if (!ModelState.IsValid)
                {
                    return View(nameof(Notify), model);
                }

                // Get the formatted body
                EmailHelper email = new(_configHelper.EmailNotifyTemplatePath, _configHelper.EmailEmailImagePath, _configHelper.EmailGitHubImagePath);
                string body = GetFormattedNotifyBody(model.Body);
                MimeEntity mineEntityBody = email.GetNotifyEmailBody(body);
                
                // Create the smtp helper
                SMTPHelper smtp = new(_configHelper.SMTPURL, _configHelper.SMTPPort, _configHelper.SMTPUsername, _configHelper.SMTPPassword, _configHelper.SMTPFromName, _configHelper.SMTPFromAddress, Log.Logger);

                // Get the recipients from the database
                IEnumerable<InternetAddress> bccList = model.IncludeDisabled ? await GetAllRecipientsAsListOfAddressesAsync(_unitOfWork) : await GetEnabledRecipientsAsListOfAddressesAsync(_unitOfWork);

                // Send the email
                await smtp.SendEmailAsync(model.Subject, mineEntityBody, new List<InternetAddress>(), new List<InternetAddress>() { InternetAddress.Parse(model.CCRecipient) }, bccList);

                Log.Information("END: SendNotifyEmailAsync");

                return RedirectToAction(nameof(Notify));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while sending the notify email.");
                return RedirectToAction("Index", "Error");
            }
        }

        private static string GetFormattedNotifyBody(string plainBody)
        {
            StringBuilder body = new();

            // Split plainBody based on new lines to add to format
            string[] bodyLines = plainBody.Split(Environment.NewLine);

            // Add each body line to the body with correct formatting
            foreach (string line in bodyLines)
            {
                body.AppendLine($"<p style=\"font-size: 14px; line-height: 170%;\">{line}</p>");
            }

            return body.ToString();
        }

        private static async Task<IEnumerable<InternetAddress>> GetEnabledRecipientsAsListOfAddressesAsync(UnitOfWork unitOfWork)
        {
            // Get all enabled recipients from the database
            IEnumerable<Recipient> recipients = await unitOfWork.RecipientRepository.GetAllEnabledRecipientsAsync();

            // Loop through each recipient and add it to the list
            List<InternetAddress> addresses = new();

            foreach (Recipient recipient in recipients)
            {
                addresses.Add(new MailboxAddress(recipient.Name, recipient.Email));
            }

            return addresses;
        }

        private static async Task<IEnumerable<InternetAddress>> GetAllRecipientsAsListOfAddressesAsync(UnitOfWork unitOfWork)
        {
            // Get all enabled recipients from the database
            IEnumerable<Recipient> recipients = await unitOfWork.RecipientRepository.GetAllRecipientsAsync();

            // Loop through each recipient and add it to the list
            List<InternetAddress> addresses = new();

            foreach (Recipient recipient in recipients)
            {
                addresses.Add(new MailboxAddress(recipient.Name, recipient.Email));
            }

            return addresses;
        }
    }
}
