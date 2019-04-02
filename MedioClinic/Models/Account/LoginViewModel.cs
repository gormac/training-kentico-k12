using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedioClinic.Models.Account
{
    public class SigninViewModel : IViewModel
    {
        protected const string MaxLengthMessage = "The text shouldn't exceed 100 characters in total.";

        [Required(ErrorMessage = "Email is required.")]
        [DisplayName("Email address")]
        [MaxLength(100, ErrorMessage = MaxLengthMessage)]
        public string Email { get; set; }


        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [MaxLength(100, ErrorMessage = MaxLengthMessage)]
        public string Password { get; set; }


        [DisplayName("Stay signed in")]
        public bool StaySignedIn { get; set; }
    }
}