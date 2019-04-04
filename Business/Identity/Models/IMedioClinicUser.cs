using Microsoft.AspNet.Identity;
using System;

namespace Business.Identity.Models
{
    // TODO: Delete
    public interface IMedioClinicUser : IUser<int>
    {
        string City { get; set; }
        DateTime DateOfBirth { get; set; }
        Gender Gender { get; set; }
        string Nationality { get; set; }
        string Phone { get; set; }
        string Street { get; set; }
    }
}