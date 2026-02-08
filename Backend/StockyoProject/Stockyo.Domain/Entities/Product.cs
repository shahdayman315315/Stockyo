using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int StoreId { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        [Column(TypeName = "decimal(18,2)")] 
        public decimal Price { get; set; }

        [ForeignKey(nameof(StoreId))]
        public Store Store { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; } 
        public int ReorderLevel { get; set; }
        public string Classification { get; set; }
        public virtual ICollection<Batch> Batches { get; set; } 
    }
}
