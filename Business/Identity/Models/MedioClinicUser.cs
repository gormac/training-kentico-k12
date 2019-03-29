using System;

using CMS.Membership;

namespace Business.Identity.Models
{
    public class MedioClinicUser : Kentico.Membership.User
    {
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Phone { get; set; }
        public string Nationality { get; set; }

        public MedioClinicUser()
        {
        }

        public MedioClinicUser(UserInfo userInfo) : base(userInfo)
        {
            if (userInfo == null)
            {
                return;
            }

            DateOfBirth = userInfo.GetDateTimeValue("DateOfBirth", DateTime.MinValue);
            Gender = (Gender)userInfo.UserSettings.UserGender;
            City = userInfo.GetStringValue("City", string.Empty);
            Street = userInfo.GetStringValue("Street", string.Empty);
            Phone = userInfo.UserSettings.UserPhone;
            Nationality = userInfo.GetStringValue("Nationality", string.Empty);
        }
    }
}
