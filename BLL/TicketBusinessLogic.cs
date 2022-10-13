using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class TicketBusinessLogic
    {
        private IRepository<Ticket> _repo;
        //private UserManager<ApplicationUser> _userManager;
        public TicketBusinessLogic(IRepository<Ticket> repository) //UserManager<ApplicationUser> userManager)
        {
            _repo = repository;
            //_userManager = userManager;
        }

        public void AddTicket(Ticket ticket)
        {
            _repo.Add(ticket);
        }

        public void DeleteTicket(Ticket ticket)
        {
            _repo.Delete(ticket);
        }

        public Ticket GetTicket(int id)
        {
            return  _repo.Get(id);
        }

        public void AddCommentToTicket(Ticket ticket, Comment comment)
        {
            
            ticket.Comments.Add(comment);
            _repo.Update(ticket);
            _repo.Save();
        }

        public Ticket GetTicket(Func<Ticket, bool> predicate)
        {
            return _repo.Get(predicate);
        }

        public List<Ticket> GetTickets()
        {
            return (List<Ticket>)_repo.GetAll();
        }

        public List<Ticket> GetTickets(Func<Ticket, bool> predicate)
        {
            return (List<Ticket>)_repo.GetList(predicate);
        }

        public Ticket UpdateTicket(Ticket ticket)
        {
            return _repo.Update(ticket);
        }

        public void SaveChanges()
        {
            _repo.Save();
        }
    }
}