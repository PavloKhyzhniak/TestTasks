using Microsoft.EntityFrameworkCore;
using System.Data;
using TestTask_InfTech.DB.Model;

namespace TestTask_InfTech.DB
{
    public class FileStorageDB:DbContext
    {
        public DbSet<Model.File> Files { get; set; } = null!;
        public DbSet<Folder> Folder { get; set; } = null!;
        public DbSet<Extension> Ext { get; set; } = null!;
        public FileStorageDB(DbContextOptions<FileStorageDB> dbContextOptions)
            :base(dbContextOptions) 
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
