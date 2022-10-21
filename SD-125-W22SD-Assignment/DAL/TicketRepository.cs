using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.DAL
{
    public class TicketRepository : IRepository<Ticket>
    {
        private ApplicationDbContext _db;
        public TicketRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public void Create(Ticket ticket)
        {
            _db.Tickets.Add(ticket);
        }

        public void Delete(Ticket ticket)
        {
            _db.Tickets.Remove(ticket);
        }

        public Ticket? GetById(int id)
        {
            return _db.Tickets.Include(t => t.Project).Include(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher).Include(u => u.Owner).Include(t => t.Comments).ThenInclude(c => c.CreatedBy).First(t => t.Id == id);
        }

        public Ticket? Get(Func<Ticket, bool> predicate)
        {
            return _db.Tickets.Include(t => t.Project).Include(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher).Include(u => u.Owner).Include(t => t.Comments).ThenInclude(c => c.CreatedBy).First(predicate);
        }

        public ICollection<Ticket> GetAll()
        {
            return _db.Tickets.Include(t => t.Project).Include(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher).Include(u => u.Owner).Include(t => t.Comments).ThenInclude(c => c.CreatedBy).ToList();
        }

        public ICollection<Ticket> GetList(Func<Ticket, bool> predicate)
        {
            return _db.Tickets.Include(t => t.Project).Include(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher).Include(u => u.Owner).Include(t => t.Comments).ThenInclude(c => c.CreatedBy).Where(predicate).ToList();
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public Ticket Update(Ticket ticket)
        {
            return _db.Tickets.Update(ticket).Entity;
        }

        public void RemoveWatcher(TicketWatcher watcher)
        {
            _db.TicketWatchers.Remove(watcher);
        }

        public void AddCommentToTicket(int ticketId, Comment comment)
        {
            Ticket ticket = _db.Tickets.Include(t => t.Comments).First(t => t.Id == ticketId);
            ticket.Comments.Add(comment);
            Save();
        }
    }
}