using System;
using System.Collections.Generic;

using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Models;
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

        protected IdentityManagerResult<TResultState> InitResult<TResultState>()
            where TResultState : Enum
            => new IdentityManagerResult<TResultState>
                {
                    Errors = new List<string>()
                };

        protected IdentityManagerResult<TResultState, TData> InitResult<TResultState, TData>()
            where TResultState : Enum
            => new IdentityManagerResult<TResultState, TData>
                {
                    Errors = new List<string>()
                };
    }
}