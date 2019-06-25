using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

using Autofac;
using Autofac.Integration.Mvc;

using Business.DependencyInjection;
using Business.Identity;
using Business.Identity.Models;
using Business.Identity.Proxies;
using Business.Repository;
using Business.Services;
using Business.Services.Context;
using MedioClinic.Config;
using MedioClinic.Utils;

namespace MedioClinic
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            // Initializes the Autofac builder instance
            var builder = new ContainerBuilder();

            // Register dependencies in controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Adds a custom registration source (IRegistrationSource) that provides all services from the Kentico API
            builder.RegisterSource(new CmsRegistrationSource());

            // Registers all services that implement IService interface
            builder.RegisterAssemblyTypes(typeof(IService).Assembly)
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IService).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerRequest();

            // Registers site context
            builder.RegisterType<SiteContextService>().As<ISiteContextService>()
                .WithParameter((parameter, context) => parameter.Name == "currentCulture",
                    (parameter, context) => CultureInfo.CurrentUICulture.Name)
                .WithParameter((parameter, context) => parameter.Name == "sitename",
                    (parameter, context) => AppConfig.Sitename)
                .InstancePerRequest();

            // Registers business dependencies
            builder.RegisterType<BusinessDependencies>().As<IBusinessDependencies>()
                .InstancePerRequest();

            // Registers all repositories that implement IRepository interface
            builder.RegisterAssemblyTypes(typeof(IRepository).Assembly)
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IRepository).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerRequest();

            // Registers the class that wraps the Kentico.Membership.UserStore class.
            builder.Register(context => new KenticoUserStore(context.Resolve<ISiteContextService>().SiteName))
                .As<IKenticoUserStore>()
                .InstancePerRequest();

            // Registers the application-level user store.
            builder.RegisterType<MedioClinicUserStore>()
                .As<IMedioClinicUserStore>()
                .InstancePerRequest();

            // Registers the application-level user manager.
            builder.RegisterType<MedioClinicUserManager>()
                .As<IMedioClinicUserManager<MedioClinicUser, int>>()
                .InstancePerRequest();

            // Registers the authentication manager of the OWIN context for DI retrieval.
            builder.Register(context =>
                HttpContext.Current.GetOwinContext().Authentication)
                .As<IAuthenticationManager>();

            // Registers the application-level sign in manager.
            builder.RegisterType<MedioClinicSignInManager>()
                .As<IMedioClinicSignInManager<MedioClinicUser, int>>()
                .InstancePerRequest();

            // Registers the account manager.
            builder.RegisterType<AccountManager>()
                .As<IAccountManager>()
                .InstancePerRequest();

            // Registers the profile manager.
            builder.RegisterType<ProfileManager>()
                .As<IProfileManager>()
                .InstancePerRequest();

            // Registers a view registration source so that views can take advantage of DI.
            // See https://autofaccn.readthedocs.io/en/latest/integration/mvc.html#enable-property-injection-for-view-pages
            builder.RegisterSource(new ViewRegistrationSource());

            // Resolves the dependencies
            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }
    }
}