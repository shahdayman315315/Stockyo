using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stockyo.Application.Interfaces;
using Stockyo.Domain.DTOs;
using System.Security.Claims;

namespace Stockyo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpPost("AddOne")]
        public async Task<IActionResult> AddSingleProduct(ProductDto dto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _productService.CreateProductAsync(dto, userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpPost("Bulk-Import/{storeId}")]
        public async Task<IActionResult> BulkImport(IFormFile file, int storeId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _productService.ImportProductsFromExcelAsync(file, storeId, userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);

        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _productService.GetProductByIdAsync(id, userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetAll(int storeId, [FromQuery] int pagenumber = 1, [FromQuery] int pagesize = 10, [FromQuery] string? searcchItem = null)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _productService.GetStoreProductsAsync(storeId, userId!, pagenumber, pagesize, searcchItem);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductDto dto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _productService.UpdateProductAsync(id, dto, userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
             
            var result=await _productService.DeleteProductAsync(id,userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpDelete("Bulk-Delete")]
        public async Task<IActionResult> BulkDelete([FromBody]List<int> productIds)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (productIds == null || !productIds.Any())
            {
                return BadRequest("Determine Bulk to delete");
            }

            var result=await _productService.BulkDeleteAsync(productIds, userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


    }
}
