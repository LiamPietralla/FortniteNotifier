using AngleSharp;
using AngleSharp.Dom;
using FortniteNotifier.Shared.Data;
using FortniteNotifier.Shared.Data.Models;
using FortniteNotifier.Shared.Data.Models.Enums;
using FortniteNotifier.Helpers;
using FortniteNotifier.Shared.Helpers;
using MimeKit;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace FortniteNotifier
{
    public class Worker : BackgroundService
    {
        private readonly ConfigHelper _config;

        public Worker(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            // Setup the configuration helper
            _config = new(configuration);

            Log.Information("Worker created");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Worker started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Log.Information("Worker running at: {time}", DateTimeOffset.Now);

                    // Create a new context for the database
                    DbContextOptions<FortniteContext> options = new DbContextOptionsBuilder<FortniteContext>().UseNpgsql(_config.ConnectionString).Options;
                    FortniteContext fortniteContext = new(options);
                    UnitOfWork unitOfWork = new(fortniteContext);

                    await CheckForUpdateAsync(unitOfWork, stoppingToken);
                    
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An unexpected error has occured");
                }

                await Task.Delay(_config.PollingTime, stoppingToken);
            }

            Log.Information("Worker stopped");
        }

        private async Task CheckForUpdateAsync(UnitOfWork unitOfWork, CancellationToken stoppingToken)
        {
            // Access the epic games store status page to check for updates
            AngleSharp.IConfiguration config = Configuration.Default.WithDefaultLoader();
            BrowsingContext browsingContext = new(config);
            Url url = new(_config.StatusPageUrl);
            IDocument document = await browsingContext.OpenAsync(url, cancel: stoppingToken);

            // Check the incidents list 
            IElement? incidentList = document.QuerySelector("div.incidents-list");
            
            if (incidentList is not null)
            {
                // Get all incident containers
                IHtmlCollection<IElement> incidentContainers = incidentList.QuerySelectorAll("div.incident-container");

                foreach (IElement incident in incidentContainers)
                {
                    if (incident.InnerHtml.Contains(_config.StatusTitlePart))
                    {
                        // Get the incident title
                        IElement? incidentTitle = incident.QuerySelector("div.incident-title");

                        if (incidentTitle is not null)
                        {
                            // Parse the version from the incident title
                            string version = GetVersionStringFromIncidentTitle(incidentTitle.TextContent);

                            // Get the version status from the incident details
                            VersionStatusEnum versionStatus = GetVersionStatusFromIncident(incident);
                            
                            // Check if a DB record already exists for this version
                            VersionRecord? versionRecord = await unitOfWork.VersionRecordRepository.GetVersionRecordByVersionAsync(version);

                            // If the DB record did not exist then insert it
                            if (versionRecord is null)
                            {
                                _ = await unitOfWork.VersionRecordRepository.InsertVersionRecordAsync(version, versionStatus);
                            }
                            else
                            {
                                // If the DB record already exists then update it if the status has changed
                                if (versionRecord.VersionStatus != versionStatus)
                                {
                                    versionRecord.VersionStatus = versionStatus;
                                    unitOfWork.VersionRecordRepository.UpdateVersionRecord(versionRecord);
                                }
                            }

                            // Save the changes to the DB
                            await unitOfWork.SaveAsync();

                            // Check if we need to send a notification
                            if (versionStatus == VersionStatusEnum.COMPLETED && versionRecord?.VersionStatus != VersionStatusEnum.COMPLETED)
                            {
                                // Send notification for completed version
                                Log.Information("Sending notification for completed version: {version}", version);

                                EmailHelper email = new(_config.EmailTemplatePath, _config.EmailEmailImagePath, _config.EmailGitHubImagePath);

                                // Get the email body
                                MimeEntity body = email.GetVersionCompletedEmailBody(version, _config.RequestUnsubscribeUrl);

                                // Create the smtp helper
                                SMTPHelper smtp = new(_config.SMTPURL, _config.SMTPPort, _config.SMTPUsername, _config.SMTPPassword, _config.SMTPFromName, _config.SMTPFromAddress, Log.Logger);

                                // Get the recipients from the database
                                IEnumerable<InternetAddress> bccList = await GetRecipientsAsListOfAddressesAsync(unitOfWork);

                                // Send the email
                                await smtp.SendEmailAsync($"Fortnite Update Completed - {version}", body, new List<InternetAddress>(), new List<InternetAddress>(), bccList);
                            }
                        }
                    }
                }
            }
        }

        private static async Task<IEnumerable<InternetAddress>> GetRecipientsAsListOfAddressesAsync(UnitOfWork unitOfWork)
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

        private static string GetVersionStringFromIncidentTitle(string title)
        {
            // Split the title into parts
            string[] titleParts = title.Split(" ");

            // Version string begins with v
            int versionIndex = Array.FindIndex(titleParts, x => x.StartsWith("v"));

            return titleParts[versionIndex];
        }

        private static VersionStatusEnum GetVersionStatusFromIncident(IElement incident)
        {
            if (incident.InnerHtml.Contains("<strong>Completed</strong>"))
            {
                return VersionStatusEnum.COMPLETED;
            }

            if (incident.InnerHtml.Contains("<strong>In progress</strong>"))
            {
                return VersionStatusEnum.IN_PROGRESS;
            }

            return VersionStatusEnum.SCHEDULED;
        }
    }
}