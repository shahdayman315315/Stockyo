using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.Entities
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public string SubscriptionStatus { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
