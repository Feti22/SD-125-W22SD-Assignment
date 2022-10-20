using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserBusinessLogic userBL;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            userBL = new UserBusinessLogic(userManager);
        }

        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> projectManager = await userBL.GetUsersByRole("ProjectManager");
            List<ApplicationUser> developers = await userBL.GetUsersByRole("Developer");
            List<ApplicationUser> allUsers = userBL.GetAllUsers();

            ProjectManagersAndDevelopersViewModels vm = new()
            {
                pms = projectManager,
                devs = developers,
                allUsers = allUsers
            };

            return View(vm);
        }

        public async Task<IActionResult> UnassignedDevelopers()
        {
            List<ApplicationUser> userWithoutRoles = await userBL.GetAllUsersWithoutRole();
            return View(userWithoutRoles);
        }

        [HttpPost]
        public async Task<IActionResult> AssignDeveloper(string userId)
        {
            ApplicationUser? user = userBL.GetUser(userId);

            if (user == null)
            {
                return BadRequest();
            }

            await userBL.AssignUserToARole(user, "Developer");

            return RedirectToAction(nameof(UnassignedDevelopers));
        }        
    }
}

