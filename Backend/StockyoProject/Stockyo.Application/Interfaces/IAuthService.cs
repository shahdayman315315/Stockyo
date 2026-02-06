using Stockyo.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto model);

        Task<AuthResultDto> LoginAsync(LoginDto model);

        Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto model);

        Task ForgetPasswordAsync(ForgetPasswordDto model);

        Task<bool> ResetPasswordAsync(ResetPasswordDto model);

        Task LogoutAsync(string refreshToken);
    }
}
