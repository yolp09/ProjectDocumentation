using KD.Data;
using KD.Model.Common;
using KD.Model.PageModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace KD.Model.ModelsForAdd
{
    public class ModelOfAddNotice : ModelBase
    {
        private DataManager _dataManager;
        private string _uploadedFileInformation;
        private int? _selectedSection, _selectedProducts;
        private int _idFile;
        private bool _progressIsIndeterminate, _progressIsIndeterminateFileDetail, _progressIsIndeterminateFile;
        private byte[] _fileData;

        public string UploadedFileInformation { get { return _uploadedFileInformation; } set { _uploadedFileInformation = value; OnPropertyChanged("UploadedFileInformation"); } }
        public string NumberNotice { get; set; }
        public string Developer { get; set; }
        public string ChangeCode { get; set; }
        public int? CountSheets { get; set; }
        public int? SelectedSection { get { return _selectedSection; } set { _selectedSection = value; GetComboProducts(_selectedSection); } }
        public int? SelectedProducts { get { return _selectedProducts; } set { _selectedProducts = value; GetComboDetails(_selectedProducts); } }
        public int? SelectedDetail { get; set; }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }
        public bool ProgressIsIndeterminateFileDetail { get { return _progressIsIndeterminateFileDetail; } set { _progressIsIndeterminateFileDetail = value; OnPropertyChanged("ProgressIsIndeterminateFileDetail"); } }
        public bool ProgressIsIndeterminateFile { get { return _progressIsIndeterminateFile; } set { _progressIsIndeterminateFile = value; OnPropertyChanged("ProgressIsIndeterminateFile"); } }
        public DateTime? DateNotice { get; set; }
        public DetailModel SelectNoticeDetail { get; set; }
        public ObservableCollection<ModelComboBox> ComboSectoins;
        public ObservableCollection<ModelComboBox> ComboProducts;
        public ObservableCollection<ModelComboBox> ComboDetails;
        public ObservableCollection<DetailModel> NoticeDetails { get; set; }


        public ModelOfAddNotice()
        {
            this._dataManager = new DataManager();

            ComboSectoins = new ObservableCollection<ModelComboBox>(){
                new ModelComboBox((int)TypeDetail.Product, "Изделия"),
                new ModelComboBox((int)TypeDetail.Complex, "Комплексы"),
                new ModelComboBox((int)TypeDetail.AssemblyUnit, "Сборочные единицы"),
                new ModelComboBox((int)TypeDetail.Komplect, "Комплекты")
            };
            ComboProducts = new ObservableCollection<ModelComboBox>();
            ComboDetails = new ObservableCollection<ModelComboBox>();
            NoticeDetails = new ObservableCollection<DetailModel>();
        }

        public void PlusDetail()
        {
            if (SelectedDetail == null) throw new Exception("Деталь для добавления не выбрана!!!");
            Detail detail = _dataManager.GetDetailById(SelectedDetail.Value);
            if(detail != null)
            {
                foreach(var detailModel in NoticeDetails)
                {
                    if (detailModel.Detail == detail)
                        throw new Exception("Данная деталь уже добавлена!!!");
                }
                NoticeDetails.Add(new DetailModel(detail.Id));

                string numberProduct;
                if (SelectedSection == (int)TypeDetail.Product)
                {
                    numberProduct = _dataManager.GetProductById(SelectedProducts.Value).Number;
                }
                else
                {
                    numberProduct = _dataManager.GetDetailById(SelectedProducts.Value).Number;
                }
                NoticeDetails[NoticeDetails.Count - 1].ContentRadioButtonAdd = String.Format("(Добавление применяемости в изделие: {0})", numberProduct);
                NoticeDetails[NoticeDetails.Count - 1].ContentRadioButtonDelete = String.Format("(Удаление применяемости из изделия:  {0})", numberProduct);

            }
        }

        public void MinusDetail()
        {
            if(SelectNoticeDetail != null)
            {
                NoticeDetails.Remove(SelectNoticeDetail);
            }
        }

        public Task AddFileDetailAsync(string filePath)
        {
            return Task.Run(() => AddFileDetail(filePath));
        }

        public Task AddFileNoticeAsync(string filePath)
        {
            return Task.Run(() => AddFileNotice(filePath));
        }

        public Task AddNoticeAssync()
        {
            return Task.Run(() => Add());
        }

        public void CleaningFields()
        {
            SelectedSection = null; OnPropertyChanged("SelectedIdSection");
            ComboDetails.Clear();
            ComboProducts.Clear();
            NoticeDetails.Clear();
            NumberNotice = String.Empty; OnPropertyChanged("NumberNotice");
            DateNotice = null; OnPropertyChanged("DateNotice");
            Developer = String.Empty; OnPropertyChanged("Developer");
            CountSheets = null; OnPropertyChanged("CountSheets");
            ChangeCode = String.Empty; OnPropertyChanged("ChangeCode");
            _fileData = null;
            UploadedFileInformation = String.Empty;
        }

        private void AddFileDetail(string filePath)
        {
            ProgressIsIndeterminateFileDetail = true;
            byte[] tmpHash;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                _fileData = new byte[fs.Length];
                fs.Read(_fileData, 0, _fileData.Length);
                SHA1 sha = new SHA1Managed();
                tmpHash = sha.ComputeHash(_fileData);
            }
            string fileName = "(SH1 = " + BitConverter.ToString(tmpHash) + ")" + filePath.Substring(filePath.LastIndexOf('\\') + 1);
            int id = NoticeDetails.IndexOf(SelectNoticeDetail);
            NoticeDetails[id].SetIdFile = _dataManager.AddFile(fileName, _fileData);
            NoticeDetails[id].DescriptionFile = filePath;
            _fileData = null;
            ProgressIsIndeterminateFileDetail = false;
        }

        private void AddFileNotice(string filePath)
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

        private void GetComboProducts(int? selected)
        {
            if (selected == null) return;
            ComboProducts.Clear();
            if (selected == (int)TypeDetail.Product)
            {
                List<Product> result = _dataManager.GetProducts();
                foreach (var product in result)
                {
                    ComboProducts.Add(new ModelComboBox(product.Id, String.Format("{0} {1}", product.Number, product.Name)));
                }
            }
            else
            {
                List<Detail> result = _dataManager.GetDetails(selected.Value);
                foreach (var detail in result)
                {
                    ComboProducts.Add(new ModelComboBox(detail.Id, String.Format("{0} {1}", detail.Number, detail.Name)));
                }
            }
        }

        private void GetComboDetails(int? selected)
        {
            if (selected == null) return;
            ComboDetails.Clear();
            if(SelectedSection == (int)TypeDetail.Product)
            {
                List<Detail> result = _dataManager.GetApplicabilityDetails(selected.Value);
                foreach (var detail in result)
                {
                    ComboDetails.Add(new ModelComboBox(detail.Id, String.Format("{0} {1}", detail.Number, detail.Name)));
                }
            }
            else
            {
                List<Detail> result = _dataManager.GetApplicabilitySBDetails(selected.Value);
                foreach (var detail in result)
                {
                    ComboDetails.Add(new ModelComboBox(detail.Id, String.Format("{0} {1}", detail.Number, detail.Name)));
                }
            }
        }

        private void Add()
        {
            ProgressIsIndeterminate = true;
            DataEntryValidation(NoticeDetails, NumberNotice, DateNotice, Developer, CountSheets, ChangeCode, _fileData);
            List<Tuple<Detail, string>> noticeDetails = new List<Tuple<Detail, string>>();
            foreach(var noticeDetail in NoticeDetails)
            {
                noticeDetails.Add(new Tuple<Detail, string>(noticeDetail.Detail, noticeDetail.GetResultRadioButton));
                if(noticeDetail.SetIdFile != null)
                {
                    _dataManager.AddApplicabiltyFile(noticeDetail.Detail, noticeDetail.SetIdFile.Value, String.Format("ver {0}", _dataManager.CountApplicabilityFile(noticeDetail.Id)), NumberNotice);
                    _dataManager.ChangeIdFileDetail(noticeDetail.Id, noticeDetail.SetIdFile.Value);
                }
                if(noticeDetail.SetArchive == true)
                {
                    _dataManager.ChangeArchiveDetail(noticeDetail.Id, NumberNotice);
                }
            }
            _dataManager.AddNotice(NumberNotice, DateNotice.Value, Developer, CountSheets.Value, ChangeCode, _idFile, noticeDetails);   
        }

        private void DataEntryValidation(ObservableCollection<DetailModel> noticeDetails, string numberNotice, DateTime? dateNotice, string developer, int? countSheets, string changeCode, byte[] fileData)
        {
            if (noticeDetails.Count == 0) throw new Exception("Не выбраны детали для данного извещения!");
            if (String.IsNullOrWhiteSpace(numberNotice)) throw new Exception("Номер извещения не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
            if (dateNotice == null) throw new Exception("Дата извещения не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
            if (String.IsNullOrWhiteSpace(developer)) throw new Exception("Разработчик извещения не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
            if (countSheets == null || countSheets < 1) throw new Exception("Количество листов не может быть меньше 1!");
            if (String.IsNullOrWhiteSpace(changeCode)) throw new Exception("Код изменения не может быть пустой строкой или строкой, состоящей только из пробельных символов!");
            if (fileData == null) throw new Exception("Не добавлен скан детали!");
        }
    }
}
