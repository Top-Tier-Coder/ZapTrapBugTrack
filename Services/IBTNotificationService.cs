using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ZapTrapBugTrack.Models;

namespace ZapTrapBugTrack.Services
{
    public interface IBTNotificationService
    {
        //When we talk about a "user" there are two versions of a user
        //A record in the Users table - represents any person who is registered
        //The other is capital U User - this is the currently logged in user ClaimsPrinciple User

        public Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(ClaimsPrincipal currentUser);
    }
}
