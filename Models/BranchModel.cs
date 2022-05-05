using System;
using System.Collections.Generic;
using Triton.Model.TritonGroup.Tables;
using Triton.Model.TritonSecurity.Tables;

namespace Triton.Operations.Models
{
    public class CovidBranchModel
    {
        public List<Branches> BranchList { get; set; }
        public int SelectedBranchID { get; set; }
        public DateTime? StartDate{ get; set; }
        public DateTime? EndDate { get; set; }
        public string Message { get; set; }
        public bool ShowReport { get; set; }
        public Branches Branches { get; set; }
        public string Url { get; set; }
        public ReportManager ReportManager { get; set; }
        public string FilterDate { get; set; }
    }
}
