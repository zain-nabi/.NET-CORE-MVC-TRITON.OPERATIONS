using System;
using System.Collections.Generic;
using System.Linq;
using Triton.Service.Model.TritonGroup.Custom;
using Triton.Service.Model.TritonGroup.Tables;

namespace Triton.Operations.Helper
{
    public class ConfigurationHelper
    {
        public static UserRoleGroup CreateUserRoleGroup(UserRoleGroupModel userRoleGroupModel, int userId)
        {
            return new UserRoleGroup
            {
                CreatedBy = userId,
                CreatedOn = DateTime.Now,
                DeletedBy = null,
                DeletedOn = null,
                RoleGroupID = userRoleGroupModel.SelectedRoleGroupId,
                UserID = Convert.ToInt32(userRoleGroupModel.Users.UserID)
            };
        }


        public static List<UserBranch> EditUserBranch(UserBranchModel userBranchModel, int userId)
        {
            var userBranches = new List<UserBranch>();
            var splitBranchIds = userBranchModel.BranchIDS.Split(',');

            
            foreach (var item in splitBranchIds)
            {
                var branch = userBranchModel.UserBranch.Where(x => x.BranchID == Convert.ToInt32(item)).FirstOrDefault();
                var userBranch = new UserBranch
                {
                    UserBranchID = branch.UserBranchID,
                    BranchID = Convert.ToInt32(item),
                    UserID = Convert.ToInt32(userBranchModel.Users.UserID),
                    CreatedBy = branch.CreatedBy,
                    DeletedBy = null,
                    CreatedOn = branch.CreatedOn,
                    DeletedOn = null
                };
                userBranches.Add(userBranch);
            }
            return userBranches;
        }

        public static List<UserBranch> CreateUserBranch(UserBranchModel userBranchModel, int userId)
        {
            var userBranches = new List<UserBranch>();
            var splitBranchIds = userBranchModel.BranchIDS.Split(',');
            foreach (var item in splitBranchIds)
            {
                var userBranch = new UserBranch
                {
                    BranchID = Convert.ToInt32(item),
                    UserID = Convert.ToInt32(userBranchModel.Users.UserID),
                    CreatedBy = userId,
                    DeletedBy = null,
                    CreatedOn = DateTime.Now,
                    DeletedOn = null
                };
                userBranches.Add(userBranch);
            }
            return userBranches;
        }

        public static List<UserBranch> DeleteUserBranch(UserBranchModel userBranchModel, int userId)
        {
            var userBranches = new List<UserBranch>();
            var splitBranchIds = userBranchModel.BranchIDS.Split(',');
            foreach (var item in splitBranchIds)
            {
                var branch = userBranchModel.UserBranch.Where(x => x.BranchID == Convert.ToInt32(item)).FirstOrDefault();
                var userBranch = new UserBranch
                {
                    UserBranchID = branch.UserBranchID,
                    BranchID = Convert.ToInt32(item),
                    UserID = Convert.ToInt32(userBranchModel.Users.UserID),
                    CreatedBy = branch.CreatedBy,
                    DeletedBy = userId,
                    CreatedOn = branch.CreatedOn,
                    DeletedOn = DateTime.Now
                };
                userBranches.Add(userBranch);
            }
            return userBranches;
        }

        public static string FindUnamatchedBranch(string alreadyExistBranches, string selectedBrached)
        {
            if (string.IsNullOrEmpty(alreadyExistBranches) || string.IsNullOrEmpty(selectedBrached))
            {
                return string.Empty;
            }
            var result = selectedBrached.Split(',').ToList().Except(alreadyExistBranches.Split(',').ToList());
            return string.Join(",", result);
        }
    }
}
