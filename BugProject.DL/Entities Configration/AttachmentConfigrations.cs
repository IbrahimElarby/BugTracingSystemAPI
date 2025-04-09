using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugProject.DL
{
    public class AttachmentConfigrations : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            // Primary key
            builder.HasKey(a => a.Id);

            // FileName is required and limited in length
            builder.Property(a => a.FileName)
                   .IsRequired()
                   .HasMaxLength(255);

            // FilePath is required
            builder.Property(a => a.FilePath)
                   .IsRequired();

            // One-to-many: Attachment -> Bug
            builder.HasOne(a => a.Bug)
                   .WithMany(b => b.Attachments)
                   .HasForeignKey(a => a.BugId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
