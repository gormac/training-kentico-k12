using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

using CMS.Membership;

using Business.Identity.Extensions;
using Business.Identity.Models;

namespace Business.Attributes
{
    public class MedioClinicAuthorizeAttribute : AuthorizeAttribute
    {
        public new Roles Roles { get; set; }

        public string SiteName { get; set; }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            var user = HttpContext.Current.User;

            // TODO: Double-check
            if (user?.Identity?.IsAuthenticated == false)
            {
                return false;
            }

            var userRoles = UserInfoProvider.GetRolesForUser(user?.Identity?.Name, SiteName).ToMedioClinicRoles();

            return Roles.HasFlag(userRoles);
        }
    }
}
