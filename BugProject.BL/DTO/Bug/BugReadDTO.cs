using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugProject.BL
{
    public class BugReadDTO
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid ProjectId { get; set; }

        public List<string> AssigneeUsernames { get; set; }
        public List<AttachmentReadDTO> Attachments { get; set; }
    }
}
