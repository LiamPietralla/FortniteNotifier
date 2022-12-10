using FortniteNotifier.Shared.Helpers;
using FortniteNotifier.Shared.Infrastructure.Exceptions;

namespace FortniteNotifier.Admin.Helpers
{
    public class ConfigHelper : SharedConfigHelper
    {
        private readonly IConfiguration _configuration;

        public ConfigHelper(IConfiguration configuration): base(configuration)
        {
            _configuration = configuration;
        }

        #region Backing Fields

        private string? _loginPassword;
        private string? _logLocation;
        private string? _emailNotifyTemplatePath;
        private string? _emailUnsubscribeTemplatePath;
        private string? _emailEmailImagePath;
        private string? _emailGitHubImagePath;
        private string? _unsubscribeUrl;

        #endregion

        #region Properties

        public string LoginPassword => _loginPassword ??= _configuration.GetValue<string>("LoginPassword") ?? throw new InvalidConfigurationException("LoginPassword is null");
        public string LogLocation => _logLocation ??= _configuration.GetValue<string>("LogLocation") ?? throw new Exception("LogLocation is null");
        public string EmailNotifyTemplatePath => _emailNotifyTemplatePath ??= _configuration.GetValue<string>("EmailNotifyTemplatePath") ?? throw new InvalidConfigurationException("EmailTemplatePath has not been provided");
        public string EmailUnsubscribeTemplatePath => _emailUnsubscribeTemplatePath ??= _configuration.GetValue<string>("EmailUnsubscribeTemplatePath") ?? throw new InvalidConfigurationException("EmailUnsubscribeTemplatePath has not been provided");
        public string EmailEmailImagePath => _emailEmailImagePath ??= _configuration.GetValue<string>("EmailEmailImagePath") ?? throw new InvalidConfigurationException("EmailEmailImagePath has not been provided");
        public string EmailGitHubImagePath => _emailGitHubImagePath ??= _configuration.GetValue<string>("EmailGitHubImagePath") ?? throw new InvalidConfigurationException("EmailGitHubImagePath has not been provided");
        public string UnsubscribeUrl => _unsubscribeUrl ??= _configuration.GetValue<string>("UnsubscribeUrl") ?? throw new InvalidConfigurationException("UnsubscribeUrl has not been provided");

        #endregion
    }
}