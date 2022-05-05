using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Triton.Operations.Helpers;
using Triton.Operations.Models;
using Triton.Service.Data;
using Triton.Service.Utils;

namespace Triton.Operations.Controllers
{
    public class CSAAuditNoteController : Controller
    {
        public async Task<IActionResult> Overview(string filterDate, int customerId)
        {
           
            var report = await CSAAuditNoteService.GetAllAsync(User.GetUserId(),customerId,  Convert.ToDateTime(filterDate), Convert.ToDateTime(filterDate));
            var customer = await CustomerService.GetCrmCustomerById(customerId);
            if (report.CSAAuditNote.Count == 0)
            {
                return RedirectToAction("Overview", "CustomerServiceAgent", new { errorMessage = $"No emails sent to {customer.Name} betweeen {filterDate} and {filterDate}  exists on database. " });
            }
            var model = new CSAAuditNoteView
            {
                CSAAuditNotes = report.CSAAuditNote,
                FilterDate = filterDate,
                Customers = report.Customers,
                Users = report.Users
            };

            return View(model);
        }

        //[HttpPost]

        //public async Task<IActionResult> Search(CSAAuditNoteView cSAAuditNoteView)
        //{
        //    var splitDate = RoadFreightAgentHelper.SplitFilterDate(cSAAuditNoteView.FilterDate);
        //    var report = await CSAAuditNoteService.GetAllAsync(User.GetUserId(), Convert.ToDateTime(splitDate[0]), Convert.ToDateTime(splitDate[1]));
        //    var model = new CSAAuditNoteView
        //    {
        //        CSAAuditNotes = report.CSAAuditNote,
        //        FilterDate = $"{cSAAuditNoteView.FilterDate}",
        //        Customers = report.Customers,
        //        Users = report.Users,
        //        ErrorMessage = (report.CSAAuditNote.Count == 0) ? $"No sent emails for {report.Customers.Name} exists on database. " : null
        //    };

        //    return View("Overview", model);

        //}

        public async Task<IActionResult> GetEmailDocumentByID(int cSAAuditNoteId, int customerId)
        {
            var report = await CSAAuditNoteService.GetByIdAsync(cSAAuditNoteId);
            var customers = await CustomerService.GetCrmCustomerById(customerId);
            var contentDispositionHeader = new System.Net.Mime.ContentDisposition
            {
                Inline = true,
                FileName = $"{customers.Name}.xls"
            };
            Response.Headers.Add("Content-Disposition", contentDispositionHeader.ToString());
            return File(report.Document, $"application/octet-stream");
        }

    }
}
