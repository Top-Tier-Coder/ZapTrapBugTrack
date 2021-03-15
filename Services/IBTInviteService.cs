using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZapTrapBugTrack.Models.ViewModel;

namespace ZapTrapBugTrack.Services
{
    public interface IBTInviteService
    {
        public Task<string> InviteWizardAsync(InviteViewModel inveiteViewModel);
    }
}
