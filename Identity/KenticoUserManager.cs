using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using Kentico.Membership;

namespace Identity
{
    public class KenticoUserManager : Kentico.Membership.UserManager, IKenticoUserManager
    {
        public KenticoUserManager(IUserStore<User, int> store) : base(store)
        {
        }

        Task<IdentityResult> IKenticoUserManager.UpdatePassword(IUserPasswordStore<User, int> passwordStore, User user, string newPassword)
        {
            return base.UpdatePassword(passwordStore, user, newPassword);
        }

        Task<bool> IKenticoUserManager.VerifyPasswordAsync(IUserPasswordStore<User, int> store, User user, string password)
        {
            return base.VerifyPasswordAsync(store, user, password);
        }
    }
}
