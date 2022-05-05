using System.Collections.Generic;
using Triton.Model.TritonExpress.Tables;
using Triton.Service.Model.TritonExpress.StoredProcs;

namespace Triton.Operations.Models
{
    public class PostalCodesViewModel
    {
        public List<PostalCodes> PostalCodes { get; set; }
        public List<proc_PostalCodes_Overview> PostalCodesOverview { get; set; }
        public string PostalCodeSearch { get; set; }
        public string ErrorMessage { get; set; }
    }
}
