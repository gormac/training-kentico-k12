using System;
using System.Threading.Tasks;
using System.Web.Mvc;

using Business.DependencyInjection;
using MedioClinic.Models;
using MedioClinic.Models.Account;
using MedioClinic.Utils;

namespace MedioClinic.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class AccountController : BaseController
    {
        /// <summary>
        /// Signals if user registration is confirmed by email.
        /// </summary>
        /// <remarks>Consider taking from environment variables.</remarks>
        public bool EmailConfirmedRegistration => true;



        public IAccountManager AccountManager { get; set; }


        public AccountController(
            IAccountManager accountManager,
            IBusinessDependencies dependencies)
            : base(dependencies)
        {
            AccountManager = accountManager ?? throw new ArgumentNullException(nameof(accountManager));
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            var model = GetPageViewModel(new RegisterViewModel(), Localize("Controllers.Account.Register.Title"));

            return View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(PageViewModel<RegisterViewModel> uploadModel)
        {
            PageViewModel<RegisterViewModel> viewModel = null;
            var errorMessage = ConcatenateContactAdmin("Controllers.Account.Register.FailureMessage");

            if (ModelState.IsValid)
            {
                var accountResult = await AccountManager.RegisterAsync(uploadModel.Data, EmailConfirmedRegistration, Request.RequestContext);
                string title = null;
                var message = ConcatenateContactAdmin("Error.Message");

                if (EmailConfirmedRegistration)
                {

                    switch (accountResult.ResultState)
                    {
                        case RegisterResultState.UserNotCreated:
                        case RegisterResultState.TokenNotCreated:
                        case RegisterResultState.NotSignedIn:
                            title = ErrorTitle;
                            message = Localize("Controllers.Account.Register.Failure.Message");
                            break;
                        case RegisterResultState.EmailSent:
                            title = Localize("Controllers.Account.Register.ConfirmedSuccess.Title");
                            message = Localize("Controllers.Account.Register.ConfirmedSuccess.Message");
                            break;
                        default:
                            break;
                    }

                    ViewBag.Message = message;
                    viewModel = GetPageViewModel(accountResult.Model, title);
                    AddErrors(accountResult);

                    return View("ViewbagMessage", viewModel);
                }
                else
                {
                    if (accountResult.ResultState == RegisterResultState.NotSignedIn)
                    {
                        title = ErrorTitle;
                        ViewBag.Message = Localize("Controllers.Account.Register.Failure.Message");
                        viewModel = GetPageViewModel(accountResult.Model, title);
                        AddErrors(accountResult);

                        return View("ViewbagMessage", viewModel);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            viewModel = GetPageViewModel(uploadModel.Data, Localize("BasicForm.InvalidInput"));

            return View(viewModel);
        }

        // Registration: Confirmed registration (begin)
        // GET: /Account/ConfirmUser
        public async Task<ActionResult> ConfirmUser(int? userId, string token)
        {
            var title = ErrorTitle;
            var message = ConcatenateContactAdmin("Error.Message");

            if (userId.HasValue)
            {
                var accountResult = await AccountManager.ConfirmUserAsync(userId.Value, token, Request.RequestContext);

                switch (accountResult.ResultState)
                {
                    case ConfirmUserResultState.EmailNotConfirmed:
                        message = Localize("Controllers.Account.ConfirmUser.ConfirmationFailure.Message");
                        break;
                    case ConfirmUserResultState.AvatarNotCreated:
                        message = Localize("Controllers.Account.ConfirmUser.AvatarFailure.Message");
                        break;
                    case ConfirmUserResultState.UserConfirmed:
                        title = Dependencies.LocalizationService.LocalizeFormat("Controllers.Account.ConfirmUser.Success.Title", Url.Action("SignIn"));
                        message = Localize("Controllers.Account.ConfirmUser.Success.Message");
                        break;
                    default:
                        break;
                }

                ViewBag.Message = message;

                return View("ViewbagMessage", GetPageViewModel(title));
            }

            return View("ViewbagMessage", GetPageViewModel(Localize("General.Error")));
        }
        // Registration: Confirmed registration (end)

        // GET: /Account/Signin
        public ActionResult SignIn()
        {
            return View(GetPageViewModel(new SignInViewModel(), Localize("LogonForm.LogonButton")));
        }

        // POST: /Account/Signin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(PageViewModel<SignInViewModel> uploadModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var accountResult = await AccountManager.SignInAsync(uploadModel.Data);

                switch (accountResult.ResultState)
                {
                    case SignInResultState.UserNotFound:
                    case SignInResultState.EmailNotConfirmed:
                    case SignInResultState.NotSignedIn:
                    default:
                        return InvalidAttempt(uploadModel);
                    case SignInResultState.SignedIn:
                        return RedirectToLocal(Server.UrlDecode(returnUrl));
                }
            }

            return InvalidAttempt(uploadModel);
        }

        // GET: /Account/Signout
        [Authorize]
        public ActionResult SignOut()
        {
            var accountResult = AccountManager.SignOut();

            if (accountResult.Success)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Message = ConcatenateContactAdmin("Controllers.Account.SignOut.Failure.Message");

            return View("ViewbagMessage", GetPageViewModel(Localize("General.Error")));
        }

        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            var model = new EmailViewModel();

            return View(GetPageViewModel(model, Localize("PassReset.Title")));
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(PageViewModel<EmailViewModel> uploadModel)
        {
            if (ModelState.IsValid)
            {
                _ = await AccountManager.ForgotPasswordAsync(uploadModel.Data, Request.RequestContext);

                return CheckEmailResetPassword();
            }

            return View(GetPageViewModel(uploadModel.Data, Localize("PassReset.Title")));
        }

        // GET: /Account/ResetPassword
        public async Task<ActionResult> ResetPassword(int? userId, string token)
        {
            ViewBag.Message = ConcatenateContactAdmin("Controllers.Account.ResetPassword.Failure.Message");

            if (userId.HasValue && !string.IsNullOrEmpty(token))
            {
                var accountResult = await AccountManager.VerifyResetPasswordTokenAsync(userId.Value, token);

                if (accountResult.Success)
                {
                    return View(GetPageViewModel(accountResult.Model, Localize("PassReset.Title")));
                }
                else
                {
                    ViewBag.Message = ConcatenateContactAdmin("Controllers.Account.InvalidToken.Message");
                }
            }

            return View("ViewbagMessage", GetPageViewModel(Localize("General.Error")));
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(PageViewModel<ResetPasswordViewModel> uploadModel)
        {
            if (ModelState.IsValid)
            {
                var accountResult = await AccountManager.ResetPasswordAsync(uploadModel.Data);

                if (accountResult.Success)
                {
                    ViewBag.Message = Dependencies.LocalizationService.LocalizeFormat("Controllers.Account.ResetPassword.Success.Message", Url.Action("Signin"));
                }
            }

            return View(GetPageViewModel(uploadModel.Data, Localize("PassReset.Title")));
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        protected ActionResult InvalidAttempt(PageViewModel<SignInViewModel> uploadModel)
        {
            ModelState.AddModelError(string.Empty, Localize("Controllers.Account.InvalidAttempt"));

            return View(GetPageViewModel(uploadModel.Data, Localize("LogonForm.LogonButton")));
        }

        protected ActionResult CheckEmailResetPassword()
        {
            ViewBag.Message = Localize("Controllers.Account.CheckEmailResetPassword.ViewbagMessage");

            return View("ViewbagMessage", GetPageViewModel(Localize("Controllers.Account.CheckEmailResetPassword.Title")));
        }

        protected void AddErrors<TResultState>(AccountResult<TResultState> result)
            where TResultState : Enum
        {

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }




    }
}