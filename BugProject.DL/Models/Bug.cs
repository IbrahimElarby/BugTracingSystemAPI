using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugProject.DL
{
    public  class Bug
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid ProjectId { get; set; }

        public Project Project { get; set; }

        public ICollection<User> Assignees { get; set; } = new List<User>();
        public ICollection<Attachment> Attachments { get; set; } = new HashSet<Attachment>();
    }
}
