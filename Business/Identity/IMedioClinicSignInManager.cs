using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Identity
{
    public interface IMedioClinicSignInManager<TUser, TKey> : IDisposable
        where TUser : class, IUser<TKey>
        where TKey: IEquatable<TKey>
    {
        string AuthenticationType { get; set; }
        IMedioClinicUserManager<TUser, TKey> UserManager { get; set; }
        IAuthenticationManager AuthenticationManager { get; set; }
        TKey ConvertIdFromString(string id);
        string ConvertIdToString(TKey id);
        Task<ClaimsIdentity> CreateUserIdentityAsync(TUser user);
        Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent);
        Task<TKey> GetVerifiedUserIdAsync();
        Task<bool> HasBeenVerifiedAsync();
        Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout);
        Task<bool> SendTwoFactorCodeAsync(string provider);
        Task SignInAsync(TUser user, bool isPersistent, bool rememberBrowser);
        Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);
    }
}