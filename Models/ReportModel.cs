using System;

namespace Triton.Operations.Models
{
    public class TarsusWaybillTrackerModel
    {
        public string FilterDate { get; set; }
        public bool ShowReport { get; set; } = false;
        public string Url { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Message { get; set; }
    }
}
