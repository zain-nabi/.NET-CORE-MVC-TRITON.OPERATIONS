using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Triton.Model.LeaveManagement.Custom;
using Triton.Model.LeaveManagement.Tables;
using Triton.Operations.Models;
using Triton.Service.Data;
using Triton.Service.Utils;

namespace Triton.Operations.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        private readonly IConfiguration _configuration;
        private const int QuestionnaireTemplateId = 1;
        private const int VisitorQuestionnaireTemplateId = 2;

        public PersonController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult EmployeeSearch()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeSearch(Employees model)
        {
            var employee = await GetEmployee(model.CurrentEmployeeCode);

            if (employee != null) return RedirectToAction("Info", new { employee.CurrentEmployeeCode });

            ModelState.AddModelError("Error", "The employee code does not exist");
            return View();

        }

        public async Task<IActionResult> Info(string currentEmployeeCode)
        {
            QuestionnaireModel questionnaire;
            var employee = await GetEmployee(currentEmployeeCode);

            try
            {
                // check the employee number of tries
                var attempt = await QuestionnaireService.GetExcessTempCountforTritonCovid(currentEmployeeCode, DateTime.Now);

                if (attempt > 3)
                {
                    // Send an email to the branch manager and Niki
                    var manager = await EmployeeService.GetBranchManager(employee.BranchID.Value);

                    var body = $"Please note that employee {employee.LeaveDisplayName} has recorded an excess temperature more than {attempt} times today.";
                    //await Email.SendIntraSystemEmail(new[]{ manager.Users.Email,"nikio@tritonexpress.co.za"},null,"administrator@tritonexpress.co.za", body,"Employee repeated high temperature.",$"{_configuration.GetSection("smtp").GetSection("ip").Value}",null);
                    //await Email.SendIntraSystemEmail(new[] { "shaneilr@tritonexpress.co.za" }, null, "administrator@tritonexpress.co.za", body, "Employee repeated high temperature.", $"{_configuration.GetSection("smtp").GetSection("ip").Value}", null);
                    //await Email.SendIntraSystemEmail(new[] { "zainn@tritonexpress.co.za", "zainn@tritonexpress.co.za" }, null, "administrator@tritonexpress.co.za", body, "Employee repeated high temperature.", null);

                    // Do not allow access
                    var messageModel = new MessageViewModel
                    {

                        Header = "WARNING",
                        Message = $"Do not allow this user access onto the premises.  Failed temperature test {attempt} times",
                        Status = "Warning",
                        Route = "Index",
                        Controller = "Home"
                    };

                    return RedirectToAction("Message", "Home", messageModel);
                }
                questionnaire = await QuestionnaireService.GetQuestionaireCreateModel(QuestionnaireTemplateId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (employee == null)
            {
                ModelState.AddModelError("Error", "The employee code does not exist");
                return View();
            }

            var model = new EmployeeModel
            {
                Employee = employee,
                QuestionnaireModel = questionnaire,
                CreatedByUserId = User.GetUserId()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Info(EmployeeModel model)
        {
            var header = "";
            var message = "";
            var isSuccessful = "Failed";

            try
            {
                var answerList = model.QuestionnaireModel.QuestionsandAnswers.Select(item => item.Answer).ToList();
                answerList = answerList.Select(c => { c.Response = c.Response == "true" ? "Yes" : c.Response; return c; }).ToList();
                answerList = answerList.Select(c => { c.Response = c.Response == "false" ? "No" : c.Response; return c; }).ToList();

                // Remove the Main question with no response
                answerList = answerList.Where(x => x.QuestionId != 15).ToList();

                var postModel = new QuestionnairePostModel
                {
                    CreatedOn = DateTime.Now,
                    CreatedByGroupUserId = User.GetUserId(),
                    QuestionnaireTemplateId = model.QuestionnaireTemplateId,
                    Answers = answerList
                };

                var result = await QuestionnaireService.Post(postModel);

                if (result > 0)
                {
                    // Check the users temperature
                    var temperature = answerList.FirstOrDefault(x => x.QuestionId == 5)?.Response;
                    var suggestedTemperature = $"{_configuration.GetSection("Corvid-19").GetSection("temperature").Value}";
                    if (decimal.Parse(temperature ?? string.Empty) > decimal.Parse(suggestedTemperature))
                    {
                        header = "WARNING";
                        message = $"Do not allow this user access onto the premises.  Their temperature is above the recommended limit of {suggestedTemperature}";
                        isSuccessful = "Warning";
                    }
                    else
                    {
                        header = "Completed";
                        message = "The record has been saved successfully";
                        isSuccessful = "Success";
                    }
                }
            }
            catch
            {
                header = "Failed";
                message = "The record has failed to save";
                isSuccessful = "Failed";
            }

            var messageModel = new MessageViewModel
            {
                Header = header,
                Message = message,
                Status = isSuccessful,
                Route = "Index",
                Controller = "Home"
            };

            return RedirectToAction("Message", "Home", messageModel);
        }

        public IActionResult CompletedSearch()
        {
            return View(new EmployeeSearchModel());
        }

        [HttpPost]
        public async Task<IActionResult> CompletedSearch(EmployeeSearchModel model)
        {
            model.QuestionnaireSearchModel = await QuestionnaireService.FindQuestionaireList(model.EmployeeCode, model.Date);

            if (model.QuestionnaireSearchModel.Count == 0)
            {
                model.QuestionnaireSearchModel = await QuestionnaireService.FindQuestionaireList(model.EmployeeCode, model.Date, 2);
            }
            return View(model);
        }

        public async Task<IActionResult> CompletedInfo(long questionnaireId, string identityResponse)
        {
            QuestionnaireModel model;
            try
            {
                model = await QuestionnaireService.GetQuestionaireModel(questionnaireId);

                if (model.Template.QuestionnaireTemplateId == 1)
                {
                    // Find the employee
                    model.Employee = await EmployeeService.GetEmployee(identityResponse);
                }
            }
            catch
            {
                var messageModel = new MessageViewModel
                {
                    Header = "Error",
                    Message = "Failed to find the record",
                    Status = "Failed",
                    Route = "Index",
                    Controller = "Home"
                };

                return RedirectToAction("Message", "Home", messageModel);
            }
            return View(model);
        }

        public async Task<IActionResult> VisitorCreate()
        {
            QuestionnaireModel questionnaire;

            try
            {
                questionnaire = await QuestionnaireService.GetQuestionaireCreateModel(VisitorQuestionnaireTemplateId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var model = new EmployeeModel
            {
                //Employee = employee,
                QuestionnaireModel = questionnaire,
                CreatedByUserId = User.GetUserId()
            };

            return View(model);
        }


        private async Task<Employees> GetEmployee(string employeeCode)
        {
            try
            {
                var employee = await EmployeeService.GetEmployee(employeeCode);
                return employee;
            }
            catch
            {
                return null;
            }
        }
    }
}