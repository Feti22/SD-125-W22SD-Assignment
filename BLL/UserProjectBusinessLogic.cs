using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class UserProjectBusinessLogic
    {
        private IRepository<UserProject> repo;

        public UserProjectBusinessLogic(IRepository<UserProject> repository)
        {
            repo = repository;
        }

        public void Delete(UserProject userProject)
        {
            repo.Delete(userProject);
        }

        public UserProject GetUserProject(Func<UserProject,bool> predicate)
        {
            return repo.Get(predicate);
        }
        
        public List<UserProject> GetUserProjects(Func<UserProject, bool> predicate)
        {
            return (List<UserProject>)repo.GetList(predicate);
        }
    }
}
