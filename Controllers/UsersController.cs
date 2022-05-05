using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Triton.Service.Data;

namespace Triton.Operations.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private const int _systemId = 5;
        public async Task<IActionResult> UsersSummary()
        {
            var users = await UserService.GetAllBySystemIdAsync(_systemId);
            return View(users);
        }

    }
}
