using Business.Identity.Models;
using CMS.Helpers;
using CMS.Membership;
using Kentico.Membership;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Identity
{
    public class MedioClinicUserManager : UserManager<MedioClinicUser, int>, IMedioClinicUserManager<MedioClinicUser, int>
    {
        public MedioClinicUserManager(IMedioClinicUserStore medioClinicUserStore) : base(medioClinicUserStore)
        {
            // TODO: Configure rules
            PasswordValidator = new PasswordValidator
            {
                RequireDigit = true,
                RequiredLength = 8,
                RequireLowercase = true,
                RequireNonLetterOrDigit = true,
                RequireUppercase = true
            };

            UserLockoutEnabledByDefault = false;
            EmailService = new EmailService();

            // TODO: Configure rules
            UserValidator = new UserValidator<MedioClinicUser, int>(this)
            {
                RequireUniqueEmail = true
            };

            // Registration: Confirmed registration
            UserTokenProvider = new EmailTokenProvider<MedioClinicUser, int>();
        }

        //IIdentityValidator<MedioClinicUser> IMedioClinicUserManager<MedioClinicUser, int>.UserValidator
        //{
        //    get => UserValidator as IIdentityValidator<MedioClinicUser>;
        //    set => UserValidator = value as IIdentityValidator<MedioClinicUser>;
        //}

        //IClaimsIdentityFactory<IMedioClinicUser, int> IMedioClinicUserManager<IMedioClinicUser, int>.ClaimsIdentityFactory
        //{
        //    get => ClaimsIdentityFactory as IClaimsIdentityFactory<IMedioClinicUser, int>;
        //    set => ClaimsIdentityFactory = value as IClaimsIdentityFactory<MedioClinicUser, int>;
        //}

        //IUserTokenProvider<IMedioClinicUser, int> IMedioClinicUserManager<IMedioClinicUser, int>.UserTokenProvider
        //{
        //    get => UserTokenProvider as IUserTokenProvider<IMedioClinicUser, int>;
        //    set => UserTokenProvider = value as IUserTokenProvider<MedioClinicUser, int>;
        //}

        //IQueryable<IMedioClinicUser> IMedioClinicUserManager<IMedioClinicUser, int>.Users =>
        //    Users as IQueryable<IMedioClinicUser>;

        //IDictionary<string, IUserTokenProvider<IMedioClinicUser, int>> IMedioClinicUserManager<IMedioClinicUser, int>.TwoFactorProviders =>
        //    TwoFactorProviders as IDictionary<string, IUserTokenProvider<IMedioClinicUser, int>>;

        //public Task<bool> CheckPasswordAsync(IMedioClinicUser user, string password) =>
        //    CheckPasswordAsync(user, password);

        //public Task<IdentityResult> CreateAsync(IMedioClinicUser user) =>
        //    CreateAsync(user);

        //public Task<IdentityResult> CreateAsync(IMedioClinicUser user, string password) =>
        //    CreateAsync(user, password);

        //public Task<ClaimsIdentity> CreateIdentityAsync(IMedioClinicUser user, string authenticationType) =>
        //    CreateIdentityAsync(user, authenticationType);

        //public Task<IdentityResult> DeleteAsync(IMedioClinicUser user) =>
        //    DeleteAsync(user);

        //public void RegisterTwoFactorProvider(string twoFactorProvider, IUserTokenProvider<IMedioClinicUser, int> provider) =>
        //    RegisterTwoFactorProvider(twoFactorProvider, provider);

        //public Task<IdentityResult> UpdateAsync(IMedioClinicUser user) =>
        //    UpdateAsync(user);

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

                // Don't change the way the passwords are hashed once the app is released in production.
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

        //Task<IMedioClinicUser> IMedioClinicUserManager<IMedioClinicUser, int>.FindAsync(string userName, string password) =>
        //    FindAsync(userName, password);

        //Task<IMedioClinicUser> IMedioClinicUserManager<IMedioClinicUser, int>.FindAsync(UserLoginInfo login)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<IMedioClinicUser> IMedioClinicUserManager<IMedioClinicUser, int>.FindByEmailAsync(string email)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<IMedioClinicUser> IMedioClinicUserManager<IMedioClinicUser, int>.FindByIdAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<IMedioClinicUser> IMedioClinicUserManager<IMedioClinicUser, int>.FindByNameAsync(string userName)
        //{
        //    throw new NotImplementedException();
        //}

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

        //Task<IdentityResult> IMedioClinicUserManager<IMedioClinicUser, int>.UpdatePassword(IUserPasswordStore<MedioClinicUser, int> passwordStore, MedioClinicUser user, string newPassword)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<bool> IMedioClinicUserManager<IMedioClinicUser, int>.VerifyPasswordAsync(IUserPasswordStore<MedioClinicUser, int> store, MedioClinicUser user, string password)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
