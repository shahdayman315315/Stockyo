using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stockyo.Application.Interfaces;
using Stockyo.Domain.DTOs;
using System.Runtime.InteropServices;

namespace Stockyo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result=await _authService.RegisterAsync(dto);

            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {

            var result = await _authService.LoginAsync(dto);

            if (!result.IsAuthenticated)
            {
                return Unauthorized(result.Message);
            }

            return Ok(result);
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
        {

            var result = await _authService.RefreshTokenAsync(dto);

            if (!result.IsAuthenticated)
            {
                return Unauthorized(result.Message);
            }

            return Ok(result);
        }


        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDto dto)
        {

            await _authService.ForgetPasswordAsync(dto);

            return Ok("If the email exists, a reset link has been sent.");
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {

            var result = await _authService.ResetPasswordAsync(dto);

            if (!result)
            {
                return Unauthorized("Invalid token or email");
            }

            return Ok("Password has been reset successfully");
        }
    }
}
