using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using EnumsNET;
using CMS.Helpers;
using Business.Attributes;
using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Extensions;
using Business.Identity.Models;
using Business.Services.ViewModel;
using MedioClinic.Config;
using MedioClinic.Models;
using MedioClinic.Models.Account;
using MedioClinic.Models.Profile;
using MedioClinic.Utils;

namespace MedioClinic.Controllers
{
    public class ProfileController : BaseController
    {
        public IMedioClinicUserManager<MedioClinicUser, int> UserManager { get; set; }

        public IMedioClinicUserStore UserStore { get; set; }

        public IUserModelService UserModelService { get; set; }

        public IErrorHelper ErrorHelper { get; set; }

        public ProfileController(
            IMedioClinicUserManager<MedioClinicUser, int> userManager,
            IMedioClinicUserStore userStore,
            IUserModelService userModelService,
            IErrorHelper errorHelper,
            IBusinessDependencies dependencies) : base(dependencies)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            UserModelService = userModelService ?? throw new ArgumentNullException(nameof(userModelService));
            ErrorHelper = errorHelper ?? throw new ArgumentNullException(nameof(errorHelper));
        }

        // GET: Profile
        [MedioClinicAuthorize(Roles = Roles.Doctor | Roles.Patient, SiteName = AppConfig.Sitename)]
        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (user != null)
            {
                return View(GetViewModelByUserRoles(user));
            }

            return HttpNotFound();
        }

        // POST: Profile
        [MedioClinicAuthorize(Roles = Roles.Doctor | Roles.Patient, SiteName = AppConfig.Sitename)]
        [HttpPost]
        public async Task<ActionResult> Index(PageViewModel<UserViewModel> uploadModel)
        {
            if (ModelState.IsValid)
            {
                var userById = await UserManager.FindByIdAsync(uploadModel.Data.Id);

                var customMappings = new Dictionary<(string propertyName, Type propertyType), object>
                {
                    { (nameof(MedioClinicUser.Email), typeof(string)), uploadModel.Data.EmailViewModel.Email },
                };

                userById = UserModelService.MapToMedioClinicUser(uploadModel.Data, userById, customMappings);

                try
                {
                    // We need to use the user store directly due to a questionable design of Microsoft.AspNet.Identity.Core.UserManager.UpdateAsync().
                    await UserStore.UpdateAsync(userById);
                }
                catch (Exception ex)
                {
                    ErrorHelper.LogException(nameof(ProfileController), nameof(Index), ex);
                }

                ViewBag.Message = ResHelper.GetString("Controllers.Profile.Index.SavedChanges");

                return View(GetViewModelByUserRoles(userById));
            }

            var user = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (user != null)
            {
                return View(GetViewModelByUserRoles(user));
            }

            return HttpNotFound();
        }

        protected PageViewModel<IViewModel> GetViewModelByUserRoles(MedioClinicUser user)
        {
            if (user != null)
            {
                var roles = user.Roles.ToMedioClinicRoles();
                Type userViewModelType = FlagEnums.HasAnyFlags(roles, Roles.Doctor) ? typeof(DoctorViewModel) : typeof(PatientViewModel);

                var customMappings = new Dictionary<(string propertyName, Type propertyType), object>
                {
                    { (nameof(UserViewModel.EmailViewModel), typeof(EmailViewModel)), new EmailViewModel { Email = user.Email } },
                };

                var mappedUserViewModel = UserModelService.MapToViewModel(user, userViewModelType, customMappings);
                var finalViewModel = GetPageViewModel((IViewModel)mappedUserViewModel, GetTitle(roles));

                return finalViewModel;
            }

            return null;
        }

        protected static string GetTitle(Roles roles) =>
            FlagEnums.HasAnyFlags(roles, Roles.Doctor)
                ? ResHelper.GetString("Controllers.Profile.Index.Title.Doctor")
                : ResHelper.GetString("Controllers.Profile.Index.Title.Patient");
    }
}