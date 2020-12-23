using KD.Data;
using KD.Model.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace KD.Model.ModelsForAdd
{
    public class ModelOfAddDetail : ModelBase
    {
        private DataManager _dataManager;
        private string _uploadedFileInformation;
        private int? _selectedSection;
        private int _idFile;
        private bool _progressIsIndeterminate, _progressIsIndeterminateFile;
        private byte[] _fileData;

        public string NumberDetail { get; set; }
        public string NameDetail { get; set; }
        public string Developer { get; set; }
        public string UploadedFileInformation { get { return _uploadedFileInformation; } set { _uploadedFileInformation = value; OnPropertyChanged("UploadedFileInformation"); } }
        public int? SelectedSection { get { return _selectedSection; } set { _selectedSection = value; GetComboProducts(_selectedSection); } }
        public int? SelectedProducts { get; set; }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }
        public bool ProgressIsIndeterminateFile { get { return _progressIsIndeterminateFile; } set { _progressIsIndeterminateFile = value; OnPropertyChanged("ProgressIsIndeterminateFile"); } }
        public bool CheckedScan { get; set; }
        public ObservableCollection<ModelComboBox> ComboProducts;
        public ObservableCollection<ModelComboBox> ComboSectoins;

        public ModelOfAddDetail()
        {
            this._dataManager = new DataManager();

            ComboSectoins = new ObservableCollection<ModelComboBox>(){
                new ModelComboBox((int)TypeDetail.Product, "Изделия"),
                new ModelComboBox((int)TypeDetail.Complex, "Комплексы"),
                new ModelComboBox((int)TypeDetail.AssemblyUnit, "Сборочные единицы"),
                new ModelComboBox((int)TypeDetail.Komplect, "Комплекты")
            };
            ComboProducts = new ObservableCollection<ModelComboBox>();
            CheckedScan = true;

        }

        public Task AddFileAsync(string filePath)
        {
            return Task.Run(() => AddFile(filePath));
        }

        public Task AddDetailAssync(TypeDetail typeDetail)
        {
            return Task.Run(() => AddDetail(typeDetail));
        }

        public void CleaningFields()
        {
            NumberDetail = String.Empty; OnPropertyChanged("NumberDetail");
            NameDetail = String.Empty; OnPropertyChanged("NameDetail");
            Developer = String.Empty; OnPropertyChanged("Developer");
            UploadedFileInformation = String.Empty;
            SelectedProducts = null; OnPropertyChanged("SelectedProducts");
            SelectedSection = null; OnPropertyChanged("SelectedSection");
            _fileData = null;
            CheckedScan = true; OnPropertyChanged("CheckedScan");
            ComboProducts.Clear();
        }

        private async void GetComboProducts(int? selected)
        {
            if (selected == null) return;

            ComboProducts.Clear();
            if (selected == 0)
            {
                List<Product> result = await _dataManager.GetProductsAsync();
                foreach (var product in result) { ComboProducts.Add(new ModelComboBox(product.Id, String.Format("{0} {1}", product.Number, product.Name))); }
            }
            else
            {
                List<Detail> result = await _dataManager.GetDetailsAsync(selected.Value);
                foreach (var detail in result) { ComboProducts.Add(new ModelComboBox(detail.Id, String.Format("{0} {1}", detail.Number, detail.Name))); }
            }
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

        private void AddDetail(TypeDetail typeDetail)
        {
            ProgressIsIndeterminate = true;
            DataEntryValidation(SelectedProducts, NameDetail, NumberDetail, Developer, _fileData);

            if (CheckedScan == true) { _dataManager.AddDetail((int)typeDetail, SelectedProducts.Value, NameDetail, NumberDetail, Developer, false, _idFile, _selectedSection.Value); }
            else { _dataManager.AddDetail((int)typeDetail, SelectedProducts.Value, NameDetail, NumberDetail, Developer, false, null, _selectedSection.Value); }

            ProgressIsIndeterminate = false;
        }

        private void DataEntryValidation(int? idProduct, string name, string number, string developer, byte[] fileData)
        {
            if (idProduct == null) throw new Exception("Не выбрана применяемость для детали!");
            if (String.IsNullOrWhiteSpace(number)) throw new Exception("Обозначение детали не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
            if (String.IsNullOrWhiteSpace(name)) throw new Exception("Наменование детали не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
            if (String.IsNullOrWhiteSpace(developer)) throw new Exception("Разработчик детали не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
            if (fileData == null && CheckedScan == true) throw new Exception("Не добавлен скан детали!");
        }
    }
}
