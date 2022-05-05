using System;

namespace Triton.Operations.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public int StatusCode { get; set; }

        public bool? DisplayHeader { get; set; }
    }

    public class MessageViewModel
    {
        public string Header { get; set; }
        public string Message { get; set; }

        public string Status { get; set; }
        public string Controller { get; set; }
        public string Route { get; set; }
        public string RouteValues { get; set; }
    }
}
