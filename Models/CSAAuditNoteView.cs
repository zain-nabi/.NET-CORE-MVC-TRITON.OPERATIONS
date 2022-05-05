using System.Collections.Generic;
using Triton.Model.CRM.Tables;
using Triton.Model.TritonSecurity.Tables;
using Triton.Service.Model.CRM.Tables;

namespace Triton.Operations.Models
{
    public class CSAAuditNoteView
    {
        public List<CSAAuditNote> CSAAuditNotes { get; set; }
        public string FilterDate { get; set; }
        public Customers Customers { get; set; }
        public Users Users { get; set; }
        public string ErrorMessage { get; set; }
        public bool ShowReport { get; set; } = false;
    }
}
