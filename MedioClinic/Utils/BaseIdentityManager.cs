using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Models;
using Business.Repository.Avatar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MedioClinic.Models;

namespace MedioClinic.Utils
{
    // TODO: Document.
    public abstract class BaseIdentityManager
    {
        public IMedioClinicUserManager<MedioClinicUser, int> UserManager { get; }

        public IBusinessDependencies Dependencies { get; }

        public BaseIdentityManager(
            IMedioClinicUserManager<MedioClinicUser, int> userManager,
            IBusinessDependencies dependencies)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        }

        protected void HandleException<TResultState>(string methodName, Exception exception, ref IdentityManagerResult<TResultState> result)
            where TResultState : Enum
        {
            Dependencies.ErrorHelperService.LogException(this.GetType().Name, methodName, exception);
            result.Success = false;
            result.Errors.Add(exception.Message);
        }
    }
}