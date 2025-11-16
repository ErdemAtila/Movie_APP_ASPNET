using CORE.APP.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APP.Domain
{
    public class Role : Entity
    {
        [Required, StringLength(50)]
        public string Name { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
