using System.Web.Mvc;

using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

using MedioClinic.Attributes;
using MedioClinic.Utils;

namespace MedioClinic
{
    public class ApplicationConfig
    {
        public static void RegisterFeatures(IApplicationBuilder builder)
        {
            // Enable required Kentico features
            builder.UsePreview();
            builder.UseDataAnnotationsLocalization();
        }
    }
}