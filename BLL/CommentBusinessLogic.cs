using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class CommentBusinessLogic
    {
        public IRepository<Comment> _repo;

        public CommentBusinessLogic(IRepository<Comment> repo)
        {
            _repo = repo;
        }

        public void AddComment(Comment comment)
        {
            _repo.Add(comment);
        }

        public void DeleteComment(Comment comment)
        {
            _repo.Delete(comment);
            _repo.Update(comment);
            _repo.Save();
        }


    }
}
