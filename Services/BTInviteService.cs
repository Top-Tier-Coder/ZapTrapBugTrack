using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZapTrapBugTrack.Data;
using ZapTrapBugTrack.Data.Enums;
using ZapTrapBugTrack.Models;
using ZapTrapBugTrack.Models.ViewModel;

namespace ZapTrapBugTrack.Services
{
    public class BTInviteService : IBTInviteService
    {
        private readonly IBTRoleService _roleService;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;
        private readonly ApplicationDbContext _context;

        public BTInviteService(ApplicationDbContext context, 
            UserManager<BTUser> userManager, 
            IBTProjectService projectService,
            IBTRoleService roleService)
        {
            _roleService = roleService;
            _projectService = projectService;
            _userManager = userManager;
            _context = context;
        }
        public async Task<string> InviteWizardAsync(InviteViewModel inviteViewModel)
        {
            //Create a New company - Database
            var companyId = await CreateCompanyAsync(inviteViewModel.CompanyName, inviteViewModel.CompanyDescription);
     
            //Create a New BTUser - UserManager
            var user = new BTUser
            {
                Email = inviteViewModel.Email,
                UserName = inviteViewModel.Email,
                FirstName = inviteViewModel.FirstName,
                LastName = inviteViewModel.LastName,
                EmailConfirmed = true,
                CompanyId = companyId
            };
            await _userManager.CreateAsync(user, "Xyz$098#");
            //Assign that user to a role - RoleService
            await _roleService.AddUserToRoleAsync(user, Roles.Admin.ToString());
            //Create a new project  - Database access
            var projectId = await CreateProjectAsync(inviteViewModel.ProjectName, inviteViewModel.ProjectDescription, companyId);
            //Assign that user to that project - ProjectService
            await _projectService.AddUserToProjectAsync(user.Id, projectId);

            return user.Id;
        }

        private async Task<int> CreateCompanyAsync(string companyName, string companyDescription)
        {
            Company company = new Company
            {
                Name = companyName,
                Description = companyDescription
            };
            if(!_context.Companies.Any(c => c.Name == company.Name))
            {
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

            }
            else
            {
                company = _context.Companies.FirstOrDefault(c => c.Name == company.Name);
            }

            return company.Id;
        }

        

        private async Task<int> CreateProjectAsync(string projectName, string projectDescription, int companyId)
        {
            var project = new Project
            {
                Name = projectName,
                Description = projectDescription,
                CompanyId = companyId

            };
            if (!_context.Projects.Any(c => c.Name == project.Name))
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

            }
            else
            {
                project = _context.Projects.FirstOrDefault(c => c.Name == project.Name);
            }

            return project.Id;
        }
    }
}
