using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class UserBusinessLogic
    {
        public UserManager<ApplicationUser> _userManager;

        public UserBusinessLogic(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser GetUser(string userId)
        {
            try
            {
                ApplicationUser currUser = _userManager.Users
                    .Include(user => user.Tickets)
                    .Include(user => user.Projects)
                    .First(user => user.Id == userId);
                return currUser;
            }
            catch
            {
                throw new NullReferenceException("User not found");
            }
        }

        public async Task<ApplicationUser> GetUserByName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public List<ApplicationUser> GetAllUsers()
        {
            return _userManager.Users.ToList();
        }

        public async Task<List<ApplicationUser>> GetUsersWithSpecificRole(string role)
        {
            if (role == "Developer" || role == "ProjectManager")
            {
                return (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync(role);
            }
            else
            {
                throw new ArgumentException("No users found with this role");
            }
        }

        public async Task<List<ApplicationUser>> GetAllUsersWithoutRole()
        {
            List<ApplicationUser> usersWithoutRole = new List<ApplicationUser>();
            List<ApplicationUser> users = _userManager.Users.ToList();

            foreach (ApplicationUser user in users)
            {
                List<string> userRoles = (List<string>)await _userManager.GetRolesAsync(user);

                if (userRoles.Count == 0)
                {
                    usersWithoutRole.Add(user);
                }
            }

            return usersWithoutRole;
        }

        public async Task<List<ApplicationUser>> GetUsersByRole(string role)
        {
            IList<ApplicationUser> users = await _userManager.GetUsersInRoleAsync(role);

            return users.ToList();
        }

        public async Task AssignUserToARole(ApplicationUser user, string role)
        {
            if (role == "Developer" || role == "ProjectManager")
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                throw new ArgumentException("Invalid Role");
            }

        }

        public async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            List<string> userRoles = (List<string>)await _userManager.GetRolesAsync(user);
            return userRoles;

        }

        public Task GetUserRoles(string invalidRole)
        {
            throw new NotImplementedException();
        }
    }
}