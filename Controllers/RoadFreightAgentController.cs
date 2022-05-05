using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Triton.Operations.Helpers;
using Triton.Operations.Models;
using Triton.Service.Data;
using Triton.Service.Model.Applications.Custom;
using Triton.Service.Utils;

namespace Triton.Operations.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator, Branch Manager")]
    public class RoadFreightAgentController : Controller
    {
        private const int _notInvoiced = 650;
        private const int _statusLookUpCategoryID = 104;
        public IActionResult Search()
        {
            return View(new RoadFreightAgentViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Search(RoadFreightAgentViewModel roadFreightAgentViewModel)
        {
            var splits = RoadFreightAgentHelper.SplitFilterDate(roadFreightAgentViewModel.FilterDate);
            var service = await RoadFreightAgentService.AdminsAndBranchManagerOverviewAsync(_notInvoiced, Convert.ToDateTime(splits[0]), Convert.ToDateTime(splits[1]), roadFreightAgentViewModel.UserID);
            if (service.RoadFreightAgent.Count == 0)
            {
                var roadFreightAgent = new RoadFreightAgentViewModel
                {
                    FilterDate = roadFreightAgentViewModel.FilterDate,
                    ErrorMessage = $"No Waybill exists between {splits[0]}  and {splits[1]}",

                };
                return View("Search", roadFreightAgent);
            }
            var model = new PurchaseOrder_Overview
            {

                ActiveTab = _notInvoiced,
                FilterDate = roadFreightAgentViewModel.FilterDate,
                RoadFreightAgent = service.RoadFreightAgent,
                AgentIssueTabs = service.AgentIssueTabs,
                SupplierTabs = service.SupplierTabs,
                SelectedUser = roadFreightAgentViewModel.UserID

            };

            return View("AdminsAndBranchManagerOverview", model);
        }


        public async Task<IActionResult> GetAgentIssuesById(int lookUpCodeId, string filterDate, int filterUserId)
        {
            var splitDate = RoadFreightAgentHelper.SplitFilterDate(filterDate);
            var purchaseOrderService = await RoadFreightAgentService.AdminsAndBranchManagerOverviewAsync(lookUpCodeId, Convert.ToDateTime(splitDate[0]), Convert.ToDateTime(splitDate[1]), filterUserId);
            var model = RoadFreightAgentHelper.PurchaseOrderOverviewDTO(purchaseOrderService, filterDate, lookUpCodeId, filterUserId);
            return View("AdminsAndBranchManagerOverview", model);
        }

        public async Task<IActionResult> RoadFreightAgentSummary(int roadFreightAgentId, string filterDate, string filteredUserId)
        {
            var purchaseOrderService = await RoadFreightAgentService.proc_RoadFreightAgent_GetByIDAsync(roadFreightAgentId);

            var model = new RoadFreightAgentViewModel
            {
                proc_RoadFreightAgent_GetByID = purchaseOrderService,
                SelectedReasonLCID = purchaseOrderService.RoadFreightAgent.CategoryLCID,
                LookUpCodes = await LookUpCodesService.GetLookUpCodeByID(purchaseOrderService.RoadFreightAgent.CategoryLCID),
                FilterDate = filterDate,
                roadFreightAgentID = roadFreightAgentId,
                StatusList = await LookUpCodesService.LookupCodesByCategoryID(_statusLookUpCategoryID),
                FilterUserID = filteredUserId,
                StatusLCID = purchaseOrderService.RoadFreightAgent.StatusLCID

            };

            return View(model);

        }

        public async Task<IActionResult> SupplierOverview(int lookupcodeId, string filterDate, int userId, int supplierId)
        {
            var splitDate = RoadFreightAgentHelper.SplitFilterDate(filterDate);
            var purchaseOrderService = await RoadFreightAgentService.SupplierBranchManagerOverviewAsync(lookupcodeId, Convert.ToDateTime(splitDate[0]), Convert.ToDateTime(splitDate[1]), userId, supplierId);
            var model = RoadFreightAgentHelper.PurchaseOrderOverviewDTO(purchaseOrderService, filterDate, lookupcodeId, userId);
            model.Active_Supplier_Tab = supplierId == 0 ? 0 : supplierId;

            return View("AdminsAndBranchManagerOverview", model);


        }

        [HttpPost]
        public async Task<IActionResult> MoveToPaid(PurchaseOrder_Overview model)
        {

            var roadFreightAgents = RoadFreightAgentHelper.RoadFreightAgentDTO(model.RoadFreightAgent);
            var result = await RoadFreightAgentService.MoveToPaidAsync(roadFreightAgents);
            var audits = RoadFreightAgentHistoryHelper.RoadFreightAgentHistoryDTO(model.RoadFreightAgent, "Move Paid", User.GetUserId());
            await RoadFreightAgentHistoryService.InsertRoadFreightAgentHistoryAsync(audits);

            var redirectUrl = $"/RoadFreightAgent/GetAgentIssuesById?lookupcodeId={model.ActiveTab}&filterDate={model.FilterDate}&filterUserId={model.SelectedUser}";

            return result ? RedirectToAction("Message", "Home", new { type = StringHelper.Types.UpdateSuccess, url = redirectUrl })
                          : RedirectToAction("Message", "Home", new { type = StringHelper.Types.UpdateFailed, url = redirectUrl });

        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoadFreightAgent(RoadFreightAgentViewModel _roadFreightAgentViewModel)
        {
            var result = false;
            var purchaseOrder = await RoadFreightAgentService.GetByIdAsync(_roadFreightAgentViewModel.proc_RoadFreightAgent_GetByID.RoadFreightAgent.RoadFreightAgentID);

            result = await RoadFreightAgentService.UpdateAsync(_roadFreightAgentViewModel.proc_RoadFreightAgent_GetByID.RoadFreightAgent);

            if (purchaseOrder.StatusLCID != _roadFreightAgentViewModel.StatusLCID)
            {
                _roadFreightAgentViewModel.proc_RoadFreightAgent_GetByID.RoadFreightAgent.StatusLCID = _roadFreightAgentViewModel.StatusLCID;
                var history = await RoadFreightAgentHistoryService.GetByIdAsync(_roadFreightAgentViewModel.proc_RoadFreightAgent_GetByID.RoadFreightAgent.RoadFreightAgentID);
                var status = await LookUpCodesService.LookupCodesByCategoryID(_statusLookUpCategoryID);
                var nextStatus = status.Where(x => x.LookUpCodeID == _roadFreightAgentViewModel.StatusLCID).FirstOrDefault();
                var comments = $"Move to {nextStatus.Name}";
                var isDuplicateComment = history.Where(x => x.RoadFreightAgentHistory.Comments == comments);
                if (isDuplicateComment.Count() == 0)
                {
                    var model = RoadFreightAgentHistoryHelper.RoadFreightAgentHistoryDTO(_roadFreightAgentViewModel.proc_RoadFreightAgent_GetByID.RoadFreightAgent, comments, User.GetUserId());
                    await RoadFreightAgentHistoryService.InsertAsync(model);
                    await RoadFreightAgentService.UpdateAsync(_roadFreightAgentViewModel.proc_RoadFreightAgent_GetByID.RoadFreightAgent);
                }
            }


            if (!string.IsNullOrEmpty(_roadFreightAgentViewModel.Comments))
            {
                var history = await RoadFreightAgentHistoryService.GetByIdAsync(_roadFreightAgentViewModel.proc_RoadFreightAgent_GetByID.RoadFreightAgent.RoadFreightAgentID);
                var isDuplicateComment = history.Where(x => x.RoadFreightAgentHistory.Comments == _roadFreightAgentViewModel.Comments);
                if (isDuplicateComment.Count() == 0)
                {
                    var model = RoadFreightAgentHistoryHelper.RoadFreightAgentHistoryDTO(_roadFreightAgentViewModel.proc_RoadFreightAgent_GetByID.RoadFreightAgent, _roadFreightAgentViewModel.Comments, User.GetUserId());
                    result = await RoadFreightAgentHistoryService.InsertAsync(model);

                }
            }

            var redirectUrl = $"/RoadFreightAgent/RoadFreightAgentSummary?roadFreightAgentId={_roadFreightAgentViewModel.proc_RoadFreightAgent_GetByID.RoadFreightAgent.RoadFreightAgentID}&filterDate={_roadFreightAgentViewModel.FilterDate}&filteredUserId={User.GetUserId()}";

            return result ? RedirectToAction("Message", "Home", new { type = StringHelper.Types.UpdateSuccess, url = redirectUrl })
                          : RedirectToAction("Message", "Home", new { type = StringHelper.Types.UpdateFailed, url = redirectUrl });

        }

    }
}
