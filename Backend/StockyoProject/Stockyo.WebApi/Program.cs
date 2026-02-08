
using Microsoft.AspNetCore.Identity;
using Stockyo.Domain.Entities;
using Stockyo.Infrastructure.Data;
using Stockyo.Infrastructure.Extensions;
using System.Threading.Tasks;

namespace Stockyo.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFlutter", policy =>
                {
                    policy.AllowAnyOrigin()   // ???? ???????? ?? ?? ???? (????? ???????)
                          .AllowAnyMethod()   // ???? ??? ????? ??????? (GET, POST, etc.)
                          .AllowAnyHeader();  // ???? ??? ??? Headers
                });
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                 await ( DataSeeder.SeedRolesAsync(RoleManager));
                 await (DataSeeder.SeedAdminAsync(UserManager));

            }
            // Configure the HTTP request pipeline.
           
                app.UseSwagger();
                app.UseSwaggerUI();
            

            app.UseCors("AllowFlutter");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
