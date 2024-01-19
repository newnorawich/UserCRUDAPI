using Infranstructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UserCRUDAPI.Controllers;
using Xunit;

namespace UserCRUDAPI.Tests
{
    public class UpdateUser
    {
        [Theory(DisplayName = "When calling put method to update user, it should update the user")]
        [InlineData(3, null, "Jack", "Jack@mail.com")]
        [InlineData(0, 3, "Jack", "Jack@mail.com")]
        public async Task WhenCallingPutMethodToUpdateUserByIdItShouldUpdateUser(int fromQueryId, uint fromRequestId, string requestName, string requestEmail)
        {
            var context = Utils.setUpDatabase("update_test.db");

            var listOfUser = new List<User>()
            {
                new User() { Id = 1, Name = "Norawich", Email = "newsnora@gmail.com"},
                new User() { Id = 2, Name = "New12345", Email = "New12345@gmail.com"},
                new User() { Id = 3, Name = "Bill", Email = "Bill@gmail.com"}
            };
            context.Users.AddRange(listOfUser);
            context.SaveChanges();

            var newUser = new User() { Id = fromRequestId, Name = requestName, Email = requestEmail };
            var controller = new UserController(context);
            var result = controller.Update(fromQueryId, newUser).Result as OkObjectResult;
            var returnUser = result.Value as User;

            var updatedUser = new User() { Id = 3, Name = "Jack", Email = "Jack@mail.com" };
            Assert.Equal(3, (Int64)returnUser.Id);
            Assert.Equal(updatedUser.Name, returnUser.Name);
            Assert.Equal(updatedUser.Email, returnUser.Email);

            var data = context.Users.Where(x => x.Id == updatedUser.Id).FirstOrDefault();
            Assert.Equal(3, (Int64)data.Id);
            Assert.Equal(updatedUser.Name, data.Name);
            Assert.Equal(updatedUser.Email, data.Email);
        }

        [Fact(DisplayName = "When calling put method to update name of user, it should update user's name")]
        public async Task WhenCallingPutMethodToUpdateNameOfUserByIdItShouldUpdateTheUsersName()
        {
            var context = Utils.setUpDatabase("update_test.db");

            var listOfUser = new List<User>()
            {
                new User() { Id = 1, Name = "Norawich", Email = "newsnora@gmail.com"},
                new User() { Id = 2, Name = "New12345", Email = "New12345@gmail.com"},
                new User() { Id = 3, Name = "Bill", Email = "Bill@gmail.com"}
            };
            context.Users.AddRange(listOfUser);
            context.SaveChanges();

            var newUser = new User() { Id = 3, Name = "BillKung", Email = null };
            var controller = new UserController(context);
            var result = controller.Update(3, newUser).Result as OkObjectResult;
            var returnUser = result.Value as User;

            var updatedUser = new User() { Id = 3, Name = "BillKung", Email = "Bill@gmail.com" };
            Assert.Equal(3, (Int64)returnUser.Id);
            Assert.Equal(updatedUser.Name, returnUser.Name);
            Assert.Equal(updatedUser.Email, returnUser.Email);

            var data = context.Users.Where(x => x.Id == updatedUser.Id).FirstOrDefault();
            Assert.Equal(3, (Int64)data.Id);
            Assert.Equal(updatedUser.Name, data.Name);
            Assert.Equal(updatedUser.Email, data.Email);
        }

        [Fact(DisplayName = "When calling put method to update email of user, it should update email's name")]
        public async Task WhenCallingPutMethodToUpdateEmailOfUserByIdItShouldUpdateTheUsersEmail()
        {
            var context = Utils.setUpDatabase("update_test.db");

            var listOfUser = new List<User>()
            {
                new User() { Id = 1, Name = "Norawich", Email = "newsnora@gmail.com"},
                new User() { Id = 2, Name = "New12345", Email = "New12345@gmail.com"},
                new User() { Id = 3, Name = "Bill", Email = "Bill@gmail.com"}
            };
            context.Users.AddRange(listOfUser);
            context.SaveChanges();

            var newUser = new User() { Id = 3, Name = null, Email = "BillKung@mail.com" };
            var controller = new UserController(context);
            var result = controller.Update(3, newUser).Result as OkObjectResult;
            var returnUser = result.Value as User;

            var updatedUser = new User() { Id = 3, Name = "Bill", Email = "BillKung@mail.com" };
            Assert.Equal(3, (Int64)returnUser.Id);
            Assert.Equal(updatedUser.Name, returnUser.Name);
            Assert.Equal(updatedUser.Email, returnUser.Email);

            var data = context.Users.Where(x => x.Id == updatedUser.Id).FirstOrDefault();
            Assert.Equal(3, (Int64)data.Id);
            Assert.Equal(updatedUser.Name, data.Name);
            Assert.Equal(updatedUser.Email, data.Email);
        }

        [Fact(DisplayName = "When calling put method with wrong Id, it should return BadRequest")]
        public async Task WhenCallingPutMethodWithWrongIdItShouldUpdateTheUsersEmail()
        {
            var context = Utils.setUpDatabase("update_test.db");

            var newUser = new User() { Id = null, Name = null, Email = null };
            var controller = new UserController(context);
            var result = controller.Update(0, newUser).Result as BadRequestObjectResult;
            var returnUser = result.Value;
            var statusCode = result.StatusCode;

            Assert.Equal("Cannot find the user id 0", returnUser);
            Assert.Equal(400, statusCode);
        }

        [Fact(DisplayName = "When calling put method with invalid email, it should return BadRequest")]
        public async Task WhenCallingPutMethodWithInvalidEmailItShouldUpdateTheUsersEmail()
        {
            var context = Utils.setUpDatabase("update_test.db");

            var listOfUser = new List<User>()
            {
                new User() { Id = 1, Name = "Norawich", Email = "newsnora@gmail.com"},
            };
            context.Users.AddRange(listOfUser);
            context.SaveChanges();
            var newUser = new User() { Id = null, Name = null, Email = "invalidEmail" };
            var controller = new UserController(context);

            var result = controller.Update(1, newUser).Result as BadRequestObjectResult;
            var returnUser = result.Value;
            var statusCode = result.StatusCode;

            Assert.Equal("Invalid Email", returnUser);
            Assert.Equal(400, statusCode);
        }

        [Theory(DisplayName = "When calling put method with null or empty name, it should not update name.")]
        [InlineData("")]
        [InlineData(null)]
        public async Task WhenCallingPutMethodWithNullOrEmptyNameItShouldNotUpdateName(string name)
        {
            var context = Utils.setUpDatabase("update_test.db");

            var listOfUser = new List<User>()
            {
                new User() { Id = 1, Name = "Norawich", Email = "newsnora@gmail.com"},
            };
            context.Users.AddRange(listOfUser);
            context.SaveChanges();
            var newUser = new User() { Id = null, Name = name };
            var controller = new UserController(context);

            var result = controller.Update(1, newUser).Result as OkObjectResult;
            var returnUser = result.Value as User;

            Assert.Equal("Norawich", returnUser.Name);
        }
    }
}
