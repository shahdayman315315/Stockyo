using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Stockyo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Infrastructure.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
    }
}
