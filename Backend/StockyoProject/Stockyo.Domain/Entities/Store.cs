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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Type {  get; set; }

        public string UserId { get; set; }

        public ApplicationUser User {  get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
