namespace FortniteNotifier.Shared.Data.Models
{
    public class Recipient
    {
        public int RecipientId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool Enabled { get; set; } = true;
        public DateTime InsertTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}