using CORE.APP.Models;
using System.ComponentModel;

namespace APP.Models
{
    public class DirectorResponse : Response
    {
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Is Retired")]
        public bool IsRetired { get; set; }

        [DisplayName("Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}

