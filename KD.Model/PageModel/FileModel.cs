using KD.Data;

namespace KD.Model.PageModel
{
    public class FileModel
    {
        public string NameFile { get; private set; }
        public int Id { get; private set; }

        public FileModel(MFile item)
        {
            this.Id = item.Id;
            this.NameFile = item.FileName;
        }
    }
}
