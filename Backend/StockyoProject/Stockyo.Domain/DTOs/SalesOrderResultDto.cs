using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.DTOs
{
    public class SalesOrderResultDto
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string UserName { get; set; } 
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<SalesOrderItemResultDto> Items { get; set; }
    }
}
    