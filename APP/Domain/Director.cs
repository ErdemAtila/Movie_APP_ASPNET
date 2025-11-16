using CORE.APP.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APP.Domain
{
    public class Director : Entity
    {
        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        public string LastName { get; set; }

        public bool IsRetired { get; set; }

        public ICollection<Movie> Movies { get; set; }
    }
}

