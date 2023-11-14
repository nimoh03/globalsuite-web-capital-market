using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Admin.Business;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Exceptions;
using GlobalSuite.Web.Attributes;
using GlobalSuite.Web.Controllers.Dto;

namespace GlobalSuite.Web.Controllers
{
    public partial class AdministratorController 
    {
        /// <summary>
        /// Get All Users 
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.Users)]
        public async Task<IHttpActionResult> CreateUser(UserFilter request)
        {
            var result = await _adminService.GetAllUsers(request);
            var response = _mapper.Map<List<UserResponse>>(result);
            return Ok(response);
        } /// <summary>
        /// Create a new User
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.Users)]
        public async Task<IHttpActionResult> CreateUser(UserRequest request)
        {
            var user = _mapper.Map<User>(request);
            var result = await _adminService.CreateUser(user);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        /// <summary>
        /// Get User profile
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.Users+"/profile")]
        public async Task<IHttpActionResult> Profile()
        {
            
            var profile = await _adminService.GetProfile();
            return Ok(profile);
        }
        
        /// <summary>
        /// Get All User Levels
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.Users+"/levels")]
        public async Task<IHttpActionResult> GetAllUserLevels()
        {
            var levels = await _adminService.GetUserLevels();
            return Ok(levels);
        }
        /// <summary>
        /// Change User's Fullname
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.Users+"/change/name")]
        public async Task<IHttpActionResult> ChangeFullName([FromBody] ChangeNameRequest request)
        {
            if (!ModelState.IsValid) return BadRequest();
           
              await _adminService.ChangeFullName(request.FullName, request.EmailAddress, request.BranchId);
            return Ok();
        }
        /// <summary>
        /// Suspend A User's Account
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.Users+"/suspend/{username}")]
        public async Task<IHttpActionResult> Suspend(string username)
        {
            try
            {
                var isSuspended = await _adminService.Suspend(username);
                if (!isSuspended) return BadRequest($"{username} cannot be suspended.");
                return Ok();
            }
            catch (AppException ex)
            {

                 return BadRequest(ex.Message);
            }
          
        }
        
        /// <summary>
        /// Unlock a Locked user account
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.Users+"/unlock/{username}")]
        public async Task<IHttpActionResult> Unlock(string username)
        {
            try
            {
                var isUnlocked = await _adminService.Unlock(username);
                if (!isUnlocked) return BadRequest($"{username} cannot be unlocked.");
                return Ok();
            }
            catch (AppException ex)
            {

                return BadRequest(ex.Message);
            }
            
        }
/// <summary>
/// Reset User's Password
/// </summary>
/// <param name="request"></param>
/// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.Users+"/reset-password")]
[AllowXRequestsEveryXSeconds(Name = "ResetPassword",
    Message = "You have performed this action more than {x} times in the last {n} seconds.",
    Requests = 2, Seconds = 30)]  public async Task<IHttpActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var isReset = await _adminService.ResetPassword(request.Username, request.Password);
                if (!isReset) return BadRequest($"Password reset not successful for {request.Username}.");
                return Ok();
            }
            catch (AppException ex)
            {

                return BadRequest(ex.Message);
            }

        }

/// <summary>
/// Self Service for Current User to Change It's Password
/// </summary>
/// <param name="request"></param>
/// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.Users+"/change-password")]
        [AllowXRequestsEveryXSeconds(Name = "ChangePassword",
            Message = "You have performed this action more than {x} times in the last {n} seconds.",
            Requests = 2, Seconds = 30)]
        public async Task<IHttpActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _adminService.ChangePassword(request.OldPassword, request.Password);
                if (!result.IsSuccess) return BadRequest(result.ToString());
                return Ok();
            }
            catch (AppException ex)
            {

                return BadRequest(ex.Message);
            }

        }
    }
}
