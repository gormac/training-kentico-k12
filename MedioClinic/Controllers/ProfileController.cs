using System;
using System.Threading.Tasks;
using System.Web.Mvc;

using Business.Attributes;
using Business.DependencyInjection;
using Business.Identity.Models;
using MedioClinic.Config;
using MedioClinic.Models;
using MedioClinic.Models.Profile;
using MedioClinic.Utils;

namespace MedioClinic.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class ProfileController : BaseController
    {
        public IProfileManager ProfileManager { get; }

        public ProfileController(
            IProfileManager profileManager,
            IBusinessDependencies dependencies) : base(dependencies)
        {
            ProfileManager = profileManager ?? throw new ArgumentNullException(nameof(profileManager));
        }

        // GET: Profile
        [MedioClinicAuthorize(Roles = Roles.Doctor | Roles.Patient, SiteName = AppConfig.Sitename)]
        public async Task<ActionResult> Index()
        {
            return await GetProfileAsync();
        }

        // POST: Profile
        [MedioClinicAuthorize(Roles = Roles.Doctor | Roles.Patient, SiteName = AppConfig.Sitename)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(PageViewModel<IUserViewModel> uploadModel)
        {
            var message = ConcatenateContactAdmin("Error.Message");

            if (ModelState.IsValid)
            {
                var profileResult = await ProfileManager.PostProfileAsync(uploadModel.Data, Request.RequestContext);

                switch (profileResult.ResultState)
                {
                    case PostProfileResultState.UserNotFound:
                        message = Localize("Controllers.Profile.Index.UserNotFound");
                        break;
                    case PostProfileResultState.UserNotMapped:
                    case PostProfileResultState.UserNotUpdated:
                        message = Localize("Controllers.Profile.Index.UserNotUpdated");
                        break;
                    case PostProfileResultState.UserUpdated:
                        message = Localize("Controllers.Profile.Index.UserUpdated");
                        break;
                    default:
                        break;
                }

                ViewBag.Message = message;
                var model = GetPageViewModel(profileResult.Data.UserViewModel, profileResult.Data.PageTitle ?? ErrorTitle);

                return View(model);
            }

            return await GetProfileAsync();
        }

        protected async Task<ActionResult> GetProfileAsync()
        {
            var userName = HttpContext.User?.Identity?.Name;

            if (!string.IsNullOrEmpty(userName))
            {
                var profileResult = await ProfileManager.GetProfileAsync(userName, Request.RequestContext);

                if (profileResult.Success)
                {
                    var model = GetPageViewModel(profileResult.Data.UserViewModel, profileResult.Data.PageTitle);

                    return View(model);
                }
            }

            return UserNotFound();
        }

        protected ActionResult UserNotFound()
        {
            ViewBag.Message = Dependencies.LocalizationService.Localize("General.UserNotFound");

            return View("ViewbagMessage", GetPageViewModel(ViewBag.Message));
        }
    }
}