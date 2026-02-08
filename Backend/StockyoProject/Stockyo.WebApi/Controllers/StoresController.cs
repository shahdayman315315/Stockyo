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
    public class StoresController : ControllerBase
    {
        private IStoreService _storeService;

        public StoresController(IStoreService storeService)
        {
            _storeService = storeService;
        }
        private string GetCurrentUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // api/Stores/create
        [HttpPost("create")] 
        public async Task<IActionResult> Create(StoreDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState); 

           
            var result = await _storeService.CreateStoreAsync(GetCurrentUserId(), dto);

            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result.Data);
        }


        //  api/Stores/my-store
        [HttpGet("my-store")]
        public async Task<IActionResult> GetMyStore()
        {
            var result = await _storeService.GetMyStoreAsync(GetCurrentUserId());
            if (!result.IsSuccess) return NotFound(result.Message); 
            return Ok(result.Data);
        }

        // PUT: api/Stores/update
        [HttpPut("update")]
        public async Task<IActionResult> Update(StoreDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _storeService.UpdateStoreAsync(dto, GetCurrentUserId());
            if (!result.IsSuccess) return BadRequest(result.Message);

            return Ok(result.Data);
        }

        // DELETE: api/Stores/delete/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _storeService.DeleteStoreAsync(id, GetCurrentUserId());
            if (!result.IsSuccess) return BadRequest(result.Message);

            return Ok(result.Data);
        }


    }
}
