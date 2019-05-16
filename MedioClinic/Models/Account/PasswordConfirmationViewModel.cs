using System.ComponentModel.DataAnnotations;

namespace MedioClinic.Models.Account
{
    public class PasswordConfirmationViewModel : PasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "general.confirmpassword")]
        [Compare("Password", ErrorMessage = "changepassword.errornewpassword")]
        public string ConfirmPassword { get; set; }
    }
}