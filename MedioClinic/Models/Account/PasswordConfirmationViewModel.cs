using System.ComponentModel.DataAnnotations;

namespace MedioClinic.Models.Account
{
    public class PasswordConfirmationViewModel : PasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}