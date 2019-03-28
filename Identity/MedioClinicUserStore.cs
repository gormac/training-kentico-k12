using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using CMS.Helpers;
using CMS.Membership;

using Business.Services.Context;
using Identity.Extensions;
using Identity.Helpers;
using Identity.Models;
using Identity.Proxies;

namespace Identity
{
    public class MedioClinicUserStore : 
        IUserPasswordStore<MedioClinicUser, int>,
        IUserLockoutStore<MedioClinicUser, int>,
        IUserTwoFactorStore<MedioClinicUser, int>,
        IUserRoleStore<MedioClinicUser, int>,
        IUserEmailStore<MedioClinicUser, int>,
        IUserLoginStore<MedioClinicUser, int>,
        IUserSecurityStampStore<MedioClinicUser, int>
    {
        private ISiteContextService SiteContextService { get; }

        private IKenticoUserStore KenticoUserStore { get; }

        public MedioClinicUserStore(ISiteContextService siteContextService, IKenticoUserStore kenticoUserStore)
        {
            SiteContextService = siteContextService ?? throw new ArgumentNullException(nameof(siteContextService));
            kenticoUserStore = kenticoUserStore ?? throw new ArgumentNullException(nameof(kenticoUserStore));
        }

        public Task CreateAsync(MedioClinicUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userInfo = user.ToUserInfo();

            userInfo.UserGUID = user.GUID;
            userInfo.PasswordFormat = UserInfoProvider.NewPasswordFormat;
            userInfo.UserPasswordLastChanged = DateTime.Now;
            userInfo.IsExternal = user.IsExternal;

            UserInfoProvider.SetUserInfo(userInfo);
            UserInfoProvider.AddUserToSite(userInfo.UserName, SiteContextService.SiteName);

            return Task.FromResult(0);
        }

        public Task DeleteAsync(MedioClinicUser user) =>
            KenticoUserStore.DeleteAsync(user);

        public Task<MedioClinicUser> FindByIdAsync(int userId) =>
            Task.FromResult(UserInfoProvider
                .GetUserInfo(userId)
                .ToMedioClinicUser(SiteContextService));

        public Task<MedioClinicUser> FindByNameAsync(string userName) =>
            Task.FromResult(UserInfoProvider
                .GetUserInfo(userName)
                .ToMedioClinicUser(SiteContextService));

        public Task<MedioClinicUser> FindByEmailAsync(string email) =>
            Task.FromResult(UserInfoProvider
                .GetUsers()
                .WhereEquals("Email", email)
                .TopN(1)
                .FirstOrDefault()
                .ToMedioClinicUser(SiteContextService));

        public Task<MedioClinicUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            var loginInfo = ExternalLoginInfoProvider.GetExternalLogins()
                                                .WhereEquals("LoginProvider", login.LoginProvider)
                                                .WhereEquals("IdentityKey", login.ProviderKey)
                                                .TopN(1)
                                                .FirstOrDefault();

            return loginInfo != null ? FindByIdAsync(loginInfo.UserID) : Task.FromResult<MedioClinicUser>(null);
        }

        public Task UpdateAsync(MedioClinicUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userInfo = UserInfoProvider.GetUserInfo(user.Id);

            if (userInfo == null)
            {
                throw new Exception(ResHelper.GetString("general.usernotfound"));
            }

            UserHelper.UpdateUserInfo(ref userInfo, user);

            UserInfoProvider.SetUserInfo(userInfo);

            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(MedioClinicUser user) =>
            KenticoUserStore.GetPasswordHashAsync(user);

        public Task<bool> HasPasswordAsync(MedioClinicUser user) =>
            KenticoUserStore.HasPasswordAsync(user);

        public Task SetPasswordHashAsync(MedioClinicUser user, string passwordHash) =>
            KenticoUserStore.SetPasswordHashAsync(user, passwordHash);

        public Task<int> GetAccessFailedCountAsync(MedioClinicUser user) =>
            KenticoUserStore.GetAccessFailedCountAsync(user);

        public Task<bool> GetLockoutEnabledAsync(MedioClinicUser user) =>
            KenticoUserStore.GetLockoutEnabledAsync(user);

        public Task<DateTimeOffset> GetLockoutEndDateAsync(MedioClinicUser user) =>
            KenticoUserStore.GetLockoutEndDateAsync(user);

        public Task<int> IncrementAccessFailedCountAsync(MedioClinicUser user) =>
            KenticoUserStore.IncrementAccessFailedCountAsync(user);

        public Task ResetAccessFailedCountAsync(MedioClinicUser user) =>
            KenticoUserStore.ResetAccessFailedCountAsync(user);

        public Task SetLockoutEnabledAsync(MedioClinicUser user, bool enabled) =>
            KenticoUserStore.SetLockoutEnabledAsync(user, enabled);

        public Task SetLockoutEndDateAsync(MedioClinicUser user, DateTimeOffset lockoutEnd) =>
            KenticoUserStore.SetLockoutEndDateAsync(user, lockoutEnd);

        public Task<bool> GetTwoFactorEnabledAsync(MedioClinicUser user) =>
            KenticoUserStore.GetTwoFactorEnabledAsync(user);

        public Task SetTwoFactorEnabledAsync(MedioClinicUser user, bool enabled) =>
            KenticoUserStore.SetTwoFactorEnabledAsync(user, enabled);

        public Task<IList<string>> GetRolesAsync(MedioClinicUser user) =>
            KenticoUserStore.GetRolesAsync(user);

        public Task<bool> IsInRoleAsync(MedioClinicUser user, string roleName) =>
            KenticoUserStore.IsInRoleAsync(user, roleName);

        public Task RemoveFromRoleAsync(MedioClinicUser user, string roleName) =>
            KenticoUserStore.RemoveFromRoleAsync(user, roleName);

        public Task AddToRoleAsync(MedioClinicUser user, string roleName) =>
            KenticoUserStore.AddToRoleAsync(user, roleName);

        public Task SetEmailAsync(MedioClinicUser user, string email) =>
            KenticoUserStore.SetEmailAsync(user, email);

        public Task<string> GetEmailAsync(MedioClinicUser user) =>
            KenticoUserStore.GetEmailAsync(user);

        public Task<bool> GetEmailConfirmedAsync(MedioClinicUser user) =>
            KenticoUserStore.GetEmailConfirmedAsync(user);

        public Task SetEmailConfirmedAsync(MedioClinicUser user, bool confirmed) =>
            KenticoUserStore.SetEmailConfirmedAsync(user, confirmed);

        public Task AddLoginAsync(MedioClinicUser user, UserLoginInfo login) =>
            KenticoUserStore.AddLoginAsync(user, login);

        public Task RemoveLoginAsync(MedioClinicUser user, UserLoginInfo login) =>
            KenticoUserStore.RemoveLoginAsync(user, login);

        public Task<IList<UserLoginInfo>> GetLoginsAsync(MedioClinicUser user) =>
            KenticoUserStore.GetLoginsAsync(user);

        public Task SetSecurityStampAsync(MedioClinicUser user, string stamp) =>
            KenticoUserStore.SetSecurityStampAsync(user, stamp);

        public Task<string> GetSecurityStampAsync(MedioClinicUser user) =>
            KenticoUserStore.GetSecurityStampAsync(user);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
