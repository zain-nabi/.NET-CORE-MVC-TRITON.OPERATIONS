using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Triton.Operations.Models
{
    public class DailySalesReportModel
    {
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public bool ShowReport { get; set; }
        public string Url { get; set; }
    }
}
