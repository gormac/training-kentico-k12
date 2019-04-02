using System.Web;
using System.Web.Mvc;

using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

using MedioClinic.Models;

namespace MedioClinic.Extensions
{
    public static class UrlExtensions
    {

        /// <summary>
        /// Custom extension method for retrieving image urls based on their path
        /// </summary>
        /// <param name="helper">Html helper</param>
        /// <param name="path">Path to file</param>
        /// <param name="size">Size constraints</param>
        /// <returns></returns>
        public static string KenticoImageUrl(this UrlHelper helper, string path, IImageSizeConstraint size = null) =>
            helper.Kentico().ImageUrl(path, size?.GetSizeConstraint() ?? SizeConstraint.Empty);

        public static string AbsoluteUrl(this UrlHelper helper, HttpRequestBase request, string action, string controller, object routeValues)
        {
            var scheme = request?.Url?.Scheme;
            var domain = request?.Url?.Host;
            var relativePath = helper.Action(action, controller, routeValues);

            return $"{scheme}://{domain}{relativePath}";
        }
    }
}
