using System.ComponentModel.DataAnnotations;

namespace FortniteNotifier.Admin.ViewModels
{
    public class EmailViewModel
    {
        [Required]
        public string Subject { get; set; } = string.Empty;
        [Required]
        public string Body { get; set; } = string.Empty;
        public string? ToList { get; set; }
        public string? CCList { get; set; }
        public string? BCCList { get; set; }
    }
}
