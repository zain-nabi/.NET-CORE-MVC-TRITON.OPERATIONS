using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Triton.Model.TritonSecurity.Tables;
using Triton.Operations.Models;
using Triton.Service.Data;
using Triton.Service.Data.Branch;
using Triton.Service.Utils;

namespace Triton.Operations.Controllers
{
    public class ReportController : Controller
    {
        //private readonly IBranches _branchService;
        //private readonly IQuestionnaire _questionnaireService;
        //private readonly IReportManager _reportManagerService;

        //public ReportController(IBranches branchService, IQuestionnaire questionnaireService, IReportManager reportManagerService)
        //{
        //    _branchService = branchService;
        //    _questionnaireService = questionnaireService;
        //    _reportManagerService = reportManagerService;

        //}

        public async Task<IActionResult> CovidBranch()
        {
            // Get the list of Branches
            var model = new CovidBranchModel
            {
                StartDate = null,
                EndDate = null
            };

            if (User.GetRoleIds().Contains("1") || User.GetRoleIds().Contains("3") || User.GetRoleIds().Contains("7"))
            {
                model.BranchList = await BranchServices.GetAllActiveBranches();
            }
            else
            {
                var branch = await BranchServices.FindBranch(int.Parse(User.GetCostCentreId()));
                var branchlist = new List<Branches>
                {
                    branch
                };
                model.BranchList = branchlist;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CovidBranch(CovidBranchModel model)
        {
            try
            {
                var dateSplit = model.FilterDate.Split("-");

                model.StartDate = Convert.ToDateTime(dateSplit[0].Trim());
                model.EndDate = Convert.ToDateTime(dateSplit[1].Trim());

                // No data
                var resetModel = new CovidBranchModel
                {
                    BranchList = await BranchServices.GetAllActiveBranches(),
                    Message = StringHelper.Types.NoRecords,
                    ShowReport = true,
                    Url = "http://tiger/ReportServer?/Leave/QuestionnaireRegister&BranchId=@BranchID&StartDate=@StartDate&EndDate=@EndDate&rs:ParameterLanguage=&rc:Parameters=Collapsed&rs:Command=Render&rs:Format=HTML4.0"
                           .Replace("@BranchID", model.SelectedBranchID.ToString())
                           .Replace("@StartDate", model.StartDate.Value.ToShortDateString())
                           .Replace("@EndDate", model.EndDate.Value.ToShortDateString())
                };

                resetModel.StartDate = model.StartDate;
                resetModel.EndDate = model.EndDate;

                return View(resetModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> ReportManager()
        {
            var model = await ReportManagerService.Get("1");
            return View(model);
        }

        public IActionResult OpsDailySales()
        {
            var model = new DailySalesReportModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult OpsDailySales(DailySalesReportModel model)
        {
            var resetmodel = new DailySalesReportModel
            {
                Message = StringHelper.Types.NoRecords,
                ShowReport = true,
                Url = "http://tiger/ReportServer/Pages/ReportViewer.aspx?/OperationalReports/DailySales&ForDate=@ForDate&rs:ParameterLanguage=&rc:Parameters=Collapsed&rs:Command=Render&rs:Format=HTML4.0"
                .Replace("@ForDate", model.Date.ToShortDateString())

            };
            return View(resetmodel);
        }

        public IActionResult OutLyingDeliveries()
        {
            return View();
        }

        public IActionResult GPSSites()
        {
            var model = new DailySalesReportModel
            {
                Message = StringHelper.Types.NoRecords,
                ShowReport = true,
                Url = "http://tiger/ReportServer/Pages/ReportViewer.aspx?/OperationalReports/CustomersSiteTotals&rs:ParameterLanguage=&rc:Parameters=Collapsed&rs:Command=Render&rs:Format=HTML4.0",
            };
            return View(model);
        }


        public IActionResult TarsusWaybillTracker()
        {
            var model = new TarsusWaybillTrackerModel();
           
            return View(model);
        }

        [HttpPost]
        public IActionResult TarsusWaybillTracker(TarsusWaybillTrackerModel model)
        {
            try
            {
                var dateSplit = model.FilterDate.Split("-");

                model.FromDate = Convert.ToDateTime(dateSplit[0].Trim());
                model.ToDate = Convert.ToDateTime(dateSplit[1].Trim());

                // No data
                var resetModel = new TarsusWaybillTrackerModel
                {                    
                    Message = StringHelper.Types.NoRecords,
                    ShowReport = true,
                    Url = "http://tiger/ReportServer?/OperationalReports/TarsusWaybillTracker&FromDate=@FromDate&ToDate=@ToDate&rs:ParameterLanguage=&rc:Parameters=Collapsed&rs:Command=Render&rs:Format=HTML4.0"                          
                           .Replace("@FromDate", model.FromDate.Value.ToShortDateString())
                           .Replace("@ToDate", model.ToDate.Value.ToShortDateString())
                };

                resetModel.FromDate = model.FromDate;
                resetModel.ToDate = model.ToDate;

                return View(resetModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}