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

        public void CreateProject(Project project)
        {
            repo.Add(project);
            repo.Save();
        }

        public void DeleteProject(Project project)
        {
            repo.Delete(project);
            repo.Save();
        }

        public Project UpdateProject(Project project)
        {
            return repo.Update(project);
        }

        public Project GetProject(int id)
        {
            return repo.Get(project => project.Id == id);
        }

        //public Project GetProject(Func<Project, bool> predicate)
        //{
        //    return repo.Get(predicate);
        //}

        public List<Project> GetAllProjects()
        {
            return (List<Project>)repo.GetAll();
        }

        //public List<Project> GetProjects(Func<Project, bool> predicate)
        //{
        //    return (List<Project>)repo.GetList(predicate);
        //}

        public void EditProject(Project project)
        {
            repo.Update(project);
            repo.Save();
        }

        public List<Project> OrderedByPriority()
        {
            return (List<Project>)repo.GetAll().OrderByDescending(p => p.Tickets.OrderByDescending(t => t.TicketPriority));
        }

        public List<Project> OrderedByPriorityAsc()
        {
            return (List<Project>)repo.GetAll().OrderBy(p => p.Tickets.OrderBy(t => t.TicketPriority));
        }

        public List<Project> OrderedByRequiredHours()
        {
            return (List<Project>)repo.GetAll().OrderByDescending(p => p.Tickets.OrderByDescending(t => t.RequiredHours));
        }

        public List<Project> OrderedByRequiredHoursAsc()
        {
            return (List<Project>)repo.GetAll().OrderBy(p => p.Tickets.OrderBy(t => t.RequiredHours));
        }

        public List<Project> GetCompletedProjects()
        {
            return (List<Project>)repo.GetAll();
        }
    }
}