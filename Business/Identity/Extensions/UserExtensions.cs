using System.Collections.Generic;

using CMS.Membership;

using Business.Config;
using Business.Services.Context;
using Business.Identity.Models;
using Business.Identity.Helpers;
using System;

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
        public static MedioClinicUser ToMedioClinicUser(this UserInfo userInfo, string siteName) =>
            new MedioClinicUser(UserInfoProvider.CheckUserBelongsToSite(userInfo, siteName));

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

        // TODO: Document
        public static bool IsDoctor(this MedioClinicUser user, string siteName) =>
            UserInfoProvider.IsUserInRole(user.UserName, BusinessConfig.DoctorRoleName, siteName);

        public static bool IsPatient(this MedioClinicUser user, string siteName) =>
            UserInfoProvider.IsUserInRole(user.UserName, BusinessConfig.PatientRoleName, siteName);

        public static Roles ToMedioClinicRoles(this IEnumerable<string> roles)
        {
            Roles foundRoles = Roles.None;

            foreach (var role in roles)
            {
                if (Enum.TryParse(role, out Roles mcRole))
                {
                    foundRoles |= mcRole;
                }
            }

            return foundRoles;
        }
    }
}
