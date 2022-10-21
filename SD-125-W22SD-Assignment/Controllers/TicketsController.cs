using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private TicketBusinessLogic ticketBL;
        private UserBusinessLogic userBL;
        private ProjectBusinessLogic projectBL;
        private CommentBusinessLogic commentBL;

        public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            userBL = new UserBusinessLogic(userManager);
            projectBL = new ProjectBusinessLogic(new ProjectRepository(context), new TicketRepository(context), userManager);
            ticketBL = new TicketBusinessLogic(userManager, new ProjectRepository(context), new TicketRepository(context), new CommentRepository(context));
            commentBL = new CommentBusinessLogic(new CommentRepository(context));
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            return View(ticketBL.GetAllTickets().ToList());
        }

        // GET: Tickets/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket? ticket = ticketBL.GetTicketById((int)id);

            if (ticket == null)
            {
                return NotFound();
            }

            List<SelectListItem> currUsers = new List<SelectListItem>();
            ticket.Project?.AssignedTo.ToList().ForEach(t =>
            {
                currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
            });
            ViewBag.Users = currUsers;

            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "ProjectManager")]
        public IActionResult Create(int projId)
        {
            Project? currProject = projectBL.GetProjectById(projId);

            if (currProject == null)
            {
                return NotFound();
            }

            List<SelectListItem> currUsers = new List<SelectListItem>();

            currProject.AssignedTo.ToList().ForEach(t =>
            {
                currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
            });

            ViewBag.Projects = currProject;
            ViewBag.Users = currUsers;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,RequiredHours,TicketPriority")] Ticket ticket, int projId, string userId)
        {
            if (ModelState.IsValid)
            {
                await ticketBL.CreateTicket(ticket, projId, userId);

                return RedirectToAction("Index", "Projects", new { area = "" });
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket? ticket = ticketBL.GetTicketById((int)id);

            if (ticket == null)
            {
                return NotFound();
            }

            List<ApplicationUser> developers = await userBL.GetUsersByRole("Developer");
            List<ApplicationUser> results = developers.Where(u => u != ticket.Owner).ToList();

            List<SelectListItem> currUsers = new List<SelectListItem>();
            results.ForEach(r =>
            {
                currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
            });
            ViewBag.Users = currUsers;

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id, string userId, [Bind("Id,Title,Body,RequiredHours")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser currUser = userBL.GetUser(userId);
                    ticket.Owner = currUser;
                    await ticketBL.UpdateTicket(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Edit), new { id = ticket.Id });
            }

            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> CommentTask(int? TaskId, string? TaskText)
        {
            try
            {
                ApplicationUser user = await userBL.GetUserByName(User.Identity.Name);
                await ticketBL.AddCommentToTicket(user.Id, (int)TaskId, TaskText);
                int Id = (int)TaskId;

                return RedirectToAction("Details", new { Id });
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> UpdateHrs(int? id, int? hrs)
        {
            if (id != null || hrs != null)
            {
                try
                {
                    Ticket? ticket = ticketBL.GetTicketById((int)id);

                    if (ticket == null)
                    {
                        return NotFound();
                    }

                    ticket.RequiredHours = (int)hrs;
                    await ticketBL.UpdateTicket(ticket);

                    return RedirectToAction("Details", new { id });
                }
                catch (Exception)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddToWatchers(int id)
        {
            if (id != null)
            {
                try
                {
                    ApplicationUser user = await userBL.GetUserByName(User.Identity.Name);
                    await ticketBL.AddTicketToWatcher(user.Id, id);

                    return RedirectToAction("Details", new { id });
                }
                catch (Exception)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnWatch(int? id)
        {
            if (id != null)
            {
                try
                {
                    ApplicationUser user = await userBL.GetUserByName(User.Identity.Name);
                    await ticketBL.RemoveTicketFromWatcher(user.Id, (int)id);

                    return RedirectToAction("Details", new { id });
                }
                catch (Exception)
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MarkAsCompleted(int? id)
        {
            if (id != null)
            {
                try
                {
                    Ticket? ticket = ticketBL.GetTicketById((int)id);
                    ticket.Completed = true;
                    await ticketBL.UpdateTicket(ticket);

                    return RedirectToAction("Details", new { id });
                }
                catch (Exception)
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnMarkAsCompleted(int? id)
        {
            if (id != null)
            {
                try
                {
                    Ticket? ticket = ticketBL.GetTicketById((int)id);
                    ticket.Completed = false;
                    await ticketBL.UpdateTicket(ticket);

                    return RedirectToAction("Details", new { id });
                }
                catch (Exception)
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            return RedirectToAction("Index");
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket? ticket = ticketBL.GetTicketById((int)id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public IActionResult DeleteConfirmed(int id)
        {
            ticketBL.DeleteTicket(id);

            return RedirectToAction("Index", "Projects");
        }
    }
}