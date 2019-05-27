using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using Business.Repository.Avatar;

namespace MedioClinic.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class ProfileController : BaseController
    {
        public string ContentFolder => "~/Content/";

        public string AvatarFolder => "Avatar";

        public IMedioClinicUserManager<MedioClinicUser, int> UserManager { get; set; }

        public IMedioClinicUserStore UserStore { get; set; }

        public IUserModelService UserModelService { get; set; }

        public IErrorHelper ErrorHelper { get; set; }

        public IFileManagementHelper FileManagementHelper { get; set; }

        public IAvatarRepository AvatarRepository { get; set; }

        public ProfileController(
            IMedioClinicUserManager<MedioClinicUser, int> userManager,
            IMedioClinicUserStore userStore,
            IUserModelService userModelService,
            IErrorHelper errorHelper,
            IFileManagementHelper fileManagementHelper,
            IAvatarRepository avatarRepository,
            IBusinessDependencies dependencies) : base(dependencies)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            UserModelService = userModelService ?? throw new ArgumentNullException(nameof(userModelService));
            ErrorHelper = errorHelper ?? throw new ArgumentNullException(nameof(errorHelper));
            FileManagementHelper = fileManagementHelper ?? throw new ArgumentNullException(nameof(fileManagementHelper));
            AvatarRepository = avatarRepository ?? throw new ArgumentNullException(nameof(avatarRepository));
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
        public async Task<ActionResult> Index(PageViewModel<IUserViewModel> uploadModel)
        {
            if (ModelState.IsValid)
            {
                var userById = await UserManager.FindByIdAsync(uploadModel.Data.CommonUserViewModel.Id);

                var commonUserModelCustomMappings = new Dictionary<(string propertyName, Type propertyType), object>
                {
                    { (nameof(MedioClinicUser.Email), typeof(string)), uploadModel.Data.CommonUserViewModel.EmailViewModel.Email },
                };

                // Map the common user properties.
                userById = UserModelService.MapToMedioClinicUser(uploadModel.Data.CommonUserViewModel, userById, commonUserModelCustomMappings);

                // Map all other potential properties of specific models (patient, doctor, etc.)
                userById = UserModelService.MapToMedioClinicUser(uploadModel.Data, userById);

                try
                {
                    // We need to use the user store directly due to the design of Microsoft.AspNet.Identity.Core.UserManager.UpdateAsync().
                    await UserStore.UpdateAsync(userById);

                    var avatarFile = uploadModel.Data.CommonUserViewModel.AvatarFile;

                    if (avatarFile != null)
                    {
                        var avatarBinary = FileManagementHelper.GetPostedFileBinary(avatarFile);
                        AvatarRepository.UploadUserAvatar(userById, avatarBinary);
                    }
                }
                catch (Exception ex)
                {
                    ErrorHelper.LogException(nameof(ProfileController), nameof(Index), ex);
                }

                ViewBag.Message = ResHelper.GetString("Controllers.Profile.Index.SavedChanges");

                return View(GetViewModelByUserRoles(userById, true));
            }

            var user = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (user != null)
            {
                return View(GetViewModelByUserRoles(user));
            }

            return HttpNotFound();
        }

        protected static string GetTitle(Roles roles) =>
            FlagEnums.HasAnyFlags(roles, Roles.Doctor)
                ? ResHelper.GetString("Controllers.Profile.Index.Title.Doctor")
                : ResHelper.GetString("Controllers.Profile.Index.Title.Patient");


        protected PageViewModel<IUserViewModel> GetViewModelByUserRoles(MedioClinicUser user, bool forceAvatarFileOverwrite = false)
        {
            if (user != null)
            {
                var roles = user.Roles.ToMedioClinicRoles();
                string avatarPhysicalPath = EnsureAvatarPhysicalPath(user, forceAvatarFileOverwrite);
                var avatarRelativePath = FileManagementHelper.GetServerRelativePath(Request, avatarPhysicalPath);

                var commonUserModelCustomMappings = new Dictionary<(string propertyName, Type propertyType), object>
                {
                    { (nameof(CommonUserViewModel.EmailViewModel), typeof(EmailViewModel)), new EmailViewModel { Email = user.Email } },
                    { (nameof(CommonUserViewModel.AvatarContentPath), typeof(string)), avatarRelativePath }
                };

                // Map the common user properties.
                var mappedCommonUserModel = UserModelService.MapToViewModel(user, typeof(CommonUserViewModel), commonUserModelCustomMappings);

                Type userViewModelType = FlagEnums.HasAnyFlags(roles, Roles.Doctor) ? typeof(DoctorViewModel) : typeof(PatientViewModel);

                var parentModelCustomMappings = new Dictionary<(string propertyName, Type propertyType), object>
                {
                    { (nameof(CommonUserViewModel), typeof(CommonUserViewModel)), mappedCommonUserModel }
                };

                // Map all other potential properties of specific models (patient, doctor, etc.)
                var mappedParentModel = UserModelService.MapToViewModel(user, userViewModelType, parentModelCustomMappings);

                var finalViewModel = GetPageViewModel((IUserViewModel)mappedParentModel, GetTitle(roles));

                return finalViewModel;
            }

            return null;
        }

        protected string EnsureAvatarPhysicalPath(MedioClinicUser user, bool forceOverwrite = false)
        {
            (var avatarFileName, var avatarBinary) = AvatarRepository.GetUserAvatar(user);
            string avatarPhysicalPath = GetAvatarContentPath(avatarFileName);
            FileManagementHelper.WriteFileIfDoesntExist(avatarPhysicalPath, avatarBinary, forceOverwrite);

            return avatarPhysicalPath;
        }

        protected string GetAvatarContentPath(string avatarFileName)
        {
            var physicalPath = Server.MapPath($"{ContentFolder}{AvatarFolder}");
            var folder = FileManagementHelper.EnsureFilePath(physicalPath);
            var fileName = FileManagementHelper.MakeStringUrlCompliant(avatarFileName);

            return $"{folder}\\{fileName}";
        }
    }
}