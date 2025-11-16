using CORE.APP.Models;
using System.ComponentModel;

namespace APP.Models
{
    public class RoleResponse : Response
    {
        [DisplayName("Role Name")]
        public string Name { get; set; }
    }
}
