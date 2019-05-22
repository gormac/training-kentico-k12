using System.Collections.Generic;
using System.Linq;

using CMS.SiteProvider;
using Business.Dto.Culture;
using Business.Services.Context;

namespace Business.Services.Culture
{
    public class CultureService : BaseService, ICultureService
    {
        private ISiteContextService SiteContextService { get; }

        public CultureService(ISiteContextService siteContextService)
        {
            SiteContextService = siteContextService;
        }

        public IEnumerable<CultureDto> GetSiteCultures()
        {
            return CultureSiteInfoProvider.GetSiteCultures(SiteContextService.SiteName).Items.Select(m =>
                new CultureDto()
                {
                    CultureCode = m.CultureCode,
                    CultureName = m.CultureName,
                    CultureShortName = m.CultureShortName
                }
            );
        }
    }
}
