using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Triton.Model.TritonGroup.Tables;
using Triton.Model.TritonOps.Custom;
using Triton.Operations.Models;
using Triton.Service.Data;
using Triton.Service.Utils;

namespace Triton.Operations.Controllers
{
    public class EmployeeGifts : Controller
    {

        public async Task<IActionResult> Index()
        {
            var lookUpCodes = await EmployeeGiftsService.GetAllLookUpCodes();
            var employeeGiftsModel = new EmployeeGiftModel
            {
                LookUpCodes = lookUpCodes
            };

            return View(employeeGiftsModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(EmployeeGiftModel model)
        {
            if (model is null)
                return RedirectToAction("Message", "Home", new { type = StringHelper.Types.NoRecords });

            if (EmployeeGiftsService.FindEmployeeByCode(model.EmployeeGifts.EmployeeCode).Result.CurrentEmployeeCode == null)
                return await GenerateErrorMessage(model, "Incorrect Employee Code {0} entered");

            if (EmployeeGiftsService.EmployeeCodeExists(model.EmployeeGifts.EmployeeCode).Result)
                return await GenerateErrorMessage(model, "Employee Code {0} exists on database");

            var result = await EmployeeGiftsService.InsertAsync(model.EmployeeGifts);

            return result ? RedirectToAction("Index", "EmployeeGifts", new { StringHelper.Types.UpdateSuccess })
           : RedirectToAction("Index", "EmployeeGifts", new { StringHelper.Types.UpdateFailed });

        }

        public async Task<JsonResult> GetAllEmployeesBranches(string employeeCode)
        {
            var employeesBranches = await EmployeeGiftsService.GetAllEmployeesBranches(employeeCode);
            return Json(employeesBranches);
        }


        private async Task<IActionResult> GenerateErrorMessage(EmployeeGiftModel model, string errorMessage)
        {
            model.LookUpCodes = await EmployeeGiftsService.GetAllLookUpCodes();
            model.ErrorMessage = string.Format(errorMessage, model.EmployeeGifts.EmployeeCode);
            return View(model);
        }

        public async Task<IActionResult> GetAllEmployeeGifts()
        {
            var employeeGifts = await EmployeeGiftsService.GetAll();
            return View(employeeGifts);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeGift(string employeeCode)
        {
            var employeeGift = await EmployeeGiftsService.GetByEmployeeCode(employeeCode);
            var employeeGiftsModel = new EmployeeGiftModel
            {
                LookUpCodes = await EmployeeGiftsService.GetAllLookUpCodes(),
                EmployeeGifts = employeeGift
            };

            return View(employeeGiftsModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditEmployeeGift(EmployeeGiftModel model)
        {
            if (model is null)
                return RedirectToAction("Message", "Home", new { type = StringHelper.Types.NoRecords });

            
            var result = await EmployeeGiftsService.UpdateAsync(model.EmployeeGifts);

            return result ? RedirectToAction("GetAllEmployeeGifts", "EmployeeGifts", new { StringHelper.Types.UpdateSuccess })
                          : RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateFailed });
        }
    }
}

