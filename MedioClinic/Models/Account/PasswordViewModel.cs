using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using MedioClinic.Helpers;

namespace MedioClinic.Models.Account
{
    public class PasswordViewModel : IViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [MaxLength(100, ErrorMessage = ViewModelHelper.MaxLengthMessage)]
        public string Password { get; set; }
    }
}