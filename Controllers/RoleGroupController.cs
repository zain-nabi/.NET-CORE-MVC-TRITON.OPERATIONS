using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Triton.Operations.Helper;
using Triton.Operations.Models;
using Triton.Service.Data;
using Triton.Service.Utils;

namespace Triton.Operations.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RoleGroupController : Controller
    {
        private const int _systemId = 5;

        public  IActionResult Create()
        {
            var model = new RoleGroupViewModel();
            return View(model);
        }

        public async Task<IActionResult> Overview()
        {
            var overview = await RoleGroupService.GetAllAsync(_systemId);
            return View(overview);
        }

        public async Task<IActionResult> Edit(int roleGroupId)
        {
            var model = new RoleGroupViewModel
            {
                RoleGroup = await RoleGroupService.FindRoleGroupByIdAsync(roleGroupId)

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleGroupViewModel roleGroupViewModel)
        {
            var isRoleNameExists = await RoleGroupService.IsRoleNameExistsAsync(roleGroupViewModel.RoleGroup.RoleName);
            if (isRoleNameExists)
            {
                roleGroupViewModel.ErrorMessage = $"{roleGroupViewModel.RoleGroup.RoleName} role already exists on database";
                return View(roleGroupViewModel);
            }
            var roleGroup = RoleGroupHelper.CreateRoleGroup(roleGroupViewModel, User.GetUserId());
            var result = await RoleGroupService.UpdateAsync(roleGroup);

            string redirectUrl = string.Format("/RoleGroup/Overview");
            return result ? RedirectToAction("Message", "Home", new { Service.Utils.StringHelper.Types.UpdateSuccess, url = redirectUrl })
                          : RedirectToAction("Message", "Home", new { Service.Utils.StringHelper.Types.UpdateFailed });
            
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleGroupViewModel roleGroupViewModel)
        {
            var isRoleNameExists = await RoleGroupService.IsRoleNameExistsAsync(roleGroupViewModel.RoleGroup.RoleName);
            if (isRoleNameExists)
            {
                roleGroupViewModel.ErrorMessage = $"{roleGroupViewModel.RoleGroup.RoleName} role already exists on database";
                return View(roleGroupViewModel);
            }

            var roleGroup = RoleGroupHelper.CreateRoleGroup(roleGroupViewModel, User.GetUserId());
            var result = await RoleGroupService.InsertAsync(roleGroup);

            string redirectUrl = string.Format("/RoleGroup/Overview");
            return result ? RedirectToAction("Message", "Home", new { Service.Utils.StringHelper.Types.UpdateSuccess, url = redirectUrl })
                          : RedirectToAction("Message", "Home", new { Service.Utils.StringHelper.Types.UpdateFailed });

        }

        public async Task<IActionResult> DeleteRoleGroup(int roleGroupId ,string actionPerfomed)
        {
            var roleGroup = await RoleGroupService.FindRoleGroupByIdAsync(roleGroupId);
            var loginUser = User.GetUserId();
            if (actionPerfomed == "Activate")
            {
                roleGroup.DeletedBy = null;
                roleGroup.DeletedOn = null;
                roleGroup.IsActive = true;
            }
            else
            {
                roleGroup.DeletedBy = User.GetUserId();
                roleGroup.DeletedOn = DateTime.Now;
                roleGroup.IsActive = false;
            }
            
            var result = await RoleGroupService.UpdateAsync(roleGroup);

            string redirectUrl = string.Format("/RoleGroup/Overview");
            return result ? RedirectToAction("Message", "Home", new { Service.Utils.StringHelper.Types.UpdateSuccess, url = redirectUrl })
                          : RedirectToAction("Message", "Home", new { Service.Utils.StringHelper.Types.UpdateFailed });
        }

    }
}
