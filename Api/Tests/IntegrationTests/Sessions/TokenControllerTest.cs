// using GlobalSuite.Web.Controllers;
// using GlobalSuite.Web.Models;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using Moq;
// using System;
// using System.Net;
// using System.Threading.Tasks;
// using System.Web.Http;
// using System.Web.Http.Results;
// using Admin.Business;
// using GlobalSuite.Core.Admin;
//
// namespace IntegrationTests.Sessions
// {
//     [TestClass]
//     public class TokenControllerTest
//     {
//         
//         [TestMethod]
//         public async Task Should_Return_BadRequest_StatusCode_For_Invalid_User()
//         {
//             User user = null;
//             var userService = new Mock<IAdminService>();
//             userService.Setup(x => x.FindUser("s", "2345"))
//                 .ReturnsAsync(user = new User { BranchId = "1" });
//             var controller = new TokenController(userService.Object);
//
//             // Act
//             IHttpActionResult actionResult = await controller.Login(new AuthenticationRequest { Username = "s" });
//             var contentResult = actionResult as BadRequestErrorMessageResult;
//             Assert.AreEqual(contentResult.Message, "Invalid Username/Password.");
//         }
//     }
// }
