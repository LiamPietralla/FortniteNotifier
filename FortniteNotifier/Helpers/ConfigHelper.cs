using FortniteNotifier.Shared.Helpers;
using FortniteNotifier.Shared.Infrastructure.Exceptions;

namespace FortniteNotifier.Helpers
{
    public class ConfigHelper : SharedConfigHelper
    {
        private readonly IConfiguration _configuration;

        public ConfigHelper(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        #region Backing Fields

        private string? _logLocation;
        private int? _pollingTime;
        private string? _statusPageUrl;
        private string? _statusTitlePart;
        private string? _emailTemplatePath;
        private string? _emailEmailImagePath;
        private string? _emailGitHubImagePath;
        private string? _errorNotifyEmails;

        #endregion

        #region Properties

        public string LogLocation => _logLocation ??= _configuration.GetValue<string>("LogLocation") ?? throw new InvalidConfigurationException("LogLocation has not been provided");
        public int PollingTime => _pollingTime ??= _configuration.GetValue<int>("PollingTime");
        public string StatusPageUrl => _statusPageUrl ??= _configuration.GetValue<string>("StatusPageUrl") ?? throw new InvalidConfigurationException("StatusPageUrl has not been provided");
        public string StatusTitlePart => _statusTitlePart ??= _configuration.GetValue<string>("StatusTitlePart") ?? throw new InvalidConfigurationException("StatusTitlePart has not been provided");
        public string EmailTemplatePath => _emailTemplatePath ??= _configuration.GetValue<string>("EmailTemplatePath") ?? throw new InvalidConfigurationException("EmailTemplatePath has not been provided");
        public string EmailEmailImagePath => _emailEmailImagePath ??= _configuration.GetValue<string>("EmailEmailImagePath") ?? throw new InvalidConfigurationException("EmailEmailImagePath has not been provided");
        public string EmailGitHubImagePath => _emailGitHubImagePath ??= _configuration.GetValue<string>("EmailGitHubImagePath") ?? throw new InvalidConfigurationException("EmailGitHubImagePath has not been provided");
        public string ErrorNotifyEmails => _errorNotifyEmails ??= _configuration.GetValue<string>("ErrorNotifyEmails") ?? throw new InvalidConfigurationException("ErrorNotifyEmails has not been provided");

        #endregion
    }
}
