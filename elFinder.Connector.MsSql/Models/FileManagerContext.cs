using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Dev.elFinder.Connector.MsSql.Models.Mapping;

namespace Dev.elFinder.Connector.MsSql.Models
{
    public partial class FileManagerContext : DbContext
    {
        static FileManagerContext()
        {
            Database.SetInitializer<FileManagerContext>(null);
        }

        public FileManagerContext()
            : base("Name=FileManagerContext")
        {
        }

        public DbSet<ElfinderFile> ElfinderFiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ElfinderFileMap());
        }
    }
}
