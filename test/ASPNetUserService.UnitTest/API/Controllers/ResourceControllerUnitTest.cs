using ASPNetUserService.API.Controllers;
using ASPNetUserService.UnitTest.API.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace ASPNetUserService.UnitTest.API.Controllers
{
    [TestFixture]
    public class ResourceControllerUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetMessage_UserIdentitySet_ReturnMessageWithUserIdentityName()
        {
            const string user = "kent";
            var expectedContentResult = $"{user} has been successfully authenticated.";

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(h => h.User.Identity.Name).Returns(user);

            var controllerContext = ControllerHelpers.GetControllerContext(httpContext.Object);
            var resourceController = new ResourceController()
            {
                ControllerContext = controllerContext,
            };

            var result = resourceController.GetMessage();

            var message = result as ContentResult;
            Assert.AreEqual(expectedContentResult, message.Content);

            httpContext.Verify(h => h.User.Identity.Name, Times.Once());
        }
    }
}