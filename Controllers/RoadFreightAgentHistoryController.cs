using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Triton.Operations.Models;
using Triton.Service.Data;

namespace Triton.Operations.Controllers
{
    [Authorize(Roles = "Administrator, Branch Manager")]
    public class RoadFreightAgentHistoryController : Controller
    {
        public async Task<IActionResult> GetById(int roadFreightAgentId, string filterDate)
        {
            var model = new AuditViewModel
            {
                RoadFreightAgentHistoryModel = await RoadFreightAgentHistoryService.GetByIdAsync(roadFreightAgentId),
                RoadFreightAgentID = roadFreightAgentId,
                SelectedDate = filterDate
            };

            return View(model);
        }
    }
}
