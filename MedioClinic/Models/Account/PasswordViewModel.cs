using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using MedioClinic.Utils;

namespace MedioClinic.Models.Account
{
    public class PasswordViewModel : IViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [MaxLength(100, ErrorMessage = ViewModelHelper.MaxLengthMessage)]
        public string Password { get; set; }
    }
}