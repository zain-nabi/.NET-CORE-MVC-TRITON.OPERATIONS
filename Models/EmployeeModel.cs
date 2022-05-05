using System;
using System.Collections.Generic;
using Triton.Model.LeaveManagement.Custom;
using Triton.Model.LeaveManagement.Tables;

namespace Triton.Operations.Models
{
    public class EmployeeModel
    {
        public Employees Employee { get; set; }
        public QuestionnaireModel QuestionnaireModel { get; set; }
        public int CreatedByUserId { get; set; }
        public int QuestionnaireTemplateId { get; set; }
    }

    public class EmployeeSearchModel
    {
        public string EmployeeCode { get; set; }
        public DateTime Date{ get; set; }
        public List<QuestionnaireSearchModel> QuestionnaireSearchModel { get; set; }
    }
}
