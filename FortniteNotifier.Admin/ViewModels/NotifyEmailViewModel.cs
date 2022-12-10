using System.ComponentModel.DataAnnotations;

namespace FortniteNotifier.Admin.ViewModels
{
    public class NotifyEmailViewModel
    {
        [Required]
        public string CCRecipient { get; set; } = string.Empty;
        [Required]
        public string Subject { get; set; } = string.Empty;
        [Required]
        public string Body { get; set; } = string.Empty;
        public bool IncludeDisabled { get; set; } = false;
    }
}
