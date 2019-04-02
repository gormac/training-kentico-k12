using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

using Business.Identity;
using Business.Identity.Models;

namespace MedioClinic
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<IMedioClinicUserManager>() as MedioClinicUserManager);
            app.CreatePerOwinContext<MedioClinicSignInManager>(MedioClinicSignInManager.Create);

            // Configure the sign in cookie
            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<MedioClinicUserManager, MedioClinicUser, int>(
                        // Sets the interval after which the validity of the user's security stamp is checked
                        validateInterval: TimeSpan.FromMinutes(1),
                        regenerateIdentityCallback: (manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie),
                        getUserIdCallback: ((claimsIdentity) => int.Parse(claimsIdentity.GetUserId()))),
                    // Redirect to logon page with return url
                    OnApplyRedirect = context => context.Response.Redirect(urlHelper.Action("Signin", "Account") + new Uri(context.RedirectUri).Query)
                },
                ExpireTimeSpan = TimeSpan.FromDays(14),
                SlidingExpiration = true
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        }
    }
}