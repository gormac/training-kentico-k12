using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using Kentico.Membership;

namespace Identity.Proxies
{
    public class KenticoUserManager : UserManager, IKenticoUserManager
    {
        public KenticoUserManager(IUserStore<User, int> store) : base(store)
        {
        }

        Task<IdentityResult> IKenticoUserManager.UpdatePassword(IUserPasswordStore<User, int> passwordStore, User user, string newPassword) =>
            base.UpdatePassword(passwordStore, user, newPassword);

        Task<bool> IKenticoUserManager.VerifyPasswordAsync(IUserPasswordStore<User, int> store, User user, string password) =>
            base.VerifyPasswordAsync(store, user, password);
    }
}
