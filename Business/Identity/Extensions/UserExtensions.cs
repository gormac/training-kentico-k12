using CMS.Membership;

using Business.Services.Context;
using Business.Identity.Models;
using Business.Identity.Helpers;

namespace Business.Identity.Extensions
{
    public static class UserExtensions
    {
        public static MedioClinicUser ToMedioClinicUser(this UserInfo userInfo, ISiteContextService siteContextService) =>
            new MedioClinicUser(UserInfoProvider.CheckUserBelongsToSite(userInfo, siteContextService.SiteName));

        public static UserInfo ToUserInfo(this MedioClinicUser user)
        {
            var userInfo = new UserInfo();
            UserHelper.UpdateUserInfo(ref userInfo, user);

            return userInfo;
        }
    }
}
