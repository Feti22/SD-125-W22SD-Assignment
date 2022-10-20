﻿using ApplicationUnitTests;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace ApplicationUnitTesting
{
    [TestClass]
    public class UserBLLTests
    {
        [TestMethod]
        public async Task GetUserByName_ReturnCorrectUserWhenNamesMatch()
        {
            var user = new ApplicationUser
            {
                UserName = "User1",
                Id = "UserId1",
                Email = "test@test.it"
            };
            var userManager = MockUserManager.GetMockUserManager(new List<ApplicationUser> { user }, null);
            var userBusinessLogic = new UserBusinessLogic(userManager);

            var expectedUser = await userBusinessLogic.GetUserByName(user.UserName);

            Assert.AreEqual(user.UserName, expectedUser.UserName);
        }

        [TestMethod]
        public async Task GetUserByName_ReturnNullWhenNoNamesMatch()
        {
            var user = new ApplicationUser
            {
                UserName = "User1",
                Id = "UserId1",
                Email = "test@test.it"
            };
            var userManager = MockUserManager.GetMockUserManager(new List<ApplicationUser> { user }, null);
            var userBusinessLogic = new UserBusinessLogic(userManager);

            var expectedUser = await userBusinessLogic.GetUserByName("UserId2");

            Assert.IsNull(expectedUser);
        }

        [TestMethod]
        public async Task GetUserById_ReturnCorrectUserWhenIdsMatch()
        {
            var user = new ApplicationUser
            {
                UserName = "User1",
                Id = "UserId1",
                Email = "test@test.it"
            };
            var userManager = MockUserManager.GetMockUserManager(new List<ApplicationUser> { user }, null);
            var userBusinessLogic = new UserBusinessLogic(userManager);

            var expectedUser = await userBusinessLogic.GetUser(user.Id);

            Assert.AreEqual(user.Id, expectedUser.Id);
        }

        [TestMethod]
        public async Task GetUserById_ReturnNullWhenNoIdsMatch()
        {
            var user = new ApplicationUser
            {
                UserName = "User1",
                Id = "UserId1",
                Email = "test@test.it"
            };
            var userManager = MockUserManager.GetMockUserManager(new List<ApplicationUser> { user }, null);
            var userBusinessLogic = new UserBusinessLogic(userManager);

            var expectedUser = await userBusinessLogic.GetUser("UserId2");

            Assert.IsNull(expectedUser);
        }

        [TestMethod]
        public void GetAllUsersInTheList_ReturnAllUsers()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "User1",
                    Id = "UserId1",
                    Email = "test@test.it"
                },
                new ApplicationUser
                {
                    UserName = "User2",
                    Id = "UserId2",
                    Email = "test@test.it"
                },
                new ApplicationUser
                {
                    UserName = "User3",
                    Id = "UserId3",
                    Email = "test@test.it"
                }
            };
            var userManager = MockUserManager.GetMockUserManager(users, null);
            var userBusinessLogic = new UserBusinessLogic(userManager);

            var expectedUsers = userBusinessLogic.GetAllUsers();

            foreach (var user in users)
            {
                Assert.IsTrue(expectedUsers.Select(user => user.UserName).Contains(user.UserName));
            }
        }

        [TestMethod]
        public void GetAllUsersInTheList_ReturnEmptyListWhenThereIsNoUser()
        {
            var users = new List<ApplicationUser>
            {
            };
            var userManager = MockUserManager.GetMockUserManager(users, null);
            var userBusinessLogic = new UserBusinessLogic(userManager);

            var expectedUsers = userBusinessLogic.GetAllUsers();

            Assert.AreEqual(users.Count, expectedUsers.Count);
        }
    }
}