namespace TestTask_InfTech.DB.Model
{
    public class File
    {
        public Guid FileId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Folder Folder { get; set; }
        public Extension Extension { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
