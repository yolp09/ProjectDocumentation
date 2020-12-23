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
    public class ModelOfPageDetail : ModelBase
    {
        private DataManager _dataManager;
        private TypeDetail _typeDetail;
        private int _scrlCount;

        public bool EndScroll;
        public List<int> IdDetails;
        public ObservableCollection<DetailModel> Details { get; set; }

        public ModelOfPageDetail(TypeDetail typeDetail)
        {
            this._dataManager = new DataManager();
            this._typeDetail = typeDetail;
            IdDetails = new List<int>();
            Details = new ObservableCollection<DetailModel>();

            GetIdCollections();
            GetCollectionStart();
        }

        public async void SearcDetail(string txt, string selectedComboBox, TypeDetail typeDetail)
        {
            List<Detail> result = await _dataManager.GetDetailsAsync((int)typeDetail);
            IdDetails = selectedComboBox == "По наименованию" ? result.Where(d => d.Name.ToLower().Contains(txt.ToLower())).Select(d => d.Id).ToList() : result.Where(d => d.Number.ToLower().Contains(txt.ToLower())).Select(d => d.Id).ToList();
            GetCollectionStart();
        }

        public void Archive(int idDetail)
        {
            _dataManager.ChangeDetailIsArchive(idDetail);
            GetCollectionStart();
        }

        public void GetCollectionStart()
        {
            Details.Clear();
            int elementsCount;
            if (IdDetails.Count > 20) elementsCount = 20; else elementsCount = IdDetails.Count;
            for (int i = 0; i < elementsCount; i++) { Details.Add(new DetailModel(IdDetails.ElementAt(i))); }
            _scrlCount = elementsCount;
            EndScroll = false;
        }

        public void NextScroll()
        {
            int addElementCount = 0;
            if (_scrlCount < IdDetails.Count)
            {
                if ((IdDetails.Count - _scrlCount) > 10) addElementCount = 10; else addElementCount = IdDetails.Count - _scrlCount;
                for (int i = _scrlCount; i < _scrlCount + addElementCount; i++)
                {
                    Details.Add(new DetailModel(IdDetails.ElementAt(i)));
                }
            }
            else
                EndScroll = true;

            if ((Details.Count > 40))
            {
                for (int i = 0; i < addElementCount; i++)
                {
                    Details.RemoveAt(0);
                }
            }
            _scrlCount = _scrlCount + addElementCount;
        }

        public void DeleteApplicabilityNotice(ApplicabilityN appN)
        {
            _dataManager.DeleteApplicabilityNotice(appN.IdDetail, appN.IdNotice);
            NoticeModel nm = Details.SingleOrDefault(d => d.ApplicabilityNotices.SingleOrDefault(apn => apn.ApplicabilityN == appN) != null).ApplicabilityNotices.SingleOrDefault(apn => apn.ApplicabilityN == appN);
            Details.SingleOrDefault(d => d.ApplicabilityNotices.SingleOrDefault(apn => apn.ApplicabilityN == appN) != null).ApplicabilityNotices.Remove(nm);
        }

        public void DeleteDetail(int idDetail)
        {
            _dataManager.DeleteDetail(idDetail);
            IdDetails.Remove(idDetail);
            DetailModel detailModel = Details.SingleOrDefault(d => d.Id == idDetail);
            if (detailModel != null) Details.Remove(detailModel);
        }

        public void DeleteApplicabilityDetail(ApplicabilitySBOrNotSb applicabil)
        {
            if (applicabil.IsSB) _dataManager.DeleteApplicabilitySB(applicabil.Id); else _dataManager.DeleteApplicability(applicabil.Id);
            ApplicabilityModel a = Details.SingleOrDefault(d => d.Applicabilities.SingleOrDefault(ap => ap.ApplicabilitySbOrNotSb == applicabil) != null).Applicabilities.SingleOrDefault(ap => ap.ApplicabilitySbOrNotSb == applicabil);
            Details.SingleOrDefault(d => d.Applicabilities.SingleOrDefault(ap => ap.ApplicabilitySbOrNotSb == applicabil) != null).Applicabilities.Remove(a);
        }

        public Task OpenFileAssync(int idFile)
        {
            return Task.Run(() => OpenFile(idFile));
        }

        private void OpenFile(int idFile)
        {
            string fileName = String.Empty;
            byte[] fileData = _dataManager.DataFile(idFile, ref fileName);
            OpenFile(fileData, fileName);
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

        private void GetIdCollections()
        {
            IdDetails = _dataManager.GetIdDetails((int)_typeDetail);
        }
    }
}
