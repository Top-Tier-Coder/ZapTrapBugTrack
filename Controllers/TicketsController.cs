using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZapTrapBugTrack.Data;
using ZapTrapBugTrack.Data.Enums;
using ZapTrapBugTrack.Services;




namespace ZapTrapBugTrack.Models
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTHistoryService _historyService;
        private readonly IBTProjectService _projectService;
        private readonly SignInManager<BTUser> _signInManager;
        private readonly IBTRoleService _roleService;

        public TicketsController(ApplicationDbContext context, 
            UserManager<BTUser> userManager, 
            IBTHistoryService historyService, 
            IBTProjectService projectService, 
            SignInManager<BTUser> signInManager,
            IBTRoleService roleService)
            
        {
            _context = context;
            _userManager = userManager;
            _historyService = historyService;
            _projectService = projectService;
            _signInManager = signInManager;
            _roleService = roleService;
        }

        public async Task<IActionResult> AcceptInvite(string userId, string code)
        {
            var realGuid = Guid.Parse(code);
            var invite = _context.Invites.FirstOrDefault(i => i.CompanyToken == realGuid && i.InviteeId == userId);
            if (invite is null)
            {
                return NotFound();
            }
            if (invite.IsValid)
            {
                invite.IsValid = false;
                var user = await _context.Users.FindAsync(userId);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Create");
            }
            return NotFound();
        }
        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType);
            return View(await applicationDbContext.ToListAsync());

        }

        public async Task<IActionResult> GoToTicket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync((int)id);
            
            if (notification == null)
            {
                return NotFound();
            }

            notification.Viewed = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = notification.TicketId });


        }

        
        //GET: TICKETS

        public async Task<IActionResult> MyTickets()
        {

            var model = new List<Ticket>();
            //test if admin
            if (User.IsInRole("Admin")) 
            {
                model =  await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType).ToListAsync();

            }  //test if project manager
            else if (User.IsInRole("ProjectManager")) 
            {
                var userId = _userManager.GetUserId(User);
                var projects = await _projectService.ListUserProjectsAsync(userId);


                model = projects.SelectMany(p => p.Tickets).ToList();

            } //test if dev
            else if (User.IsInRole("Developer")) 
            {
                var userId = _userManager.GetUserId(User);
                model = _context.Tickets.Where(t => t.DeveloperUserId == userId).ToList();

            }//test if owner
            else 
            {
                var userId = _userManager.GetUserId(User);
                model = _context.Tickets.Where(t => t.OwnerUserId == userId).ToList();

            }

            return View(model);

        }


        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Comments)
                .ThenInclude(t => t.User)
                .Include(t => t.Histories)
                .Include(t => t.Attachments)
                

                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [Authorize(Roles = "Admin")]
        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["DeveloperUserId"] = new SelectList(_context.Set<BTUser>(), "Id", "FullName");
            ViewData["OwnerUserId"] = new SelectList(_context.Set<BTUser>(), "Id", "FullName");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,DeveloperUserId")] Ticket ticket)
        {
            //If logged in as demo-user, no database alteration permitted. Return warning view

            if (!(await _roleService.IsUserInRoleAsync(await _userManager.GetUserAsync(User), Roles.DemoUser.ToString())))
            {

                if (ModelState.IsValid)
                {
                    ticket.Created = DateTime.Now;
                    ticket.OwnerUserId = _userManager.GetUserId(User);

                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                var currentStatus = _context.TicketStatuses.FirstOrDefault(t => t.Name == "Closed").Id;


                if (ticket.DeveloperUserId != null && ticket.TicketStatusId != currentStatus)
                {
                    Notification notification = new Notification
                    {
                        TicketId = ticket.Id,
                        Description = "You have a new ticket.",
                        Created = DateTime.Now,
                        SenderId = ticket.OwnerUserId,
                        RecipientId = ticket.DeveloperUserId,
                    };
                    await _context.Notifications.AddAsync(notification);
                    await _context.SaveChangesAsync();
                }


                ViewData["DeveloperUserId"] = new SelectList(_context.Set<BTUser>(), "Id", "Id", ticket.DeveloperUserId);
                ViewData["OwnerUserId"] = new SelectList(_context.Set<BTUser>(), "Id", "Id", ticket.OwnerUserId);
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
                ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
                ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticket.TicketStatusId);
                ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticket.TicketTypeId);
                return View(ticket);
            }

            return RedirectToAction("DemoUser", "Projects");

        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Set<BTUser>(), "Id", "FullName", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Set<BTUser>(), "Id", "FullName", ticket.OwnerUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {
           

            if (id != ticket.Id)
            {
                return NotFound();
            }

            //Get Old Ticket
            Ticket oldTicket = await _context.Tickets
                .Include(t => t.TicketType)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.Project)
                .Include(t => t.DeveloperUser)
                .AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

            if (ModelState.IsValid)
            {
                try
                {
                    ticket.Updated = DateTime.Now;

                    _context.Update(ticket);
                    await _context.SaveChangesAsync();

                    var currentStatus = _context.TicketStatuses.FirstOrDefault(t => t.Name == "Closed").Id;


                    if (ticket.DeveloperUserId != null && ticket.TicketStatusId != currentStatus)
                    {
                        Notification notification = new Notification
                        {
                            TicketId = ticket.Id,
                            Description = "One of your tickets has been modified!",
                            Created = DateTime.Now,
                            SenderId = _userManager.GetUserId(User),
                            RecipientId = ticket.DeveloperUserId,
                        };

                        await _context.Notifications.AddAsync(notification);
                        await _context.SaveChangesAsync();
                    }


                    //Add History 
                    //GetUserId
                    string userId = _userManager.GetUserId(User);

                    //Get New Ticket
                    Ticket newTicket = await _context.Tickets
                        .Include(t => t.TicketType)
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.Project)
                        .Include(t => t.DeveloperUser)
                        .AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

                    //Call History Service
                    await _historyService.AddHistoryAsync(oldTicket, newTicket, userId);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Set<BTUser>(), "Id", "FullName", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Set<BTUser>(), "Id", "FullName", ticket.OwnerUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}
