using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Triton.Operations.Helper;
using Triton.Operations.Models;
using Triton.Service.Data;
using Triton.Service.Model.TritonGroup.Custom;
using Triton.Service.Utils;

namespace Triton.Operations.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ConfigurationsController : Controller
    {
        private const int _systemId = 5;
        private const int _roleGroupId = 17;
        public async Task<IActionResult> ConfigureUserRole(int userId)
        {
            var model = await UserRoleGroupService.GetByIdAsync(userId, _systemId);

            return View(model);
        }

        public async Task<IActionResult> EditUserRole(int userId)
        {
            var model = await UserRoleGroupService.GetByIdAsync(userId, _systemId);
            return View(model);
        }

        public async Task<IActionResult> EditBranches(int userId)
        {
            var model = await UserBranchService.GetByIdAsync(userId);

            return View(model);
        }

        public async Task<IActionResult> ConfigureBranches(int userId)
        {
            var model = await UserBranchService.GetByIdAsync(userId);

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditUserRole(UserRoleGroupModel userRoleGroupModel)
        {
            var IsUserNameExists = await UserRoleGroupService.IsUserNameExistsAsync(userRoleGroupModel.UserRoleGroup.UserID, userRoleGroupModel.SelectedRoleGroupId);
            if (IsUserNameExists)
            {
                var model = await UserRoleGroupService.GetByIdAsync(userRoleGroupModel.UserRoleGroup.UserID, _systemId);
                var roloGroup = await RoleGroupService.FindRoleGroupByIdAsync(model.UserRoleGroup.RoleGroupID);
                model.ErrorMessage = $"{userRoleGroupModel.Users.sAMAccountName} already assigned to {roloGroup.RoleName} role.";
                return View(model);
            }
            var userRoleGroup = UserRoleGroupHelper.CreateUserRoleGroup(userRoleGroupModel, User.GetUserId());
            var result = await UserRoleGroupService.UpdateAsync(userRoleGroup);

            string redirectUrl = string.Format("/Users/UsersSummary");
            return result ? RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateSuccess, url = redirectUrl })
                          : RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateFailed });
        }

        [HttpPost]
        public async Task<IActionResult> ConfigureUserRole(UserRoleGroupModel userRoleGroupModel)
        {
            var IsUserNameExists = await UserRoleGroupService.IsUserNameExistsAsync(Convert.ToInt32(userRoleGroupModel.Users.UserID), _roleGroupId);
            if (IsUserNameExists)
            {
                var model = await UserRoleGroupService.GetByIdAsync(Convert.ToInt32(userRoleGroupModel.Users.UserID), _systemId);
                var roloGroup = await RoleGroupService.FindRoleGroupByIdAsync(model.UserRoleGroup.RoleGroupID);
                model.ErrorMessage = $"{userRoleGroupModel.Users.sAMAccountName} already assigned to {roloGroup.RoleName} role.";
                model.SelectedRoleGroupId = userRoleGroupModel.SelectedRoleGroupId;

                return View(model);
            }
            var userRoleGroup = ConfigurationHelper.CreateUserRoleGroup(userRoleGroupModel, User.GetUserId());
            var result = await UserRoleGroupService.InsertAsync(userRoleGroup);

            string redirectUrl = string.Format("/Users/UsersSummary");
            return result ? RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateSuccess, url = redirectUrl })
                          : RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateFailed });

        }

        [HttpPost]
        public async Task<IActionResult> ConfigureBranches(UserBranchModel userBranchModel)
        {
            var result = false;
            var isBrancExists = await UserBranchService.IsBrancExistsAsync(Convert.ToInt32(userBranchModel.Users.UserID), userBranchModel.BranchIDS);
            var unMatchedBranchID = ConfigurationHelper.FindUnamatchedBranch(isBrancExists.BranchIds, userBranchModel.BranchIDS);
            if (isBrancExists.IsBranchesExists)
            {
                userBranchModel.BranchIDS = isBrancExists.BranchIds;
                var updateBranch = ConfigurationHelper.CreateUserBranch(userBranchModel, User.GetUserId());
                result = await UserBranchService.UpdateAsync(updateBranch);
            }
            if (!string.IsNullOrEmpty(unMatchedBranchID))
            {
                userBranchModel.BranchIDS = unMatchedBranchID;
                var userBranch = ConfigurationHelper.CreateUserBranch(userBranchModel, User.GetUserId());
                result = await UserBranchService.InsertAsync(userBranch);
            }
            if (string.IsNullOrEmpty(unMatchedBranchID) && !isBrancExists.IsBranchesExists)
            {
                var userBranch = ConfigurationHelper.CreateUserBranch(userBranchModel, User.GetUserId());
                result = await UserBranchService.InsertAsync(userBranch);

            }

            string redirectUrl = string.Format("/Users/UsersSummary");
            return result ? RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateSuccess, url = redirectUrl })
                          : RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateFailed });

        }

        [HttpPost]
        public async Task<IActionResult> EditBranches(UserBranchModel userBranchModel)
        {
            var result = false;
            var isBrancExists = await UserBranchService.IsBrancExistsAsync(Convert.ToInt32(userBranchModel.Users.UserID), userBranchModel.EditBranchIDS);
            var insertRecord = ConfigurationHelper.FindUnamatchedBranch(userBranchModel.BranchIDS, userBranchModel.EditBranchIDS);
            var deleteRecord = ConfigurationHelper.FindUnamatchedBranch(userBranchModel.EditBranchIDS, userBranchModel.BranchIDS);
            result = await Branches(userBranchModel, result, isBrancExists, insertRecord, deleteRecord);

            string redirectUrl = string.Format("/Users/UsersSummary");
            return result ? RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateSuccess, url = redirectUrl })
                         : RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateFailed });

        }

        private async Task<bool> Branches(UserBranchModel userBranchModel, bool result, Triton.Service.Model.TritonGroup.StoredProcs.proc_UserBranches_IsBranchesExists isBrancExists, string insertRecord, string deleteRecord)
        {
            var model = await UserBranchService.GetByIdAsync(Convert.ToInt32(userBranchModel.Users.UserID));
            userBranchModel.UserBranch = model.UserBranch;
            if (isBrancExists.IsBranchesExists)
            {
                userBranchModel.BranchIDS = isBrancExists.BranchIds;
                var updateBranch = ConfigurationHelper.EditUserBranch(userBranchModel, User.GetUserId());
                result = await UserBranchService.UpdateAsync(updateBranch);
            }
            if (!string.IsNullOrEmpty(deleteRecord))
            {
                userBranchModel.BranchIDS = deleteRecord;
                var userBranch = ConfigurationHelper.DeleteUserBranch(userBranchModel, User.GetUserId());
                result = await UserBranchService.UpdateAsync(userBranch);
            }
            if (!string.IsNullOrEmpty(insertRecord))
            {
                userBranchModel.BranchIDS = insertRecord;
                var userBranch = ConfigurationHelper.CreateUserBranch(userBranchModel, User.GetUserId());
                result = await UserBranchService.InsertAsync(userBranch);

            }
            if (!isBrancExists.IsBranchesExists && !string.IsNullOrEmpty(userBranchModel.EditBranchIDS))
            {
                userBranchModel.BranchIDS = userBranchModel.EditBranchIDS;
                var userBranch = ConfigurationHelper.CreateUserBranch(userBranchModel, User.GetUserId());
                result = await UserBranchService.InsertAsync(userBranch);
            }

            return result;
        }

        public async Task<IActionResult> Configurations(int userId)
        {
            var configurations = new ConfigurationsViewModel
            {
                UserRoleGroupModel = await UserRoleGroupService.GetByIdAsync(userId, _systemId),
                UserBranchModel = await UserBranchService.GetByIdAsync(userId)

            };

            return View(configurations);
        }

        [HttpPost]
        public async Task<IActionResult> Configurations(ConfigurationsViewModel configurationsViewModel)
        {
            var result = false;
            var IsUserNameExists = await UserRoleGroupService.IsUserNameExistsAsync(Convert.ToInt32(configurationsViewModel.UserRoleGroupModel.Users.UserID), configurationsViewModel.UserRoleGroupModel.SelectedRoleGroupId);

            var isBrancExists = await UserBranchService.IsBrancExistsAsync(Convert.ToInt32(configurationsViewModel.UserBranchModel.Users.UserID), configurationsViewModel.UserBranchModel.EditBranchIDS);
            var insertRecord = ConfigurationHelper.FindUnamatchedBranch(configurationsViewModel.UserBranchModel.BranchIDS, configurationsViewModel.UserBranchModel.EditBranchIDS);
            var deleteRecord = ConfigurationHelper.FindUnamatchedBranch(configurationsViewModel.UserBranchModel.EditBranchIDS, configurationsViewModel.UserBranchModel.BranchIDS);

            result = await UserRoleBranch(configurationsViewModel, result, IsUserNameExists);
            result = await Branches(configurationsViewModel.UserBranchModel, result, isBrancExists, insertRecord, deleteRecord);


            string redirectUrl = string.Format("/Users/UsersSummary");
            return result ? RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateSuccess, url = redirectUrl })
                          : RedirectToAction("Message", "Home", new { StringHelper.Types.UpdateFailed });

        }

        private async Task<bool> UserRoleBranch(ConfigurationsViewModel configurationsViewModel, bool result, bool IsUserNameExists)
        {
            if (IsUserNameExists)
            {
                var updateRoleGroup = await UserRoleGroupService.GetByIdAsync(Convert.ToInt32(configurationsViewModel.UserRoleGroupModel.Users.UserID), _systemId);
                updateRoleGroup.UserRoleGroup.RoleGroupID = configurationsViewModel.UserRoleGroupModel.SelectedRoleGroupId;
                result = await UserRoleGroupService.UpdateAsync(updateRoleGroup.UserRoleGroup);
            }
            else
            {
                var userRoleGroup = ConfigurationHelper.CreateUserRoleGroup(configurationsViewModel.UserRoleGroupModel, User.GetUserId());
                result = await UserRoleGroupService.InsertAsync(userRoleGroup);
            }

            return result;
        }
    }
}

