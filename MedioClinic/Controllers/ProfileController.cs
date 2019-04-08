using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Business.Attributes;
using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Extensions;
using Business.Identity.Models;
using MedioClinic.Config;
using MedioClinic.Models.Account;

namespace MedioClinic.Controllers
{
    public class ProfileController : BaseController
    {
        public IMedioClinicUserManager<MedioClinicUser, int> UserManager { get; set; }

        public ProfileController(IMedioClinicUserManager<MedioClinicUser, int> userManager, IBusinessDependencies dependencies) : base(dependencies)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: Profile
        [MedioClinicAuthorize(Roles = Roles.Doctor | Roles.Patient, SiteName = AppConfig.Sitename)]
        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByEmailAsync(HttpContext.User.Identity.Name);

            if (user != null)
            {
                var roles = (await UserManager.GetRolesAsync(user.Id)).ToMedioClinicRoles();
                var viewName = "Patient";
                var title = viewName;

                if (roles.HasFlag(Roles.Doctor))
                {
                    viewName = "Doctor";
                    title = viewName;
                }

                var model = GetPageViewModel(user as UserViewModel, title);

                return View(viewName, model);
            }

            return HttpNotFound();
        }
    }
}