using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Triton.Service.Data;
using Triton.Service.Utils;

namespace Triton.Operations.Utils
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly IConfiguration _configuration;
        private const int _systemId = 5;
        public ClaimsTransformer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var user = await UserService.FindBysAmAccountName(principal.GetUserName().Replace("TRITONEXPRESS\\", ""), _systemId);
            //var user = await UserService.FindBysAmAccountName("BalanC", _systemId); // BalanC AshaltarS Ashnee ShaumilanM WaheedK

            if (user == null) return principal;

            var ci = (ClaimsIdentity)principal.Identity;
            ci.AddClaim(new Claim("UserID", user.UserID.ToString()));
            ci.AddClaim(new Claim("Name", $"{user.FirstName} {user.LastName}"));
            ci.AddClaim(new Claim("EmployeeID", user.EmployeeID.ToString()));
            ci.AddClaim(new Claim("CostCentreID", user.CostCentreID.ToString()));
            ci.AddClaim(new Claim("Roles", user.RoleIds));
            ci.AddClaim(new Claim("sAMAccountName", user.sAMAccountName));
            ci.AddClaim(new Claim("RoleNames", user.RoleNames.ToString()));
            ci.AddClaim(new Claim("Email", user.Email));
            ci.AddClaim(new Claim("JobProfile", user.JobProfile));
            ci.AddClaim(new Claim("Employee", user.Employee));

            var roleSplit = user.RoleNames.Split(",");

            foreach (var item in roleSplit)
            {
                var c = new Claim(ci.RoleClaimType, item);
                ci.AddClaim(c);
            }

            return principal;
        }
    }
}
