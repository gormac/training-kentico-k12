using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Models;
using MedioClinic.Extensions;
using MedioClinic.Models;
using MedioClinic.Models.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MedioClinic.Controllers
{
    public class AccountController : BaseController
    {
        // TODO: Underscores?
        private MedioClinicSignInManager _signInManager;

        public MedioClinicUserManager UserManager { get; }

        public MedioClinicSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<MedioClinicSignInManager>();
            private set => _signInManager = value;
        }

        private IAuthenticationManager AuthenticationManager =>
            HttpContext.GetOwinContext().Authentication;

        public AccountController(IMedioClinicUserManager userManager, IBusinessDependencies dependencies) : base(dependencies)
        {
            UserManager = userManager as MedioClinicUserManager ?? throw new ArgumentNullException(nameof(UserManager));
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            var model = GetPageViewModel(new RegisterViewModel(), "Register");

            return View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(PageViewModel<RegisterViewModel> uploadModel)
        {
            if (ModelState.IsValid)
            {
                var user = new MedioClinicUser
                {
                    UserName = uploadModel.Data.Email,
                    Email = uploadModel.Data.Email,
                    FirstName = uploadModel.Data.FirstName,
                    LastName = uploadModel.Data.LastName,

                    // Registration: Confirmed registration
                    Enabled = false
                    
                    // Registration: Direct sign in
                    //Enabled = true
                };

                var result = await UserManager.CreateAsync(user, uploadModel.Data.Password);

                if (result.Succeeded)
                {
                    // Registration: Confirmed registration (begin)
                    var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var confirmationUrl = Url.Action("ConfirmUser", "Account", new { userId = user.Id, token });
                    var confirmationUrl = Url.AbsoluteUrl(Request, "ConfirmUser", "Account", new { userId = user.Id, token });

                    // TODO: Localize
                    await UserManager.SendEmailAsync(user.Id, "Confirm your new account",
                        $"To finish the registration process at Medio Clinic, please <a href=\"{confirmationUrl}\">confirm your new account</a>.");

                    ViewBag.Message = "Thank you for registering at Medio Clinic! Please check your mailbox and click the confirmation link in the message we've just sent to you.";

                    return View("SimpleMessage", GetPageViewModel("Registration started"));
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
                    ViewBag.Message = "The user was not found.";
                }

                if (confirmResult.Succeeded)
                {
                    ViewBag.Message = $"Your registration was successfull. You can now <a href=\"{Url.Action("Signin", "Account")}\">sign in</a> to your account.";
                }
            }

            return View("SimpleMessage", GetPageViewModel("Account confirmed"));
        }
        // Registration: Confirmed registration (end)

        // GET: /Account/Signin
        public ActionResult Signin()
        {
            return View(GetPageViewModel(new SigninViewModel(), "Sign in"));
        }

        // POST: /Account/Signin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Signin(PageViewModel<SigninViewModel> uploadModel, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                // TODO: Localize
                return View(GetPageViewModel(uploadModel.Data, "Sign in"));
            }

            var user = await UserManager.FindByEmailAsync(uploadModel.Data.Email);

            // Registration: Confirmed registration (begin)
            if (user != null && !await UserManager.IsEmailConfirmedAsync(user.Id))
            {
                return InvalidAttempt(uploadModel);
            }
            // Registration: Confirmed registration (end)

            var result = await SignInManager.PasswordSignInAsync(uploadModel.Data.Email, uploadModel.Data.Password, uploadModel.Data.StaySignedIn, false);

            switch (result)
            {
                case SignInStatus.Success:
                    var actionResult = string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index", "Home") : RedirectToLocal(returnUrl);

                    return actionResult;
                case SignInStatus.Failure:
                default:
                    return InvalidAttempt(uploadModel);
            }
        }

        // GET: /Account/Signout
        public ActionResult Signout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            // TODO: Constant
            return RedirectToAction("Index", "Home");
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
            if (Url.IsLocalUrl(returnUrl))
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
    }
}