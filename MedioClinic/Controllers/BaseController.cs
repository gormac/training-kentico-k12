using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

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

        public PageViewModel GetPageViewModel(string title) 
        {
            return new PageViewModel()
            {
                MenuItems = Dependencies.MenuRepository.GetMenuItems() ?? new List<MenuItemDto>(),
                Metadata = GetPageMetadata(title),
                Company = GetCompany(),
                Cultures = Dependencies.CultureService.GetSiteCultures(),
                SocialLinks = GetSocialLinks(),
            };
        }

        public PageViewModel<TViewModel> GetPageViewModel<TViewModel>(TViewModel data, string title) where TViewModel : IViewModel
        {
            return new PageViewModel<TViewModel>()
            {
                MenuItems = Dependencies.MenuRepository.GetMenuItems() ?? new List<MenuItemDto>(),
                Metadata = GetPageMetadata(title),
                Company = GetCompany(),
                Cultures = Dependencies.CultureService.GetSiteCultures(),
                SocialLinks = GetSocialLinks(),
                Data = data
            };
        }

        protected string ConcatenateContactAdmin(string messageKey) =>
            Localize(messageKey)
                + " "
                + Localize("ContactAdministrator");

        protected string Localize(string resourceKey) =>
            Dependencies.LocalizationService.Localize(resourceKey);

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