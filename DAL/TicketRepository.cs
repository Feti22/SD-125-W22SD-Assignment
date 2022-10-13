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

        public void Add(Ticket ticket)
        {
            _db.Tickets.Add(ticket);
        }

        public void Delete(Ticket ticket)
        {
            _db.Tickets.Remove(ticket);
        }

        public async Task<Ticket> Get(int id)
        {
            return await _db.Tickets.Include(t => t.Owner).Include(t => t.TicketWatchers).Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == id);
        }

        public Ticket Get(Func<Ticket, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public ICollection<Ticket> GetAll()
        {
            return _db.Tickets.Include(t => t.Owner).Include(t => t.TicketWatchers).Include(t => t.Comments).ToList();
        }

        public ICollection<Ticket> GetList(Func<Ticket, bool> predicate)
        {
            return _db.Tickets.Include(t => t.Owner).Include(t => t.TicketWatchers).Include(t => t.Comments).Where(predicate).ToList();
        }

        

        public void Save()
        {
            _db.SaveChanges();
        }

        public Ticket Update(Ticket ticket)
        {
            _db.Tickets.Update(ticket);
            return ticket;
        }

        Ticket IRepository<Ticket>.Get(int id)
        {
            throw new NotImplementedException();
        }
    }
}
