using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.DTOs
{
    public class BatcheDto
    {

        public int Id { get; set; }

        [Required]
        public string Barcode {  get; set; }

        [Required]
        public int StoreId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal CostPrice { get; set; }

        [Required]

        public DateTime ProductionDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }


        public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;

    }
}
