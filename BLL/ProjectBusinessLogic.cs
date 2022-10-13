using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class ProjectBusinessLogic
    {
        private IRepository<Project> repo;
        public ProjectBusinessLogic(IRepository<Project> repository)
        {
            repo = repository;
        }

        public void AddProject(Project project)
        {
            repo.Add(project);
        }

        public void DelelteProject(Project project)
        {
            repo.Delete(project);
        }

        

        public Project GetProject(int id)
        {
            return repo.Get(id);
        }

        public Project GetProject(Func<Project, bool> predicate)
        {
            return repo.Get(predicate);
        }

        public List<Project> GetProjects()
        {
            return (List<Project>)repo.GetAll();
        }

        public List<Project> GetProjects(Func<Project, bool> predicate)
        {
            return (List<Project>)repo.GetList(predicate);
        }

        public void AddTicketToProject(Project project, Ticket ticket)
        {
            project.Tickets.Add(ticket);
            repo.Update(project);
            repo.Save();
        }

        public void RemoveTicketFromProject(Project project, Ticket ticket)
        {
            project.Tickets.Remove(ticket);
            repo.Update(project);
            repo.Save();
        }

        public Project UpdateProject(Project project)
        {
            return repo.Update(project);
        }

        public void SaveChanges()
        {
            repo.Save();
        }
    }
}