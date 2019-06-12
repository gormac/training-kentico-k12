using System.Threading.Tasks;
using System.Web.Routing;
using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Models;
using MedioClinic.Models.Account;

namespace MedioClinic.Utils
{
    // TODO: Document.
    public interface IAccountManager
    {
        Task<AccountResult<RegisterResultState, RegisterViewModel>> RegisterAsync(RegisterViewModel model, bool emailConfirmed, RequestContext requestContext);
        Task<AccountResult<ConfirmUserResultState>> ConfirmUserAsync(int userId, string token, RequestContext requestContext);
        Task<AccountResult<SignInResultState, SignInViewModel>> SignInAsync(SignInViewModel model);
        AccountResult<SignOutResultState> SignOut();
        Task<AccountResult<ForgotPasswordResultState, EmailViewModel>> ForgotPasswordAsync(EmailViewModel model, RequestContext requestContext);
        Task<AccountResult<ResetPasswordResultState, ResetPasswordViewModel>> VerifyResetPasswordTokenAsync(int userId, string token);
        Task<AccountResult<ResetPasswordResultState, ResetPasswordViewModel>> ResetPasswordAsync(ResetPasswordViewModel model)
    }
}