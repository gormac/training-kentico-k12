using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Models;
using MedioClinic.Models;
using MedioClinic.Models.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MedioClinic.Controllers
{
    public class AccountController : BaseController
    {
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
        public async Task<ActionResult> Register(PageViewModel<RegisterViewModel> model)
        {
            if (ModelState.IsValid)
            {
                var user = new MedioClinicUser
                {
                    UserName = model.Data.Email,
                    Email = model.Data.Email,
                    FirstName = model.Data.FirstName,
                    LastName = model.Data.LastName,
                    Enabled = true
                };

                var result = await UserManager.CreateAsync(user, model.Data.Password);

                if (result.Succeeded)
                {
                    // TODO: UserId not found
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Index", "Home");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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