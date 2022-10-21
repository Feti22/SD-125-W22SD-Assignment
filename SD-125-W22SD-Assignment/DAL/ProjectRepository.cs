using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.DAL
{
    public class ProjectRepository : IRepository<Project>
    {
        private ApplicationDbContext _db;
        public ProjectRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public void Create(Project project)
        {
            _db.Projects.Add(project);
        }

        public void Delete(Project project)
        {
            _db.Projects.Remove(project);
        }

        public Project? GetById(int id)
        {
            return _db.Projects.Include(p => p.CreatedBy).Include(p => p.AssignedTo).Include(p => p.Tickets).ThenInclude(t => t.Owner).Include(p => p.Tickets).ThenInclude(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher).First(p => p.Id == id);
        }

        public Project? Get(Func<Project, bool> predicate)
        {
            return _db.Projects.Include(p => p.CreatedBy).Include(p => p.AssignedTo).Include(p => p.Tickets).ThenInclude(t => t.Owner).Include(p => p.Tickets).ThenInclude(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher).First(predicate);
        }

        public ICollection<Project> GetAll()
        {
            return _db.Projects.Include(p => p.AssignedTo).Include(p => p.Tickets).ToList();
        }

        public ICollection<Project> GetList(Func<Project, bool> predicate)
        {
            return _db.Projects.Include(p => p.CreatedBy).Include(p => p.AssignedTo).ThenInclude(at => at.ApplicationUser).Include(p => p.Tickets).ThenInclude(t => t.Owner).Include(p => p.Tickets).ThenInclude(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher).Where(predicate).ToList();
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public Project Update(Project project)
        {
            return _db.Projects.Update(project).Entity;
        }

        public void RemoveAssignedUser(int projectId, string userId)
        {
            List<UserProject> userProjects = _db.UserProjects.Where(up => up.ProjectId == projectId && up.UserId == userId).ToList();

            foreach (UserProject userProject in userProjects)
            {
                _db.UserProjects.Remove(userProject);
            }
        }
    }
}
