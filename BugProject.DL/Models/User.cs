using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace BugProject.DL
{
    public class User : IdentityUser<Guid>
    {
        public ICollection<Bug> AssignedBugs { get; set; } = new List<Bug>();
    }
}
