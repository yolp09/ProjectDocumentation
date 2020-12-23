using KD.Data;
using KD.Model.Common;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace KD.Model.ModelsForAdd
{
    public class ModelOfAddStandardProduct : ModelBase
    {
        private DataManager _dataManager;
        private string _uploadedFileInformation;
        private int _idFile;
        private bool _progressIsIndeterminate, _progressIsIndeterminateFile;
        private byte[] _fileData;

        public string NameProduct { get; set; }
        public string UploadedFileInformation { get { return _uploadedFileInformation; } set { _uploadedFileInformation = value; OnPropertyChanged("UploadedFileInformation"); } }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }
        public bool ProgressIsIndeterminateFile { get { return _progressIsIndeterminateFile; } set { _progressIsIndeterminateFile = value; OnPropertyChanged("ProgressIsIndeterminateFile"); } }
        public bool CheckedScan { get; set; }

        public ModelOfAddStandardProduct()
        {
            this._dataManager = new DataManager();
            CheckedScan = false;
        }

        public Task AddStandardProductAsync(TypeDetail typeDetail)
        {
            return Task.Run(() => AddStandardProduct(typeDetail));
        }

        public void CleaningFields()
        {
            NameProduct = ""; OnPropertyChanged("NameProduct");
            CheckedScan = false; OnPropertyChanged("CheckedScan");
        }

        public Task AddFileAsync(string filePath)
        {
            return Task.Run(() => AddFile(filePath));
        }

        private void AddStandardProduct(TypeDetail typeDetail)
        {
            ProgressIsIndeterminate = true;
            if (String.IsNullOrWhiteSpace(NameProduct)) throw new Exception("Наменование изделия не может быть пустой строкой или строкой, состоящей только из пробельных символов");
            if (_fileData == null && CheckedScan == true) throw new Exception("Не добавлен скан детали!");
            if (CheckedScan == false)
                _dataManager.AddStandardDetail((int)typeDetail, NameProduct, false, null);
            else
            {
                _dataManager.AddStandardDetail((int)typeDetail, NameProduct, false, _idFile);
            }
            ProgressIsIndeterminate = false;
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
    }
}
