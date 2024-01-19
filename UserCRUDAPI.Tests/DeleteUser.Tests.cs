using Infranstructure;
using Microsoft.AspNetCore.Mvc;
using UserCRUDAPI.Controllers;
using Xunit;

namespace UserCRUDAPI.Tests
{
    public class DeleteUser
    {
        [Fact(DisplayName = "When calling delete method, it should delete the user.")]
        public async Task WhenCallingDeleteMethodItShouldDeleteTheUser()
        {
            var context = Utils.setUpDatabase("delete_test.db");

            var user = new User() { Id = 1, Name = "Norawich", Email = "newsnora@gmail.com" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            var result = controller.Delete(1).Result as OkObjectResult;
            var returnUser = result.Value;

            Assert.Equal("Remove Successfully", returnUser);
        }

        [Fact(DisplayName = "When calling delete method by another Id, it should not delete user and return BadRequest.")]
        public async Task WhenCallingDeleteMethodByAnotherIdItShouldNotDeleteUserAndReturnBadRequest()
        {
            var context = Utils.setUpDatabase("delete_test.db");

            var user = new User() { Id = 1, Name = "Norawich", Email = "newsnora@gmail.com" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            var result = controller.Delete(0).Result as BadRequestObjectResult;
            var returnUser = result.Value;
            var statusCode = result.StatusCode;

            Assert.Equal(400, statusCode);
            Assert.Equal("Cannot find the user id 0", returnUser);
        }
    }
}
