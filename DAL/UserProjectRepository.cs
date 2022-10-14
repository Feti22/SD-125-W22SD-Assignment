using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.DAL
{
    public class UserProjectRepository : IRepository<UserProject>
    {
        private ApplicationDbContext _db;
        public UserProjectRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public void Add(UserProject entity)
        {
            _db.UserProjects.Add(entity);
        }

        // READ
        public UserProject Get(int id)
        {
            return _db.UserProjects.Include(p => p.ProjectId).Include(p => p.UserId).First(p => p.Id == id);
        }

        /*public UserProject GetByTwoValue(string userId, int projId)
        {
            return _db.UserProjects.Include(p => p.ProjectId).Include(p => p.UserId).First(p => p.UserId == userId && p.ProjectId == projId);
        }*/

        public UserProject Get(Func<UserProject, bool> predicate)
        {
            return _db.UserProjects.Include(p => p.ProjectId).Include(p => p.UserId).First(predicate);
        }
        public ICollection<UserProject> GetAll()
        {
            return _db.UserProjects.Include(p => p.ProjectId).Include(p => p.UserId).ToList();
        }
        public ICollection<UserProject> GetList(Func<UserProject, bool> predicate)
        {
            return _db.UserProjects.Include(p => p.ProjectId).Include(p => p.UserId).Where(predicate).ToList();
        }

        // UPDATE
        public UserProject Update(UserProject entity)
        {
            _db.UserProjects.Update(entity);
            return entity;
        }

        // DELETE
        public void Delete(UserProject entity)
        {
            _db.UserProjects.Remove(entity);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
