using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Triton.Operations.Helpers;
using Triton.Operations.Models;
using Triton.Service.Data;
using Triton.Service.Model.CRM.Tables;
using Triton.Service.Utils;


namespace Triton.Operations.Controllers
{
    public class CustomerServiceAgentController : Controller
    {
        private const int _statusLookUpCategoryID = 107;
        private const int _statusLookUpID = 717;
        private const bool _includePOD = true;
        private static IConfiguration _config;
        private  byte[] _embeddedReport = null;
        private  byte[] _UploadReport = null;

        public CustomerServiceAgentController(IConfiguration config)
        {
            _config = config;
            //'_env = env;
        }

        public async Task<IActionResult> Overview(string errorMessage = null)
        {

            var model = new CSAViewModel
            {
                proc_CSA_GetByUserID = await CustomerServiceAgentService.FindCSAByUserIdAsync(User.GetUserId(), DateTime.Now, DateTime.Now),
                FilterDate = DateTime.Now.ToString("yyyy-MM-dd"),
                ErrorMessage = $"{errorMessage}"
            };

            return View(model);
        }

       

        [HttpPost]

        public async Task<IActionResult> Search(CSAViewModel sAViewModel)
        {
            
            var model = new CSAViewModel
            {
                proc_CSA_GetByUserID = await CustomerServiceAgentService.FindCSAByUserIdAsync(User.GetUserId(), Convert.ToDateTime(sAViewModel.FilterDate), Convert.ToDateTime(sAViewModel.FilterDate)),
                CSAList = await LookUpCodesService.LookupCodesByCategoryID(_statusLookUpCategoryID),
                FilterDate = sAViewModel.FilterDate
            };


            return View("Overview", model);

        }
        public IActionResult ViewCustomerServiceAgentReport(int customerId, string filterDate)
        {
            try
            {
                var dateFilter = DateTime.Now;
                
                var startDate = dateFilter.ToString("yyyy-MM-dd");
                var endDate   = dateFilter.ToString("yyyy-MM-dd");

                if (filterDate != null && !string.IsNullOrEmpty(filterDate))
                {
                    
                    startDate = filterDate;
                    endDate   = filterDate;
                }

                var model = new CSAViewModel
                {
                    ShowReport = true,
                    CutomerID = customerId,
                    StartDate = startDate,
                    endDate = endDate,
                    FilterDate = filterDate,
                    Url = "http://Tiger/ReportServer/Pages/ReportViewer.aspx?/OperationalReports/CSADailyReport&CustomerID=@CustomerID&includePOD=@includePOD&rs:ParameterLanguage=&rc:Parameters=Collapsed&rs:Command=Render&rs:Format=HTML4.0"
                   .Replace("@CustomerID", customerId.ToString())
                   .Replace("@includePOD", _includePOD.ToString())
                };

                return View(model);
            }
            catch (System.Exception)
            {
                throw;
            }

        }


        public async Task<IActionResult> PrepareEmail(int customerId, string accountCode, string filterDate, string name)
        {
            try
            {

                var model = new CSAViewModel
                {
                    
                    CSAList = await LookUpCodesService.LookupCodesByCategoryID(_statusLookUpCategoryID),
                    FilterDate = filterDate,
                    CutomerID = customerId,
                    AccountCode = accountCode,
                    Name = name
                };


                return View(model);
            }
            catch (System.Exception)
            {
                throw;
            }

        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail(IFormFile file, CSAViewModel sAViewModel)
        {

            try
            {
                                
                var customer = await CustomerService.GetCrmCustomerById(sAViewModel.CutomerID);

                await EmailSCAReport(customer.CustomerID,customer.Name, customer.AccountCode, sAViewModel.To, sAViewModel.CC, sAViewModel.EmailBody, sAViewModel.FilterDate, file, sAViewModel.CSALCID, sAViewModel.Subject);
                var model = new CSAAuditNote
                {
                    CustomerID = customer.CustomerID,
                    CSALCID = sAViewModel.CSALCID,
                    CC = sAViewModel.CC,
                    CreatedByUserId = User.GetUserId(),
                    CreatedOn = DateTime.Now,
                    EmailBody = sAViewModel.EmailBody,
                    To = sAViewModel.To,
                    Subject = sAViewModel.Subject,
                    WayBillDate = Convert.ToDateTime(sAViewModel.FilterDate),
                    Document = _embeddedReport == null ? _UploadReport : _embeddedReport
                };
                var results = await CSAAuditNoteService.InsertAsync(model);

                var redirectUrl = $"/CustomerServiceAgent/Overview";
                return results ? RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateSuccess, url = redirectUrl })
                               : RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateFailed, url = redirectUrl });

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = StringHelper.Types.UpdateFailed, url = "/PurchaseOrder/Upload", title = ex.ToString() });

            }
        }


        private static async Task<byte[]> FileUploadToByteArray(IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private  static byte[] DownloadExcelDocument(int customerId, string filterDate)
        {
            var section = _config.GetSection("Credentials");
            
            var client = new WebClient();
            {
                client.Credentials = new NetworkCredential(section.GetSection("username").Value, section.GetSection("password").Value, section.GetSection("domain").Value);
            }
            var url = $"http://Tiger/ReportServer/Pages/ReportViewer.aspx?/OperationalReports/CSADailyReport&CustomerID={customerId}&includePOD={_includePOD}&rs:ParameterLanguage=&rc:Parameters=Collapsed&rs:Command=Render&rs:Format=EXCEL&rs:ClearSession=true";

            try
            {
                return client.DownloadData(url);
            }
            catch
            {
                return null;
            }
        }
        private async Task EmailSCAReport(int customerId, string name, string accountCode, string to, string cc, string emailBody, string filterDate, IFormFile file, int csaLCID, string subject)
        {
            
            var attachments = new List<System.Net.Mail.Attachment>();
            var ccEmail = (cc == null) ? null : new[] { cc };

            if (csaLCID == _statusLookUpID)
            {
                _embeddedReport = DownloadExcelDocument(customerId, filterDate);
                var stream = new MemoryStream(_embeddedReport, 0, _embeddedReport.Length, false, true);
                attachments.Add(new System.Net.Mail.Attachment(stream, $"{name}.xls"));
            }
            else
            {
                _UploadReport = await FileUploadToByteArray(file);
                var stream = new MemoryStream(_UploadReport, 0, _UploadReport.Length, false, true);
                attachments.Add(new System.Net.Mail.Attachment(stream, $"{name}.xlsx"));
            }

            var body = CustomerServiceAgentHelper.GenerateEmailBody(emailBody, accountCode, name);
            await Email.SendIntraSystemEmail(new[] { to }, ccEmail, User.GetUserEmail(), body, $"{subject}", attachments);
        }
    }
}
