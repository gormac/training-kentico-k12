using System.ComponentModel.DataAnnotations;

namespace MedioClinic.Models.Account
{
    public class RegisterViewModel : IViewModel
    {
        // TODO: Make localized
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public EmailViewModel EmailViewModel { get; set; }

        public PasswordConfirmationViewModel PasswordConfirmationViewModel { get; set; }

        [Display(Name = "I hereby accept that these provided information can be used for marketing purposes and targeted website content.")]
        public bool MarketingConsent { get; set; }
    }
}