using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.Entities
{
    public class Batche
    {
        public int Id { get; set; }
        public int StoreId { get; set; }

        public int ProductId { get; set; }
       
        public  Product Product { get; set; } 

        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }

        public DateTime ProductionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime ReceivedDate { get; set; }
    }
}
