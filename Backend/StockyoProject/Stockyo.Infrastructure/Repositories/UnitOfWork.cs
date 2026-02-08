using Stockyo.Domain.Entities;
using Stockyo.Domain.Interfaces;
using Stockyo.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly AppDbContext _context;

        private Lazy<IBaseRepository<AISuggestions>> _aiSuggestions;
        private Lazy<IBaseRepository<Store>> _stores;
        private Lazy<IBaseRepository<Product>> _products;
        private Lazy<IBaseRepository<Category>> _categories;
        private Lazy<IBaseRepository<SalesOrder>> _salesOrders;
        private Lazy<IBaseRepository<SalesOrderItem>> _salesOrderItems;
        private Lazy<IBaseRepository<LostSales>> _lostSales;
        private Lazy<IBaseRepository<Notification>> _notifications;
        private Lazy<IBaseRepository<Batch>> _batches;
        private Lazy<IBaseRepository<RefreshToken>> _refreshTokens;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _aiSuggestions=CreateRepository<IBaseRepository<AISuggestions>,BaseRepository<AISuggestions>>();
            _batches = CreateRepository<IBaseRepository<Batch>, BaseRepository<Batch>>();
            _notifications = CreateRepository<IBaseRepository<Notification>, BaseRepository<Notification>>();
            _lostSales = CreateRepository<IBaseRepository<LostSales>, BaseRepository<LostSales>>();
            _salesOrderItems = CreateRepository<IBaseRepository<SalesOrderItem>, BaseRepository<SalesOrderItem>>();
            _categories = CreateRepository<IBaseRepository<Category>, BaseRepository<Category>>();
            _stores = CreateRepository<IBaseRepository<Store>, BaseRepository<Store>>();
            _salesOrders = CreateRepository<IBaseRepository<SalesOrder>, BaseRepository<SalesOrder>>();
            _products = CreateRepository<IBaseRepository<Product>, BaseRepository<Product>>();
            _refreshTokens= CreateRepository<IBaseRepository<RefreshToken>, BaseRepository<RefreshToken>>();


        }

        private Lazy<T1> CreateRepository<T1,T2>() where T1 : class where T2 : class
        {
            return new Lazy<T1>(() => (T1)Activator.CreateInstance(typeof(T2), _context)!);
        }

        public IBaseRepository<AISuggestions> AISuggestions => _aiSuggestions.Value;

        public IBaseRepository<Store> Stores => _stores.Value;

        public IBaseRepository<Product> Products => _products.Value;

        public IBaseRepository<Category> Categories => _categories.Value;

        public IBaseRepository<SalesOrder> SalesOrders => _salesOrders.Value;

        public IBaseRepository<SalesOrderItem> SalesOrderItems => _salesOrderItems.Value;

        public IBaseRepository<LostSales> LostSales => _lostSales.Value;

        public IBaseRepository<Notification> Notifications => _notifications.Value;

        public IBaseRepository<Batch> Batches => _batches.Value;

        public IBaseRepository<RefreshToken> RefreshTokens => _refreshTokens.Value;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken=default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
