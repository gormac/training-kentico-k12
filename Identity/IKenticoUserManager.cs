using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using Kentico.Membership;

namespace Identity
{
    public interface IKenticoUserManager
    {
        Task<IdentityResult> UpdatePassword(IUserPasswordStore<User, int> passwordStore, User user, string newPassword);
        Task<bool> VerifyPasswordAsync(IUserPasswordStore<User, int> store, User user, string password);
    }
}