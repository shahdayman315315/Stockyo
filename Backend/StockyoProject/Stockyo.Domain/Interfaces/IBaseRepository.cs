using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {

        IQueryable<T> Query { get; }
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken=default);

        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<T> AddAsync(T item, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<T> values, CancellationToken cancellationToken = default);
        void RemoveRange(IEnumerable<T> entities);
        
        void UpdateAsync(T item);

        void DeleteAsync(T item);
    }
}
