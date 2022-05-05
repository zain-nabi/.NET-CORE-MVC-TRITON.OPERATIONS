using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Triton.Model.TritonExpress.Tables;
using Triton.Operations.Helpers;
using Triton.Operations.Models;
using Triton.Service.Data;
using Triton.Service.Model.TritonExpress.Custom;
using Triton.Service.Utils;

namespace Triton.Operations.Controllers
{
    [Authorize(Roles = "Administrator, Customer Services Manager")]
    public class PostalCodesController : Controller
    {
        private readonly IConfiguration _configuaration;

        public PostalCodesController(IConfiguration configuration)
        {
            _configuaration = configuration;
        }

        public IActionResult Search()
        {
            return View(new PostalCodesViewModel());
        }


        public async Task<IActionResult> Create()
        {
            var model = await PostalCodeService.GetLookUpAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostalCodesModel postalCodesModel)
        {

            var isExists = await PostalCodeService.IsNameAndSuburbAndPostalCodeExistsAsync(postalCodesModel.PostalCodes.Name, postalCodesModel.PostalCodes.Suburb, postalCodesModel.PostalCodes.PostalCode);
            if (isExists)
            {
                var model = await PostalCodeService.GetLookUpAsync();
                model.ErrorMessage = $"Name : {postalCodesModel.PostalCodes.Name}, Suburb : {postalCodesModel.PostalCodes.Suburb}, PostalCode : {postalCodesModel.PostalCodes.PostalCode} already exists on database";
                model.PostalCodes = postalCodesModel.PostalCodes;

                return View("Create", model);
            }
            var postalCode = PostalCodesHelper.CreatePostalCode(postalCodesModel, User.GetUserId());
            var results = await PostalCodeService.InsertAsync(postalCode);
            var redirectUrl = $"/PostalCodes/GetByPostalCode?postalCode={postalCode.PostalCode}";
            return results ? RedirectToAction("Message", "Home", new { StringHelper.Types.SaveSuccess, url = redirectUrl })
                           : RedirectToAction("Message", "Home", new { StringHelper.Types.SaveFailed, url = redirectUrl });

        }


        [HttpPost]
        public async Task<IActionResult> UpdatePostalCodes(PostalCodesModel postalCodesModel)
        {
            var postalCode = PostalCodesHelper.CreatePostalCode(postalCodesModel, User.GetUserId());
            var results = await PostalCodeService.UpdateAsync(postalCode);
            var redirectUrl = $"/PostalCodes/GetByPostalCode?postalCode={postalCode.PostalCode}";
            return results ? RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateSuccess, url = redirectUrl })
                           : RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateFailed, url = redirectUrl });

        }

        public async Task<IActionResult> GetByPostalCode(string postalCode)
        {
            var model = new PostalCodesViewModel
            {
                PostalCodesOverview = await PostalCodeService.PostalCodesOverviewAsync(postalCode),
                PostalCodeSearch = postalCode
            };
            return View("Search", model);
        }


        public async Task<IActionResult> GetByPostalCodeID(int postalCodeId)
        {
            var model = await PostalCodeService.GetByIdAsync(postalCodeId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Search(PostalCodesViewModel postalCodesViewModel)
        {

            var model = new PostalCodesViewModel
            {
                PostalCodesOverview = await PostalCodeService.PostalCodesOverviewAsync(postalCodesViewModel.PostalCodeSearch),
                PostalCodeSearch = postalCodesViewModel.PostalCodeSearch
            };
            return View(model);
        }

        public async Task<IActionResult> DownloadPostalCodes(string postalCode)
        {
            var postalCodes = await PostalCodeService.FindByPostalCodeAsync(postalCode);
            var dataTable = DocumentsHelper.ConvertListToDataTable(postalCodes);
            var descriptionName = postalCode == null ? "ALL" : postalCode;
            var fileName = $"{DateTime.Now.ToString("dd_MMMM_yyyy_HH_mm_ss")}_PostalCode_{descriptionName}.xlsx";

            await ExportToExcel(postalCodes, fileName);

            var model = new PostalCodesViewModel
            {
                PostalCodes = postalCodes,
                PostalCodeSearch = postalCode
            };

            return View(model);
        }



        private async Task ExportToExcel(List<PostalCodes> postalCodes, string fileName)
        {

            using (var workBook = new XLWorkbook())

            {
                var worksheet = workBook.Worksheets.Add("PostalCodes");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "PostalCodeID";
                worksheet.Cell(currentRow, 2).Value = "PostalCode";
                worksheet.Cell(currentRow, 3).Value = "Name";
                worksheet.Cell(currentRow, 4).Value = "Suburb";
                worksheet.Cell(currentRow, 5).Value = "RateArea";
                worksheet.Cell(currentRow, 6).Value = "RateAreaID";
                worksheet.Cell(currentRow, 7).Value = "BranchCode";
                worksheet.Cell(currentRow, 8).Value = "BayNo";
                worksheet.Cell(currentRow, 9).Value = "BayName";
                worksheet.Cell(currentRow, 10).Value = "BayRoute";
                worksheet.Cell(currentRow, 11).Value = "KnownAs";
                worksheet.Cell(currentRow, 12).Value = "ServicedByLookUpCodeID";
                worksheet.Cell(currentRow, 13).Value = "PostalCodeTransitTimeID";
                worksheet.Cell(currentRow, 14).Value = "PostalCodeStatusID";
                worksheet.Cell(currentRow, 15).Value = "ApprovalUserID";
                worksheet.Cell(currentRow, 16).Value = "ActionedOn";
                worksheet.Cell(currentRow, 17).Value = "Active";
                worksheet.Cell(currentRow, 18).Value = "PostalCodeRequestID";
                worksheet.Cell(currentRow, 19).Value = "KMsFromBranch";

                for (int i = 0; i < postalCodes.Count; i++)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = postalCodes[i].PostalCodeID;
                    worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 2).Value = postalCodes[i].PostalCode;

                    worksheet.Cell(currentRow, 3).Value = string.IsNullOrEmpty(postalCodes[i].Name) ? "N/A" : postalCodes[i].Name;
                    worksheet.Cell(currentRow, 4).Value = string.IsNullOrEmpty(postalCodes[i].Suburb) ? "N/A" : postalCodes[i].Suburb;
                    worksheet.Cell(currentRow, 5).Value = string.IsNullOrEmpty(postalCodes[i].RateArea) ? "N/A" : postalCodes[i].RateArea;
                    worksheet.Cell(currentRow, 6).Style.NumberFormat.Format = $"@";
                    worksheet.Cell(currentRow, 6).Value = (!postalCodes[i].RateAreaID.HasValue) ? 0 : postalCodes[i].RateAreaID;
                    worksheet.Cell(currentRow, 7).Value = string.IsNullOrEmpty(postalCodes[i].BranchCode) ? "N/A" : postalCodes[i].BranchCode;
                    worksheet.Cell(currentRow, 8).Value = string.IsNullOrEmpty(postalCodes[i].BayNo) ? "N/A" : postalCodes[i].BayNo;
                    worksheet.Cell(currentRow, 9).Value = string.IsNullOrEmpty(postalCodes[i].BayName) ? "N/A" : postalCodes[i].BayName;
                    worksheet.Cell(currentRow, 10).Value = string.IsNullOrEmpty(postalCodes[i].BayRoute) ? "N/A" : postalCodes[i].BayRoute;
                    worksheet.Cell(currentRow, 11).Value = string.IsNullOrEmpty(postalCodes[i].KnownAs) ? "N/A" : postalCodes[i].KnownAs;
                    worksheet.Cell(currentRow, 12).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 12).Value = (!postalCodes[i].ServicedByLookUpCodeID.HasValue) ? 0 : postalCodes[i].ServicedByLookUpCodeID;
                    worksheet.Cell(currentRow, 13).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 13).Value = (!postalCodes[i].PostalCodeTransitTimeID.HasValue) ? 0 : postalCodes[i].PostalCodeTransitTimeID;
                    worksheet.Cell(currentRow, 14).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 14).Value = (!postalCodes[i].PostalCodeStatusID.HasValue) ? 0 : postalCodes[i].PostalCodeStatusID;
                    worksheet.Cell(currentRow, 15).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 15).Value = (!postalCodes[i].ApprovalUserID.HasValue) ? 0 : postalCodes[i].ApprovalUserID;
                    worksheet.Cell(currentRow, 16).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 16).Value = (!postalCodes[i].ActionedOn.HasValue) ? DateTime.Now : postalCodes[i].ActionedOn;
                    worksheet.Cell(currentRow, 17).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 17).Value = (!postalCodes[i].Active.HasValue) ? 0 : 1;
                    worksheet.Cell(currentRow, 18).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 18).Value = (!postalCodes[i].PostalCodeRequestID.HasValue) ? 0 : postalCodes[i].PostalCodeRequestID;
                    worksheet.Cell(currentRow, 19).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 19).Value = (!postalCodes[i].KMsFromBranch.HasValue) ? 0 : postalCodes[i].KMsFromBranch;

                }

                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    var content = stream.ToArray();
                    Response.Clear();
                    Response.Headers.Add("content-disposition", $"attachment;filename={fileName}");
                    Response.ContentType = "application/text";
                    await Response.Body.WriteAsync(content);
                    await Response.Body.FlushAsync().ConfigureAwait(false);
                }
            }


        }

        [HttpPost]
        public async Task<ActionResult> UploadPostalCodes(IFormFile File, string postalCode)
        {
            try
            {
                var fileExtension = Path.GetExtension(File.FileName).ToLower();

                var sharedFolder = _configuaration.GetSection("RemoteServer").GetSection("SharedFolder").Value;

                var path = $"{sharedFolder}{File.FileName}";

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await File.CopyToAsync(fileStream);
                }

                var result = await PostalCodeService.UploadPostalCodeAsync(File.FileName);

                var urlRedirect = string.IsNullOrEmpty(postalCode) ? "/PostalCodes/Search" : $"/PostalCodes/GetByPostalCode?postalCode={postalCode}";
                var resultsMessage = (result) ? StringHelper.Types.UpdateSuccess : StringHelper.Types.UpdateFailed;

                return Ok(new { success = true, message = resultsMessage, url = urlRedirect });

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = StringHelper.Types.UpdateFailed, url = "/PostalCodes/Search", title = ex.ToString() });

            }

        }

        public async Task<IActionResult> DeletePostalCodes(int postalCodeId, string postalCode)
        {

            var result = await PostalCodeService.DeletePostalCodeAsync(postalCodeId);

            var urlRedirect = $"/PostalCodes/GetByPostalCode?postalCode={postalCode}";

            return result ? RedirectToAction("Message", "Home", new { type = StringHelper.Types.UpdateSuccess, url = urlRedirect })
            : RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateFailed, url = urlRedirect });
        }
    }
}

