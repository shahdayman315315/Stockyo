using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.Entities
{
    public class AISuggestions
    {
        public int Id { get; set; }
        public int StoreId { get; set; }

        public int ProductId { get; set; }
        public  Product Product { get; set; }

        public string Type { get; set; }
        public decimal SuggestedValue { get; set; }
        public string Status { get; set; }
    }
}
