using System.Collections.Generic;
using Triton.Model.TritonGroup.Tables;
using Triton.Service.Model.Applications.Custom;
using Triton.Service.Model.Applications.StoredProcs;

namespace Triton.Operations.Models
{
    public class RoadFreightAgentViewModel
    {
        public proc_RoadFreightAgent_GetByID proc_RoadFreightAgent_GetByID { get; set; }
        public RoadFreightAgentModel RoadFreightAgentModel { get; set; }
        public LookUpCodes LookUpCodes { get; set; }
        public string SelectedDate { get; set; }
        public string FilterDate { get; set; }
        public int LookUpCodeId { get; set; }
        public int SelectedReasonLCID { get; set; }
        public List<LookUpCodes> StatusList { get; set; }
        public int roadFreightAgentID { get; set; }
        public string ErrorMessage { get; set; }
        public string FilterUserID { get; set; }
        public int StatusLCID { get; set; }
        public int UserID { get; set; }
        public string Comments { get; set; }
    }
}
