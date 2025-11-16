using CORE.APP.Models;
using System.ComponentModel;

namespace APP.Models
{
    public class GroupResponse : Response
    {
        [DisplayName("Group Name")]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
