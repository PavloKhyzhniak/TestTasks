using TestTask_InfTech.DB.Model;

namespace TestTask_InfTech.ViewModels
{
    public class SubNodeModel
    {
        public Folder CurrentFolder { get; set; }
        public IEnumerable<SubNodeModel> ChildrenFolder { get; set; }
        public IEnumerable<DB.Model.File> ChildrenFile { get; set; }
    }
}
