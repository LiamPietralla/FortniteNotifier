using FortniteNotifier.Shared.Data.Models.Enums;

namespace FortniteNotifier.Shared.Data.Models
{
    public class VersionRecord
    {
        public int Id { get; set; }
        public string Version { get; set; } = null!;
        public VersionStatusEnum VersionStatus { get; set; }
        public DateTime InsertTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
