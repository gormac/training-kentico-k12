using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using Kentico.Membership;

namespace Identity.Proxies
{
    public interface IKenticoUserManager
    {
        Task<IdentityResult> UpdatePassword(IUserPasswordStore<User, int> passwordStore, User user, string newPassword);
        Task<bool> VerifyPasswordAsync(IUserPasswordStore<User, int> store, User user, string password);
    }
}