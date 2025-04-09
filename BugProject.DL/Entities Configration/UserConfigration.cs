using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugProject.DL
{
    public class UserConfigration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Many-to-many: User <-> Bug (AssignedBugs)
            builder.HasMany(u => u.AssignedBugs)
                   .WithMany(b => b.Assignees)
                   .UsingEntity(j => j.ToTable("BugAssignees"));
        }
    }
}
