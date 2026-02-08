using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public int CategoryId {  get; set; }

        [Required]
        public int StoreId { get; set; }

        [Required]
        public string Barcode { get; set; }

        [Required]
        public int ReorderLevel { get; set; }
        public string Classification { get; set; } = string.Empty;
    }
}
