using Dapper;
using FortniteNotifier.Shared.Data.Models;
using FortniteNotifier.Shared.Data.Models.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace FortniteNotifier.Shared.Data.Repositories
{
    public class VersionRecordRepository
    {
        private readonly FortniteContext _context;

        public VersionRecordRepository(FortniteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a version record from the datbase with the matching version, returns null if nothing is found
        /// </summary>
        /// <param name="version">The version to retrive from the DB</param>
        /// <returns>The version or null</returns>
        public async Task<VersionRecord?> GetVersionRecordByVersionAsync(string version)
        {
            return await _context.VersionRecords.FirstOrDefaultAsync(x => x.Version == version);
        }

        public async Task<IEnumerable<VersionRecord>> GetAllVersionRecordsAsync()
        {
            return await _context.VersionRecords.ToListAsync();
        }

        /// <summary>
        /// Delete all versions from the database
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAllVersionsAsync()
        {
            // Get all the versions
            IEnumerable<VersionRecord> versions = await GetAllVersionRecordsAsync();

            // Delete all the versions
            _context.VersionRecords.RemoveRange(versions);
        }

        public async Task<VersionRecord> InsertVersionRecordAsync(string version, VersionStatusEnum versionStatus)
        {
            // Create the version record object
            VersionRecord versionRecord = new()
            {
                Version = version,
                VersionStatus = versionStatus,
                InsertTimestamp = DateTime.UtcNow
            };

            // Add the version record to the database
            await _context.VersionRecords.AddAsync(versionRecord);

            // Return the version record
            return versionRecord;
        }

        public void UpdateVersionRecord(VersionRecord versionRecord)
        {
            // Update the version record
            _context.VersionRecords.Update(versionRecord);
        }    
    }
}
