using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

using EnumsNET;
using Business.Attributes;
using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Extensions;
using Business.Identity.Models;
using Business.Repository.Avatar;
using Business.Services.Errors;
using Business.Services.FileManagement;
using Business.Services.Localization;
using Business.Services.ViewModel;
using MedioClinic.Config;
using MedioClinic.Models;
using MedioClinic.Models.Account;
using MedioClinic.Models.Profile;

namespace MedioClinic.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class ProfileController : BaseController
    {
        public IMedioClinicUserManager<MedioClinicUser, int> UserManager { get; }

        public IMedioClinicUserStore UserStore { get; }

        public IUserModelService UserModelService { get; }

        public IErrorHelperService ErrorHelperService { get; }

        public IFileManagementService FileManagementService { get; }

        public ILocalizationService LocalizationService { get; }

        public IAvatarRepository AvatarRepository { get; }

        public ProfileController(
            IMedioClinicUserManager<MedioClinicUser, int> userManager,
            IMedioClinicUserStore userStore,
            IUserModelService userModelService,
            IErrorHelperService errorHelper,
            IFileManagementService fileManagementHelper,
            ILocalizationService localizationService,
            IAvatarRepository avatarRepository,
            IBusinessDependencies dependencies) : base(dependencies)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            UserModelService = userModelService ?? throw new ArgumentNullException(nameof(userModelService));
            ErrorHelperService = errorHelper ?? throw new ArgumentNullException(nameof(errorHelper));
            FileManagementService = fileManagementHelper ?? throw new ArgumentNullException(nameof(fileManagementHelper));
            LocalizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            AvatarRepository = avatarRepository ?? throw new ArgumentNullException(nameof(avatarRepository));
        }

        // GET: Profile
        [MedioClinicAuthorize(Roles = Roles.Doctor | Roles.Patient, SiteName = AppConfig.Sitename)]
        public async Task<ActionResult> Index()
        {
            MedioClinicUser user = null;

            try
            {
                user = await UserManager.FindByNameAsync(HttpContext.User?.Identity?.Name);
            }
            catch (Exception ex)
            {
                ErrorHelperService.LogException(nameof(ProfileController), nameof(Index), ex);
            }

            var model = GetViewModelByUserRoles(user);

            if (model != null)
            {
                return View(model);
            }

            return UserNotFound();
        }

        // POST: Profile
        [MedioClinicAuthorize(Roles = Roles.Doctor | Roles.Patient, SiteName = AppConfig.Sitename)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(PageViewModel<IUserViewModel> uploadModel)
        {
            PageViewModel<IUserViewModel> model = null;
            MedioClinicUser user = null;

            if (ModelState.IsValid)
            {

                try
                {
                    user = await UserManager.FindByIdAsync(uploadModel.Data.CommonUserViewModel.Id);
                }
                catch (Exception ex)
                {
                    ErrorHelperService.LogException(nameof(ProfileController), nameof(Index), ex);
                }

                var commonUserModelCustomMappings = new Dictionary<(string propertyName, Type propertyType), object>
                {
                    { (nameof(MedioClinicUser.Email), typeof(string)), uploadModel.Data.CommonUserViewModel.EmailViewModel.Email },
                };

                try
                {
                    // Map the common user properties.
                    user = UserModelService.MapToMedioClinicUser(uploadModel.Data.CommonUserViewModel, user, commonUserModelCustomMappings);

                    // Map all other potential properties of specific models (patient, doctor, etc.)
                    user = UserModelService.MapToMedioClinicUser(uploadModel.Data, user);
                }
                catch (Exception ex)
                {
                    ErrorHelperService.LogException(nameof(ProfileController), nameof(Index), ex);
                }

                try
                {
                    // We need to use the user store directly due to the design of Microsoft.AspNet.Identity.Core.UserManager.UpdateAsync().
                    await UserStore.UpdateAsync(user);

                    var avatarFile = uploadModel.Data.CommonUserViewModel.AvatarFile;

                    if (avatarFile != null)
                    {
                        var avatarBinary = FileManagementService.GetPostedFileBinary(avatarFile);
                        AvatarRepository.UploadUserAvatar(user, avatarBinary);
                    }
                }
                catch (Exception ex)
                {
                    ErrorHelperService.LogException(nameof(ProfileController), nameof(Index), ex);
                }

                ViewBag.Message = LocalizationService.Localize("Controllers.Profile.Index.SavedChanges");
                model = GetViewModelByUserRoles(user, true);

                if (model != null)
                {
                    return View(model);
                }
            }

            try
            {
                user = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);
            }
            catch (Exception ex)
            {
                ErrorHelperService.LogException(nameof(ProfileController), nameof(Index), ex);
            }

            model = GetViewModelByUserRoles(user);

            if (model != null)
            {
                return View(model);
            }

            return UserNotFound();
        }


        /// <summary>
        /// Computes the user view model, based on roles.
        /// </summary>
        /// <param name="user">User to compute the view model by.</param>
        /// <param name="forceAvatarFileOverwrite">Flag that signals the need to update the app-local physical avatar file.</param>
        /// <returns>Master <see cref="PageViewModel{TViewModel}"/> filled with models of specific user roles.</returns>
        protected PageViewModel<IUserViewModel> GetViewModelByUserRoles(MedioClinicUser user, bool forceAvatarFileOverwrite = false)
        {
            if (user != null)
            {
                var roles = user.Roles.ToMedioClinicRoles();
                string avatarPhysicalPath = EnsureAvatarPhysicalPath(user, forceAvatarFileOverwrite);
                var avatarRelativePath = avatarPhysicalPath != null ? FileManagementService.GetServerRelativePath(Request, avatarPhysicalPath) : string.Empty;

                var commonUserModelCustomMappings = new Dictionary<(string propertyName, Type propertyType), object>
                {
                    { (nameof(CommonUserViewModel.EmailViewModel), typeof(EmailViewModel)), new EmailViewModel { Email = user.Email } },
                    { (nameof(CommonUserViewModel.AvatarContentPath), typeof(string)), avatarRelativePath }
                };

                object mappedParentModel = null;

                try
                {
                    // Map the common user properties.
                    var mappedCommonUserModel = UserModelService.MapToViewModel(user, typeof(CommonUserViewModel), commonUserModelCustomMappings);

                    Type userViewModelType = FlagEnums.HasAnyFlags(roles, Roles.Doctor) ? typeof(DoctorViewModel) : typeof(PatientViewModel);

                    var parentModelCustomMappings = new Dictionary<(string propertyName, Type propertyType), object>
                    {
                        { (nameof(CommonUserViewModel), typeof(CommonUserViewModel)), mappedCommonUserModel }
                    };

                    // Map all other potential properties of specific models (patient, doctor, etc.)
                    mappedParentModel = UserModelService.MapToViewModel(user, userViewModelType, parentModelCustomMappings);
                }
                catch (Exception ex)
                {
                    ErrorHelperService.LogException(nameof(ProfileController), nameof(GetViewModelByUserRoles), ex);
                }

                var finalViewModel = GetPageViewModel((IUserViewModel)mappedParentModel, GetTitle(roles));

                return finalViewModel;
            }

            return null;
        }

        protected string GetTitle(Roles roles) =>
            FlagEnums.HasAnyFlags(roles, Roles.Doctor)
                ? LocalizationService.Localize("Controllers.Profile.Index.Title.Doctor")
                : LocalizationService.Localize("Controllers.Profile.Index.Title.Patient");


        protected string EnsureAvatarPhysicalPath(MedioClinicUser user, bool forceOverwrite = false)
        {
            (var avatarFileName, var avatarBinary) = AvatarRepository.GetUserAvatar(user);

            avatarFileName = avatarFileName ?? DefaultAvatarFileName;
            string avatarPhysicalPath = GetAvatarContentPath(avatarFileName);

            if (!avatarFileName.Equals(DefaultAvatarFileName, StringComparison.OrdinalIgnoreCase))
            {
                FileManagementService.WriteFileIfDoesntExist(avatarPhysicalPath, avatarBinary, forceOverwrite);
            }

            return avatarPhysicalPath;
        }

        protected string GetAvatarContentPath(string avatarFileName)
        {
            var physicalPath = Server.MapPath($"{ContentFolder}/{AvatarFolder}");
            var folder = FileManagementService.EnsureFilePath(physicalPath);
            var fileName = FileManagementService.MakeStringUrlCompliant(avatarFileName);

            return $"{folder}\\{fileName}";
        }

        protected ActionResult UserNotFound()
        {
            ViewBag.Message = LocalizationService.Localize("general.usernotfound");

            return View("ViewbagMessage", GetPageViewModel(ViewBag.Message));
        }
    }
}