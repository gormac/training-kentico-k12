using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedioClinic.Models.Profile
{
    public interface IUserViewModel : IViewModel
    {
        CommonUserViewModel CommonUserViewModel { get; set; }
    }
}
