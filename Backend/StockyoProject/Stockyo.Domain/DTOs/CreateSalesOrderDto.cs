using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.DTOs
{
    public class CreateSalesOrderDto
    {
        [Required]
        public int StoreId { get; set; }

        [Required]
        public List<SalesOrderItemDto> Items { get; set; } = new List<SalesOrderItemDto>();
    }
}
