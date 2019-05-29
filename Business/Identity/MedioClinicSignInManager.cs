using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using Business.Identity.Models;

namespace Business.Identity
{
    /// <summary>
    /// App-level implementation of the ASP.NET Identity <see cref="SignInManager{TUser, TKey}"/> base class.
    /// </summary>
    public class MedioClinicSignInManager : SignInManager<MedioClinicUser, int>, IMedioClinicSignInManager<MedioClinicUser, int>
    {
        /// <summary>
        /// Makes the <see cref="UserManager{MedioClinicUser, int}"/> property accessible through the <see cref="IMedioClinicUserManager{MedioClinicUser, int}"/> interface.
        /// </summary>
        IMedioClinicUserManager<MedioClinicUser, int> IMedioClinicSignInManager<MedioClinicUser, int>.UserManager
        {
            get => UserManager as IMedioClinicUserManager<MedioClinicUser, int>;
            set => UserManager = value as UserManager<MedioClinicUser, int>;
        }

        /// <summary>
        /// Creates the instance of <see cref="MedioClinicSignInManager"/>.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        /// <param name="authenticationManager">Authentication manager.</param>
        public MedioClinicSignInManager(IMedioClinicUserManager<MedioClinicUser, int> userManager, IAuthenticationManager authenticationManager)
        : base(userManager as UserManager<MedioClinicUser, int>, authenticationManager)
        {
        }
    }
}
