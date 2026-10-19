using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Controllers;
using RaceDayAPI.Models;
using System.Threading.Tasks;

namespace RaceDayAPI.Tests
{
    public class AuthTests
    {
        // TEST 1: Registration succeeds with valid data
        [Fact]
        public async Task Register_ValidUser_ReturnsOk()
        {
            // 1. Arrange (Fake the UserManager)
            var mockStore = new Mock<IUserStore<User>>();
            var mockUserManager = new Mock<UserManager<User>>(mockStore.Object, null, null, null, null, null, null, null, null);

            // Tell the fake manager to return "Success" when asked to create a user
            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);

            var controller = new AuthController(mockUserManager.Object, null);

            var dto = new AuthController.RegisterDto
            {
                Email = "test@test.com",
                Password = "Password123!",
                FullName = "Test User",
                Role = "Participant"
            };

            // 2. Act
            var result = await controller.Register(dto);

            // 3. Assert
            Assert.IsType<OkObjectResult>(result);
        }

        // TEST 2: Registration fails if the role is invalid
        [Fact]
        public async Task Register_InvalidRole_ReturnsBadRequest()
        {
            // 1. Arrange
            var controller = new AuthController(null, null);
            var dto = new AuthController.RegisterDto { Role = "Hacker" }; // Invalid role

            // 2. Act
            var result = await controller.Register(dto);

            // 3. Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}