using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class ProjectBusinessLogic
    {
        private readonly ProjectRepository _projectRepo;
        private readonly TicketRepository _ticketRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectBusinessLogic(ProjectRepository projectRepository, TicketRepository ticketRepository, UserManager<ApplicationUser> userManager)
        {
            _projectRepo = projectRepository;
            _ticketRepo = ticketRepository;
            _userManager = userManager;
        }

        public Project? GetProjectById(int id)
        {
            try
            {
               return _projectRepo.GetById(id);
            }
            catch
            {
                throw new NullReferenceException("No project found with this Id");
            }
        }

        public List<Project> GetAllProjects()
        {
            return (List<Project>)_projectRepo.GetAll();
        }

        public void CreateProject(Project project)
        {
            _projectRepo.Create(project);
            _projectRepo.Save();
        }

        public void EditProject(int projectId, string name)
        {
            Project? project = _projectRepo.GetById(projectId);

            if (project == null)
            {
                throw new ArgumentException("No project found with this Id");
            }

            project.ProjectName = name;
            _projectRepo.Update(project);
            _projectRepo.Save();
        }

        public void DeleteProject(int projectId)
        {
            Project? project = _projectRepo.GetById(projectId);

            if (project == null)
            {
                throw new ArgumentException("No project found with this Id");
            }

            foreach (Ticket ticket in project.Tickets)
            {
                _ticketRepo.Delete(ticket);
            }
            _ticketRepo.Save();

            foreach (UserProject userProject in project.AssignedTo)
            {
                _projectRepo.RemoveAssignedUser((int)userProject.ProjectId, userProject.UserId);
            }

            _projectRepo.Delete(project);
            _projectRepo.Save();
        }

        public List<Project> OrderByPriority()
        {
            return (List<Project>)_projectRepo.GetAll().OrderByDescending(p => p.Tickets.OrderByDescending(t => t.TicketPriority));
        }

        public List<Project> OrderByPriorityAsc()
        {
            return (List<Project>)_projectRepo.GetAll().OrderBy(p => p.Tickets.OrderBy(t => t.TicketPriority));
        }

        public List<Project> OrderByRequiredHours()
        {
            return (List<Project>)_projectRepo.GetAll().OrderByDescending(p => p.Tickets.OrderByDescending(t => t.RequiredHours));
        }

        public List<Project> OrderByRequiredHoursAsc()
        {
            return (List<Project>)_projectRepo.GetAll().OrderBy(p => p.Tickets.OrderBy(t => t.RequiredHours));
        }

        public async Task AssignUsers(int projectId, List<string> usersAssignedId)
        {
            Project? project = _projectRepo.GetById(projectId);

            if (project == null)
            {
                throw new ArgumentException("projectId does not exist");
            }

            foreach (string userId in usersAssignedId)
            {
                ApplicationUser assignedUser = await _userManager.FindByIdAsync(userId);
                UserProject userProject = new UserProject();

                userProject.ApplicationUser = assignedUser;
                userProject.UserId = assignedUser.Id;
                userProject.Project = project;

                if (project.AssignedTo.FirstOrDefault(up => up.UserId == assignedUser.Id) == null)
                {
                    project.AssignedTo.Add(userProject);
                }
            }

            _projectRepo.Update(project);
            _projectRepo.Save();
        }

        public void RemoveAssignedUser(int projectId, string userId)
        {
            _projectRepo.RemoveAssignedUser(projectId, userId);
            _projectRepo.Save();
        }

        public void AddTicketToProject(Project project, Ticket ticket)
        {
            project.Tickets.Add(ticket);
            _projectRepo.Update(project);
            _projectRepo.Save();
        }

        public void RemoveTicketFromProject(Project project, Ticket ticket)
        {
            project.Tickets.Remove(ticket);
            _projectRepo.Update(project);
            _projectRepo.Save();
        }

        public List<Project> GetCompletedProjects()
        {
            return (List<Project>)_projectRepo.GetAll();
        }

        public bool Exists(int id)
        {
            Project? project = _projectRepo.GetById(id);

            return project != null;
        }
    }
}