using System.Collections.Generic;
using System.Linq;
using Triton.Service.Model.Applications.Custom;
using Triton.Service.Model.Applications.Tables;

namespace Triton.Operations.Helpers
{
    public class RoadFreightAgentHistoryHelper
    {

        private const int _paidLookUpCodeID = 707;
        public static RoadFreightAgentHistory RoadFreightAgentHistoryDTO(RoadFreightAgent roadFreightAgent, string comments, int userId)
        {
            return new RoadFreightAgentHistory
            {
                Comments = string.IsNullOrEmpty(comments) ? string.Empty : comments,
                RoadFreightAgentID = roadFreightAgent.RoadFreightAgentID,
                CreatedByUserID = userId,
                CreatedOn = System.DateTime.Now,
                CategoryLCID = roadFreightAgent.CategoryLCID,
                StatusLCID = roadFreightAgent.StatusLCID,
                DeletedByUserID = null,
                DeletedOn = null
            };
        }

        public static List<RoadFreightAgentHistory> RoadFreightAgentHistoryDTO(List<RoadFreightAgentModel> roadFreightAgent, string comments, int userId)
        {
            var roadFreightAgentsHistory = new List<RoadFreightAgentHistory>();

            var roadFreightAgentsList = roadFreightAgent.Where(x => x.IsChecked).ToList();

            foreach (var item in roadFreightAgentsList)
            {
                roadFreightAgentsHistory.Add(new RoadFreightAgentHistory
                {
                    Comments = string.IsNullOrEmpty(comments) ? string.Empty : comments,
                    RoadFreightAgentID = item.RoadFreightAgentID,
                    CreatedByUserID = userId,
                    CreatedOn = System.DateTime.Now,
                    CategoryLCID = item.CategoryLCID,
                    StatusLCID = _paidLookUpCodeID
                });
            }

            return roadFreightAgentsHistory;
        }

    }
}
