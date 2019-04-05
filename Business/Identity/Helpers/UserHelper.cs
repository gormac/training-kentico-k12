using CMS.Membership;
using Business.Identity.Models;

namespace Business.Identity.Helpers
{
    public static class UserHelper
    {
        /// <summary>
        /// Updates the custom fields of the <see cref="UserInfo"/> object with strongly-typed properties of the <see cref="MedioClinicUser"/> object.
        /// </summary>
        /// <param name="userInfo">The object to update.</param>
        /// <param name="medioClinicUser">The input object.</param>
        public static void UpdateUserInfo(ref UserInfo userInfo, MedioClinicUser medioClinicUser)
        {
            userInfo.UserName = medioClinicUser.UserName;
            userInfo.FullName = UserInfoProvider.GetFullName(medioClinicUser.FirstName, null, medioClinicUser.LastName);
            userInfo.FirstName = medioClinicUser.FirstName;
            userInfo.LastName = medioClinicUser.LastName;
            userInfo.Email = medioClinicUser.Email;
            userInfo.Enabled = medioClinicUser.Enabled;
            userInfo.UserSecurityStamp = medioClinicUser.SecurityStamp;
            userInfo.UserNickName = userInfo.GetFormattedUserName(true);
            userInfo.SetValue("UserPassword", medioClinicUser.PasswordHash);
            userInfo.SetValue("DateOfBirth", medioClinicUser.DateOfBirth);
            userInfo.UserSettings.UserGender = (int)medioClinicUser.Gender;
            userInfo.SetValue("City", medioClinicUser.City);
            userInfo.SetValue("Street", medioClinicUser.Street);
            userInfo.UserSettings.UserPhone = medioClinicUser.Phone;
            userInfo.SetValue("Nationality", medioClinicUser.Nationality);
        }
    }
}
