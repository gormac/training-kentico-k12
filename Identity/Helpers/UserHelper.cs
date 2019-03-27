using CMS.Membership;
using Identity.Models;

namespace Identity.Helpers
{
    public static class UserHelper
    {
        public static void UpdateUserInfo(ref UserInfo userInfo, MedioClinicUser user)
        {
            userInfo.UserName = user.UserName;
            userInfo.FullName = UserInfoProvider.GetFullName(user.FirstName, null, user.LastName);
            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;
            userInfo.Email = user.Email;
            userInfo.Enabled = user.Enabled;
            userInfo.UserSecurityStamp = user.SecurityStamp;
            userInfo.UserNickName = userInfo.GetFormattedUserName(true);
            userInfo.SetValue("UserPassword", user.PasswordHash);
            userInfo.SetValue("DateOfBirth", user.DateOfBirth);
            userInfo.UserSettings.UserGender = (int)user.Gender;
            userInfo.SetValue("City", user.City);
            userInfo.SetValue("Street", user.Street);
            userInfo.UserSettings.UserPhone = user.Phone;
            userInfo.SetValue("Nationality", user.Nationality);
        }
    }
}
