using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Business.Identity;
using Business.Identity.Models;
using MedioClinic.Models.Profile;
using MedioClinic.Utils;

namespace MedioClinic
{
    public class ModelBindingConfig
    {
        public static void RegisterModelBinders()
        {
            ModelBinders.Binders.Add(typeof(IUserViewModel),
                new UserViewModelBinder(DependencyResolver.Current.GetService<IMedioClinicUserManager<MedioClinicUser, int>>()));
        }
    }
}