using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private TicketBusinessLogic ticketBL;

        private UserBusinessLogic userBL;
        private ProjectBusinessLogic projectBL;
        private CommentBusinessLogic commentBL;
        
        public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            
            ticketBL = new TicketBusinessLogic(new TicketRepository(context));
            userBL = new UserBusinessLogic(userManager);
            projectBL = new ProjectBusinessLogic(new ProjectRepository(context));
            commentBL = new CommentBusinessLogic(new CommentRepository(context));
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            //return _context.tickets != null ? 
            //            view() :
            //            problem("entity set 'applicationdbcontext.tickets'  is null.");
            return View(ticketBL.GetTickets().ToList());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            
            Ticket ticket =  ticketBL.GetTicket((int)id);

            if (ticket == null)
            {
                return NotFound();
            }

            List<SelectListItem> currUsers = new List<SelectListItem>();
            ticket.Project.AssignedTo.ToList().ForEach(t =>
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
            //Project currProject = _context.Projects.Include(p => p.AssignedTo).ThenInclude(at => at.ApplicationUser).FirstOrDefault(p => p.Id == projId);
            Project currProject = projectBL.GetProject(projId);

            List<SelectListItem> currUsers = new List<SelectListItem>();
            currProject.AssignedTo.ToList().ForEach(t =>
            {
                currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
            });

            ViewBag.Projects = currProject;
            ViewBag.Users = currUsers;

            return View();

        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,RequiredHours,TicketPriority")] Ticket ticket, int projId, string userId)
        {
            if (ModelState.IsValid)
            { 
                ticket.Project = projectBL.GetProject(projId);
                Project currProj = projectBL.GetProject(projId);
                ApplicationUser owner = await userBL.GetUser(userId);
                ticket.Owner = owner;
                ticketBL.AddTicket(ticket);
                projectBL.AddTicketToProject(currProj, ticket);
                return RedirectToAction("Index","Projects", new { area = ""});
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            Ticket ticket = ticketBL.GetTicket((int)id);
            

            
      
            if (ticket == null)
            {
                return NotFound();
            }

            //List<ApplicationUser> results = _context.Users.Where(u => u != ticket.Owner).ToList();
            List<ApplicationUser> results = userBL.GetAllUsers(ticket.Owner.Id).ToList();

            List<SelectListItem> currUsers = new List<SelectListItem>();
            results.ForEach(r =>
            {
                currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
            });
            ViewBag.Users = currUsers;

            return View(ticket);
        }

        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> RemoveAssignedUser(string id, int ticketId)
        {
            if (id == null)
            {
                return NotFound();
            }
            Ticket currTicket = ticketBL.GetTicket(ticketId);
            ApplicationUser currUser = await userBL.GetUser(id);
            //To be fixed ASAP
            currTicket.Owner = currUser;
            ticketBL.UpdateTicket(currTicket);
            
            
            return RedirectToAction("Edit", new { id = ticketId });
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id,string userId, [Bind("Id,Title,Body,RequiredHours")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser currUser = await userBL.GetUser(userId);
                    ticket.Owner = currUser;
                    ticketBL.UpdateTicket(ticket);
                   
                }
                catch (DbUpdateConcurrencyException)
                {
                   return NotFound();
                    
                }
                return RedirectToAction(nameof(Edit), new {id = ticket.Id});
            }
            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> CommentTask(int TaskId, string? TaskText)
        {
            if (TaskId != null || TaskText != null)
            {
                try
                {
                    Comment newComment = new Comment();
                    string userName = User.Identity.Name;
                    ApplicationUser user = await userBL.GetUserByName(userName);
                    Ticket ticket = ticketBL.GetTicket(TaskId);

                    newComment.CreatedBy = user;
                    newComment.Description = TaskText;
                    newComment.Ticket = ticket;
                    commentBL.AddComment(newComment);
                    //user.Comments.Add(newComment);
                    ticketBL.AddCommentToTicket(ticket, newComment);

                    int Id = TaskId;
                    
                    return RedirectToAction("Details", new {Id});

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateHrs(int id, int hrs)
        {
            if (id != null || hrs != null)
            {
                try
                {
                    Ticket ticket = ticketBL.GetTicket(id);
                    ticket.RequiredHours = hrs;
                    ticketBL.UpdateTicket(ticket);
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
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
                    TicketWatcher newTickWatch = new TicketWatcher();
                    string userName = User.Identity.Name;
                    ApplicationUser user = await userBL.GetUserByName(userName);
                    Ticket ticket = ticketBL.GetTicket(id);

                    newTickWatch.Ticket = ticket;
                    newTickWatch.Watcher = user;
                    user.TicketWatching.Add(newTickWatch);
                    ticket.TicketWatchers.Add(newTickWatch);
                    //_context.Add(newTickWatch);

                    //await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        //public async Task<IActionResult> UnWatch(int id)
        //{
        //    if (id != null)
        //    {
        //        try
        //        {
                    
        //            string userName = User.Identity.Name;
        //            ApplicationUser user = await userBL.GetUserByName(userName);
        //            Ticket ticket = ticketBL.GetTicket(id);
        //            Project project = ticket.Project;                                       
        //            ticket.TicketWatchers.Remove(currTickWatch);
        //            user.TicketWatching.Remove(currTickWatch);

        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("Details", new { id });

        //        }
        //        catch (Exception ex)
        //        {
        //            return RedirectToAction("Error", "Home");
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            if (id != null)
            {
                try
                {
                    Ticket ticket = ticketBL.GetTicket(id);
                    ticket.Completed = true;

                    ticketBL.UpdateTicket(ticket);
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnMarkAsCompleted(int id)
        {
            if (id != null)
            {
                try
                {
                    Ticket ticket = ticketBL.GetTicket(id);
                    ticket.Completed = false;

                    ticketBL.UpdateTicket(ticket);
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
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
            if (id == null )
            {
                return NotFound();
            }

            Ticket ticket = ticketBL.GetTicket((int)id);
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
        public async Task<IActionResult> DeleteConfirmed(int id, int projId)
        {

            Ticket ticket = ticketBL.GetTicket(id);
            Project currProj = projectBL.GetProject(projId);
            if (ticket != null)
            {
                projectBL.RemoveTicketFromProject(currProj, ticket);
                ticketBL.DeleteTicket(ticket);
                
            }

            ticketBL.UpdateTicket(ticket);
            return RedirectToAction("Index", "Projects");
        }

    }
}

