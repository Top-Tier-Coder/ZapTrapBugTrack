using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public HomeController(ILogger<HomeController> 
            logger,
            IBTRoleService roleService,
            ApplicationDbContext context)
        {
            _logger = logger;
            _roleService = roleService;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard(string filter)
        {
            var Critical = _context.Tickets.Where(t => t.TicketPriorityId == (_context.TicketPriorities.FirstOrDefault(t => t.Name == "Critical").Id)).ToList().Count;
            var Urgent = _context.Tickets.Where(t => t.TicketPriorityId == (_context.TicketPriorities.FirstOrDefault(t => t.Name == "Urgent").Id)).ToList().Count;
            var High = _context.Tickets.Where(t => t.TicketPriorityId == (_context.TicketPriorities.FirstOrDefault(t => t.Name == "High").Id)).ToList().Count;
            var Medium = _context.Tickets.Where(t => t.TicketPriorityId == (_context.TicketPriorities.FirstOrDefault(t => t.Name == "Medium").Id)).ToList().Count;
            var Low = _context.Tickets.Where(t => t.TicketPriorityId == (_context.TicketPriorities.FirstOrDefault(t => t.Name == "Low").Id)).ToList().Count;
            var Hold = _context.Tickets.Where(t => t.TicketPriorityId == (_context.TicketPriorities.FirstOrDefault(t => t.Name == "Hold").Id)).ToList().Count;

            var listCritical = _context.Tickets.Include(p => p.Project)
                .Include(y => y.TicketType)
                .Include(s => s.TicketStatus)
                .Include(o => o.OwnerUser)
                .Include(d => d.DeveloperUser)
                .Where(t => t.TicketPriorityId == (_context.TicketPriorities.FirstOrDefault(t => t.Name == "Critical").Id)).ToList();
            var listUrgent = _context.Tickets
                .Include(p => p.Project)
                .Include(y => y.TicketType)
                .Include(s => s.TicketStatus)
                .Include(o => o.OwnerUser)
                .Include(d => d.DeveloperUser)
                .Where(t => t.TicketPriorityId == (_context.TicketPriorities.FirstOrDefault(t => t.Name == "Urgent").Id)).ToList();
            var listHigh = _context.Tickets
                .Include(p => p.Project)
                .Include(y => y.TicketType)
                .Include(s => s.TicketStatus)
                .Include(o => o.OwnerUser)
                .Include(d => d.DeveloperUser)
                .Where(t => t.TicketPriorityId == (_context.TicketPriorities.FirstOrDefault(t => t.Name == "High").Id)).ToList();
            var listMedium = _context.Tickets
                .Include(p => p.Project)
                .Include(y => y.TicketType)
                .Include(s => s.TicketStatus)
                .Include(o => o.OwnerUser)
                .Include(d => d.DeveloperUser)
                .Where(t => t.TicketPriorityId == (_context.TicketPriorities.FirstOrDefault(t => t.Name == "Medium").Id)).ToList();
            var listLow = _context.Tickets
                .Include(p => p.Project)
                .Include(y => y.TicketType)
                .Include(s => s.TicketStatus)
                .Include(o => o.OwnerUser)
                .Include(d => d.DeveloperUser)
                .Where(t => t.TicketPriorityId == (_context.TicketPriorities.FirstOrDefault(t => t.Name == "Low").Id)).ToList();
            var listHold = _context.Tickets
                .Include(p => p.Project)
                .Include(y => y.TicketType)
                .Include(s => s.TicketStatus)
                .Include(o => o.OwnerUser)
                .Include(d => d.DeveloperUser)
                .Where(t => t.TicketPriorityId == (_context.TicketPriorities.FirstOrDefault(t => t.Name == "Hold").Id)).ToList();


            ViewData["Critical"] = Critical;
            ViewData["Urgent"] = Urgent;
            ViewData["High"] = High;
            ViewData["Medium"] = Medium;
            ViewData["Low"] = Low;
            ViewData["Hold"] = Hold;

            ViewData["ListCritical"] = listCritical;
            ViewData["ListUrgent"] = listUrgent;
            ViewData["ListHigh"] = listHigh;
            ViewData["ListMedium"] = listMedium;
            ViewData["ListLow"] = listLow;
            ViewData["ListHold"] = listHold;



            return View();
        }

        public IActionResult Dashboard()
        {
            DashboardViewModel model = new DashboardViewModel();


            var tickets = _context.Tickets
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .ToList();

            var projects = _context.Projects
                .Include(p => p.Company)
                .Include(p => p.Members)
                .ToList();


            model.Tickets = tickets;
            model.Projects = projects;

            return View(model);

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



