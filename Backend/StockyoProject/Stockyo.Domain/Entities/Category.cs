using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public int StoreId { get; set; }
    
        public  Store Store { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
    }
