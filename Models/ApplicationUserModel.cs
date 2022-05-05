using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triton.Model.Applications.Custom;
using Triton.Model.Applications.Tables;

namespace Triton.CRM.Models
{
    public class ApplicationUserModel
    {
        public List<tblUserModel> UsersList { get; set; }
        public tblUsers User { get; set; }
        public tblUsers Users { get; set; }
        public tblUsers UserDetails { get; set; }

        public IEnumerable<SelectListItem> BranchSelectList { get; set; }
        public IEnumerable<SelectListItem> DepartmentSelectList { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string UserNameErrorMessage { get; set; }
        public string EmailAddressErrorMessage { get; set; }

    }
}
