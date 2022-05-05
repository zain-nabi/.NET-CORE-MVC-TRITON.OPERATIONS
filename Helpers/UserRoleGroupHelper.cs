using System;
using Triton.Service.Model.TritonGroup.Custom;
using Triton.Service.Model.TritonGroup.Tables;

namespace Triton.Operations.Helper
{
    public class UserRoleGroupHelper
    {
        public static UserRoleGroup CreateUserRoleGroup(UserRoleGroupModel userRoleGroupModel , int userId)
        {
            return new UserRoleGroup
            {
              CreatedBy = userRoleGroupModel.UserRoleGroup.CreatedBy == 0 ? userId : userRoleGroupModel.UserRoleGroup.CreatedBy,
              DeletedBy = null,
              CreatedOn = userRoleGroupModel.UserRoleGroup.CreatedOn == DateTime.MinValue ? DateTime.Now : userRoleGroupModel.UserRoleGroup.CreatedOn,
              DeletedOn = null,
              RoleGroupID = userRoleGroupModel.SelectedRoleGroupId,
              UserID = userRoleGroupModel.UserRoleGroup.UserID,
              UserRoleGroupID = userRoleGroupModel.UserRoleGroup.UserRoleGroupID
            };
        }
    }
}
