using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stockyo.Application.Helper;
using Stockyo.Application.Interfaces;
using Stockyo.Domain.DTOs;
using Stockyo.Domain.Entities;
using Stockyo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwt;
        public AuthService(UserManager<ApplicationUser> userManager, IMapper mapper, 
            IEmailService emailService, IUnitOfWork unitOfWork, JwtSettings jwt)
        {
            _emailService = emailService;
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _jwt = jwt;
        }

        public async Task ForgetPasswordAsync(ForgetPasswordDto dto)
        {
            var user=await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
            {
                return;
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"https://localhost:7002/api/auth/resetpassword?email={dto.Email}&token={Uri.EscapeDataString(resetToken)}";

            await _emailService.SendEmailAsync(dto.Email, "Reset Your Password",
            $"Click here to reset your password: <a href='{resetLink}'>Reset Password</a>"
            );
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
            {
                return false;
            }

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            return result.Succeeded;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var authResult = new AuthResultDto();
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                authResult.Message = "Incorrect Email or Password.";
            }

            var jwtToken = await GenerateJWTToken(user!);

            var refreshToken = new RefreshToken()
            {
                Token = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user!.Id
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            authResult.IsAuthenticated = true;
            authResult.ExpirationDate = jwtToken.ValidTo;
            authResult.RefreshToken = refreshToken.Token;
            authResult.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var roles = await _userManager.GetRolesAsync(user!);
            authResult.Role = roles.FirstOrDefault()!;
            authResult.UserName = user!.UserName!;
            authResult.Message = "Logged in Successfully";

            return authResult;

        }

        public async Task LogoutAsync(string refreshToken)
        {
            await RevokeRefreshTokenAsync(refreshToken);
        }

        public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var authResultDto=new AuthResultDto();

            var principal = GetPrincipalFromExpiredToken(dto.AccessToken);

            var userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                authResultDto.Message = "Invalid Token";
                return authResultDto;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                authResultDto.Message = "User Not Found";
                return authResultDto;
            }

            var storedRefreskToken = _unitOfWork.RefreshTokens.Query.FirstOrDefault(t => t.Token == dto.Refreshtoken&&t.UserId==userId);
        
            if(  storedRefreskToken is null || storedRefreskToken.IsExpired ||!storedRefreskToken.IsActive )
            {
                authResultDto.Message = "Invalid RefreshToken";
                return authResultDto;
            }

            storedRefreskToken.RevokedAt = DateTime.UtcNow;

            var jwtToken = await GenerateJWTToken(user);

            var RefreshToken = new RefreshToken()
            {
                Token = GenerateRefreshToken(),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.RefreshTokens.AddAsync(RefreshToken);
            await _unitOfWork.SaveChangesAsync();

            authResultDto.IsAuthenticated = true;
            authResultDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authResultDto.RefreshToken = RefreshToken.Token;
            authResultDto.ExpirationDate = jwtToken.ValidTo;
            authResultDto.UserName = user.UserName!;

            return authResultDto;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ValidIssuer = _jwt.Issuer,
                ValidAudience = _jwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwt.Key))
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }

            return principal;
        }
        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            var user=await _userManager.FindByEmailAsync(dto.Email);

            if(user is not null)
            {
                return new AuthResultDto
                {
                    Message = "Email already exists"
                };
            }

            var newUser=_mapper.Map<ApplicationUser>(dto);

            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (!result.Succeeded)
            {
                return new AuthResultDto()
                {
                    Message = String.Join(',', result.Errors.Select(e => e.Description))
                };
            }

            await _userManager.AddToRoleAsync(newUser, "BusinessOwner");

            var jwtToken = await GenerateJWTToken(user!);

            var refreshToken = new RefreshToken()
            {
                Token = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user!.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResultDto()
            {
                UserName = user.UserName!,
                IsAuthenticated = true,
                Role = "BusinessOwner",
                Message = "Registered Successfully",
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                RefreshToken = refreshToken.Token,
                ExpirationDate = jwtToken.ValidTo
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64]; // 512 bits

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private async Task<JwtSecurityToken> GenerateJWTToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in userRoles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email!),
                new Claim(ClaimTypes.NameIdentifier,user.Id)

            }.Union(userClaims)
             .Union(roleClaims);

            var symmeticSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmeticSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken
                (
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: jwtClaims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }

        

        private async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null) return;

            var rt = user.RefreshTokens.First(t => t.Token == refreshToken);
            if (!rt.IsActive) return;

            rt.RevokedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
    }
}
