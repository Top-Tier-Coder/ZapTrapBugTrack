using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<BTUser> _userManager;

        public HomeController(ILogger<HomeController>
            logger,
            IBTRoleService roleService,
            ApplicationDbContext context, UserManager<BTUser> userManager)
        {
            _logger = logger;
            _roleService = roleService;
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard(string filter)
        {
            BTUser btUser = await _userManager.GetUserAsync(User);

            List<Ticket> tickets = await _context.Projects
                                                 .Include(t => t.Tickets)
                                                    .ThenInclude(t=>t.TicketPriority)
                                                 .Include(t => t.Tickets)
                                                    .ThenInclude(t => t.TicketStatus)
                                                 .Include(t => t.Tickets)
                                                    .ThenInclude(t => t.TicketType)
                                                 .Include(t => t.Tickets)
                                                    .ThenInclude(p => p.Project)
                                                        .ThenInclude(m => m.Members)
                                                  .Include(t => t.Tickets)
                                                    .ThenInclude(p => p.Project)
                                                        .ThenInclude(m => m.Company)
                                                  .Include(t => t.Tickets)
                                                    .ThenInclude(o => o.OwnerUser)
                                                  .Include(t => t.Tickets)
                                                    .ThenInclude(d => d.DeveloperUser)
                                                 .Where(p => p.CompanyId == btUser.CompanyId)
                                                 .SelectMany(p => p.Tickets).ToListAsync();


            int Critical = tickets.Where(t => t.TicketPriority.Name == "Critical").Count();
            int Urgent = tickets.Where(t => t.TicketPriority.Name == "Urgent").Count();
            int High = tickets.Where(t => t.TicketPriority.Name == "High").Count();
            int Medium = tickets.Where(t => t.TicketPriority.Name == "Medium").Count();
            int Low = tickets.Where(t => t.TicketPriority.Name == "Low").Count();
            int Hold = tickets.Where(t => t.TicketPriority.Name == "Hold").Count();

            List<Ticket> listCritical = tickets.Where(t => t.TicketPriority.Name == "Critical").ToList();
            List<Ticket> listUrgent = tickets.Where(t => t.TicketPriority.Name == "Urgent").ToList();
            List<Ticket> listHigh = tickets.Where(t => t.TicketPriority.Name == "High").ToList();
            List<Ticket> listMedium = tickets.Where(t => t.TicketPriority.Name == "Medium").ToList();
            List<Ticket> listLow = tickets.Where(t => t.TicketPriority.Name == "Low").ToList();
            List<Ticket> listHold = tickets.Where(t => t.TicketPriority.Name == "Hold").ToList();


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



            DashboardViewModel model = new DashboardViewModel();


            //var allTickets = _context.Tickets
            //    .Include(t => t.TicketStatus)
            //    .Include(t => t.TicketPriority)
            //    .ToList();

            List<Project> projects = tickets.Select(t => t.Project).ToList();


            model.Tickets = tickets;
            model.Projects = projects;

            return View(model);
        }

        //public IActionResult Dashboard()
        //{
        //    DashboardViewModel model = new DashboardViewModel();


        //    var tickets = _context.Tickets
        //        .Include(t => t.TicketStatus)
        //        .Include(t => t.TicketPriority)
        //        .ToList();

        //    var projects = _context.Projects
        //        .Include(p => p.Company)
        //        .Include(p => p.Members)
        //        .ToList();


        //    model.Tickets = tickets;
        //    model.Projects = projects;

        //    return View(model);

        //}

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



