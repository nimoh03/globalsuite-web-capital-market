using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Exceptions;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.Admin
{
    public partial class AdminService 
    {
        private readonly string _encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
        public async Task<List<User>> GetAllUsers(UserFilter filter)
        {
            return await Task.Run(() =>
            {
                var oUser = new User();
                var ds = oUser.GetAll();
                if (filter.OnlyActive)
                   ds= oUser.GetAllActive();
                if (filter.OnlySuspended)
                    ds = oUser.GetAllSuspend();
                return ds.Tables[0].ToList<User>();
            });

        }
        public async Task<ResponseResult> CreateUser(User oUser)
        {
          return  await Task.Run(() =>
            {
              var status=  oUser.Save();
              if (status != DataGeneral.SaveStatus.Saved) return ResponseResult.Error(status.ToString());
              try
              {
                  var oUserProfile = new UserProfile()
                  {
                      UserName = oUser.UserNameAccount.ToUpper()
                  };
                  oUserProfile.GetUserProfile();
                  oUserProfile.Save();
              }
              catch (Exception e)
              {
                  Logger.Error(e.Message,e);
                  return  ResponseResult.Success("User Account created but user profile cannot be created.");

              }


              return  ResponseResult.Success();
            });
        }
        public async Task ChangeFullName(string fullName, string emailAddress, string branchId)
        {
           var oUser=new User
           {
               FullName = fullName,
               EmailAddress = emailAddress,
               BranchId = branchId,
               UserNameAccount = GeneralFunc.UserName
           };
           await Task.Run(() =>
            {
                var isSaved = oUser.ChangeFullName();
                if (!isSaved) throw new AppException("FullName cannot be changed at the moment. Try again.");
            });
        }

        public async Task<User> FindUser(string username, string password)
        {
            var oUser = new User
            {
                UserNameAccount = username.ToUpper()
            };
         return   await Task.Run(() =>
            {
                
                var isUser = oUser.GetUser(password);
                return isUser ? oUser : null;
            });
             
        }

        public async Task<UserProfile> GetProfile()
        {
            var oUserProfile = new UserProfile
            {
                UserName = GeneralFunc.UserName
            };
            await Task.Run(() =>
            {
               var isProfile= oUserProfile.GetUserProfile();
                return isProfile ? oUserProfile : throw new AppUnAuthorizedException();
            });
            return oUserProfile;    
        }

        public async Task<User> GetUser(string username)
        {
            
                var oUser = new User
                {
                    UserNameAccount = username.ToUpper()
                };
                await Task.Run(() =>
                {
                    var isUser = oUser.GetUserNoPassword();
                    if (isUser) return oUser;
                    throw new AppUnAuthorizedException();
                });
                return oUser;
             

        }

        public async Task<List<UserLevel>> GetUserLevels()
        {
            var oUser = new UserLevel(); 
          return  await Task.Run(() => oUser.GetAll().Tables[0].ToList<UserLevel>()); 
        }

        public async Task<bool> ResetPassword(string username, string password)
        {
            var oUser = new User
            {
                UserNameAccount = username.ToUpper(),
                Password = StringCipher.Encrypt(password, _encryptionKey)
            };
            return await Task.Run(() => oUser.ResetPassword());
        }
        public async Task<ResponseResult> ChangePassword(string oldPassword, string password)
        {
            if(oldPassword.ToLower().Equals(password.ToLower())) return ResponseResult.Error("Old and New password cannot be the same.");
            var oUser = new User
            {
                UserNameAccount = GeneralFunc.UserName.Trim(),
                Password = password
            };
            return await Task.Run( async () =>
            {
                var result =await FindUser(oUser.UserNameAccount, oldPassword);
                if(result ==null) return ResponseResult.Error("Password does not match.");
                var isChanged = oUser.ChangePassword();
              return  isChanged
                    ? ResponseResult.Success()
                    : ResponseResult.Error("Password cannot be changed at the moment.");
            });
        }
        public async Task<bool> Suspend(string username)
        {
            var uid = username.ToUpper();
            if(uid == GeneralFunc.UserName) throw new AppException("You cannot suspend self.");
            var oUser = new User
            {
                UserNameAccount = uid
            };
            return await Task.Run(() => oUser.Suspend());
        }
        public async Task<bool> Unlock(string username)
        {
            var uid = username.ToUpper();
            if (uid == GeneralFunc.UserName) throw new AppException("You cannot unlock self.");
            var oUser = new User
            {
                UserNameAccount = uid
            };
            return await Task.Run(() => oUser.UnLocked());
        }
    }
}
