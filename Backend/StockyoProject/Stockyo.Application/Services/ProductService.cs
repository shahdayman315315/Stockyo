using AutoMapper;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stockyo.Application.Helper;
using Stockyo.Application.Interfaces;
using Stockyo.Domain.DTOs;
using Stockyo.Domain.Entities;
using Stockyo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<ProductDto>> CreateProductAsync(ProductDto dto, string userId)
        {
           var result=await ValidateAndMapProductAsync(dto, userId);

            if (!result.IsSuccess)
            {
                return Result<ProductDto>.Failure(result.Message!);
            }

            await _unitOfWork.Products.AddAsync(result.Data!);
            await _unitOfWork.SaveChangesAsync();

            var productDto=_mapper.Map<ProductDto>(result.Data);
            return Result<ProductDto>.Success(productDto);
        }


        public async Task<Result<BulkProductResultDto>> ImportProductsFromExcelAsync(IFormFile file, int storeId,string userId)
        {
            var bulkResult = new BulkProductResultDto();

            if(file is null ||file.Length==0)
            {
                return Result<BulkProductResultDto>.Failure("file is empty");
            }


            int rowNumber = 0;

            var products = new List<Product>();

            // للتعامل مع اللغه العربيه والرموز
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream=file.OpenReadStream())
            {
                using(var reader=ExcelReaderFactory.CreateReader(stream))
                {
                    
                    var processedBarcodes = new HashSet<string>();

                    while (reader.Read())
                    {
                        rowNumber++;

                        if (rowNumber == 1) continue;

                        var name = reader[0]?.ToString();
                        var barcode = reader[1]?.ToString();
                        var categoryId = (int)reader[2];
                        var reorderLevel = (int)reader[3];

                        if (processedBarcodes.Contains(barcode))
                        {
                            bulkResult.Errors.Add(new BulkErrorDetails
                            {
                                ErrorMessage = "This barcode is duplicated in tis file"
                            });
                            continue;
                        }

                        var productDto = new ProductDto
                        {
                            Name = name,
                            Barcode = barcode,
                            StoreId = storeId,
                            CategoryId = categoryId,
                            ReorderLevel = reorderLevel
                        };

                        var result = await ValidateAndMapProductAsync(productDto, userId);

                        if (!result.IsSuccess)
                        {
                            bulkResult.FailureCount++;
                            bulkResult.Errors.Add(new BulkErrorDetails
                            {
                                RowNumber = rowNumber,
                                ProductName = name,
                                ErrorMessage = result.Message
                            });
                        }

                        else
                        {
                            bulkResult.SuccessCount++;
                            products.Add(result.Data);
                            processedBarcodes.Add(barcode);

                        }
                    }

                    
                }
            }

            if (products.Any())
            {
                await _unitOfWork.Products.AddRangeAsync(products);
                await _unitOfWork.SaveChangesAsync();
            }


            bulkResult.TotalRows = rowNumber-1;


            return Result<BulkProductResultDto>.Success(bulkResult);
        }

        private async Task<Result<Product>> ValidateAndMapProductAsync(ProductDto dto, string userId)
        {
            var store = await _unitOfWork.Stores.Query.FirstOrDefaultAsync(s => s.Id == dto.StoreId && s.UserId == userId);

            if (store is null)
            {
                return Result<Product>.Failure("Store not found or you don't have permission.");
            }

            var category = await _unitOfWork.Categories.Query.FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.StoreId == dto.StoreId);

            if (category is null)
            {
                return Result<Product>.Failure("Category is not found");
            }

            var existBarcode = await _unitOfWork.Products.Query.FirstOrDefaultAsync(p => p.Barcode == dto.Barcode && p.StoreId == dto.StoreId);

            if (existBarcode is not null)
            {
                return Result<Product>.Failure("This Product Barcode already exists");
            }

            var product = _mapper.Map<Product>(dto);


            return Result<Product>.Success(product);
        }


        public async Task<Result<ProductDto>> GetProductByIdAsync(int productId,string userId)
        {
            var existProduct = await _unitOfWork.Products.Query.Include(p => p.Store).FirstOrDefaultAsync(p => p.Id == productId && p.Store.UserId == userId);

            if (existProduct is null)
            {
                return Result<ProductDto>.Failure("Product doesn't exist or U don't have permission to see this product");
            }

            var productDto=_mapper.Map<ProductDto>(existProduct);

            return Result<ProductDto>.Success(productDto);
        }
        public async Task<Result<PagedResult<ProductDto>>> GetStoreProductsAsync(int storeId, string userId, int pageNumber, int pageSize, string? searchTerm = null)
        {
            var store = await _unitOfWork.Stores.Query.FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

            if (store is null)
            {
                return Result<PagedResult<ProductDto>>.Failure("Store not found or you don't have permission.");
            }

            var products =  _unitOfWork.Products.Query.Where(p => p.StoreId == storeId).AsNoTracking();

            var totalcount=await products.CountAsync();

            if (searchTerm is not null)
            {
                searchTerm=searchTerm.Trim().ToLower();
                
                products=products.Where(p=>p.Name.Trim().ToLower().Contains(searchTerm) || p.Barcode.Contains(searchTerm));
            }

            products =  products.Skip((pageNumber - 1) * 10).Take(pageSize);

            var productsDtos= _mapper.Map<List<ProductDto>>(await products.ToListAsync());
            var pagedResult = new PagedResult<ProductDto>
            {
                Items = productsDtos,
                TotalCount = totalcount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };


            return Result<PagedResult<ProductDto>>.Success(pagedResult);
        }

        public async Task<Result<bool>> UpdateProductAsync(int productId, ProductDto dto, string userId)
        {
            var existProduct=await _unitOfWork.Products.Query.Include(p=>p.Store).FirstOrDefaultAsync(p=>p.Id==productId && p.Store.UserId==userId);
           
            if(existProduct is null)
            {
                return Result<bool>.Failure("Product doesn't exist or U don't have permission to update this product");
            }

            var barcodeExists = await _unitOfWork.Products.Query
              .AnyAsync(p => p.Barcode == dto.Barcode
                  && p.StoreId == existProduct.StoreId
                  && p.Id != productId); // استبعاد المنتج الحالي من البحث

            if (barcodeExists)
                return Result<bool>.Failure($"Barcode {dto.Barcode} is already used by another product.");

            _mapper.Map(dto, existProduct);
            _unitOfWork.Products.UpdateAsync(existProduct);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteProductAsync(int productId, string userId)
        {
            var existProduct = await _unitOfWork.Products.Query.Include(p => p.Store).FirstOrDefaultAsync(p => p.Id == productId && p.Store.UserId == userId);

            if (existProduct is null)
            {
                return Result<bool>.Failure("Product doesn't exist or U don't have permission to delete this product");
            }

             _unitOfWork.Products.DeleteAsync(existProduct);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> BulkDeleteAsync(List<int> productIds, string userId)
        {
            var products =  await _unitOfWork.Products.Query.Include(p => p.Store).Where(p => productIds.Contains(p.Id) && p.Store.UserId == userId).ToListAsync();
        
            if(!products.Any())
            {
                return Result<bool>.Failure("Products don't exist or U don't have permission to delete these products");
            }

             _unitOfWork.Products.RemoveRange(products);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}
