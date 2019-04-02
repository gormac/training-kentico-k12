using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Models;
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
                    Enabled = true
                };

                var result = await UserManager.CreateAsync(user, uploadModel.Data.Password);

                if (result.Succeeded)
                {
                    //var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var confirmationUrl = Url.Action("ConfirmUser", "Account", new { userId = user.Id, token });

                    // TDOO: Localize
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your new account",
                        //$"To finish the registration process at Medio Clinic, please <a href=\"{confirmationUrl}\">confirm your new account</a> at Medio Clinic.");
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // TODO: Thank you, check your inbox
                    return RedirectToAction("Index", "Home");
                }

                AddErrors(result);
            }

            // TODO: Localize
            var viewModel = GetPageViewModel(uploadModel.Data, "Error");

            return View(viewModel);
        }

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

            return View();
        }

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

            var result = await SignInManager.PasswordSignInAsync(uploadModel.Data.Email, uploadModel.Data.Password, uploadModel.Data.StaySignedIn, false);
            var actionResult = string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index", "Home") : RedirectToLocal(returnUrl);

            switch (result)
            {
                case SignInStatus.Success:
                    return actionResult;
                case SignInStatus.Failure:
                default:
                    // TODO: Localize
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                    return View(GetPageViewModel(uploadModel.Data, "Sign in"));
            }
        }

        // GET: /Account/Signout
        public ActionResult Signout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            // TODO: Constant
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}