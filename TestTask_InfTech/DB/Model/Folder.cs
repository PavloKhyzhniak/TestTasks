namespace TestTask_InfTech.DB.Model
{
    public class Folder
    {
        public Guid FolderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Folder Parental { get; set; }

    }
}
