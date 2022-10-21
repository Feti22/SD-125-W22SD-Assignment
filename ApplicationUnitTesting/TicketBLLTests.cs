using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace ApplicationUnitTesting
{

    [TestClass]
    public class TicketBLLTests
    {
        private TicketBusinessLogic ticketBusinessLogic;
        private ProjectBusinessLogic projectBusinessLogic;
        private CommentBusinessLogic commentBusinessLogic;
        private readonly UserBusinessLogic userBusinessLogic;
        private UserManager<ApplicationUser> userManager;
        ProjectRepository projectRepository;
        TicketRepository ticketRepository;
        CommentRepository commentRepository;

        public TicketBLLTests()
        {
            
            var projectData = new List<Project>
            {
                new Project{Id = 1, ProjectName = "Project 1"},
                new Project{Id = 2, ProjectName = "Project 2"},
                new Project{Id = 3, ProjectName = "Project 3"},
            }.AsQueryable();

            var ProjMock = new Mock<DbSet<Project>>();

            ProjMock.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(projectData.Provider);
            ProjMock.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projectData.Expression);
            ProjMock.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projectData.ElementType);
            ProjMock.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projectData.GetEnumerator());

            var projMockContext = new Mock<ApplicationDbContext>();
            projMockContext.Setup(m => m.Projects).Returns(ProjMock.Object);

            projectBusinessLogic = new ProjectBusinessLogic(new ProjectRepository(projMockContext.Object), ticketRepository, userManager);

            var initialTicketData = new List<Ticket>
            {
                new Ticket{Id = 1, Project = projectData.First(p => p.Id == 1), TicketPriority = Ticket.Priority.High, RequiredHours = 12, Title = "Ticket 1"},
                new Ticket{Id = 2, Project = projectData.First(p => p.Id == 2), TicketPriority = Ticket.Priority.Medium, RequiredHours = 24, Title = "Ticket 2"},
                new Ticket{Id = 3, Project = projectData.First(p => p.Id == 3), TicketPriority = Ticket.Priority.Low, RequiredHours = 10, Title = "Ticket 3"},
            };

            var ticketData = initialTicketData.AsQueryable();

            var ticketMockDbSet = new Mock<DbSet<Ticket>>();

            ticketMockDbSet.As<IQueryable<Ticket>>().Setup(m => m.Provider).Returns(ticketData.Provider);
            ticketMockDbSet.As<IQueryable<Ticket>>().Setup(m => m.Expression).Returns(ticketData.Expression);
            ticketMockDbSet.As<IQueryable<Ticket>>().Setup(m => m.ElementType).Returns(ticketData.ElementType);
            ticketMockDbSet.As<IQueryable<Ticket>>().Setup(m => m.GetEnumerator()).Returns(() => ticketData.GetEnumerator());
            ticketMockDbSet.Setup(d => d.Add(It.IsAny<Ticket>())).Callback<Ticket>((s) => initialTicketData.Add(s));
            ticketMockDbSet.Setup(d => d.Remove(It.IsAny<Ticket>())).Callback<Ticket>((s) => initialTicketData.Remove(s));

            var ticketMockContext = new Mock<ApplicationDbContext>();
            ticketMockContext.Setup(m => m.Tickets).Returns(ticketMockDbSet.Object);

            ticketBusinessLogic = new TicketBusinessLogic(userManager, projectRepository, new TicketRepository(ticketMockContext.Object), commentRepository);

            var commentData = new List<Comment>
            {
                new Comment{Id = 1, Description = "Comment 1", Ticket = ticketData.First(t => t.Id == 1)},
                new Comment{Id = 2, Description = "Comment 2", Ticket = ticketData.First(t => t.Id == 2)},
                new Comment{Id = 3, Description = "Comment 3", Ticket = ticketData.First(t => t.Id == 3)},
                };
            var newCommentData = commentData.AsQueryable();

            var commentMockDbSet = new Mock<DbSet<Comment>>();
            
            commentMockDbSet.As<IQueryable<Comment>>().Setup(m => m.Provider).Returns(newCommentData.Provider);
            commentMockDbSet.As<IQueryable<Comment>>().Setup(m => m.Expression).Returns(newCommentData.Expression);
            commentMockDbSet.As<IQueryable<Comment>>().Setup(m => m.ElementType).Returns(newCommentData.ElementType);
            commentMockDbSet.As<IQueryable<Comment>>().Setup(m => m.GetEnumerator()).Returns(newCommentData.GetEnumerator());
            commentMockDbSet.Setup(d => d.Add(It.IsAny<Comment>())).Callback<Comment>((s) => commentData.Add(s));

            var commentMockContext = new Mock<ApplicationDbContext>();
            commentMockContext.Setup(m => m.Comments).Returns(commentMockDbSet.Object);

            commentBusinessLogic = new CommentBusinessLogic(new CommentRepository(commentMockContext.Object), projectRepository, ticketRepository, userManager);

            var userData = new List<ApplicationUser>
            {
                new ApplicationUser{Id = "UserId1", Tickets = ticketData.ToList(), Comments = commentData.ToList()},
                new ApplicationUser{Id = "UserId2", Tickets = ticketData.ToList(), Comments = commentData.ToList()},
                new ApplicationUser{Id = "UserId3", Tickets = ticketData.ToList(), Comments = commentData.ToList()},
            }.AsQueryable();

            var userMockDbSet = new Mock<DbSet<ApplicationUser>>();

            userMockDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(userData.Provider);
            userMockDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(userData.Expression);
            userMockDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(userData.ElementType);
            userMockDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(userData.GetEnumerator());

            var userMockContext = new Mock<ApplicationDbContext>();
            userMockContext.Setup(m => m.Users).Returns(userMockDbSet.Object);

            userBusinessLogic = new UserBusinessLogic(userManager);
        }
                
        [DataRow(1)]
        [TestMethod]
        public void GetTicketById_ValidInput(int expectedId)
        {
            int actualId = ticketBusinessLogic.GetTicketById(1).Id;
            Assert.AreEqual(expectedId, actualId);
        }

        [TestMethod]
        public void GetTicketById_InvalidId_ReturnNullReferenceException()
        {
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                ticketBusinessLogic.GetTicketById(4);
            });
        }
        [DataRow(3)]
        [TestMethod]
        public void GetAllTickets(int expectedTotal)
        {
            int actualCount = ticketBusinessLogic.GetAllTickets().Count;
            Assert.AreEqual(expectedTotal, actualCount);
        }

        

        [DataRow(2)]
        [TestMethod]
        public void DeleteTicket(int expectedCount)
        {

            ticketBusinessLogic.DeleteTicket(1);
            Assert.AreEqual(expectedCount, ticketBusinessLogic.GetAllTickets().Count);
        }
        [DataRow(5)]
        [TestMethod]
        public void DeleteTicket_InvalidInput_ThrowNullExceptionIfTicketNotFound(int invalidInput)
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                ticketBusinessLogic.DeleteTicket(invalidInput);
            });
        }
        [DataRow(3)] //since we already deleted one ticket.
        [TestMethod]
        public void AddTicket(int expectedCount)
        {
            Ticket ticket = new Ticket { Id = 1, Project = projectBusinessLogic.GetProjectById(1), TicketPriority = Ticket.Priority.High, RequiredHours = 12, Title = "Ticket test" };
            ticketBusinessLogic.CreateTicket(ticket, 1, "UserId1");
            Assert.AreEqual(expectedCount, ticketBusinessLogic.GetAllTickets().Count);

        }
        [DataRow(1)]
        [TestMethod]
        public void AddComment(int expectedCommentCount)
        {
            Comment comment = new Comment();
            comment.Id = 1;
            comment.Description = "test comment";
            commentBusinessLogic.AddComment(comment);
            Assert.AreEqual(expectedCommentCount, commentBusinessLogic.GetAllComments().Count);

        }

        
    }
}