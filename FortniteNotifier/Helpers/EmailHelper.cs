using FortniteNotifier.Shared.Helpers;
using MimeKit;

namespace FortniteNotifier.Helpers
{
    internal class EmailHelper
    {
        private readonly string _emailTemplatePath;
        private readonly string _emailEmailImagePath;
        private readonly string _emailGitHubImagePath;

        internal EmailHelper(string emailTemplatePath, string emailEmailImagePath, string emailGitHubImagePath)
        {
            _emailTemplatePath = emailTemplatePath;
            _emailEmailImagePath = emailEmailImagePath;
            _emailGitHubImagePath = emailGitHubImagePath;
        }
        
        internal MimeEntity GetVersionCompletedEmailBody(string version, string unsubscribeUrl)
        {
            // First we need to load the email template
            string emailTemplate = File.ReadAllText(CrossPlatformHelper.PathCombine(AppContext.BaseDirectory, _emailTemplatePath));

            // Update the template with the version
            emailTemplate = emailTemplate.Replace("{{version}}", version);

            // Update the template with the unsubscribe url
            emailTemplate = emailTemplate.Replace("{{unsubscribeUrl}}", unsubscribeUrl);

            // Set the body 
            BodyBuilder bodyBuilder = new()
            {
                HtmlBody = emailTemplate
            };

            // Set the images
            MimeEntity emailImage = bodyBuilder.LinkedResources.Add(CrossPlatformHelper.PathCombine(AppContext.BaseDirectory, _emailEmailImagePath));
            MimeEntity githubImage = bodyBuilder.LinkedResources.Add(CrossPlatformHelper.PathCombine(AppContext.BaseDirectory, _emailGitHubImagePath));

            // Set the content id
            emailImage.ContentId = "email.png";
            githubImage.ContentId = "github.png";

            return bodyBuilder.ToMessageBody();
        }
    }
}
