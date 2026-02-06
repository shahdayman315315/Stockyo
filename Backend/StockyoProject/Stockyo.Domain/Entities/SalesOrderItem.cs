using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.Entities
{
    public class SalesOrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual SalesOrder Order { get; set; } 
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public int BatchId { get; set; }
        [ForeignKey("BatchId")]
        public virtual Batche Batch { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
