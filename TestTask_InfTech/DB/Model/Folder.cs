using System.ComponentModel.DataAnnotations;

namespace TestTask_InfTech.DB.Model
{
    public class Folder
    {
        public Guid FolderId { get; set; }

        [Required(ErrorMessage = "Не указано имя")]
        public string? Name { get; set; }

        public Folder? Parental { get; set; }
    }
}
