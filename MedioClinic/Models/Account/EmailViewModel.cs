using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using MedioClinic.Helpers;

namespace MedioClinic.Models.Account
{
    public class EmailViewModel : IViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [DisplayName("Email address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = ViewModelHelper.EmailFormatMessage)]
        [MaxLength(100, ErrorMessage = ViewModelHelper.MaxLengthMessage)]
        public string Email { get; set; }
    }
}