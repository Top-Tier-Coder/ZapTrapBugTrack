using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ZapTrapBugTrack.Data;
using ZapTrapBugTrack.Models;

namespace ZapTrapBugTrack.Services
{
    public class BTNotificationService : IBTNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;

        //I need access to the database to get the notifications 
        //I need the userManager to convert the ClaimsPrincipal to BTUser


        public BTNotificationService(
            ApplicationDbContext context,
            UserManager<BTUser> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(ClaimsPrincipal currentUser)
        {
            //Convert the GetPrincipal into a BTUser - database doesn't know ClaimsPrincipal
            BTUser user = await  _userManager.GetUserAsync(currentUser);

            var userNotifications = _context.Notifications.Where(n => n.RecipientId == user.Id)
                .Include(n => n.Sender);
            var unreadNotifications = await userNotifications.Where(n => !n.Viewed).ToListAsync();
            //var unreadNotifications = _context.Notifications.Where(n => n.RecipientId == user.Id && !n.Viewed).ToList();
            //var unreadNotifications = _context.Notifications.Where(n => n.RecipientId == user.Id && !n.Viewed).ToList();


            return unreadNotifications;
        }
    }
}
