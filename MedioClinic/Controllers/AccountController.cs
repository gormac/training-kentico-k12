using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Models;
using MedioClinic.Models;
using MedioClinic.Models.Account;

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
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Index", "Home");
                }

                AddErrors(result);
            }

            // TODO: Localize?
            var viewModel = GetPageViewModel(uploadModel.Data, "Error");

            return View(viewModel);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}