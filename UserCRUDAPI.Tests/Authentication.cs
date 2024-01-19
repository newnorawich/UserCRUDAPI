using Infranstructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using UserCRUDAPI.Auth;
using UserCRUDAPI.Controllers;
using Xunit;

namespace UserCRUDAPI.Tests
{
    public class Authentication
    {
        [Fact(DisplayName = "When register the user, the new user should be registered.")]
        public async Task WhenRegisterTheUserTheNewUserShouldBeRegistered()
        {
            var context = Utils.setUpDatabase("auth_test.db");

            var userAuth = new UserAuthRequest()
            {
                Username = "new_norawich",
                Password = "P@ssw0rd",
            };
            var configuration = new Mock<IConfiguration>();
            var authController = new AuthController(configuration.Object, context);

            var result = authController.Register(userAuth).Result as OkObjectResult;
            var userNameResponse = result.Value as string;

            Assert.Equal("new_norawich", userNameResponse);
        }

        [Fact(DisplayName = "When register the user with the duplicate username, the new user should be registered.")]
        public async Task WhenRegisterTheUserWithTheDuplicateUsernameTheNewUserShouldBeRegistered()
        {
            var context = Utils.setUpDatabase("auth_test.db");

            var userAuth = new UserAuth() { Username = "new_norawich", HashPassword = "$2a$11$YVgegmnsJY57igSw5wvEuORk9I2gEgAkTf7wWemaWP.VqJxB80Wve" };
            await context.UserAuth.AddAsync(userAuth);
            await context.SaveChangesAsync();

            var duplicateUser = new UserAuthRequest()
            {
                Username = "new_norawich",
                Password = "123456789",
            };
            var configuration = new Mock<IConfiguration>();
            var authController = new AuthController(configuration.Object, context);

            var result = authController.Register(duplicateUser).Result as ConflictObjectResult;

            Assert.Equal(409, result.StatusCode);
            Assert.Equal("Username has been taken.", result.Value);
        }

        [Fact(DisplayName = "When login the correct user, it should return jwt.")]
        public async Task WhenLoginItShouldReturnJwt()
        {
            var context = Utils.setUpDatabase("auth_test.db");
            var userAuth = new UserAuthRequest()
            {
                Username = "new_norawich",
                Password = "P@ssw0rd",
            };
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(x => x.Value).Returns("my testing key for this assignment");

            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x.GetSection("AppSettings:Token")).Returns(configurationSection.Object);

            var authController = new AuthController(configuration.Object, context);
            var register = authController.Register(userAuth);

            var result = authController.LogIn(userAuth).Result as OkObjectResult;
            var jwtResponse = result.Value as string;

            var actualJwt = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibmV3X25vcmF3aWNoIiwiZXhwIjoxNzA1NjgzNjAwfQ.h_Rz0Tpvx5h-oq_zlP4_US-cq5EgR_LT1BOf-sH3vCk";

            Assert.Equal(actualJwt, jwtResponse);
        }

        [Theory(DisplayName = "When login the wrong username or password, it should response BadRequest.")]
        [InlineData("new_norawich", "1234")]
        [InlineData("new", "P@ssw0rd")]
        [InlineData("new", "1234")]
        public async Task WhenLoginTheWrongUsernameOrPasswordItShouldResponseBadRequest(string username, string password)
        {
            var context = Utils.setUpDatabase("auth_test.db");
            var userAuth = new UserAuthRequest()
            {
                Username = "new_norawich",
                Password = "P@ssw0rd",
            };
            var configuration = new Mock<IConfiguration>();
            var authController = new AuthController(configuration.Object, context);
            var register = authController.Register(userAuth);
            var user = new UserAuthRequest()
            {
                Username = username,
                Password = password
            };
            var result = authController.LogIn(user).Result as BadRequestObjectResult;

            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Username or Password is wrong.", result.Value);
        }
 
    }
}
