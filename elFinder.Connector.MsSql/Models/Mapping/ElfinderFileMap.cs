using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dev.elFinder.Connector.MsSql.Models.Mapping
{
    public class ElfinderFileMap : EntityTypeConfiguration<ElfinderFile>
    {
        public ElfinderFileMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.Content)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.Mime)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            this.ToTable("ElfinderFile");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Parent_id).HasColumnName("Parent_id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Content).HasColumnName("Content");
            this.Property(t => t.Size).HasColumnName("Size");
            this.Property(t => t.Mtime).HasColumnName("Mtime");
            this.Property(t => t.Mime).HasColumnName("Mime");
            this.Property(t => t.Read).HasColumnName("Read");
            this.Property(t => t.Write).HasColumnName("Write");
            this.Property(t => t.Locked).HasColumnName("Locked");
            this.Property(t => t.Hidden).HasColumnName("Hidden");
            this.Property(t => t.Width).HasColumnName("Width");
            this.Property(t => t.Height).HasColumnName("Height");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
        }
    }
}
