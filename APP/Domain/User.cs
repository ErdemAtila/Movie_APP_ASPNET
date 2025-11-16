using CORE.APP.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Domain
{
    public class User : Entity
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

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Score { get; set; }

        public bool IsActive { get; set; }

        public string? Address { get; set; }

        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
