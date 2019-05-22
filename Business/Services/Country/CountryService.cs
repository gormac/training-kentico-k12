using System.Collections.Generic;
using System.Linq;

using CMS.Globalization;
using Business.Dto.Country;

namespace Business.Services.Country
{
    class CountryService : BaseService, ICountryService
    {
        public IEnumerable<CountryDto> GetCountries() =>
            CountryInfoProvider.GetCountries()
                .TypedResult
                .Items
                .Select(country => new CountryDto
                {
                    CountryName = country.CountryName,
                    CountryDisplayName = country.CountryDisplayName
                });
    }
}
