using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class UserBusinessLogic
    {
        public UserManager<ApplicationUser> _userManager;

        public UserBusinessLogic(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ProjectManagersAndDevelopersViewModels> GetIndexAsync()
        {
            ProjectManagersAndDevelopersViewModels viewModel = new ProjectManagersAndDevelopersViewModels();

            List<ApplicationUser> pmUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("ProjectManager");
            List<ApplicationUser> devUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Developer");
            List<ApplicationUser> allUsers = await _userManager.Users.ToListAsync();

            viewModel.pms = pmUsers;
            viewModel.devs = devUsers;
            viewModel.allUsers = allUsers;

            return viewModel;
        }

        public async Task<ApplicationUser> GetUser(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
        public async Task<ApplicationUser> GetUserByName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<object[]> GetAllUsers()
        {
            List<ApplicationUser> allUsers = await _userManager.Users.ToListAsync();

            List<SelectListItem> users = new List<SelectListItem>();

            allUsers.ForEach(u =>
            {
                users.Add(new SelectListItem(u.UserName, u.Id.ToString()));
            });

            return new object[]
            {
                allUsers, users
            };
        }

        public List<ApplicationUser> GetAllUsers(string id)
        {
            return _userManager.Users.Where(u=> u.Id != id).ToList();
        }

        public async Task ReassignRole(string role, string userId)
        {
            ApplicationUser user = _userManager.Users.First(u => u.Id == userId);
            ICollection<string> roleUser = await _userManager.GetRolesAsync(user);

            if (roleUser.Count == 0)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, roleUser.First());
                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }
}