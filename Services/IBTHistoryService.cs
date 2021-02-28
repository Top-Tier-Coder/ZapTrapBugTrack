using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZapTrapBugTrack.Models;

namespace ZapTrapBugTrack.Services
{
    public interface IBTHistoryService
    {

        public Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId);

    }
}
