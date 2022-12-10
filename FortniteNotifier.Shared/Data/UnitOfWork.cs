using FortniteNotifier.Shared.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FortniteNotifier.Shared.Data
{
    public class UnitOfWork : IDisposable
    {
        private readonly FortniteContext _context;

        public UnitOfWork(FortniteContext context)
        {
            _context = context;
        }

        private RecipientRepository? recipientRepository;
        private UnsubscribeRequestRepository? unsubscribeRequestRepository;
        private VersionRecordRepository? versionRecordRepository;

        public RecipientRepository RecipientRepository => recipientRepository ??= new RecipientRepository(_context);
        public UnsubscribeRequestRepository UnsubscribeRequestRepository => unsubscribeRequestRepository ??= new UnsubscribeRequestRepository(_context);
        public VersionRecordRepository VersionRecordRepository => versionRecordRepository ??= new VersionRecordRepository(_context);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        } 

        #region IDisposable Logic

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                    _context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UnitOfWork()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Logic
    }
}