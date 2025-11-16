using CORE.APP.Models;
using System.ComponentModel.DataAnnotations;

namespace APP.Models
{
    public class RoleRequest : Request
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
