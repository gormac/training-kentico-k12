using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Models;
using MedioClinic.Extensions;
using MedioClinic.Models;
using MedioClinic.Models.Account;
using CMS.EventLog;

namespace MedioClinic.Controllers
{
    // TODO: Improve robustness (try-catch'es, null checks, logging, etc.)
    public class AccountController : BaseController
    {
        public IMedioClinicUserManager<MedioClinicUser, int> UserManager { get; }

        public IMedioClinicSignInManager<MedioClinicUser, int> SignInManager { get; }

        protected IAuthenticationManager AuthenticationManager { get; }

        public AccountController(
            IMedioClinicUserManager<MedioClinicUser, int> userManager,
            IMedioClinicSignInManager<MedioClinicUser, int> signInManager,
            IAuthenticationManager authenticationManager,
            IBusinessDependencies dependencies) 
            : base(dependencies)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            AuthenticationManager = authenticationManager ?? throw new ArgumentNullException(nameof(authenticationManager));
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            var model = GetPageViewModel(new RegisterViewModel(), "Register");

            return View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(PageViewModel<RegisterViewModel> uploadModel)
        {
            if (ModelState.IsValid)
            {
                var user = new MedioClinicUser
                {
                    UserName = uploadModel.Data.EmailViewModel.Email,
                    Email = uploadModel.Data.EmailViewModel.Email,
                    FirstName = uploadModel.Data.FirstName,
                    LastName = uploadModel.Data.LastName,

                    // Registration: Confirmed registration
                    Enabled = false

                    // Registration: Direct sign in
                    //Enabled = true
                };

                IdentityResult result = null;

                try
                {
                    result = await UserManager.CreateAsync(user, uploadModel.Data.PasswordConfirmationViewModel.Password);
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException(nameof(AccountController), nameof(Register), ex);
                }

                if (result != null && result.Succeeded)
                {
                    // Registration: Confirmed registration (begin)
                    var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var confirmationUrl = Url.AbsoluteUrl(Request, "ConfirmUser", routeValues: new { userId = user.Id, token });

                    // TODO: Localize
                    await UserManager.SendEmailAsync(user.Id, "Confirm your new account",
                        $"To finish the registration process at Medio Clinic, please <a href=\"{confirmationUrl}\">confirm your new account</a>.");

                    ViewBag.Message = "Thank you for registering at Medio Clinic! Please check your mailbox and click the confirmation link in the message we've just sent to you.";

                    return View("ViewbagMessage", GetPageViewModel("Registration started"));
                    // Registration: Confirmed registration (end)

                    // Registration: Direct sign in (begin)
                    /*await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Index", "Home");*/
                    // Registration: Direct sign in (end)
                }

                AddErrors(result);
            }

            // TODO: Localize
            var viewModel = GetPageViewModel(uploadModel.Data, "Error");

            return View(viewModel);
        }

        // Registration: Confirmed registration (begin)
        // GET: /Account/ConfirmUser
        public async Task<ActionResult> ConfirmUser(int? userId, string token)
        {
            if (userId.HasValue)
            {
                IdentityResult confirmResult = IdentityResult.Failed();

                try
                {
                    confirmResult = await UserManager.ConfirmEmailAsync(userId.Value, token);
                }
                catch (InvalidOperationException)
                {
                    return InvalidToken();
                }

                if (confirmResult.Succeeded)
                {
                    ViewBag.Message = $"Your registration was successfull. You can now <a href=\"{Url.Action("Signin")}\">sign in</a> to your account.";
                }
            }

            return View("ViewbagMessage", GetPageViewModel("Account confirmed"));
        }
        // Registration: Confirmed registration (end)

        // GET: /Account/Signin
        public ActionResult Signin()
        {
            return View(GetPageViewModel(new SigninViewModel(), "Sign in"));
        }

        // POST: /Account/Signin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Signin(PageViewModel<SigninViewModel> uploadModel, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                // TODO: Localize
                return View(GetPageViewModel(uploadModel.Data, "Sign in"));
            }

            var user = await UserManager.FindByEmailAsync(uploadModel.Data.EmailViewModel.Email);

            // Registration: Confirmed registration (begin)
            if (user != null && !await UserManager.IsEmailConfirmedAsync(user.Id))
            {
                return InvalidAttempt(uploadModel);
            }
            // Registration: Confirmed registration (end)

            SignInStatus status = SignInStatus.Failure;

            try
            {
                status = await SignInManager.PasswordSignInAsync(uploadModel.Data.EmailViewModel.Email, uploadModel.Data.PasswordViewModel.Password, uploadModel.Data.StaySignedIn, false);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException(nameof(AccountController), nameof(Signin), ex);
            }

            if (status == SignInStatus.Success)
            {
                return RedirectToLocal(Server.UrlDecode(returnUrl));
            }

            return InvalidAttempt(uploadModel);
        }

        // GET: /Account/Signout
        [Authorize]
        public ActionResult Signout()
        {
            try
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException(nameof(AccountController), nameof(Signout), ex);
            }

            // TODO: Constant
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            var model = new EmailViewModel();

            return View(GetPageViewModel(model, "Reset password"));
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(PageViewModel<EmailViewModel> uploadModel)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(uploadModel.Data.Email);

                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed.
                    return CheckEmailResetPassword();
                }

                string token = null;

                try
                {
                    token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException(nameof(AccountController), nameof(ForgotPassword), ex);
                }

                var resetUrl = Url.AbsoluteUrl(Request, "ResetPassword", "Account", new { userId = user.Id, token });
                await UserManager.SendEmailAsync(user.Id, "Reset your password",
                    $"Please reset your password by clicking this <a href=\"{resetUrl}\">link</a>");

                return CheckEmailResetPassword();
            }

            return View(GetPageViewModel(uploadModel.Data, "Reset password"));
        }

        // GET: /Account/ResetPassword
        public async Task<ActionResult> ResetPassword(int? userId, string token)
        {
            var tokenVerified = false;

            try
            {
                tokenVerified = await UserManager.VerifyUserTokenAsync(userId.Value, "ResetPassword", token);
            }
            catch (InvalidOperationException ex)
            {
                EventLogProvider.LogException(nameof(AccountController), nameof(ResetPassword), ex);

                return InvalidToken();
            }

            var model = new ResetPasswordViewModel
            {
                UserId = userId.Value,
                Token = token
            };

            return View(GetPageViewModel(model, "Reset password"));
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(PageViewModel<ResetPasswordViewModel> uploadModel)
        {
            if (!ModelState.IsValid)
            {
                return View(GetPageViewModel(uploadModel.Data, "Reset password"));
            }

            var result = IdentityResult.Failed();

            try
            {
                result = await UserManager.ResetPasswordAsync(
                        uploadModel.Data.UserId,
                        uploadModel.Data.Token,
                        uploadModel.Data.PasswordConfirmationViewModel.Password);

            }
            catch (InvalidOperationException ex)
            {
                EventLogProvider.LogException(nameof(AccountController), nameof(ResetPassword), ex);
                ViewBag.Message = "User was not found.";
            }

            if (result.Succeeded)
            {
                ViewBag.Message = $"Your password was successfully reset. You can now <a href=\"{Url.Action("Signin")}\">sign in</a>.";

                return View("ViewbagMessage", GetPageViewModel("Success"));
            }

            return InvalidToken();
        }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        protected ActionResult InvalidAttempt(PageViewModel<SigninViewModel> uploadModel)
        {
            // TODO: Localize
            ModelState.AddModelError(string.Empty, "Invalid sign in attempt.");

            return View(GetPageViewModel(uploadModel.Data, "Sign in"));
        }

        protected ActionResult CheckEmailResetPassword()
        {
            // TODO: Localize
            ViewBag.Message = "Please check your email to reset your password.";

            return View("ViewbagMessage", GetPageViewModel("Check email"));
        }

        protected ActionResult InvalidToken()
        {
            ViewBag.Message = "The operation cannot be done. The security token is incorrect.";

            return View("ViewbagMessage", GetPageViewModel("Invalid token"));
        }
    }
}