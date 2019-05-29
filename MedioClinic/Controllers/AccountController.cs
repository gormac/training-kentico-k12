using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Models;
using Business.Repository.Avatar;
using Business.Services.Errors;
using Business.Services.Localization;
using MedioClinic.Extensions;
using MedioClinic.Models;
using MedioClinic.Models.Account;

namespace MedioClinic.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class AccountController : BaseController
    {
        public IMedioClinicUserManager<MedioClinicUser, int> UserManager { get; }

        public IMedioClinicSignInManager<MedioClinicUser, int> SignInManager { get; }

        public ILocalizationService LocalizationService { get; }

        public IAuthenticationManager AuthenticationManager { get; }

        public IErrorHelperService ErrorHelperService { get; }

        public IAvatarRepository AvatarRepository { get; }

        public AccountController(
            IMedioClinicUserManager<MedioClinicUser, int> userManager,
            IMedioClinicSignInManager<MedioClinicUser, int> signInManager,
            IAuthenticationManager authenticationManager,
            IErrorHelperService errorHelperService,
            ILocalizationService localizationService,
            IAvatarRepository avatarRepository,
            IBusinessDependencies dependencies) 
            : base(dependencies)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            AuthenticationManager = authenticationManager ?? throw new ArgumentNullException(nameof(authenticationManager));
            ErrorHelperService = errorHelperService ?? throw new ArgumentNullException(nameof(errorHelperService));
            LocalizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            AvatarRepository = avatarRepository ?? throw new ArgumentNullException(nameof(avatarRepository));
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            var model = GetPageViewModel(new RegisterViewModel(), LocalizationService.Localize("Controllers.Account.Register.Title"));

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
                    ErrorHelperService.LogException(nameof(AccountController), nameof(Register), ex);
                }

                if (result != null && result.Succeeded)
                {
                    // Registration: Confirmed registration (begin)
                    var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var confirmationUrl = Url.AbsoluteUrl(Request, "ConfirmUser", routeValues: new { userId = user.Id, token });

                    await UserManager.SendEmailAsync(user.Id,
                        LocalizationService.Localize("Controllers.Account.Register.Email.Confirm.Subject"),
                        LocalizationService.LocalizeFormat("Controllers.Account.Register.Email.Confirm.Body", confirmationUrl));

                    ViewBag.Message = LocalizationService.Localize("Controllers.Account.Register.ViewbagMessage");

                    return View("ViewbagMessage", GetPageViewModel(LocalizationService.Localize("Controllers.Account.Register.RegistrationStarted")));
                    // Registration: Confirmed registration (end)

                    // Registration: Direct sign in (begin)
                    /*result = await AddToPatientRole(user.Id);
                    await CreateNewAvatar(user);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Index", "Home");*/
                    // Registration: Direct sign in (end)
                }

                AddErrors(result);
            }

            var viewModel = GetPageViewModel(uploadModel.Data, LocalizationService.Localize("general.error"));

            return View(viewModel);
        }

        // Registration: Confirmed registration (begin)
        // GET: /Account/ConfirmUser
        public async Task<ActionResult> ConfirmUser(int? userId, string token)
        {
            if (userId.HasValue)
            {
                IdentityResult result = IdentityResult.Failed();

                try
                {
                    result = await UserManager.ConfirmEmailAsync(userId.Value, token);
                }
                catch (InvalidOperationException)
                {
                    return InvalidToken();
                }

                if (result.Succeeded && (await AddToPatientRole(userId.Value)).Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(userId.Value);
                    await CreateNewAvatar(user);
                    ViewBag.Message = LocalizationService.LocalizeFormat("Controllers.Account.ConfirmUser.ViewbagMessage", Url.Action("Signin"));
                }

                AddErrors(result);
            }

            return View("ViewbagMessage", GetPageViewModel(LocalizationService.Localize("Controllers.Account.ConfirmUser.Title")));
        }
        // Registration: Confirmed registration (end)

        // GET: /Account/Signin
        public ActionResult Signin()
        {
            return View(GetPageViewModel(new SigninViewModel(), LocalizationService.Localize("logonform.logonbutton")));
        }

        // POST: /Account/Signin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Signin(PageViewModel<SigninViewModel> uploadModel, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(GetPageViewModel(uploadModel.Data, LocalizationService.Localize("logonform.logonbutton")));
            }

            MedioClinicUser user = null;

            try
            {
                user = await UserManager.FindByNameAsync(uploadModel.Data.EmailViewModel.Email);
            }
            catch (Exception ex)
            {
                ErrorHelperService.LogException(nameof(AccountController), nameof(Signin), ex);

                return InvalidAttempt(uploadModel);
            }

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
                ErrorHelperService.LogException(nameof(AccountController), nameof(Signin), ex);

                return InvalidAttempt(uploadModel);
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
                ErrorHelperService.LogException(nameof(AccountController), nameof(Signout), ex);
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            var model = new EmailViewModel();

            return View(GetPageViewModel(model, LocalizationService.Localize("passreset.title")));
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(PageViewModel<EmailViewModel> uploadModel)
        {
            if (ModelState.IsValid)
            {
                MedioClinicUser user = null;

                try
                {
                    user = await UserManager.FindByEmailAsync(uploadModel.Data.Email);
                }
                catch (Exception ex)
                {
                    ErrorHelperService.LogException(nameof(AccountController), nameof(ForgotPassword), ex);
                }

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
                    ErrorHelperService.LogException(nameof(AccountController), nameof(ForgotPassword), ex);
                    var viewModel = GetPageViewModel(uploadModel.Data, LocalizationService.Localize("general.error"));

                    return View(viewModel);
                }

                var resetUrl = Url.AbsoluteUrl(Request, "ResetPassword", "Account", new { userId = user.Id, token });
                await UserManager.SendEmailAsync(user.Id, LocalizationService.Localize("passreset.title"),
                    LocalizationService.LocalizeFormat("Controllers.Account.ForgotPassword.Email.Body", resetUrl));

                return CheckEmailResetPassword();
            }

            return View(GetPageViewModel(uploadModel.Data, LocalizationService.Localize("passreset.title")));
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
                ErrorHelperService.LogException(nameof(AccountController), nameof(ResetPassword), ex);

                return InvalidToken();
            }

            var model = new ResetPasswordViewModel
            {
                UserId = userId.Value,
                Token = token
            };

            return View(GetPageViewModel(model, LocalizationService.Localize("passreset.title")));
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(PageViewModel<ResetPasswordViewModel> uploadModel)
        {
            if (!ModelState.IsValid)
            {
                return View(GetPageViewModel(uploadModel.Data, LocalizationService.Localize("passreset.title")));
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
                ErrorHelperService.LogException(nameof(AccountController), nameof(ResetPassword), ex);
                ViewBag.Message = LocalizationService.Localize("general.usernotfound");

                return View("ViewbagMessage", GetPageViewModel(ViewBag.Message));
            }

            if (result.Succeeded)
            {
                ViewBag.Message = LocalizationService.LocalizeFormat("Controllers.Account.ResetPassword.ViewbagMessage", Url.Action("Signin"));

                return View("ViewbagMessage", GetPageViewModel(LocalizationService.Localize("general.success")));
            }

            AddErrors(result);

            return View(GetPageViewModel(uploadModel.Data, LocalizationService.Localize("general.error")));
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
            ModelState.AddModelError(string.Empty, LocalizationService.Localize("Controllers.Account.InvalidAttempt"));

            return View(GetPageViewModel(uploadModel.Data, LocalizationService.Localize("logonform.logonbutton")));
        }

        protected ActionResult CheckEmailResetPassword()
        {
            ViewBag.Message = LocalizationService.Localize("Controllers.Account.CheckEmailResetPassword.ViewbagMessage");

            return View("ViewbagMessage", GetPageViewModel(LocalizationService.Localize("Controllers.Account.CheckEmailResetPassword.Title")));
        }

        protected ActionResult InvalidToken()
        {
            ViewBag.Message = LocalizationService.Localize("Controllers.Account.InvalidToken.ViewbagMessage");

            return View("ViewbagMessage", GetPageViewModel(LocalizationService.Localize("Controllers.Account.InvalidToken.Title")));
        }

        protected async Task<IdentityResult> AddToPatientRole(int userId)
        {
            var patientRole = Roles.Patient.ToString();
            return await UserManager.AddToRolesAsync(userId, patientRole);
        }

        protected async Task CreateNewAvatar(MedioClinicUser user)
        {
            var path = Server.MapPath($"{ContentFolder}/{AvatarFolder}/{DefaultAvatarFileName}");

            user.AvatarId = AvatarRepository.CreateUserAvatar(path, $"Custom {user.UserName}");

            await UserManager.UpdateAsync(user);
        }
    }
}