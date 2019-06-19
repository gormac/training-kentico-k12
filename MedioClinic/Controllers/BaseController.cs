using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Business.DependencyInjection;
using Business.Dto.Company;
using Business.Dto.Menu;
using Business.Dto.Page;
using Business.Dto.Social;
using MedioClinic.Models;

namespace MedioClinic.Controllers
{
    public class BaseController : Controller
    {
        public string ErrorTitle => Localize("General.Error");

        protected IBusinessDependencies Dependencies { get; }

        protected BaseController(IBusinessDependencies dependencies)
        {
            Dependencies = dependencies;
        }

        public PageViewModel GetPageViewModel(
            string title, 
            string message = null, 
            bool displayAsRaw = false, 
            MessageType messageType = MessageType.Info) 
        {
            return new PageViewModel()
            {
                MenuItems = Dependencies.MenuRepository.GetMenuItems() ?? new List<MenuItemDto>(),
                Metadata = GetPageMetadata(title),
                Company = GetCompany(),
                Cultures = Dependencies.CultureService.GetSiteCultures(),
                SocialLinks = GetSocialLinks(),
                UserMessage = new UserMessage
                {
                    Message = message,
                    MessageType = messageType,
                    DisplayAsRaw = displayAsRaw
                }
            };
        }

        public PageViewModel<TViewModel> GetPageViewModel<TViewModel>(
            TViewModel data, 
            string title, 
            string message = null,
            bool displayAsRaw = false,
            MessageType messageType = MessageType.Info) 
            where TViewModel : IViewModel
        {
            return new PageViewModel<TViewModel>()
            {
                MenuItems = Dependencies.MenuRepository.GetMenuItems() ?? new List<MenuItemDto>(),
                Metadata = GetPageMetadata(title),
                Company = GetCompany(),
                Cultures = Dependencies.CultureService.GetSiteCultures(),
                SocialLinks = GetSocialLinks(),
                UserMessage = new UserMessage
                {
                    Message = message,
                    MessageType = messageType,
                    DisplayAsRaw = displayAsRaw
                },
                Data = data
            };
        }

        protected string ConcatenateContactAdmin(string messageKey) =>
            Localize(messageKey)
                + " "
                + Localize("ContactAdministrator");

        protected string Localize(string resourceKey) =>
            Dependencies.LocalizationService.Localize(resourceKey);

        protected ActionResult InvalidInput<TUploadViewModel>(
            PageViewModel<TUploadViewModel> uploadModel)
            where TUploadViewModel : IViewModel
        {
            var viewModel = GetPageViewModel(
                uploadModel.Data,
                Localize("BasicForm.InvalidInput"),
                Localize("Controllers.Base.InvalidInput.Message"),
                false,
                MessageType.Error);

            return View(viewModel);
        }

        protected void AddErrors<TResultState>(IdentityManagerResult<TResultState> result)
            where TResultState : Enum
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        private IEnumerable<SocialLinkDto> GetSocialLinks()
        {
            return Dependencies.SocialLinkRepository.GetSocialLinks();
        }

        private PageMetadataDto GetPageMetadata(string title)
        {
            return new PageMetadataDto()
            {
                Title = title,
                CompanyName = Dependencies.SiteContextService.SiteName
            };
        }

        private CompanyDto GetCompany()
        {
            return Dependencies.CompanyRepository.GetCompany();
        }


    }
}