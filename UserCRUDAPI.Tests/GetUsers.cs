using Infranstructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Dynamic;
using UserCRUDAPI.Controllers;
using Xunit;

namespace UserCRUDAPI.Tests
{
    public class GetUsers
    {
        [Fact(DisplayName = "When calling get method, it should return list of user.")]
        public async Task WhenCallingGetMethodItShouldReturnListOfUser()
        {
            var context = Utils.setUpDatabase("read_test.db");

            var listOfUser = new List<User>()
            {
                new User() { Id = 1, Name = "Norawich", Email = "newsnora@gmail.com"},
                new User() { Id = 2, Name = "New12345", Email = "New12345@gmail.com"}
            };
            context.Users.AddRange(listOfUser);
            context.SaveChanges();

            var controller = new UserController(context);
            var result = controller.Get().Result as OkObjectResult;
            var returnUser = result.Value as IEnumerable<User>;

            Assert.Equal(listOfUser, returnUser);
        }

        [Fact(DisplayName = "When calling get method by partial name, it should return that user.")]
        public async Task WhenCallingGetMethodByPartialNameItShouldReturnThatUser()
        {
            var context = Utils.setUpDatabase("read_test.db");

            var listOfUser = new List<User>()
            {
                new User() { Id = 1, Name = "Norawich", Email = "newsnora@gmail.com"},
                new User() { Id = 2, Name = "New12345", Email = "New12345@gmail.com"}
            };
            context.Users.AddRange(listOfUser);
            context.SaveChanges();

            var user = new User() { Id = 1, Name = "Norawich", Email = "newsnora@gmail.com" };
            var controller = new UserController(context);
            var result = controller.GetByName("rawic").Result as OkObjectResult;
            var returnUser = result.Value as List<User>;
            
            Assert.Equal(user.Id, returnUser.FirstOrDefault().Id);
            Assert.Equal(user.Name, returnUser.FirstOrDefault().Name);
            Assert.Equal(user.Email, returnUser.FirstOrDefault().Email);
        }
    }
}
