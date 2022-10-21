using ApplicationUnitTests;
using Microsoft.AspNetCore.Identity;
using Moq;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace ApplicationUnitTesting
{
    [TestClass]
    public class UserBLLTests
    {
        private readonly UserBusinessLogic userBusinessLogic;
        public readonly UserManager<ApplicationUser> userManager;

        public UserBLLTests()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { UserName = "TestUser", Id = "TestUserId", Email = "test@test.it" }
            };

            var mockUserManager = new Mock<MockUserManager>();

            mockUserManager.Setup(um => um.Users).Returns(users.AsQueryable());
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(um => um.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((string userId) => userManager.Users.SingleOrDefault(u => u.Id == userId));
            mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((string userName) => userManager.Users.SingleOrDefault(u => u.UserName == userName));
            mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "Admin", "ProjectManager", "Developer" });
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(um => um.GetUsersInRoleAsync(It.IsAny<string>())).ReturnsAsync(new List<ApplicationUser> { new ApplicationUser { } });
            
            userManager = mockUserManager.Object;
            userBusinessLogic = new UserBusinessLogic(userManager);
        }

        [DataRow("TestUser")]
        [TestMethod]
        public async Task GetUserByName_ReturnCorrectUserWhenNamesMatch(string name)
        {
            ApplicationUser user = await userBusinessLogic.GetUserByName(name);

            Assert.IsNotNull(user);
        }

        [DataRow("InvalidName")]
        [TestMethod]
        public void GetUserByName_InvalidInput_ReturnNullWhenNoNamesMatch(string name)
        {
            Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await userBusinessLogic.GetUserByName(name);
            });
        }

        [DataRow("TestUserId")]
        [TestMethod]
        public Task GetUserById_ValidUserId_ReturnCorrectUserWhenIdsMatch(string userId)
        {
            ApplicationUser user = userBusinessLogic.GetUser(userId);

            Assert.IsNotNull(user);

            return Task.CompletedTask;
        }

        [DataRow("InvalidUserId")]
        [TestMethod]
        public void GetUserById_InvalidUserId_ReturnNullWhenNoIdMatch(string userId)
        {
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                userBusinessLogic.GetUser(userId);
            });
        }

        [TestMethod]
        public async Task GetAllUsersWithoutRolec()
        {
            List<ApplicationUser> applicationUsers = await userBusinessLogic.GetAllUsersWithoutRole();

            Assert.IsNotNull(applicationUsers);
        }

        [TestMethod]
        public async Task GetAllUsersWithSpecificRole_ValidRoleInput_ReturnAllUsersWithGivenRoleArgument()
        {
            ApplicationUser user = userBusinessLogic.GetUser("TestUserId");
            await userBusinessLogic.AssignUserToARole(user, "Developer");
            List<ApplicationUser> users = await userBusinessLogic.GetUsersWithSpecificRole("Developer");

            Assert.IsNotNull(users);
        }

        [DataRow("InavalidRole")]
        [TestMethod]
        public void GetAllUsersWithSpecificRole_InvalidRoleInput_ArgumentException(string role)
        {
            Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await userBusinessLogic.GetUserRoles(role);
            });
        }

        [DataRow("TestUserId", 3)]
        [TestMethod]
        public async Task AssignUserToARole_ValidInput(string userId, int expected)
        {
            ApplicationUser user = userBusinessLogic.GetUser(userId);
            await userBusinessLogic.AssignUserToARole(user, "ProjectManager");
            List<string> roles = await userBusinessLogic.GetUserRoles(user);
            int actualCount = roles.Count;

            Assert.AreEqual(expected, actualCount);
        }

        [DataRow("TestUserId")]
        [TestMethod]
        public Task AssignUserToARole_InvalidRole_ArgumentException(string userId)
        {
            ApplicationUser user = userBusinessLogic.GetUser(userId);

            Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await userBusinessLogic.AssignUserToARole(user, "InvalidRole");
            });

            return Task.CompletedTask;
        }
    }
}