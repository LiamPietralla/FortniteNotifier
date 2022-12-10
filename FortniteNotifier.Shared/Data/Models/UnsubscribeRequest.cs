namespace FortniteNotifier.Shared.Data.Models
{
    public class UnsubscribeRequest
    {
        public int UnsubscribeRequestId { get; set; }
        public Guid UnsubscribeRequestUrlId { get; set; }
        public string Email { get; set; } = null!;
        public int RecipientId { get; set; }
        public DateTime InsertTimestamp { get; set; }
        public bool Completed { get; set; } = false;
        public DateTime? CompleteTimestamp { get; set; }

        public virtual Recipient Recipient { get; set; } = null!;
    }
}