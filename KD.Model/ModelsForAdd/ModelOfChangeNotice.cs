using KD.Data;
using KD.Model.Common;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace KD.Model.ModelsForAdd
{
    public class ModelOfChangeNotice : ModelBase
    {
        private DataManager _dataManager;
        private DateTime _oldDateNotice;
        private string _uploadedFileInformation;
        private string _oldNumber, _oldDeveloper, _oldChangeCode;
        private int? _idFile;
        private int _oldCountShets, _idNotice;
        private bool _progressIsIndeterminate, _progressIsIndeterminateFile;
        private byte[] _fileData;

        public string Title { get; set; }
        public string NumberNotice { get; set; }
        public string Developer { get; set; }
        public string ChangeCode { get; set; }
        public string UploadedFileInformation { get { return _uploadedFileInformation; } set { _uploadedFileInformation = value; OnPropertyChanged("UploadedFileInformation"); } }
        public int? CountSheets { get; set; }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }
        public bool ProgressIsIndeterminateFile { get { return _progressIsIndeterminateFile; } set { _progressIsIndeterminateFile = value; OnPropertyChanged("ProgressIsIndeterminateFile"); } }
        public DateTime? DateNotice { get; set; }

        public ModelOfChangeNotice(int idNotice)
        {
            _dataManager = new DataManager();
            Initialize(idNotice);
        }

        public Task<string> ChangeNoticeAssync()
        {
            return Task.Run(() => ChangeNotice());
        }

        public Task AddFileAsync(string filePath)
        {
            return Task.Run(() => AddFile(filePath));
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

        private string ChangeNotice()
        {
            string msg = String.Empty;
            ProgressIsIndeterminate = true;
            DataEntryValidation(NumberNotice, DateNotice, Developer, CountSheets, ChangeCode);

            _dataManager.ChangeNotice(_idNotice, NumberNotice, DateNotice.Value, Developer, CountSheets.Value, ChangeCode, _idFile);

            if (_oldNumber != NumberNotice) msg += String.Format("Изменение обозначения {0} на {1}. ", _oldNumber, NumberNotice);
            if (_oldDateNotice != DateNotice) msg += String.Format("Изменение даты {0} на {1}. ", _oldDateNotice, DateNotice);
            if (_oldDeveloper != Developer) msg += String.Format("Изменение разработчика {0} на {1}. ", _oldDeveloper, Developer);
            if (_oldCountShets != CountSheets.Value) msg += String.Format("Изменение кол-ва листов {0} на {1}. ", _oldDeveloper, Developer);
            if (_oldChangeCode != ChangeCode) msg += String.Format("Изменение кода изменения {0} на {1}. ", _oldDeveloper, Developer);
            if (_fileData != null) msg += String.Format("Изменение файла выполнено. ", _oldDeveloper, Developer);

            ProgressIsIndeterminate = false;

            return msg;
        }

        private void Initialize(int idNotice)
        {
            Notice notice = _dataManager.GetNoticeById(idNotice);

            _idNotice = notice.Id;
            NumberNotice = _oldNumber = notice.Number;
            DateNotice = _oldDateNotice = notice.Date;
            Developer = _oldDeveloper = notice.Developer;
            CountSheets = _oldCountShets = notice.CountSheets;
            ChangeCode = _oldChangeCode = notice.ChangeCode;
            Title = String.Format("Изменение {0}", NumberNotice);
        }

        private void DataEntryValidation(string numberNotice, DateTime? dateNotice, string developer, int? countSheets, string changeCode)
        {
            if (String.IsNullOrWhiteSpace(numberNotice)) throw new Exception("Номер извещения не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
            if (dateNotice == null) throw new Exception("Дата извещения не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
            if (String.IsNullOrWhiteSpace(developer)) throw new Exception("Разработчик извещения не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
            if (countSheets == null || countSheets < 1) throw new Exception("Количество листов не может быть меньше 1!");
            if (String.IsNullOrWhiteSpace(changeCode)) throw new Exception("Код изменения не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
        }
    }
}
