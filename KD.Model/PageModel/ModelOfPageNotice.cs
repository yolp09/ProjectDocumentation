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
    public class ModelOfPageNotice : ModelBase
    {
        private DataManager _dataManager;
        private int _scrlCount;

        public bool EndScroll;
        public List<int> IdNotices;
        public ObservableCollection<NoticeModel> Notices;

        public ModelOfPageNotice()
        {
            this._dataManager = new DataManager();

            Notices = new ObservableCollection<NoticeModel>();
            GetIdNotices();
            GetNotices();
        }

        public void DeleteApplicabilityNotice(ApplicabilityN appN)
        {
            _dataManager.DeleteApplicabilityNotice(appN.IdDetail, appN.IdNotice);

            DetailModel dm = Notices.SingleOrDefault(n => n.ApplicabilityDetails.SingleOrDefault(apd => apd.ApplicabilityN == appN) != null).ApplicabilityDetails.SingleOrDefault(apd => apd.ApplicabilityN == appN);
            Notices.SingleOrDefault(n => n.ApplicabilityDetails.SingleOrDefault(apn => apn.ApplicabilityN == appN) != null).ApplicabilityDetails.Remove(dm);
        }

        public void GetNotices()
        {
            Notices.Clear();
            int elementsCount;
            if (IdNotices.Count > 20) elementsCount = 20; else elementsCount = IdNotices.Count;
            for (int i = 0; i < elementsCount; i++) { Notices.Add(new NoticeModel(IdNotices.ElementAt(i), true)); }
            _scrlCount = elementsCount;
            EndScroll = false;
        }

        public void NextScroll()
        {
            int addElementCount = 0;
            if (_scrlCount < IdNotices.Count)
            {
                if ((IdNotices.Count - _scrlCount) > 10) addElementCount = 10; else addElementCount = IdNotices.Count - _scrlCount;
                for (int i = _scrlCount; i < _scrlCount + addElementCount; i++)
                {
                    Notices.Add(new NoticeModel(IdNotices.ElementAt(i), true));
                }
            }
            else
                EndScroll = true;

            if ((Notices.Count > 40))
            {
                for (int i = 0; i < addElementCount; i++)
                {
                    Notices.RemoveAt(0);
                }
            }
            _scrlCount = _scrlCount + addElementCount;
        }

        public Task OpenFileAssync(int id)
        {
            return Task.Run(() => OpenFile(id));
        }

        public void ChangeProgress(int id, bool value)
        {
            Notices.SingleOrDefault(d => d.Id == id).ProgressIsIndeterminate = value;
        }

        public void DeleteNotice(int idNotice)
        {
            _dataManager.DeleteNotice(idNotice);
            NoticeModel nm = Notices.SingleOrDefault(n => n.Id == idNotice);
            Notices.Remove(nm);
        }

        public async void SearchNotice(string txt, string selectedComboBox)
        {
            List<Notice> result = await _dataManager.GetNoticesAsync();
            IdNotices = result.Where(n => n.Number.ToLower().Contains(txt.ToLower())).Select(d => d.Id).ToList();
            GetNotices();
        }

        private void GetIdNotices()
        {
            IdNotices = _dataManager.GetIdNotices();
        }

        private void OpenFile(int id)
        {
            NoticeModel notice = Notices.SingleOrDefault(n => n.Id == id);

            notice.ProgressIsIndeterminate = true;
            int idFile = notice.IdFile;
            string fileName = String.Empty;

            byte[] fileData = _dataManager.DataFile(idFile, ref fileName);

            OpenFile(fileData, fileName);

            notice.ProgressIsIndeterminate = false;
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
    }
}
