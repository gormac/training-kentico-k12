using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using MedioClinic.Utils;

namespace MedioClinic.Models.Account
{
    public class EmailViewModel : IViewModel
    {
        [Required(ErrorMessage = "general.requireemail")]
        [DisplayName("general.emailaddress")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = ViewModelHelper.EmailFormatMessage)]
        [MaxLength(100, ErrorMessage = ViewModelHelper.MaxLengthMessage)]
        public string Email { get; set; }
    }
}