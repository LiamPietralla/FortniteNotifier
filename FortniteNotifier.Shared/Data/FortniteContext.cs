using FortniteNotifier.Shared.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FortniteNotifier.Shared.Data
{
    public class FortniteContext : DbContext
    {
        public FortniteContext(DbContextOptions<FortniteContext> options) : base(options) { }    

        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<UnsubscribeRequest> UnsubscribeRequests { get; set; }
        public DbSet<VersionRecord> VersionRecords { get; set; }
    }
}