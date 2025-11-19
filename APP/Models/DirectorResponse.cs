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

        // assigning custom or formatted properties to the response
        [DisplayName("Is Retired")]
        public string IsRetiredF { get; set; }

        [DisplayName("Movie Count")]
        public int MovieCount { get; set; }

        [DisplayName("Movies")]
        public string MovieNames { get; set; }

        [DisplayName("Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}

