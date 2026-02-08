using Microsoft.EntityFrameworkCore;
using Stockyo.Domain.Interfaces;
using Stockyo.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {

        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public IQueryable<T> Query => _dbSet;

        public async Task<T> AddAsync(T item, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(item,cancellationToken);
            return item;
        }
       
        public async Task AddRangeAsync(IEnumerable<T> values, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(values,cancellationToken);

        }

        public void DeleteAsync(T item)
        {
            _dbSet.Remove(item); 
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(id, cancellationToken);
        }

        public void UpdateAsync(T item)
        {
            _dbSet.Update(item);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
