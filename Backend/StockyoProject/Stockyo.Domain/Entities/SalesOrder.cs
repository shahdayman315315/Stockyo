using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.Entities
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public ApplicationUser? User { get; set; } 
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
    }
}
