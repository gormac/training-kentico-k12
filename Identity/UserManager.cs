using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Identity
{
    public class UserManager : UserManager<Models.User, int>
    {
        public IKenticoUserManager KenticoUserManager { get; }

        public UserManager(IUserStore<Models.User, int> store, IKenticoUserManager kenticoUserManager) : base(store) =>
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

        protected override Task<IdentityResult> UpdatePassword(IUserPasswordStore<Models.User, int> passwordStore, Models.User user, string newPassword) =>
            KenticoUserManager.UpdatePassword((IUserPasswordStore<Kentico.Membership.User, int>)passwordStore, user, newPassword);

        protected override Task<bool> VerifyPasswordAsync(IUserPasswordStore<Models.User, int> store, Models.User user, string password) =>
            KenticoUserManager.VerifyPasswordAsync((IUserPasswordStore<Kentico.Membership.User, int>)store, user, password);
    }
}
