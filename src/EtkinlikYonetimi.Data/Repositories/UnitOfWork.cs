using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using EtkinlikYonetimi.Data.Context;
using System;
using System.Threading.Tasks;

namespace EtkinlikYonetimi.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EtkinlikYonetimiDbContext _context;
        private IDbContextTransaction? _transaction;
        
        public UnitOfWork(EtkinlikYonetimiDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Events = new EventRepository(_context);
        }

        public IUserRepository Users { get; private set; }
        public IEventRepository Events { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}