using System.Threading.Tasks;
using System.Web.Routing;
using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Models;
using MedioClinic.Models.Account;
using MedioClinic.Models;

namespace MedioClinic.Utils
{
    // TODO: Document.
    public interface IAccountManager
    {
        Task<IdentityManagerResult<RegisterResultState>> RegisterAsync(RegisterViewModel model, bool emailConfirmed, RequestContext requestContext);
        Task<IdentityManagerResult<ConfirmUserResultState>> ConfirmUserAsync(int userId, string token, RequestContext requestContext);
        Task<IdentityManagerResult<SignInResultState>> SignInAsync(SignInViewModel model);
        IdentityManagerResult<SignOutResultState> SignOut();
        Task<IdentityManagerResult<ForgotPasswordResultState>> ForgotPasswordAsync(EmailViewModel model, RequestContext requestContext);
        Task<IdentityManagerResult<ResetPasswordResultState, ResetPasswordViewModel>> VerifyResetPasswordTokenAsync(int userId, string token);
        Task<IdentityManagerResult<ResetPasswordResultState>> ResetPasswordAsync(ResetPasswordViewModel model);
    }
}