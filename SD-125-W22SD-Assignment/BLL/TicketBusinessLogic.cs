using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class TicketBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProjectRepository _projectRepo;
        private readonly TicketRepository _ticketRepo;
        private readonly CommentRepository _commentRepo;

        public TicketBusinessLogic(UserManager<ApplicationUser> userManager, ProjectRepository projectRepo, TicketRepository ticketRepo, CommentRepository commentRep)
        {
            _userManager = userManager;
            _projectRepo = projectRepo;
            _ticketRepo = ticketRepo;
            _commentRepo = commentRep;
        }

        public Ticket? GetTicketById(int id)
        {
            try
            {
                return _ticketRepo.GetById(id);
            }
            catch
            {
                throw new NullReferenceException("No ticket found with this Id");
            }
        }

        public List<Ticket> GetAllTickets()
        {
            return (List<Ticket>)_ticketRepo.GetAll();
        }

        public async Task CreateTicket(Ticket ticket, int projectId, string userId)
        {
            Project? project = _projectRepo.GetById(projectId);
            ApplicationUser owner = await _userManager.FindByIdAsync(userId);

            if (project == null)
            {
                throw new ArgumentException("No project found with this Id");
            }

            ticket.Project = project;
            ticket.Owner = owner;
            _ticketRepo.Create(ticket);
            _ticketRepo.Save();
        }

        public async Task UpdateTicket(Ticket ticket)
        {
            _ticketRepo.Update(ticket);
            _ticketRepo.Save();

        }

        public void DeleteTicket(int id)
        {
            try
            {
                Ticket? ticket = _ticketRepo.GetById(id);
                if (ticket == null)
                {
                    throw new ArgumentException("No ticket found with this Id");
                }

                _ticketRepo.Delete(ticket);
                _ticketRepo.Save();
            }
            catch
            {
                throw new ArgumentException("No ticket found with this Id");
            }
        }

        public async Task AddCommentToTicket(string userId, int ticketId, string description)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            Ticket? ticket = _ticketRepo.GetById(ticketId);

            try
            {
                Comment comment = new Comment();
                comment.CreatedBy = user;
                comment.Description = description;
                comment.Ticket = ticket;
                _ticketRepo.AddCommentToTicket(ticketId, comment);
                _ticketRepo.Update(ticket);
                _ticketRepo.Save();


                _commentRepo.Create(comment);
                _commentRepo.Save();
            }
            catch
            {
                throw new ArgumentException("No ticket found with this Id");
            }
        }

        public async Task AddTicketToWatcher(string userId, int ticketId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            Ticket? ticket = _ticketRepo.GetById(ticketId);

            if (ticket == null)
            {
                throw new ArgumentException("No ticket found with this Id");
            }

            TicketWatcher ticketWatcher = new TicketWatcher();
            ticketWatcher.Ticket = ticket;
            ticketWatcher.Watcher = user;
            ticket.TicketWatchers.Add(ticketWatcher);
            _ticketRepo.Update(ticket);
            _ticketRepo.Save();
        }

        public async Task RemoveTicketFromWatcher(string userId, int ticketId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            Ticket? ticket = _ticketRepo.GetById(ticketId);

            if (ticket == null)
            {
                throw new ArgumentException("No ticket found with this Id");
            }

            TicketWatcher ticketWatcher = ticket.TicketWatchers.First(tw => tw.Watcher.Id == user.Id);

            _ticketRepo.RemoveWatcher(ticketWatcher);
            _ticketRepo.Save();
        }

        public bool Exists(int id)
        {
            Ticket? ticket = _ticketRepo.GetById(id);

            return ticket != null;
        }
    }
}