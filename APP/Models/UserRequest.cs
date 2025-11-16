using APP.Domain;
using CORE.APP.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace APP.Models
{
    public class UserRequest : Request
    {
        [Required, StringLength(100)]
        public string UserName { get; set; }

        [Required, StringLength(100)]
        public string Password { get; set; }

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        public Genders Gender { get; set; }

        public DateTime? BirthDate { get; set; }

        public decimal Score { get; set; }

        public bool IsActive { get; set; }

        public string? Address { get; set; }

        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public int? GroupId { get; set; }
    }
}
