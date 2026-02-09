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
    public class BatchesController : ControllerBase
    {
        private readonly IBatchService _batchService;

        public BatchesController(IBatchService batchService)
        {
            _batchService = batchService;
        }


        [HttpPost]
        public async Task<IActionResult> AddBatch(BatchDto batchDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _batchService.AddBatcheAsync(batchDto, userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBatchById(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _batchService.GetBatcheById(id, userId!);

            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Data);
        }


        [HttpGet("total-stock/{storeId}/{productId}")]
        public async Task<IActionResult> GetTotalStock(int storeId, int productId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _batchService.CalculateTotalStockAsync(storeId, productId, userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpGet("batches-by-product/{storeId}/{productId}")]
        public async Task<IActionResult> GetBatchesByProduct(int storeId, int productId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _batchService.GetBatchesByProductAsync(storeId, productId, userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpGet("expiring-batches/{storeId}")]

        public async Task<IActionResult> GetExpiringBatches(int storeId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _batchService.GetProductsNearingExpiryDateAsync(storeId, userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatch(int id, BatchDto batchDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _batchService.UpdateBatchAsync(id, batchDto, userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatch(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _batchService.DeleteBatchAsync(id, userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }
    }
}
