using CMS.Membership;

using Business.Services.Context;
using Business.Identity.Models;
using Business.Identity.Helpers;

namespace Business.Identity.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Creates a <see cref="MedioClinicUser"/> object out of a <see cref="UserInfo"/> one.
        /// </summary>
        /// <param name="userInfo">The original object.</param>
        /// <param name="siteContextService">Service that supplies the site name.</param>
        /// <returns>The <see cref="MedioClinicUser"/> object.</returns>
        public static MedioClinicUser ToMedioClinicUser(this UserInfo userInfo, ISiteContextService siteContextService) =>
            new MedioClinicUser(UserInfoProvider.CheckUserBelongsToSite(userInfo, siteContextService.SiteName));

        /// <summary>
        /// Creates a <see cref="UserInfo"/> out of a <see cref="MedioClinicUser"/> one.
        /// </summary>
        /// <param name="medioClinicUser">The original object.</param>
        /// <returns>The <see cref="UserInfo"/> object.</returns>
        public static UserInfo ToUserInfo(this MedioClinicUser medioClinicUser)
        {
            var userInfo = new UserInfo();
            UserHelper.UpdateUserInfo(ref userInfo, medioClinicUser);

            return userInfo;
        }
    }
}
