using Stockyo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.Interfaces
{
    public interface IUnitOfWork
    {

        IBaseRepository<AISuggestions> AISuggestions {  get; }
        IBaseRepository<Store> Stores {  get; }
        IBaseRepository<Product> Products { get; }
        IBaseRepository<Category> Categories { get; }
        IBaseRepository<SalesOrder> SalesOrders { get; }
        IBaseRepository<SalesOrderItem> SalesOrderItems { get; }
        IBaseRepository<LostSales> LostSales {  get; }
        IBaseRepository<Notification> Notifications {  get; }
        IBaseRepository<Batche> Batches {  get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken=default);
         
    }
}
