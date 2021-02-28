using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ZapTrapBugTrack.Data;
using ZapTrapBugTrack.Models;
using ZapTrapBugTrack.Services;

namespace ZapTrapBugTrack.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBTRoleService _roleService;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, IBTRoleService roleService, ApplicationDbContext context)
        {
            _logger = logger;
            _roleService = roleService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ManageRoles()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(List<string> userIds, string roleName)
        {  
            foreach(var userId in userIds)
            {
                BTUser user = await _context.Users.FindAsync(userId);
                if(!await _roleService.IsUserInRoleAsync(user, roleName))
                {
                    var userRoles = await _roleService.ListUserRolesAsync(user);
                    foreach(var role in userRoles)
                    {
                        await _roleService.RemoveUserFromRoleAsync(user, role);
                    }
                        await _roleService.AddUserToRoleAsync(user, roleName);
                }
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



