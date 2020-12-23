using KD.Data;
using KD.Model.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KD.Model.PageModel
{
    public class ModelOfPageDetailsInProduct : ModelBase
    {
        private DataManager _dataManger;
        private TypeDetail _typeDetail;
        private List<Detail> _result;
        private List<Detail> _searchDetail;
        private int _idProduct;

        public string Title { get; set; }
        public ObservableCollection<DetailModel> Details;

        public ModelOfPageDetailsInProduct(int idProduct, TypeDetail typeDetail)
        {
            this._dataManger = new DataManager();
            this._idProduct = idProduct;
            this._typeDetail = typeDetail;

            Details = new ObservableCollection<DetailModel>();
            Initializ(_idProduct);
        }

        public void DeleteApplicabilityNotice(ApplicabilityN appN)
        {
            _dataManger.DeleteApplicabilityNotice(appN.IdDetail, appN.IdNotice);
            NoticeModel nm = Details.SingleOrDefault(d => d.ApplicabilityNotices.SingleOrDefault(apn => apn.ApplicabilityN == appN) != null).ApplicabilityNotices.SingleOrDefault(apn => apn.ApplicabilityN == appN);
            Details.SingleOrDefault(d => d.ApplicabilityNotices.SingleOrDefault(apn => apn.ApplicabilityN == appN) != null).ApplicabilityNotices.Remove(nm);
        }

        public async void SearcDetail(string txt, string selectedComboBox)
        {
            if (_typeDetail == TypeDetail.Product)
                _searchDetail = await _dataManger.GetApplicabilityDetailsAsync(_idProduct);
            else
                _searchDetail = await _dataManger.GetApplicabilitySBDetailsAsync(_idProduct);

            _searchDetail = selectedComboBox == "По наименованию" ? _searchDetail.Where(p => p.Name.ToLower().Contains(txt.ToLower())).ToList() : _searchDetail.Where(p => p.Number.ToLower().Contains(txt.ToLower())).ToList();

            Details.Clear();
            foreach (var item in _searchDetail)
            {
                Details.Add(new DetailModel(item.Id));
            }
            
        }

        public void GetDetail()
        {
            Details.Clear();
            
            if (_typeDetail == TypeDetail.Product)
                _result = _dataManger.GetApplicabilityDetails(_idProduct);
            else
                _result = _dataManger.GetApplicabilitySBDetails(_idProduct);
            foreach (var item in _result)
            {
                Details.Add(new DetailModel(item.Id));
            }
        }

        public void DetailViewSelection(TypeDetail typeDetail)
        {
            Details.Clear();
            if (typeDetail == TypeDetail.Product)
            {
                foreach (var item in _result) { Details.Add(new DetailModel(item.Id)); }
            }
            else
            {
                foreach (var item in _result)
                {
                    if (item.TypeDetail == (int)typeDetail)
                        Details.Add(new DetailModel(item.Id));
                }
            }
        }

        public Task OpenFileAssync(int id)
        {
            return Task.Run(() => OpenFile(id));
        }

        public Task OpenFileApplicabilityAssync(int id)
        {
            return Task.Run(() => OpenFileApplicability(id));
        }

        public void ChangeProgress(int id, bool value)
        {
            Details.SingleOrDefault(d => d.Id == id).ProgressIsIndeterminate = value;
        }

        public void ChangeProgressApplicability(int id, bool value)
        {
            Details.SingleOrDefault(d => d.ApplicabilityFs.SingleOrDefault(da => da.IdFile == id) != null).ApplicabilityFs.SingleOrDefault(da => da.IdFile == id).ProgressIsIndeterminate = value;
        }

        public void DeleteApplicabilityDetail(ApplicabilitySBOrNotSb applicabil)
        {
            if (applicabil.IsSB) _dataManger.DeleteApplicabilitySB(applicabil.Id); else _dataManger.DeleteApplicability(applicabil.Id);
            ApplicabilityModel a = Details.SingleOrDefault(d => d.Applicabilities.SingleOrDefault(ap => ap.ApplicabilitySbOrNotSb == applicabil) != null).Applicabilities.SingleOrDefault(ap => ap.ApplicabilitySbOrNotSb == applicabil);
            Details.SingleOrDefault(d => d.Applicabilities.SingleOrDefault(ap => ap.ApplicabilitySbOrNotSb == applicabil) != null).Applicabilities.Remove(a);
        }

        private void OpenFile(int id)
        {
            DetailModel detail = Details.SingleOrDefault(d => d.Id == id);

            detail.ProgressIsIndeterminate = true;
            int idFile = detail.IdFile.Value;
            string fileName = String.Empty;

            byte[] fileData = _dataManger.DataFile(idFile, ref fileName);

            OpenFile(fileData, fileName);

            detail.ProgressIsIndeterminate = false;
        }

        private void Initializ(int idProduct)
        {
            if (_typeDetail == TypeDetail.Product)
            {
                Product product = _dataManger.GetProductById(idProduct);
                Title = String.Format("Детали и документация изделия: {0} {1}", product.Number, product.Name);
            }
            else
            {
                Detail detail = _dataManger.GetDetailById(idProduct);

                TypeDetail tD = TypeDetail.Archive;
                foreach (Model.TypeDetail typeDetail in Enum.GetValues(typeof(Model.TypeDetail)))
                { if ((int)typeDetail == detail.TypeDetail) { tD = typeDetail; break; } }

                switch (tD)
                {
                    case TypeDetail.AssemblyUnit: { Title = String.Format("Детали и документация сборочной единицы: {0} {1}", detail.Number, detail.Name); break; }
                    case TypeDetail.Complex: { Title = String.Format("Детали и документация комплекса: {0} {1}", detail.Number, detail.Name); break; }
                    case TypeDetail.Komplect: { Title = String.Format("Детали и документация комплекта: {0} {1}", detail.Number, detail.Name); break; }
                }
            }

            GetDetail();
        }

        private void OpenFile(byte[] fileData, string fileName)
        {
            if (File.Exists(fileName))
            {
                using (Process.Start(fileName)) { }
            }
            else
            {
                throw new Exception("Файла не существует!!!");
            }
        }

        private void OpenFileApplicability(int id)
        {
            ApplicabilityFileModel applicabilityFileModel = Details.SingleOrDefault(d => d.ApplicabilityFs.SingleOrDefault(da => da.IdFile == id) != null).ApplicabilityFs.SingleOrDefault(da => da.IdFile == id);
            applicabilityFileModel.ProgressIsIndeterminate = true;

            int idFile = applicabilityFileModel.IdFile.Value;
            string fileName = String.Empty;

            byte[] fileData = _dataManger.DataFile(idFile, ref fileName);

            OpenFile(fileData, fileName);
            applicabilityFileModel.ProgressIsIndeterminate = false;
        }
    }
}
