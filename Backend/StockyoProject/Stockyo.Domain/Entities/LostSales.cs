using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.Entities
{
    public class LostSales
    {
        public int Id { get; set; }

        public int StoreId { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public int RequestedQuantity { get; set; } 
        public DateTime CreatedAt { get; set; } 
    }
}
