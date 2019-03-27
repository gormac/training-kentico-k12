using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;

using CMS.Membership;

using Business.Services.Context;
using Identity.Models;
using System.Threading.Tasks;

namespace Identity
{
    public class UserStore : IUserPasswordStore<User, int>,
                             IUserLockoutStore<User, int>,
                             IUserTwoFactorStore<User, int>,
                             IUserRoleStore<User, int>,
                             IUserEmailStore<User, int>,
                             IUserLoginStore<User, int>,
                             IUserSecurityStampStore<User, int>
    {
        private ISiteContextService SiteContextService { get; }

        public UserStore(ISiteContextService siteContextService)
        {
            SiteContextService = siteContextService;
        }


        public Task<User> FindByIdAsync(int userId) =>
            Task.FromResult(UserInfoProvider
                .GetUsers()
                .OnSite(SiteContextService.SiteName)
                .WhereEquals("UserId", userId)
                .TypedResult
                .Select(user => UserPropertyMapper(user))
                .FirstOrDefault());

        public Task CreateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userInfo = new UserInfo()
            {
                UserName = user.UserName,
                FullName = UserInfoProvider.GetFullName(user.FirstName, null, user.LastName),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Enabled = user.Enabled,
                UserGUID = user.GUID,
                PasswordFormat = UserInfoProvider.NewPasswordFormat,
                UserPasswordLastChanged = DateTime.Now,
                IsExternal = user.IsExternal,
                UserSecurityStamp = user.SecurityStamp
            };

            userInfo.UserNickName = userInfo.GetFormattedUserName(true);

            userInfo.SetValue("UserPassword", user.PasswordHash);
            userInfo.SetValue("DateOfBirth")
        }

        Func<UserInfo, User> UserPropertyMapper = user =>
            new User()
            {
                UserId = user.UserID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.GetDateTimeValue("DateOfBirth", DateTime.MinValue),
                Gender = (Gender)user.UserSettings.UserGender,
                City = user.GetStringValue("City", string.Empty),
                Street = user.GetStringValue("Street", string.Empty),
                Email = user.Email,
                Phone = user.UserSettings.UserPhone,
                Nationality = user.GetStringValue("Nationality", string.Empty)
            };


    }
}
