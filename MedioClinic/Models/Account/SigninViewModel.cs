using System.ComponentModel;

namespace MedioClinic.Models.Account
{
    public class SigninViewModel : IViewModel
    {
        public EmailViewModel EmailViewModel { get; set; }

        public PasswordViewModel PasswordViewModel { get; set; }

        [DisplayName("Stay signed in")]
        public bool StaySignedIn { get; set; }
    }
}