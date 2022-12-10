using System.ComponentModel.DataAnnotations;

namespace FortniteNotifier.Admin.ViewModels
{
    public class RecipientAddEditViewModel
    {
        public int? RecipientId { get; set; } = null;
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(255)]
        public string Email { get; set; } = string.Empty;
        public bool Enabled { get; set; } = false;
    }
}