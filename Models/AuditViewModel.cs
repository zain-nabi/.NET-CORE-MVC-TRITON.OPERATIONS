using System.Collections.Generic;
using Triton.Service.Model.Applications.Custom;

namespace Triton.Operations.Models
{
    public class AuditViewModel
    {
        public List<RoadFreightAgentHistoryModel> RoadFreightAgentHistoryModel { get; set; }
        public int RoadFreightAgentID { get; set; }
        public string  SelectedDate { get; set; }
    }
}
