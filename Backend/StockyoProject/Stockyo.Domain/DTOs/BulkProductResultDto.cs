using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Domain.DTOs
{
    public class BulkProductResultDto
    {
        public int TotalRows { get; set; } 
        public int SuccessCount { get; set; } 
        public int FailureCount { get; set; } 

        public List<BulkErrorDetails> Errors { get; set; } = new List<BulkErrorDetails>();

        public bool IsFullSuccess => FailureCount == 0;
    }
}
