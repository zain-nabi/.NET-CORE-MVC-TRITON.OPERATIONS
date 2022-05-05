using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Triton.Model.CRM.Tables;

namespace Triton.Operations.Controllers
{
    public class TarsusController : Controller
    {
        public IActionResult Index()
        {
            var model = new Waybills();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(Waybills model)
        {

            var t = model.WaybillNo;
            return View();
        }
    }
}
