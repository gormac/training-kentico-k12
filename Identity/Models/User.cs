using System;

namespace Identity.Models
{
    public class User : Kentico.Membership.User
    {

        public int UserId { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        //public string Email { get; set; }
        public string Phone { get; set; }
        public string Nationality { get; set; }
    }
}
