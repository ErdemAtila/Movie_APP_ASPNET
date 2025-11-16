using CORE.APP.Models;
using System.ComponentModel;

namespace APP.Models
{
    public class GenreResponse : Response
    {
        [DisplayName("Genre Name")]
        public string Name { get; set; }
    }
}

