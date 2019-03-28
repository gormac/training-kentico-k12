using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Owin;

using Kentico.Membership;

using Identity.Models;
using Identity.Proxies;

namespace Identity
{
    public class MedioClinicUserManager : Microsoft.AspNet.Identity.UserManager<MedioClinicUser, int>
    {
        public IKenticoUserManager KenticoUserManager { get; }

        public MedioClinicUserManager(IAppBuilder app, IUserStore<MedioClinicUser, int> store, IKenticoUserManager kenticoUserManager) : base(store)
        {
            KenticoUserManager = KenticoUserManager ?? throw new ArgumentNullException(nameof(kenticoUserManager));
            var provider = app.GetDataProtectionProvider();

            if (provider != null)
            {
                // TODO: Can the "Kentico.Membership" purpose identifier be reused?
                UserTokenProvider = new DataProtectorTokenProvider<MedioClinicUser, int>(provider.Create("Kentico.Membership"));
            }

            PasswordValidator = new PasswordValidator();
            UserLockoutEnabledByDefault = false;
            EmailService = new EmailService();
            UserValidator = new UserValidator<MedioClinicUser, int>(this);
        }

        protected override Task<IdentityResult> UpdatePassword(IUserPasswordStore<MedioClinicUser, int> passwordStore, MedioClinicUser user, string newPassword) =>
            KenticoUserManager.UpdatePassword((IUserPasswordStore<User, int>)passwordStore, user, newPassword);

        protected override Task<bool> VerifyPasswordAsync(IUserPasswordStore<MedioClinicUser, int> store, MedioClinicUser user, string password) =>
            KenticoUserManager.VerifyPasswordAsync((IUserPasswordStore<User, int>)store, user, password);
    }
}
