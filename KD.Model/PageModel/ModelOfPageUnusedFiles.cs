using KD.Data;
using KD.Model.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace KD.Model.PageModel
{
    public class ModelOfPageUnusedFiles : ModelBase
    {
        private DataManager _dataManager;

        public ObservableCollection<FileModel> UnusedFiles { get; set; }

        public ModelOfPageUnusedFiles()
        {
            _dataManager = new DataManager();

            UnusedFiles = new ObservableCollection<FileModel>();
            Initializ();
        }

        public void DeleteFile(int idFile)
        {
            _dataManager.DeleteFile(idFile);
            FileModel fm = UnusedFiles.SingleOrDefault(uf => uf.Id == idFile);
            UnusedFiles.Remove(fm);
        }

        private void Initializ()
        {
            List<MFile> result = _dataManager.GetUnusedFiles();
            foreach (var item in result)
            {
                UnusedFiles.Add(new FileModel(item));
            }
        }
    }
}
