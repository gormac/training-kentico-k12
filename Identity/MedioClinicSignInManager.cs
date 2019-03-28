using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

using Identity.Models;

namespace Identity
{
    public class MedioClinicSignInManager : SignInManager<MedioClinicUser, int>
    {
        /// <summary>
        /// Creates the instance of <see cref="MedioClinicSignInManager"/>.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        /// <param name="authenticationManager">Authentication manager.</param>
        public MedioClinicSignInManager(MedioClinicUserManager userManager, IAuthenticationManager authenticationManager)
        : base(userManager, authenticationManager)
        {
        }


        /// <summary>
        /// Factory method that creates the <see cref="MedioClinicSignInManager"/> instance.
        /// </summary>
        /// <param name="options">Identity factory options.</param>
        /// <param name="context">OWIN context.</param>
        public static MedioClinicSignInManager Create(IdentityFactoryOptions<MedioClinicSignInManager> options, IOwinContext context)
        {
            return new MedioClinicSignInManager(context.GetUserManager<MedioClinicUserManager>(), context.Authentication);
        }
    }
}
