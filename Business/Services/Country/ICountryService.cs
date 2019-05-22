using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.Dto.Country;

namespace Business.Services.Country
{
    public interface ICountryService
    {
        IEnumerable<CountryDto> GetCountries();
    }
}
