using System;
using Triton.Operations.Models;
using Triton.Service.Model.TritonGroup.Tables;

namespace Triton.Operations.Helper
{
    public class RoleGroupHelper
    {
        private const int _systemId = 5;
        public static  RoleGroup CreateRoleGroup(RoleGroupViewModel roleGroupViewModel, int userId)
        {
            return new RoleGroup
            {
                CreatedBy = userId,
                CreatedOn = DateTime.Now,
                DeletedBy = null,
                Description = roleGroupViewModel.RoleGroup.Description,
                RoleName = roleGroupViewModel.RoleGroup.RoleName,
                SystemID = _systemId,
                DeletedOn = null,
                RoleGroupID = roleGroupViewModel.RoleGroup.RoleGroupID,
                IsActive = true
            };
        }
    }
}
