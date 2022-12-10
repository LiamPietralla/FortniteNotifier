using Microsoft.Extensions.Configuration;

namespace FortniteNotifier.Shared.Helpers
{
    public class SharedConfigHelper
    {
        private readonly IConfiguration _configuration;
        
        public SharedConfigHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Backing Fields

        private string? _sMTPURL;
        private int? _sMTPPort;
        private string? _sMTPUsername;
        private string? _sMTPPassword;
        private string? _sMTPFromName;
        private string? _sMTPFromAddress;
        private string? _connectionString;
        private string? _requestUnsubscribeUrl;

        #endregion

        #region Properties

        public string SMTPURL => _sMTPURL ??= _configuration.GetValue<string>("SMTPURL") ?? throw new Exception("SMTPURL is null");
        public int SMTPPort => _sMTPPort ??= _configuration.GetValue<int>("SMTPPort");
        public string SMTPUsername => _sMTPUsername ??= _configuration.GetValue<string>("SMTPUsername") ?? throw new Exception("SMTPUsername is null");
        public string SMTPPassword => _sMTPPassword ??= _configuration.GetValue<string>("SMTPPassword") ?? throw new Exception("SMTPPassword is null");
        public string SMTPFromName => _sMTPFromName ??= _configuration.GetValue<string>("SMTPFromName") ?? throw new Exception("SMTPFromName is null");
        public string SMTPFromAddress => _sMTPFromAddress ??= _configuration.GetValue<string>("SMTPFromAddress") ?? throw new Exception("SMTPFromAddress is null");
        public string ConnectionString => _connectionString ??= _configuration.GetValue<string>("ConnectionString") ?? throw new Exception("ConnectionString is null");
        public string RequestUnsubscribeUrl => _requestUnsubscribeUrl ??= _configuration.GetValue<string>("RequestUnsubscribeUrl") ?? throw new Exception("RequestUnsubscribeUrl is null");
        
        #endregion
    }
}