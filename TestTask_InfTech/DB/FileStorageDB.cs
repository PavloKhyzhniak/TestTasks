using Microsoft.EntityFrameworkCore;
using System.Data;
using TestTask_InfTech.DB.Model;

namespace TestTask_InfTech.DB
{
    public interface IFolderStorageController
    {
        //создать папку
        System.Threading.Tasks.Task<string> CreateFolderAsync(string aFolderName, string aParentFolder);
        //удалить папку
        System.Threading.Tasks.Task<string> DeleteFolderAsync(string aFolderName, string aParentFolder);
        //переименовать папку
        System.Threading.Tasks.Task<string> RenameFolderAsync(string aFolderName, string aParentFolder);
        //скачать файл
        System.Threading.Tasks.Task<string> DownloadFileAsync(string aFolderName);
        //загрузить файл
        System.Threading.Tasks.Task<string> SendFileAsync(string aFolderName);
        //удалить файл
        System.Threading.Tasks.Task<string> DeleteFileAsync(string aFolderName);
        //переименовать файл
        System.Threading.Tasks.Task<string> RenameFileAsync(string aFolderName);

    }

    public interface IFolderStorage
    {
        //получить все папки
        System.Threading.Tasks.Task<string[]> GetFoldersAsync();
        //получить все файлы
        System.Threading.Tasks.Task<string[]> GetFilessAsync();
        //получить все разрешения
        System.Threading.Tasks.Task<string[]> GetExtensionAsync();
    }


    public class FileStorageDB:DbContext,IFolderStorage
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

        public Task<string[]> GetFoldersAsync()
        {
            return (from t in this.Folder
                    select t.Name).ToArrayAsync();

            throw new NotImplementedException();
        }

        public Task<string[]> GetFilessAsync()
        {
            return (from t in this.Files
                    select t.Name).ToArrayAsync();

            throw new NotImplementedException();
        }

        public Task<string[]> GetExtensionAsync()
        {
            return (from t in this.Ext
                    select t.Type).ToArrayAsync();

            throw new NotImplementedException();
        }
    }
}
