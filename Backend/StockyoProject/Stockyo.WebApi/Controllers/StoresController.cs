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

        [HttpPost]
        public async Task<IActionResult> Create(StoreDto dto)
        {
            var userId=User.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.NameIdentifier)?.Value;

            var result=await _storeService.CreateStoreAsync(userId!, dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }
    }
}
