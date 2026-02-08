using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stockyo.Application.Interfaces;
using Stockyo.Domain.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Stockyo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var result = await _categoryService.CreateCategoryAsync(dto, userId);

            if (!result.IsSuccess) return BadRequest(result.Message);

            return Ok(result.Data); 
        }

     
        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetAll(int storeId, [FromQuery] int pagenumber = 1, [FromQuery] int pagesize = 10, [FromQuery] string? search = null)
        {
            var userId = GetCurrentUserId();
            var result = await _categoryService.GetAllCategoriesAsync(storeId, userId, pagenumber, pagesize, search);

            if (!result.IsSuccess) return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _categoryService.GetCategoryByIdAsync(id, userId);

            if (!result.IsSuccess) return NotFound(result.Message);

            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var result = await _categoryService.UpdateCategoryAsync(id, dto, userId);

            if (!result.IsSuccess) return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _categoryService.DeleteCategoryAsync(id, userId);

            if (!result.IsSuccess) return BadRequest(result.Message);

            return Ok(result.Data); 
        }
    }
}