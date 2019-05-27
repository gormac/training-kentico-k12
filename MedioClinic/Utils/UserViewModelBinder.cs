using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EnumsNET;
using Business.Identity;
using Business.Identity.Extensions;
using Business.Identity.Models;
using MedioClinic.Models.Profile;

namespace MedioClinic.Utils
{
    public class UserViewModelBinder : IModelBinder
    {
        public IMedioClinicUserManager<MedioClinicUser, int> UserManager { get; set; }

        public DefaultModelBinder DefaultModelBinder => new DefaultModelBinder();

        public UserViewModelBinder(IMedioClinicUserManager<MedioClinicUser, int> userManager)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(IUserViewModel))
            {
                throw new Exception($"The model is not of type {nameof(IUserViewModel)}.");
            }
            else
            {
                var user = UserManager.FindByNameAsync(controllerContext.HttpContext.User?.Identity?.Name).Result;

                if (user == null)
                {
                    throw new Exception($"The {nameof(IUserViewModel)} model cannot be bound because the user could not be retrieved.");
                }
                else
                {
                    var userRoles = UserManager.GetRolesAsync(user.Id).Result.ToMedioClinicRoles();

                    // The roles should be evaluated from the highest to the lowest.
                    if (FlagEnums.HasAnyFlags(Roles.Doctor, userRoles))
                    {
                        bindingContext.ModelMetadata.Model = new DoctorViewModel();
                    }
                    else if (FlagEnums.HasAnyFlags(Roles.Patient, userRoles))
                    {
                        bindingContext.ModelMetadata.Model = new PatientViewModel();
                    }

                    return DefaultModelBinder.BindModel(controllerContext, bindingContext);
                }
            }
        }

        //protected CommonUserViewModel MapCommonUserModel(ModelBindingContext bindingContext)
        //{
        //    var properties = typeof(CommonUserViewModel).GetProperties();

        //    foreach (var property in properties)
        //    {
        //        var prefix = "Data.CommonUserViewModel.";

        //        var valueResult = bindingContext.ValueProvider.GetValue($"{prefix}{property.Name}");

        //        ;
        //    }

        //    return new CommonUserViewModel();
        //}
    }
}