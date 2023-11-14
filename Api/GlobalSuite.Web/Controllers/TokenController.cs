using GlobalSuite.Core.Exceptions;
using GlobalSuite.Web.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Admin.Business;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core;
using GlobalSuite.Core.Admin;

using GlobalSuite.Web.Attributes;
using Serilog;

namespace GlobalSuite.Web.Controllers
{
    [RoutePrefix("/token")]
    public class TokenController : ApiController
    {
        private string _strErrMessage = string.Empty;
        private int _intNoNonSuccessLogin = 0;
        private readonly IAdminService _adminService;
        private readonly ILogger _logger=Log.ForContext<TokenController>();
        public TokenController(IAdminService adminService)
        {
            _adminService=adminService  ;
        }
        [HttpPost]
        [AllowAnonymous]
        // [ValidateAntiForgeryToken]
         [AllowXRequestsEveryXSeconds(Name = "Login",
            Message = "You have performed this action more than {x} times in the last {n} seconds.",
            Requests = 3, Seconds = 60)]
        public async Task<IHttpActionResult> Login([FromBody]AuthenticationRequest request)
        {
            if(!ModelState.IsValid) return BadRequest("Invalid Username/Password.");

            try
            {
                #region Version Incompatibility

                //int intRetMajor = Assembly.GetExecutingAssembly().GetName().Version.Major;

                //int intRetMajorRevision = Assembly.GetExecutingAssembly().GetName().Version.MajorRevision;
                //int intRetMinor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
                //int intRetMinorRevision = Assembly.GetExecutingAssembly().GetName().Version.MinorRevision;
                //ParameterTable oParameterTable = new ParameterTable();
                //if ((!oParameterTable.GetParameterTable()) || oParameterTable.Major != intRetMajor
                //    || oParameterTable.MajorRevision != intRetMajorRevision || oParameterTable.Minor != intRetMinor
                //    || oParameterTable.MinorRevision != intRetMinorRevision)
                //{
                //    return Content(HttpStatusCode.BadRequest,new AuthenticationErrorResponse("Cannot Login The Database Version Is Not Compatible With The Application Version"));
                //}

                #endregion

                #region Triggers Missing

                var oAudit = new Audit();
                if (oAudit.GetAllTablesWithoutTriggers().Tables[0].Rows.Count > 0)
                {
                    StreamWriter writerTableName;
                    writerTableName = File.CreateText(@"C:\GlobalSuiteFolder\" + @"DatabaseAutoMissing.txt");
                    writerTableName.WriteLine("List Of Database Automation Processes Missing");
                    foreach (DataRow rwTableName in oAudit.GetAllTablesWithoutTriggers().Tables[0].Rows)
                    {
                        var builderTableName = new StringBuilder(1000); // None row is bigger than this
                        builderTableName.Append(rwTableName["name"]);
                        writerTableName.WriteLine(builderTableName.ToString());
                    }

                    writerTableName.Close();
                    writerTableName.Dispose();
                    _strErrMessage =
                        "Some Database Automation Processes Is Missing, Please Contact Your Software Vendor With the DatabaseAutoMissing Text File";
                   _logger.Information(_strErrMessage);
                    System.Diagnostics.Process.Start(@"C:\GlobalSuiteFolder\" + @"DatabaseAutoMissing.txt");
                    return BadRequest(_strErrMessage);
                }

                #endregion

                var user = await _adminService.FindUser(request.Username, request.Password);
                if (user == null || string.IsNullOrEmpty(user.FullName))
                    return BadRequest("Invalid Username/Password.");
                var strLockPassword = "";
                var oGlParamLockPass = new GLParam
                {
                    Type = "LOCKPASS"
                };
                strLockPassword = oGlParamLockPass.CheckParameter();
                var oGlParam = new GLParam();
                var strSetPasswordExpire = oGlParam.PayablePayChargeVAT.Trim();
                var datCurrentDate = GeneralFunc.GetTodayDate();

                if (user != null)
                {
                    #region Check User Login Deactivated,Password Locked,Password Expired Or System Expired

                    if (user.UserDeactivated.Trim() == "Y")
                    {
                        return Content(HttpStatusCode.BadRequest,
                            new AuthenticationErrorResponse("Cannot Login User Name Has Been Suspended." +
                                                            "Please Contact Your System Administrator"));
                    }
                    else if (user.PassLockup)
                    {
                        return Content(HttpStatusCode.BadRequest,
                            "Cannot Login User Profile Has Been Locked.Please Contact Your System Administrator");
                    }
                    else if (user.ChkExpiredPassWithSpecificDate(user, datCurrentDate))
                    {
                        return Content(HttpStatusCode.BadRequest,
                            new AuthenticationErrorResponse("User Password Set With Specific Expiry Date Has Expired," +
                                                            " Please Contact Your System Administrator"));
                    }
                    else if (user.ChkExpiredPassWithValidityDay(user, datCurrentDate))
                    {
                        return Content(HttpStatusCode.BadRequest, new AuthenticationErrorResponse(
                            "User Password Set With Validity Days Has Expired, " +
                            "Please Contact Your System Administrator"));
                    }
                    else if (strSetPasswordExpire == "Y" || GeneralFunc.ExpireDate <= datCurrentDate)
                    {
                        if (strSetPasswordExpire != "Y")
                        {
                            oGlParam.FlagExpireStatus();
                        }

                        return Content(HttpStatusCode.BadRequest,
                            new AuthenticationErrorResponse(
                                "User Password Has Expired, Please Contact Your Software Provider"));
                    }

                    #endregion

                    else
                    {
                        #region Assign User Name,Company Name,User Branch,Default Branch

                        GeneralFunc.UserName = user.UserNameAccount.Trim().ToUpper();
                        GeneralFunc.CompanyNumber = 1;
                        GeneralFunc.UserBranchNumber = user.GetBranchId();
                        var oBranch = new Branch();
                        GeneralFunc.DefaultBranch = oBranch.DefaultBranch;

                        #endregion

                        #region Remind Password Change

                        if ((user.PswExpDate != DateTime.MinValue) && (user.PswExpDate > datCurrentDate) &&
                            (user.PswExpDate.Subtract(datCurrentDate).TotalDays < 5))
                        {
                            return Content(HttpStatusCode.BadRequest, new AuthenticationErrorResponse(
                                "Your Password Set With Specific Expiry Date Will Expire In Less " +
                                "Than Five Days.Change Your Password Now!", requirePasswordChange: true));
                        }
                        else if ((user.PassAge != 0) && (user.PassChangeDate != DateTime.MinValue) &&
                                 (user.PassChangeDate.AddDays(user.PassAge).Subtract(datCurrentDate).TotalDays < 5))
                        {
                            return Content(HttpStatusCode.BadRequest,
                                new AuthenticationErrorResponse("Your Password Set With Validity Days Will Expire" +
                                                                " In Less Than Five Days.Do You Want To Change Your Password Now?",
                                    requirePasswordChange: true));

                        }

                        #endregion

                        #region WeekEnd Access, Late Night Access,Multi WorkStation Accesss Profile Checks And New User Change Password Flag

                        var userProfile = new UserProfile();
                        userProfile.UserName = user.UserNameAccount;
                        if (!userProfile.GetUserProfile())
                        {
                            return Content(HttpStatusCode.BadRequest,
                                new AuthenticationErrorResponse("Cannot Login User Profile Does Not Exist"));
                        }

                        if (!userProfile.WkEndAccess)
                        {
                            if (datCurrentDate.DayOfWeek == DayOfWeek.Saturday ||
                                datCurrentDate.DayOfWeek == DayOfWeek.Sunday)
                            {
                                _strErrMessage = "User Does Not Have WeekEnd Access";
                                return Content(HttpStatusCode.BadRequest,
                                    new AuthenticationErrorResponse(_strErrMessage));
                            }
                        }

                        if (!userProfile.LateNight && (DateTime.Now.Hour > 21 || DateTime.Now.Hour < 7))
                        {
                            return Content(HttpStatusCode.BadRequest,
                                new AuthenticationErrorResponse(
                                    "Cannot Login.Your User Profile Does Not Allow Late Night Access"));
                        }

                        if (!userProfile.MultStation && user.LoggedIn)
                        {
                            return Content(HttpStatusCode.BadRequest,
                                new AuthenticationErrorResponse(
                                    "Cannot Login User Currently Logon On Another System/Session"));
                        }

                        if (user.GetCreatedNewStatus().Trim() != "N")
                        {
                            //TODO
                        }

                        #endregion

                        #region Logged In Before

                        if (!user.LoggedInAction())
                        {
                            return Content(HttpStatusCode.BadRequest,
                                new AuthenticationErrorResponse("Cannot Log On User As Logged In"
                                ));

                        }

                        #endregion

                        //    #region Update Account List
                        //    Account oAccount = new Account();
                        //    GeneralFunc.AccountNumberWithDetail oAccountNumberWithDetail = new GeneralFunc.AccountNumberWithDetail();
                        //    GeneralFunc.lstAccountNumberWithDetail = oAccountNumberWithDetail.ConvertDataSetToList(oAccount.GetAll());
                        //#endregion

                        var response = GetToken(user);
                        return Ok(response);
                    }

                }
                else
                {
                    #region After Unsuccessful Login, Lockup The Password Or Log In Lock Or Invalid

                    if (!user.CheckUserLocked())
                    {
                        user.UnSuccessLogin();
                        _intNoNonSuccessLogin = _intNoNonSuccessLogin + 1;
                        if (_intNoNonSuccessLogin == 3)
                        {
                            if (strLockPassword.Trim() == "YES")
                            {
                                user.Lockedup();
                                return Content(HttpStatusCode.BadRequest,
                                    new AuthenticationErrorResponse(
                                        "User Name Or Password Is InValid!Your User Profile Will Be Locked And The Application Terminated"));
                            }
                            else
                            {
                                return Content(HttpStatusCode.BadRequest,
                                    new AuthenticationErrorResponse(
                                        "You Are Not An Authorised User!The Application Will Be Terminated"));
                            }
                        }
                        else
                        {
                            return Content(HttpStatusCode.BadRequest,
                                new AuthenticationErrorResponse("User Name Or Password Is InValid"));
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.BadRequest,
                            new AuthenticationErrorResponse(
                                "Cannot Login User Profile Has Been Locked.Please Contact Your System Administrator"));
                    }

                    #endregion
                }



            }
            catch (AppUnAuthorizedException authorizedException)
            {
                _logger.Error(authorizedException.Message, authorizedException);
            }
            catch (Exception ex)
            {
                _logger.Error<TokenController>(ex, ex.Message,null);
            }
            return BadRequest("Invalid Username/Password");
        }

        private static AuthenticationResponse GetToken(User user)
        {
            var key = ConfigurationManager.AppSettings["JwtKey"];

            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserNameAccount),
                new Claim(Constants.GClaimTypes.CompanyName, "1"),
                new Claim(Constants.GClaimTypes.BranchNumber, user.GetBranchId())
            };
            var token = new JwtSecurityToken(issuer, //Issure    
                            issuer,  //Audience    
                            permClaims,
                            expires: DateTime.Now.AddDays(365),
                            signingCredentials: credentials);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return new AuthenticationResponse { Token = jwtToken };
        }
    }
}
