using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Triton.Model.TritonGroup.Tables;
using Triton.Service.Model.TritonOps.StoredProcs;

namespace Triton.Operations.Models
{
    public class CSAViewModel
    {
        public List<proc_CSA_GetByUserID> proc_CSA_GetByUserID { get; set; }
        public List<LookUpCodes> CSAList { get; set; }
        public int CSALCID { get; set; }
        public string StartDate { get; set; }
        public string endDate { get; set; }
        public string AccountCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Url { get; set; }
        public bool ShowReport { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int CutomerID { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string EmailBody { get; set; }
        public string Subject { get; set; }
        public IFormFile File { get; set; }
        public string Name { get; set; }
        public byte [] Attachement { get; set; }
        public string FilterDate { get; set; }

    }
}
