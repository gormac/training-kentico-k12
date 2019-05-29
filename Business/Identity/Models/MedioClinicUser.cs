using System;

using CMS.Membership;

namespace Business.Identity.Models
{
    /// <summary>
    /// A derived <see cref="Kentico.Membership.User"/> class created for the purpose of the Medio Clinic website.
    /// </summary>
    /// <remarks>Designed to contain all role-specific properties of all users.</remarks>
    public class MedioClinicUser : Kentico.Membership.User
    {
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string Phone { get; set; }

        public string Nationality { get; set; }

        public int AvatarId { get; set; }

        /// <summary>
        /// Explicit default constructor due to the existence of <see cref="MedioClinicUser(UserInfo)"/>.
        /// </summary>
        public MedioClinicUser()
        {
        }

        /// <summary>
        /// Creates a <see cref="MedioClinicUser"/> object out of a <see cref="UserInfo"/> one.
        /// </summary>
        /// <param name="userInfo">The input object.</param>
        public MedioClinicUser(UserInfo userInfo) : base(userInfo)
        {
            if (userInfo == null)
            {
                return;
            }

            DateOfBirth = userInfo.GetDateTimeValue("UserDateOfBirth", DateTime.MinValue);
            Gender = (Gender)userInfo.UserSettings.UserGender;
            City = userInfo.GetStringValue("City", string.Empty);
            Street = userInfo.GetStringValue("Street", string.Empty);
            Phone = userInfo.UserSettings.UserPhone;
            Nationality = userInfo.GetStringValue("Nationality", string.Empty);
            AvatarId = userInfo.UserAvatarID;
        }
    }
}
