using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; } 

        public List<RefreshToken> RefreshTokens { get; set; } = new();

        public Store? Store { get; set; }


    }
}
