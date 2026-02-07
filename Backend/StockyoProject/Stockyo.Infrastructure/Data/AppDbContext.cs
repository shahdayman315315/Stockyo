using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stockyo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Infrastructure.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Batche> Batches { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }
        public DbSet<LostSales> LostSales { get; set; }
        public DbSet<AISuggestions> AISuggestions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
               .HasMany(u => u.RefreshTokens)
               .WithOne(r=>r.User)
               .HasForeignKey(rt => rt.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Store>()
                .HasMany(s => s.Products)
                .WithOne()
                .HasForeignKey(p => p.StoreId);

            builder.Entity<Store>()
                .HasMany(s => s.Categories)
                .WithOne(c => c.Store)
                .HasForeignKey(c => c.StoreId);

            builder.Entity<ApplicationUser>()
             .HasOne(u => u.Store)
             .WithOne(s => s.User)
             .HasForeignKey<Store>(s => s.UserId); 

            builder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

           
            builder.Entity<Product>()
                .HasMany<Batche>()
                .WithOne(b => b.Product)
                .HasForeignKey(b => b.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

         
            builder.Entity<Batche>()
                .Property(b => b.CostPrice)
                .HasPrecision(18, 2);

           
            builder.Entity<SalesOrder>()
                .HasMany(o => o.SalesOrderItems)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SalesOrder>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<SalesOrder>()
               .Property(s => s.TotalAmount)
               .HasColumnType("decimal(18,2)");  

            builder.Entity<SalesOrderItem>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SalesOrderItem>()
                .HasOne(i => i.Batch)
                .WithMany()
                .HasForeignKey(i => i.BatchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SalesOrderItem>()
                 .Property(u => u.UnitPrice)
                 .HasColumnType("decimal(18,2)");
            builder.Entity<LostSales>()
                .HasOne(ls => ls.Product)
                .WithMany()
                .HasForeignKey(ls => ls.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

           
            builder.Entity<AISuggestions>()
                .HasOne(ai => ai.Product)
                .WithMany()
                .HasForeignKey(ai => ai.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AISuggestions>()
        .Property(p => p.SuggestedValue)
        .HasColumnType("decimal(18,2)");


            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

           
            builder.Entity<Product>()
                .HasIndex(p => new { p.StoreId, p.Barcode })
                .IsUnique();

            builder.Entity<Batche>()
                .HasIndex(b => new { b.ProductId, b.ExpiryDate });
        }

    }
}
