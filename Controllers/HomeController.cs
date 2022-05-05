using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Triton.Operations.Models;
using Microsoft.AspNetCore.Http;
using Triton.Model.Utils;

namespace Triton.Operations.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string NotApplicable = "N/A";

        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            if (User.FindFirst("UserID") == null)
            {
                return RedirectToAction("Access", "Home");
            }

            if (User.FindFirst("Employee").Value.ToString().Equals(NotApplicable))
            {
                var username = User.FindFirst("Name").Value == null ? "User" : User.FindFirst("Name").Value;

                return RedirectToAction("Roles", "Home", new { message = $"{username} is not mapped to employee.", title = "Employee", rootPath = "Contact the HR Administrator." });
            }

            if (User.FindFirst("Roles").Value.ToString().Equals(NotApplicable) || User.FindFirst("Rolenames").Value.ToString().Equals(NotApplicable))
            {
                var username = User.FindFirst("Name").Value == null ? "User" : User.FindFirst("Name").Value;

                return RedirectToAction("Roles", "Home", new { message = $"{username} is not assigned to a role.", title = "Roles", rootPath = "Contact the Sales Administrator." });
            }

            return View();
        }

        public IActionResult Roles(string message, string title, string rootPath)
        {
            var messageModel = new MessageModel
            {
                Message = message,
                Title = title,
                RootPath = rootPath

            };
            return View(messageModel);
        }

        public IActionResult Access()
        {
            return View();
        }

        public IActionResult Message(string type, string url, string title, string message)
        {
            if (url == null)
            {
                url = "Home/Index";
            }

            var model = new MessageModel
            {
                Message = Service.Utils.StringHelper.Html.UpdateRecordSuccessMessage,
                //Route = "Index",
                // Controller = "Home",
                Title = Service.Utils.StringHelper.Html.UpdateRecordSuccessTitle,
                Icon = "fas fa-check-circle text-success",
                Url = url,
                RootPath = $"http://{_httpContextAccessor.HttpContext.Request.Host.Value}"
            };

            switch (type)
            {
                case Service.Utils.StringHelper.Types.NoRecords:
                    model.Title = title ?? "Oops";
                    model.Message = message ?? "Something has gone wrong... Contact IT";
                    model.Icon = Service.Utils.StringHelper.Html.FailedIcon;
                    model.Type = "NoRecords";
                    break;
                case Service.Utils.StringHelper.Types.UpdateSuccess:
                    model.Message = Service.Utils.StringHelper.Html.UpdateRecordSuccessMessage;
                    model.Title = Service.Utils.StringHelper.Html.UpdateRecordSuccessTitle;
                    model.Type = null;
                    break;
                case Service.Utils.StringHelper.Types.UpdateFailed:
                    model.Title = Service.Utils.StringHelper.Html.UpdateRecordFailedTitle;
                    model.Message = Service.Utils.StringHelper.Html.UpdateRecordFailedMessage;
                    model.Icon = Service.Utils.StringHelper.Html.FailedIcon;
                    model.Type = null;
                    break;
                case Service.Utils.StringHelper.Types.SaveSuccess:
                    model.Title = Service.Utils.StringHelper.Html.SaveRecordSuccessMessage;
                    model.Message = Service.Utils.StringHelper.Html.SaveRecordSuccessTitle;
                    model.Icon = Service.Utils.StringHelper.Html.SuccessIcon;
                    model.Type = null;
                    break;
                case Service.Utils.StringHelper.Types.DeleteSuccess:
                    model.Title = Service.Utils.StringHelper.Html.DeleteRecordSuccessMessage;
                    model.Message = Service.Utils.StringHelper.Html.DeleteRecordSuccessTitle;
                    model.Icon = Service.Utils.StringHelper.Html.SuccessIcon;
                    model.Type = null;
                    break;
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode != null)
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, StatusCode = int.Parse(statusCode.ToString()) });
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
