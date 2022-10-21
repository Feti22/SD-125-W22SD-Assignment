using ApplicationUnitTests;
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
    public class ProjectBLLTests
    {
        private readonly ProjectBusinessLogic projectBusinessLogic;
        public readonly UserManager<ApplicationUser> userManager;
        public readonly UserProjectBusinessLogic userProjectBL;
        public TicketRepository ticketRepository;

        public ProjectBLLTests()
        {
            var projects = new List<Project>
                {
                    new Project{ Id = 1, ProjectName = "First Project" },
                    new Project{ Id = 2, ProjectName = "Second Project" },
                    new Project{ Id = 3, ProjectName = "Third Project" },
                }.AsQueryable();

            var MockProject = new Mock<DbSet<Project>>();

            MockProject.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(projects.Provider);
            MockProject.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projects.Expression);
            MockProject.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projects.ElementType);
            MockProject.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projects.GetEnumerator());

            var projMockContext = new Mock<ApplicationDbContext>();
            projMockContext.Setup(m => m.Projects).Returns(MockProject.Object);

            projectBusinessLogic = new ProjectBusinessLogic(new ProjectRepository(projMockContext.Object), ticketRepository, userManager);
        }               

        [DataRow(1)]
        [TestMethod]
        public void GetProjectById_ValidInput(int projId)
        {
            Project currProj = projectBusinessLogic.GetProjectById(projId);

            Assert.IsTrue(currProj != null);
        }

        [DataRow(4)]
        [TestMethod]
        public void GetProjectById_InvalidInput_ReturnNullWhenNoIdMatch(int projId)
        {
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                projectBusinessLogic.GetProjectById(projId);
            });
        }

        [DataRow(3)]
        [TestMethod]
        public void GetAllProjects(int expectedTotal)
        {
            int actualCount = projectBusinessLogic.GetAllProjects().Count;

            Assert.AreEqual(expectedTotal, actualCount);
        }

        [DataRow(4)]
        [TestMethod]
        public void CreateNewProject_ValidInput(int expected)
        {
            projectBusinessLogic.CreateProject(new Project { Id = 4, ProjectName = "Fourth Project" });
            int projectsCount = projectBusinessLogic.GetAllProjects().Count;

            Assert.AreEqual(expected, projectsCount);
        }
    }
}
