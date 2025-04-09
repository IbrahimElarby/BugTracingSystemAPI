using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugProject.DL
{
    public class ProjectConfigration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            // Primary key
            builder.HasKey(p => p.Id);

            // Name is required and has max length
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            // Location is optional, add max length for safety
            builder.Property(p => p.Location)
                   .HasMaxLength(200);

            // One-to-many: Project -> Bugs
            builder.HasMany(p => p.Bugs)
                   .WithOne(b => b.Project)
                   .HasForeignKey(b => b.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
