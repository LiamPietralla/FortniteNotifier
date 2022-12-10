using MailKit.Net.Smtp;
using MimeKit;
using Serilog;

namespace FortniteNotifier.Shared.Helpers
{
    public class SMTPHelper
    {
        // SMTP Settings
        private readonly int _smtpPort;
        private readonly string _smtpURL;
        private readonly string _smtpUserName;
        private readonly string _smtpPassword;
        private readonly string _smtpFromName;
        private readonly string _smtpFromAddress;

        // Logger
        private readonly ILogger? Log;

        // From Address
        private MailboxAddress? _fromAddress;
        private MailboxAddress FromAddress => _fromAddress ??= new(_smtpFromName, _smtpFromAddress);

        public SMTPHelper(string smtpURL, int smtpPort, string smtpUserName, string smtpPassword, string smtpFromName, string smtpFromAddress, ILogger? logger = null)
        {
            _smtpPort = smtpPort;
            _smtpURL = smtpURL;
            _smtpUserName = smtpUserName;
            _smtpPassword = smtpPassword;
            _smtpFromName = smtpFromName;
            _smtpFromAddress = smtpFromAddress;

            Log = logger;
        }

        public async Task SendEmailAsync(string subject, MimeEntity body, string toList, string ccList, string bccList)
        {
            // Convert to, cc and bcc lists to internet addresses
            List<InternetAddress> to = new();
            foreach (string address in toList.Split(","))
            {
                if (!string.IsNullOrWhiteSpace(address))
                {
                    to.Add(InternetAddress.Parse(address));
                }
            }

            List<InternetAddress> cc = new();
            foreach (string address in ccList.Split(","))
            {
                if (!string.IsNullOrWhiteSpace(address))
                {
                    cc.Add(InternetAddress.Parse(address));
                }
            }

            List<InternetAddress> bcc = new();
            foreach (string address in bccList.Split(","))
            {
                if (!string.IsNullOrWhiteSpace(address))
                {
                    bcc.Add(InternetAddress.Parse(address));
                }
            }

            // Send the email
            await SendEmailAsync(subject, body, to, cc, bcc);
        }

        public async Task SendEmailAsync(string subject, MimeEntity body, IEnumerable<InternetAddress> toList, IEnumerable<InternetAddress> ccList, IEnumerable<InternetAddress> bccList)
        {
            // Create the message
            MimeMessage message = new()
            {
                Subject = subject,
                Body = body
            };

            // Add the recipients
            try
            {
                message.To.AddRange(toList);
                message.Cc.AddRange(ccList);
                message.Bcc.AddRange(bccList);
            }
            catch (Exception ex)
            {
                Log?.Error(ex, "An error occurred while adding recipients to the email.");
                throw;
            }

            // Add the from
            message.From.Add(FromAddress);

            // Send the message
            await SendEmailAsync(message);
        }

        private async Task SendEmailAsync(MimeMessage message)
        {
            using SmtpClient client = new();
            try
            {
                await client.ConnectAsync(_smtpURL, _smtpPort);
            }
            catch (Exception ex)
            {
                Log?.Error(ex, "An error occured when connecting to the mail server: {smtpURL}:{smtpPort}", _smtpURL, _smtpPort);
                throw;
            }

            try
            {
                // Authenticate if required
                if (_smtpUserName != "" && _smtpPassword != "")
                {
                    await client.AuthenticateAsync(_smtpUserName, _smtpPassword);
                }
            }
            catch (Exception ex)
            {
                Log?.Error(ex, "An error occured when authenticating to the mail server: {smtpURL}:{smtpPort}", _smtpURL, _smtpPort);
                throw;
            }

            try
            {
                // Send the email
                _ = await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                Log?.Error(ex, "An error occured when sending the email: {smtpURL}:{smtpPort}", _smtpURL, _smtpPort);
                throw;
            }

            // Cleanup
            await client.DisconnectAsync(true);
        }
    }
}