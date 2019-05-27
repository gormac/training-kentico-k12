using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

using Business.Identity.Models;
using MedioClinic.Models.Account;
using MedioClinic.Utils;

namespace MedioClinic.Models.Profile
{
    public class CommonUserViewModel
    {
        [Required]
        [Display(Name = "First name")]
        [MaxLength(100, ErrorMessage = ViewModelHelper.MaxLengthMessage)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [MaxLength(100, ErrorMessage = ViewModelHelper.MaxLengthMessage)]
        public string LastName { get; set; }

        [Display(Name = "Full name")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        [Display(Name = "ID")]
        //[ReadOnly(true)]
        public int Id { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public EmailViewModel EmailViewModel { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = ViewModelHelper.PhoneFormatMessage)]
        public string Phone { get; set; }

        public string Nationality { get; set; }

        [HiddenInput]
        public string AvatarContentPath { get; set; }

        [Display(Name = "Upload an avatar")]
        public HttpPostedFileBase AvatarFile { get; set; }
    }
}