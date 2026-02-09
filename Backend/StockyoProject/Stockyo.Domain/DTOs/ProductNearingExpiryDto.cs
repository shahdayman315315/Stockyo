using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.DTOs
{
    public class ProductNearingExpiryDto
    {
        public string ProductName { get; set; }

        public int Quantity { get; set; }   

        public DateOnly  ExpiryDate { get; set; }
}
}
