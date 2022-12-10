using System.ComponentModel.DataAnnotations;

namespace FortniteNotifier.Admin.ViewModels
{
    public class RequestingUnsubscibeViewModel
    {
        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;
    }
}