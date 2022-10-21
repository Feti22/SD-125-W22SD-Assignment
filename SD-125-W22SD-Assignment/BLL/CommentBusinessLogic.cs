using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class CommentBusinessLogic
    {
        private readonly CommentRepository _commentRepo;
        private readonly ProjectRepository _projectRepo;
        private readonly TicketRepository _ticketRepo;
        private readonly UserManager<ApplicationUser> _userManager;


        public CommentBusinessLogic(CommentRepository commentRepo, ProjectRepository projectRepo, TicketRepository ticketRepo, UserManager<ApplicationUser> userManager)
        {
            _commentRepo = commentRepo;
            _projectRepo = projectRepo;
            _ticketRepo = ticketRepo;
            _userManager = userManager;
        }

        public void AddComment(Comment comment)
        {
            _commentRepo.Create(comment);
        }

        public void DeleteComment(Comment comment)
        {
            _commentRepo.Delete(comment);
            _commentRepo.Update(comment);
            _commentRepo.Save();
        }

        public List<Comment> GetAllComments()
        {
            return _commentRepo.GetAll().ToList();
        }

        public Comment GetCommentById(int id)
        {
            return _commentRepo.GetById(id);
        }
    }
}