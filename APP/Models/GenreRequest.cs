using CORE.APP.Models;
using System.ComponentModel.DataAnnotations;

namespace APP.Models
{
    public class GenreRequest : Request
    {
        [Required, StringLength(50)]
        public string Name { get; set; }
    }
}

