using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using X.PagedList;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "ProjectManager, Developer")]
    public class ProjectsController : Controller
    {
        private readonly ProjectBusinessLogic projectBL;
        private UserBusinessLogic userBL;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            userBL = new UserBusinessLogic(userManager);
            projectBL = new ProjectBusinessLogic(new ProjectRepository(context), new TicketRepository(context), userManager);
        }

        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index(string? sortOrder, int? page, bool? sort, string? userId)
        {
            List<Project> SortedProjs = new List<Project>();
            List<ApplicationUser> allUsers = await userBL.GetUsersByRole("Developer");
            List<SelectListItem> users = new List<SelectListItem>();

            allUsers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });
            ViewBag.Users = users;
            switch (sortOrder)
            {
                case "Priority":
                    if (sort == true)
                    {
                        SortedProjs = projectBL.OrderByPriority();
                    }
                    else
                    {
                        SortedProjs = projectBL.OrderByPriorityAsc();
                    }

                    break;
                case "RequiredHrs":
                    if (sort == true)
                    {
                        SortedProjs = projectBL.OrderByRequiredHours();
                    }
                    else
                    {
                        SortedProjs = projectBL.OrderByRequiredHoursAsc();
                    }

                    break;
                case "Completed":
                    SortedProjs = projectBL.GetCompletedProjects();
                    break;
                default:
                    if (userId != null)
                    {
                        SortedProjs = projectBL.GetAllProjects();
                    }
                    else
                    {
                        SortedProjs = projectBL.GetAllProjects();
                    }
                    break;
            }

            var LogedUserName = User.Identity.Name;  // logined user name
            var user = userBL.GetUserByName(LogedUserName);
            var rolenames = userBL.ToString();
            var AssinedProject = new List<Project>();

            if (rolenames.Contains("Developer"))
            {
                AssinedProject = SortedProjs;
            }
            else
            {
                AssinedProject = SortedProjs;
            }
            IPagedList<Project> projList = AssinedProject.ToPagedList(page ?? 1, 3);

            return View(projList);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || projectBL == null)
            {
                return NotFound();
            }

            var project = projectBL.GetProjectById((int)id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> CreateAsync()
        {
            List<ApplicationUser> allUsers = await userBL.GetUsersByRole("Developer");

            List<SelectListItem> users = new List<SelectListItem>();
            allUsers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });
            ViewBag.Users = users;

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id, ProjectName")] Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                projectBL.CreateProject(project);

                return View(project);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project? project = projectBL.GetProjectById((int)id);

            if (project == null)
            {
                return NotFound();
            }

            List<ApplicationUser> results = await userBL.GetUsersByRole("Developer");

            List<SelectListItem> currUsers = new List<SelectListItem>();
            results.ForEach(r =>
            {
                currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
            });
            ViewBag.Users = currUsers;

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id, List<string> usersId, [Bind("Id,ProjectName")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    projectBL.EditProject(project.Id, project.ProjectName);
                    await projectBL.AssignUsers(project.Id, usersId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!projectBL.Exists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Edit), new { id = id });
            }

            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project? project = projectBL.GetProjectById((int)id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            projectBL.DeleteProject(id);

            return RedirectToAction(nameof(Index));
        }
    }
}