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
        private readonly UserBusinessLogic userBusinessLogic;
        public readonly UserManager<ApplicationUser> userManager;
        public readonly UserProjectBusinessLogic userProjectBL;
        public TicketRepository ticketRepository;

        public ProjectBLLTests()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { UserName = "TestUser", Id = "TestUserId", Email = "test@test.it" }
            };

            var mockUserManager = new Mock<MockUserManager>();

            mockUserManager.Setup(x => x.Users).Returns(users.AsQueryable());
            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((string userId) => userManager.Users.SingleOrDefault(u => u.Id == userId));

            userManager = mockUserManager.Object;
            userBusinessLogic = new UserBusinessLogic(userManager);


            var InitialProjects = new List<Project>
                {
                    new Project{ Id = 1, ProjectName = "FirstProject" ,AssignedTo = new HashSet<UserProject>(),},
                    new Project{ Id = 2, ProjectName = "SecondProject" ,AssignedTo = new HashSet<UserProject>()},
                    new Project{ Id = 3, ProjectName = "ThirdProject" , AssignedTo = new HashSet<UserProject>()},
                };

            var projects = InitialProjects.AsQueryable();

            var MockProject = new Mock<DbSet<Project>>();

            MockProject.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(projects.Provider);
            MockProject.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projects.Expression);
            MockProject.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projects.ElementType);
            MockProject.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projects.GetEnumerator());
            MockProject.Setup(m => m.Add(It.IsAny<Project>())).Callback<Project>((s) => InitialProjects.Add(s));
            MockProject.Setup(m => m.Remove(It.IsAny<Project>())).Callback<Project>((s) => InitialProjects.Remove(s));

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

        [DataRow(3)]
        [TestMethod]
        public async void CreateProject_ValidInput(int expectedTotal)
        {
            Project project = new Project();
            string userId = userBusinessLogic.GetUser("1").Id;
            projectBusinessLogic.CreateProject(project, userId);
            int actualCount = projectBusinessLogic.GetAllProjects().Count;

            Assert.AreEqual(expectedTotal, actualCount);
        }

        //[DataRow(4)]
        [TestMethod]
        public async void CreateProject_InValidInput_ReturnNullWhenNoIdMatch()
        {
            Project project = new Project();
            string userId = userBusinessLogic.GetUser("1").Id;
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                projectBusinessLogic.CreateProject(project, userId);
            });
        }

        [DataRow(3)]
        [TestMethod]
        public void GetCompletedProjects(int expectedTotal)
        {
            int actualCount = projectBusinessLogic.GetCompletedProjects().Count;

            Assert.AreEqual(actualCount, expectedTotal);
        }

        [DataRow(1, 4)]
        [TestMethod]
        public void DeleteProject_ValidInput(int projectId, int expectedTotal)
        {
            projectBusinessLogic.DeleteProject(projectId);
            int actualCount = projectBusinessLogic.GetAllProjects().Count;

            Assert.AreEqual(actualCount, expectedTotal);
        }

        [DataRow(4)]
        [TestMethod]
        public void DeleteProject_InvalidInput_ReturnNullWhenNoIdMatch(int projectId)
        {
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                projectBusinessLogic.DeleteProject(projectId);
            });
        }

        [DataRow(1, 3)]
        [TestMethod]
        public async Task AssignUsers_ValidInput(int projectId, int expectedTotal)
        {
            List<string> usersAssignedId = new List<string> { "1", "2" };
            await projectBusinessLogic.AssignUsers(projectId, usersAssignedId);
            Project project = projectBusinessLogic.GetProjectById(projectId);
            int actualCount = project.AssignedTo.Count;

            Assert.AreEqual(expectedTotal, actualCount);
        }

        [DataRow(4, 3)]
        [TestMethod]
        public async Task AssignUsers_InValidInput_ThrowNullReferenceException(int projectId)
        {
            List<string> usersAssignedId = new List<string> { "1", "2" };
            await projectBusinessLogic.AssignUsers(projectId, usersAssignedId);
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                projectBusinessLogic.AssignUsers(projectId, new List<string>() { "1", "2" });
            });
        }

        [DataRow(1, 3, 0)]
        [TestMethod]
        public void RemoveAssignedUser_ValidInput(int projectId, string userId, int expectedTotal)
        {
            projectBusinessLogic.RemoveAssignedUser(projectId, userId);
            Project project = projectBusinessLogic.GetProjectById(projectId);
            int actualCount = project.AssignedTo.Count;

            Assert.AreEqual(expectedTotal, actualCount);
        }

        [DataRow(4, 3, 0)]
        [TestMethod]
        public void RemoveAssignedUser_InValidInput_ThrowNullWhenNoIdMatch(int projectId, string userId)
        {
            Project project = projectBusinessLogic.GetProjectById(projectId);
            int actualCount = project.AssignedTo.Count;
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                projectBusinessLogic.RemoveAssignedUser(projectId, userId);
            });
        }
    }
}