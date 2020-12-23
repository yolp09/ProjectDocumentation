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
    public class ModelOfAddApplicabilityNotice : ModelBase
    {
        private DataManager _dataManager;
        private Notice _notice;
        private int? _selectedSection, _selectedProducts;
        private bool _progressIsIndeterminate, _progressIsIndeterminateFileDetail;
        private byte[] _fileData;

        public string Title { get; set; }
        public int? SelectedSection { get { return _selectedSection; } set { _selectedSection = value; GetComboProducts(_selectedSection); } }
        public int? SelectedProducts { get { return _selectedProducts; } set { _selectedProducts = value; GetComboDetails(_selectedProducts); } }
        public int? SelectedDetail { get; set; }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }
        public bool ProgressIsIndeterminateFileDetail { get { return _progressIsIndeterminateFileDetail; } set { _progressIsIndeterminateFileDetail = value; OnPropertyChanged("ProgressIsIndeterminateFileDetail"); } }
        public DetailModel SelectNoticeDetail { get; set; }
        public ObservableCollection<ModelComboBox> ComboSectoins;
        public ObservableCollection<ModelComboBox> ComboProducts;
        public ObservableCollection<ModelComboBox> ComboDetails;
        public ObservableCollection<DetailModel> NoticeDetails { get; set; }

        public ModelOfAddApplicabilityNotice(int idNotice)
        {
            this._dataManager = new DataManager();

            _notice = _dataManager.GetNoticeById(idNotice);
            Title = String.Format("Добавление деталей в извещение {0}", _notice.Number);

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
            if (detail != null)
            {
                foreach (var detailModel in NoticeDetails)
                {
                    if (detailModel.Detail == detail)
                        throw new Exception("Данная деталь уже добавлена!!!");
                }
                NoticeDetails.Add(new DetailModel(detail.Id));
            }

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

        public void MinusDetail()
        {
            if (SelectNoticeDetail != null)
            {
                NoticeDetails.Remove(SelectNoticeDetail);
            }
        }

        public Task AddAssync()
        {
            return Task.Run(() => Add());
        }

        public Task AddFileDetailAsync(string filePath)
        {
            return Task.Run(() => AddFileDetail(filePath));
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
            List<Detail> result;
            List<ApplicabilityNotice> alreadyAdded = _dataManager.GetApplicabilityNoticeByIdNotice(_notice.Id);

            if (SelectedSection == (int)TypeDetail.Product)
            {
                result = _dataManager.GetApplicabilityDetails(selected.Value);
                foreach (var item in alreadyAdded)
                {
                    result.Remove(item.Detail);
                }
                foreach (var detail in result)
                {
                    ComboDetails.Add(new ModelComboBox(detail.Id, String.Format("{0} {1}", detail.Number, detail.Name)));
                }
            }
            else
            {
                result = _dataManager.GetApplicabilitySBDetails(selected.Value);
                foreach (var item in alreadyAdded)
                {
                    result.Remove(item.Detail);
                }
                foreach (var detail in result)
                {
                    ComboDetails.Add(new ModelComboBox(detail.Id, String.Format("{0} {1}", detail.Number, detail.Name)));
                }
            }
        }

        private void Add()
        {
            ProgressIsIndeterminate = true;
            List<Detail> noticeDetails = new List<Detail>();
            List<Tuple<Detail, string>> noticeDetails1 = new List<Tuple<Detail, string>>();
            foreach (var noticeDetail in NoticeDetails)
            {
                noticeDetails.Add(noticeDetail.Detail);
                noticeDetails1.Add(new Tuple<Detail, string>(noticeDetail.Detail, noticeDetail.GetResultRadioButton));
                if (noticeDetail.SetIdFile != null)
                {
                    _dataManager.AddApplicabiltyFile(noticeDetail.Detail, noticeDetail.SetIdFile.Value, String.Format("ver {0}", _dataManager.CountApplicabilityFile(noticeDetail.Id)), _notice.Number);
                    _dataManager.ChangeIdFileDetail(noticeDetail.Id, noticeDetail.SetIdFile.Value);
                }
                _dataManager.AddApplicabilityNotice(_notice.Id, noticeDetails1);
            }

            ProgressIsIndeterminate = false;
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
    }
}
