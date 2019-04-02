using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using Kentico.Membership;

using Business.Identity.Models;
using CMS.Membership;
using CMS.Helpers;

namespace Business.Identity
{
    public class MedioClinicUserManager : UserManager<MedioClinicUser, int>, IMedioClinicUserManager
    {
        public MedioClinicUserManager(IMedioClinicUserStore medioClinicUserStore) : base(medioClinicUserStore)
        {
            PasswordValidator = new PasswordValidator();
            UserLockoutEnabledByDefault = false;
            EmailService = new EmailService();
            UserValidator = new UserValidator<MedioClinicUser, int>(this);

            // Registration: Confirmed registration
            UserTokenProvider = new EmailTokenProvider<MedioClinicUser, int>();
        }

        /// <summary>
        /// Updates the user password.
        /// </summary>
        /// <param name="passwordStore">Unused implementation of UserPasswordStore.</param>
        /// <param name="user">User.</param>
        /// <param name="newPassword">New password in plain text format.</param>
        protected override async Task<IdentityResult> UpdatePassword(IUserPasswordStore<MedioClinicUser, int> passwordStore, MedioClinicUser user, string newPassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result = await PasswordValidator.ValidateAsync(newPassword);
            if (!result.Succeeded)
            {
                return result;
            }

            UserInfo userInfo = UserInfoProvider.GetUserInfo(user.Id);

            if (userInfo == null)
            {
                user.GUID = Guid.NewGuid();
                user.PasswordHash = UserInfoProvider.GetPasswordHash(newPassword, UserInfoProvider.NewPasswordFormat, user.GUID.ToString());
            }
            else
            {
                UserInfoProvider.SetPassword(userInfo, newPassword);
                user.PasswordHash = ValidationHelper.GetString(userInfo.GetValue("UserPassword"), string.Empty);
                await UpdateSecurityStampInternal(user);
            }

            return IdentityResult.Success;
        }


        /// <summary>
        /// Verifies the user password.
        /// </summary>
        /// <param name="store">Unused implementation of UserPasswordStore.</param>
        /// <param name="user">User.</param>
        /// <param name="password">Password in plain text format.</param>
        protected override Task<bool> VerifyPasswordAsync(IUserPasswordStore<MedioClinicUser, int> store, MedioClinicUser user, string password)
        {
            if (user == null)
            {
                return Task.FromResult(false);
            }

            var userInfo = UserInfoProvider.GetUserInfo(user.UserName);
            var result = !userInfo.IsExternal && !userInfo.UserIsDomain && !UserInfoProvider.IsUserPasswordDifferent(userInfo, password);

            return Task.FromResult(result);
        }


        /// <summary>
        /// Updates the security stamp if the store supports it.
        /// </summary>
        /// <param name="user">User whose stamp should be updated.</param>
        internal async Task UpdateSecurityStampInternal(MedioClinicUser user)
        {
            if (SupportsUserSecurityStamp)
            {
                await GetSecurityStore().SetSecurityStampAsync(user, NewSecurityStamp());
            }
        }


        private IUserSecurityStampStore<MedioClinicUser, int> GetSecurityStore()
        {
            var cast = Store as IUserSecurityStampStore<MedioClinicUser, int>;
            if (cast == null)
            {
                throw new NotSupportedException("Current Store does not implement the IUserSecurityStore interface.");
            }
            return cast;
        }


        private string NewSecurityStamp()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
