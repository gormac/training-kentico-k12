using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using Business.Identity.Models;

namespace Business.Identity
{
    public class MedioClinicSignInManager : SignInManager<MedioClinicUser, int>, IMedioClinicSignInManager<MedioClinicUser, int>
    {
        /// <summary>
        /// Creates the instance of <see cref="MedioClinicSignInManager"/>.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        /// <param name="authenticationManager">Authentication manager.</param>
        public MedioClinicSignInManager(IMedioClinicUserManager<MedioClinicUser, int> userManager, IAuthenticationManager authenticationManager)
        : base(userManager as UserManager<MedioClinicUser, int>, authenticationManager)
        {
        }

        IMedioClinicUserManager<MedioClinicUser, int> IMedioClinicSignInManager<MedioClinicUser, int>.UserManager
        {
            get => UserManager as IMedioClinicUserManager<MedioClinicUser, int>;
            set => UserManager = value as UserManager<MedioClinicUser, int>;
        }
    }
}
