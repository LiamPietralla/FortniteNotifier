using FortniteNotifier.Shared.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FortniteNotifier.Shared.Data.Repositories
{
    public class RecipientRepository
    {
        private readonly FortniteContext _context;

        public RecipientRepository(FortniteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all recipients from the database
        /// </summary>
        /// <returns>A list of all recipients, an empty list is returned if no recipients exist.</returns>
        public async Task<List<Recipient>> GetAllRecipientsAsync()
        {
            return await _context.Recipients.ToListAsync();
        }

        /// <summary>
        /// Gets all recipients from the database that are enabled to receive the notifiation
        /// </summary>
        /// <returns>A list of enabled recipients, an empty list is returned if no enabled recipients exist.</returns>
        public async Task<List<Recipient>> GetAllEnabledRecipientsAsync()
        {
            return await _context.Recipients.Where(x => x.Enabled).ToListAsync();
        }

        /// <summary>
        /// Gets a single recipient from the database by ID
        /// </summary>
        /// <returns>The specified recipient, or null if they could not be found.</returns>
        public async Task<Recipient?> GetRecipientByIdAsync(int recipientId)
        {
            return await _context.Recipients.FirstOrDefaultAsync(x => x.RecipientId == recipientId);
        }

        /// <summary>
        /// Insert a recipient into the database
        /// </summary>
        /// <param name="name">The name of the recipient, this will be used for the email name as well.</param>
        /// <param name="email">Email of the recipient.</param>
        /// <param name="enabled">Whether the recipient is enabled for email notifications</param>
        /// <returns>A completed task when inserted.</returns>
        public async Task InsertRecipientAsync(string name, string email, bool enabled)
        {
            // Create the recipient object
            Recipient recipient = new()
            {
                Name = name,
                Email = email,
                Enabled = enabled,
                InsertTimestamp = DateTime.UtcNow
            };

            // Insert the recipient into the table
            await _context.Recipients.AddAsync(recipient);
        }

        /// <summary>
        /// Update a recipient in the database
        /// </summary>
        /// <param name="recipient">The changed recipient object</param>       
        /// <returns>A completed task when updated.</returns>
        public EntityEntry<Recipient> UpdateRecipient(Recipient recipient)
        {
            return _context.Recipients.Update(recipient);
        }   
        
        public async Task<Recipient?> GetRecipientByEmailAsync(string email)
        {
            return await _context.Recipients.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}