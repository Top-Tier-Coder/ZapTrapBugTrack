using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZapTrapBugTrack.Models
{
    public class DashboardViewModel
    {
        public ICollection<Project> Projects { get; set; } = new List<Project>();

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        //public int PercentStatusNew { get; set; }

        //public int PercentStatusOpen { get; set; }

        //public int PercentStatusDev { get; set; }

        //public int PercentStatusClosed { get; set; }
    }
}
