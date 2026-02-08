using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Stockyo.Application.Helper;
using Stockyo.Application.Interfaces;
using Stockyo.Domain.DTOs;
using Stockyo.Domain.Entities;
using Stockyo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stockyo.Application.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SalesOrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<SalesOrderResultDto>> CreateSalesOrderAsync(string userId, CreateSalesOrderDto dto)
        {
           
            var existstore = await _unitOfWork.Stores.Query.FirstOrDefaultAsync(s => s.Id == dto.StoreId && s.UserId == userId);
            if (existstore is null) return Result<SalesOrderResultDto>.Failure("Store not found.");
//نعمل فاتورة
            var salesOrder = new SalesOrder
            {
                StoreId = dto.StoreId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = 0,
                SalesOrderItems = new List<SalesOrderItem>()
            };

            //  نمشي على كل منتج  في الفاتورة
            foreach (var itemDto in dto.Items)
            {
                var product = await _unitOfWork.Products.Query
                    .Include(p => p.Batches).FirstOrDefaultAsync(p => p.Id == itemDto.ProductId && p.StoreId == dto.StoreId);

                if (product is null)
                    return Result<SalesOrderResultDto>.Failure($"Product with ID {itemDto.ProductId} not found in this store.");

                // هل فيه رصيد كافي توتل
                var totalStock = product.Batches.Where(b => b.Quantity > 0).Sum(b => b.Quantity);
                if (totalStock < itemDto.Quantity)
                    return Result<SalesOrderResultDto>.Failure($"Not enough stock for product '{product.Name}'. Available: {totalStock}, Requested: {itemDto.Quantity}");

                // 4. (FIFO Logic) 
                var availableBatches = product.Batches
                    .Where(b => b.Quantity > 0)
                    .OrderBy(b => b.ExpiryDate)
                    .ToList();

                int remainingQtyToDeduct = itemDto.Quantity;

                foreach (var batch in availableBatches)
                {
                    if (remainingQtyToDeduct <= 0) break; // خلاص خصمنا كل المطلوب

                    int qtyToTakeFromBatch;

                    if (batch.Quantity >= remainingQtyToDeduct)
                    {
                       
                        qtyToTakeFromBatch = remainingQtyToDeduct;
                        batch.Quantity -= remainingQtyToDeduct;
                        remainingQtyToDeduct = 0;
                    }
                    else
                    {
                        // الباتش ده مفيهوش كفاية، هناخده كله ونشوف اللي بعده
                        qtyToTakeFromBatch = batch.Quantity;
                        remainingQtyToDeduct -= batch.Quantity;
                        batch.Quantity = 0; 
                    }
                    var orderItem = new SalesOrderItem
                    {
                        ProductId = product.Id,
                        BatchId = batch.Id, 
                        Quantity = qtyToTakeFromBatch,
                        UnitPrice = product.Price,
                        
                    };

                    salesOrder.SalesOrderItems.Add(orderItem);

                    _unitOfWork.Batches.UpdateAsync(batch);
                }
            }

            //  نحسب الإجمالي النهائي
            salesOrder.TotalAmount = salesOrder.SalesOrderItems.Sum(i => i.Quantity * i.UnitPrice);

        
            await _unitOfWork.SalesOrders.AddAsync(salesOrder);
            await _unitOfWork.SaveChangesAsync();

            //  نرجع النتيجة لليوزر
            var createdOrder = await _unitOfWork.SalesOrders.Query
                .Include(o => o.Store)
                .Include(o => o.User)
                .Include(o => o.SalesOrderItems).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == salesOrder.Id);

            var resultDto = _mapper.Map<SalesOrderResultDto>(createdOrder);
            return Result<SalesOrderResultDto>.Success(resultDto);
        }

     
        public async Task<Result<PagedResult<SalesOrderResultDto>>> GetAllSalesOrdersAsync(int storeId, string userId, int pageNumber, int pageSize)
        {
        
            var isOwner = await _unitOfWork.Stores.Query.AnyAsync(s => s.Id == storeId && s.UserId == userId);
            if (!isOwner) return Result<PagedResult<SalesOrderResultDto>>.Failure("Access denied.");

            var query = _unitOfWork.SalesOrders.Query
                .Where(o => o.StoreId == storeId)
                .Include(o => o.Store)
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt) // الأحدث الأول
                .AsNoTracking();

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var dtos = _mapper.Map<List<SalesOrderResultDto>>(items);

            return Result<PagedResult<SalesOrderResultDto>>.Success(new PagedResult<SalesOrderResultDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

      
        public async Task<Result<SalesOrderResultDto>> GetSalesOrderByIdAsync(int id, string userId)
        {
            var order = await _unitOfWork.SalesOrders.Query
                .Include(o => o.Store)
                .Include(o => o.User)
                .Include(o => o.SalesOrderItems).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.Store.UserId == userId);

            if (order is null) return Result<SalesOrderResultDto>.Failure("Order not found.");

            var dto = _mapper.Map<SalesOrderResultDto>(order);
            return Result<SalesOrderResultDto>.Success(dto);
        }

       
        public async Task<Result<bool>> DeleteSalesOrderAsync(int id, string userId)
        {
            //  نجيب التفاصيل 
            var order = await _unitOfWork.SalesOrders.Query
                .Include(o => o.Store)
                .Include(o => o.SalesOrderItems)
                .FirstOrDefaultAsync(o => o.Id == id && o.Store.UserId == userId);

            if (order is null) return Result<bool>.Failure("Order not found.");

            //  (Reverse FIFO)
            foreach (var item in order.SalesOrderItems)
            {
                var batch = await _unitOfWork.Batches.Query.FirstOrDefaultAsync(b => b.Id == item.BatchId);
                if (batch != null)
                {
                    batch.Quantity += item.Quantity; 
                    _unitOfWork.Batches.UpdateAsync(batch);
                }
                
            }

            _unitOfWork.SalesOrders.DeleteAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}