using KD.Data;
using KD.Model.Common;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace KD.Model.ModelsForAdd
{
    public class ModelOfChangeDetail : ModelBase
    {
        private DataManager _dataManager;
        private Manager _manager;
        private string _uploadedFileInformation, _oldNumber, _oldName, _oldDeveloper;
        private int? _idFile;
        private int _idDetail;
        private bool _progressIsIndeterminate, _progressIsIndeterminateFile;
        private byte[] _fileData;

        public string Title { get; set; }
        public string NumberDetail { get; set; }
        public string NameDetail { get; set; }
        public string Developer { get; set; }
        public string UploadedFileInformation { get { return _uploadedFileInformation; } set { _uploadedFileInformation = value; OnPropertyChanged("UploadedFileInformation"); } }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }
        public bool ProgressIsIndeterminateFile { get { return _progressIsIndeterminateFile; } set { _progressIsIndeterminateFile = value; OnPropertyChanged("ProgressIsIndeterminateFile"); } }
        public bool CheckedScan { get; set; }

        public ModelOfChangeDetail(int idDetail, Manager manager)
        {
            _dataManager = new DataManager();
            this._manager = manager;
            Initialize(idDetail);
        }

        public Task AddFileAsync(string filePath)
        {
            return Task.Run(() => AddFile(filePath));
        }

        public Task<string> ChangeDetailAssync()
        {
            return Task.Run(() => ChangeDetail());
        }

        private string ChangeDetail()
        {
            string msg = String.Empty;
            ProgressIsIndeterminate = true;
            DataEntryValidation(NameDetail, NumberDetail, Developer);

            _dataManager.ChangeDetail(_idDetail, NumberDetail, NameDetail, Developer, _idFile);

            ProgressIsIndeterminate = false;
            if (_oldNumber != NumberDetail) msg += String.Format("Изменение обозначения {0} на {1}. ", _oldNumber, NumberDetail);
            if (_oldName != NameDetail) msg += String.Format("Изменение наименования {0} на {1}. ", _oldName, NameDetail);
            if (_oldDeveloper != Developer) msg += String.Format("Изменение разработчика {0} на {1}. ", _oldDeveloper, Developer);
            if (_fileData != null) msg += String.Format("Изменение файла выполнено. ", _oldDeveloper, Developer);

            return msg;
        }

        private void Initialize(int idDetail)
        {
            Detail detail = _dataManager.GetDetailById(idDetail);

            _idDetail = detail.Id;
            NumberDetail = _oldNumber = detail.Number;
            NameDetail = _oldName = detail.Name;
            Developer = _oldDeveloper = detail.Developer;
            CheckedScan = detail.IdFile == null ? false : true;
            Title = String.Format("Изменение {0} {1}", NumberDetail, NameDetail);
        }

        private void AddFile(string filePath)
        {
            ProgressIsIndeterminateFile = true;
            UploadedFileInformation = "Идет загрузка файла...";
            byte[] tmpHash;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                _fileData = new byte[fs.Length];
                fs.Read(_fileData, 0, _fileData.Length);
                SHA1 sha = new SHA1Managed();
                tmpHash = sha.ComputeHash(_fileData);
            }
            string fileName = "(SH1 = " + BitConverter.ToString(tmpHash) + ")" + filePath.Substring(filePath.LastIndexOf('\\') + 1);
            _idFile = _dataManager.AddFile(fileName, _fileData);
            UploadedFileInformation = "Загруженный файл: " + filePath;
            ProgressIsIndeterminateFile = false;
        }

        private void DataEntryValidation(string name, string number, string developer)
        {
            if (String.IsNullOrWhiteSpace(number) && _oldNumber != String.Empty) throw new Exception("Обозначение детали не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
            if (String.IsNullOrWhiteSpace(name)) throw new Exception("Наменование детали не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
            if (String.IsNullOrWhiteSpace(developer) && _oldDeveloper != String.Empty) throw new Exception("Разработчик детали не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
        }
    }
}
