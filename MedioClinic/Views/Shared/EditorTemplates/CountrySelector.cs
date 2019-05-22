using System.Web.Mvc;

using Business.Services.Country;

namespace MedioClinic.Views
{
    public class CountrySelector<TModel> : WebViewPage<TModel>
    {
        public ICountryService CountryService { get; set; }

        /// <summary>
        /// Left empty in purpose as countries should be loaded explicitly.
        /// </summary>
        public override void Execute()
        {
        }
    }
}