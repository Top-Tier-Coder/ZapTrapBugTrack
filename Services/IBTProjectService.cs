using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZapTrapBugTrack.Models;

namespace ZapTrapBugTrack.Services
{
    public interface IBTProjectService
    {
        //Is the User on a project
        public Task<bool> IsUserOnProjectAsync(string userId, int projectId);

        //All users on the project
        public Task<IEnumerable<BTUser>> UsersOnProjectAsync(int projectId);

        //All users not on the project
        public Task<IEnumerable<BTUser>> UsersNotOnProjectAsync(int projectId);

        //Assign/add users to project
        public Task AddUserToProject(string userId, int projectId);

        //Remove from project
        public Task RemoveUserFromProject(string userId, int projectId);

        //All projects for one user
        public Task<IEnumerable<Project>> ListUserProjectsAsync(string userId);

        //Developers on Projects
        public Task<IEnumerable<BTUser>> DevelopersOnProjectAsync(int projectId);

        //Submitters on Projects
        public Task<IEnumerable<BTUser>> SubmittersOnProjectAsync(int projectId);

        //Project Manager on Project
        public Task<BTUser> ProjectManagerOnProjectAsync(int projectId);

    }
}
