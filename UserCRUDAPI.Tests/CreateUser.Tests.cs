using Infranstructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCRUDAPI.Controllers;
using Xunit;

namespace UserCRUDAPI.Tests
{
    public class CreateUser
    {
        [Fact(DisplayName = "When calling post method to create user, it should create user in database and return that user.")]
        public async Task WhenCallingPostMethodToCreateUserItShouldCreateUserAndThatUser()
        {
            var context = Utils.setUpDatabase("create_test.db");

            var user = new User() { Id = 1, Name = "Norawich", Email = "newsnora@gmail.com" };

            var controller = new UserController(context);
            var result = controller.Create(user).Result as ObjectResult;
            var returnUser = result.Value;

            Assert.Equal(new List<User> { user }, context.Users);
            Assert.Equal(user, returnUser);
        }

        [Fact(DisplayName = "When calling post method to create user that id has already existed, it should not create any new user and return conflict.")]
        public async Task WhenCallingPostMethodToCreateUserThatIdHasAlreadyExistedItShouldNotCreateAnyNewUserAndReturnConflict()
        {
            var context = Utils.setUpDatabase("create_test.db");

            var user = new User() { Id = 1, Name = "Norawich", Email = "newsnora@gmail.com" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            var result = controller.Create(user).Result as ConflictObjectResult;
            var returnString = result.Value;
            var statusCode = result.StatusCode;

            Assert.Equal(new List<User> { user }, context.Users);
            Assert.Equal(409, statusCode);
            Assert.Equal("Id already existed.", returnString);
        }

        [Fact(DisplayName = "When calling post method to create user that id is 0, it should not create any new user and return BadRequest.")]
        public async Task WhenCallingPostMethodToCreateUserThatIdIs0ItShouldNotCreateAnyNewUserAndReturnBadRequest()
        {
            var context = Utils.setUpDatabase("create_test.db");

            var user = new User() { Id = 0, Name = "Norawich", Email = "newsnora@gmail.com" };

            var controller = new UserController(context);
            var result = controller.Create(user).Result as BadRequestObjectResult;
            var returnString = result.Value;
            var statusCode = result.StatusCode;

            Assert.Equal(400, statusCode);
            Assert.Equal("Id must not be 0", returnString);
        }

        [Fact(DisplayName = "When calling post method with invalid email, it should return BadRequest")]
        public async Task WhenCallingPutMethodWithInvalidEmailItShouldUpdateTheUsersEmail()
        {
            var context = Utils.setUpDatabase("create_test.db");
            var newUser = new User() { Name = "Norawich", Email = "invalidEmail" };
            var controller = new UserController(context);

            var result = controller.Create(newUser).Result as BadRequestObjectResult;
            var returnUser = result.Value;
            var statusCode = result.StatusCode;

            Assert.Equal("Invalid Email", returnUser);
            Assert.Equal(400, statusCode);
        }

        [Theory(DisplayName = "When calling post method with null or empty name, it should return BadRequest")]
        [InlineData("")]
        [InlineData(null)]
        public async Task WhenCallingPutMethodWithNullOrEmptyItShouldUpdateTheUsersEmail(string name)
        {
            var context = Utils.setUpDatabase("create_test.db");
            var newUser = new User() { Name = name };
            var controller = new UserController(context);

            var result = controller.Create(newUser).Result as BadRequestObjectResult;
            var returnUser = result.Value;
            var statusCode = result.StatusCode;

            Assert.Equal("Name must not be null or empty", returnUser);
            Assert.Equal(400, statusCode);
        }
    }
}
