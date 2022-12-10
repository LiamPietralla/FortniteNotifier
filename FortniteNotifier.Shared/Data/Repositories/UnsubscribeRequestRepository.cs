using FortniteNotifier.Shared.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FortniteNotifier.Shared.Data.Repositories
{
    public class UnsubscribeRequestRepository
    {
        private readonly FortniteContext _context;

        public UnsubscribeRequestRepository(FortniteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all unsubscribe requests from the database
        /// </summary>
        /// <returns>A list of unsubscribe requests.</returns>
        public async Task<List<UnsubscribeRequest>> GetAllUnsubscribeRequestAsync()
        {
            return await _context.UnsubscribeRequests.ToListAsync();
        }

        /// <summary>
        /// Get a unsubscribe request from the database using the GUID
        /// </summary>
        /// <returns>The unsubscribe request.</returns>
        public async Task<UnsubscribeRequest?> GetUnsubscribeRequestByUnsubscribeRequestUrlIdAsync(Guid guid)
        {
            return await _context.UnsubscribeRequests.Include(x => x.Recipient).FirstOrDefaultAsync(x => x.UnsubscribeRequestUrlId == guid);
        }

        /// <summary>
        /// Get a unsubscribe request from the database using the GUID
        /// </summary>
        /// <returns>The unsubscribe request.</returns>
        public async Task<UnsubscribeRequest?> GetUnsubscribeRequestByUnsubscribeRequestUrlIdAsync(string guid)
        {
            return await GetUnsubscribeRequestByUnsubscribeRequestUrlIdAsync(Guid.Parse(guid));
        } 

        /// <summary>
        /// Insert a new unsubscribe request into the database
        /// </summary>
        /// <param name="email">The email for the request</param>
        /// <returns>A completed task when the record is inserted.</returns>
        public async Task<UnsubscribeRequest> InsertUnsubsribeRequestAsync(string email)
        {
            // Create the unsubscribe request object
            UnsubscribeRequest request = new()
            {
                Email = email,
                Recipient = await _context.Recipients.FirstAsync(x => x.Email == email),
                UnsubscribeRequestUrlId = Guid.NewGuid(),
                InsertTimestamp = DateTime.UtcNow,
                Completed = false
            };

            // Insert the record
            await _context.UnsubscribeRequests.AddAsync(request);

            // Return the request
            return request;
        }

        /// <summary>
        /// Complete a existing unsubscribe request in the database
        /// </summary>
        public async Task CompleteUnsubsribeRequestAsync(Guid guid)
        {
            // Get the request
            UnsubscribeRequest? request = await GetUnsubscribeRequestByUnsubscribeRequestUrlIdAsync(guid);

            // Check if the request exists
            if (request == null)
            {
                throw new Exception("Unsubscribe request does not exist");
            }

            // Update the request
            request.Completed = true;
            request.CompleteTimestamp = DateTime.UtcNow;

            _context.UnsubscribeRequests.Update(request);
        }
    }
}