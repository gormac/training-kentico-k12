using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using Kentico.Membership;

using Identity.Models;

namespace Identity
{
    public class MedioClinicUserManager : UserManager<MedioClinicUser, int>
    {
        public IKenticoUserManager KenticoUserManager { get; }

        public MedioClinicUserManager(IUserStore<MedioClinicUser, int> store, IKenticoUserManager kenticoUserManager) : base(store) =>
            KenticoUserManager = KenticoUserManager ?? throw new ArgumentNullException(nameof(kenticoUserManager));

        //public static T Initialize<T>(IAppBuilder app, T manager) where T : UserManager
        //{
        //    var provider = app.GetDataProtectionProvider();
        //    if (provider != null)
        //    {
        //        manager.UserTokenProvider = new DataProtectorTokenProvider<Models.User, int>(provider.Create("Kentico.Membership"));
        //    }

        //    manager.PasswordValidator = new PasswordValidator();
        //    manager.UserLockoutEnabledByDefault = false;
        //    manager.EmailService = new EmailService();
        //    manager.UserValidator = new UserValidator<Models.User, int>(manager);

        //    return manager;
        //}

        protected override Task<IdentityResult> UpdatePassword(IUserPasswordStore<MedioClinicUser, int> passwordStore, MedioClinicUser user, string newPassword) =>
            KenticoUserManager.UpdatePassword((IUserPasswordStore<User, int>)passwordStore, user, newPassword);

        protected override Task<bool> VerifyPasswordAsync(IUserPasswordStore<MedioClinicUser, int> store, MedioClinicUser user, string password) =>
            KenticoUserManager.VerifyPasswordAsync((IUserPasswordStore<User, int>)store, user, password);
    }
}
