using System;
using System.Web;
using System.Web.Mvc;

using EnumsNET;
using CMS.Membership;
using Business.Identity.Extensions;
using Business.Identity.Models;

namespace Business.Attributes
{
    public class MedioClinicAuthorizeAttribute : AuthorizeAttribute
    {
        public new Roles Roles { get; set; }

        public string SiteName { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            var user = HttpContext.Current.User;
            var userRoles = UserInfoProvider.GetRolesForUser(user?.Identity?.Name, SiteName).ToMedioClinicRoles();

            if (user?.Identity?.IsAuthenticated == false || !FlagEnums.HasAnyFlags(Roles, userRoles))
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
