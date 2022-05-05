using System.Collections.Generic;
using System.Linq;
using Triton.Service.Model.Applications.Custom;
using Triton.Service.Model.Applications.Tables;

namespace Triton.Operations.Helpers
{
    public class RoadFreightAgentHelper
    {
        private const int _paidLookUpCodeID = 707;
        public static string[] SplitFilterDate(string filterDate)
        {
            return filterDate.Split('-');
        }

        public static List<RoadFreightAgent> RoadFreightAgentDTO(List<RoadFreightAgentModel> model)
        {
            var roadFreightAgents = new List<RoadFreightAgent>();

            var roadFreightAgentsList = model.Where(x => x.IsChecked).ToList();

            foreach (var item in roadFreightAgentsList)
            {
                roadFreightAgents.Add(new RoadFreightAgent
                {
                    RoadFreightAgentID = item.RoadFreightAgentID,
                    CategoryLCID = item.CategoryLCID,
                    CreatedByUserID = item.CreatedByUserID,
                    CreatedOn = item.CreatedOn,
                    DeletedByUserID = item.DeletedByUserID,
                    DeletedOn = item.DeletedOn,
                    Invoice = item.Invoice,
                    Mass = item.Mass,
                    StatusLCID = _paidLookUpCodeID,
                    Subtotal = item.Subtotal,
                    SupplierDate = item.SupplierDate,
                    SupplierID = item.SupplierID,
                    Surcharge = item.Surcharge,
                    Total = item.Total,
                    Vat = item.Vat,
                    Volumetric = item.Volumetric,
                    WaybillID = item.WaybillID,
                    WaybillNo = item.WaybillNo

                });
            }

            return roadFreightAgents;

        }

        public static PurchaseOrder_Overview PurchaseOrderOverviewDTO(PurchaseOrder_Overview purchaseOrder_Overview, string filterDate, int lookUpCodeId, int filterUserId)
        {
            return new PurchaseOrder_Overview
            {
                ActiveTab = purchaseOrder_Overview.ActiveTab,
                FilterDate = filterDate,
                RoadFreightAgent = purchaseOrder_Overview.RoadFreightAgent,
                AgentIssueTabs = purchaseOrder_Overview.AgentIssueTabs,
                LookUpCodeID = lookUpCodeId,
                SelectedUser = filterUserId,
                SupplierTabs = purchaseOrder_Overview.SupplierTabs,
                Active_Supplier_Tab = 0
            };
        }
    }
}
