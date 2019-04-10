using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Business.Identity.Models;
using MedioClinic.Helpers;
using MedioClinic.Models.Account;

namespace MedioClinic.Models.Profile
{
    public class UserViewModel : IViewModel
    {
        [Required]
        [Display(Name = "First name")]
        [MaxLength(100, ErrorMessage = ViewModelHelper.MaxLengthMessage)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [MaxLength(100, ErrorMessage = ViewModelHelper.MaxLengthMessage)]
        public string LastName { get; set; }

        [Display(Name = "Date of birth")]
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        [Display(Name = "ID")]
        [ReadOnly(true)]
        public string Id { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public EmailViewModel EmailViewModel { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = ViewModelHelper.PhoneFormatMessage)]
        public string Phone { get; set; }

        public string Nationality { get; set; }
    }
}