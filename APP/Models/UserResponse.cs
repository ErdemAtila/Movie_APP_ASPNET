using APP.Domain;
using CORE.APP.Models;
using System;
using System.ComponentModel;

namespace APP.Models
{
    public class UserResponse : Response
    {
        [DisplayName("Username")]
        public string UserName { get; set; }

        [DisplayName("First Name")]
        public string? FirstName { get; set; }

        [DisplayName("Last Name")]
        public string? LastName { get; set; }

        public Genders Gender { get; set; }

        [DisplayName("Birth Date")]
        public DateTime? BirthDate { get; set; }

        // assigning custom or formatted properties to the response
        [DisplayName("Birth Date")]
        public string BirthDateF { get; set; }

        [DisplayName("Registration Date")]
        public DateTime RegistrationDate { get; set; }

        // assigning custom or formatted properties to the response
        [DisplayName("Registration Date")]
        public string RegistrationDateF { get; set; }

        public decimal Score { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        public string? Address { get; set; }
        
        [DisplayName("Group")]
        public string GroupName { get; set; }
    }
}
