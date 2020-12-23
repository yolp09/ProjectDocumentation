
using KD.Data;
using KD.Model.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
namespace KD.Model.PageModel
{
    public class ApplicabilityN
    {
        public int IdDetail { get; private set; }
        public int IdNotice { get; private set; }

        public ApplicabilityN(int idDetail, int idNotice)
        {
            this.IdDetail = idDetail;
            this.IdNotice = idNotice;
        }
    }
    public class NoticeModel : ModelBase
    {
        private DataManager _dataManager;
        private bool _progressIsIndeterminate;

        internal Notice Notice { get; set; }

        public string Title { get; set; }
        public string Number { get { return Notice.Number; } }
        public string Date { get { return Notice.Date.ToString("dd.MM.yyyy"); } }
        public string Developer { get { return Notice.Developer; } }
        public string ChangeCode { get { return Notice.ChangeCode; } }
        public string Adress { get; set; }
        public string DescriptionDetail { get; set; }
        public int Id { get { return Notice.Id; } }
        public int CountSheets { get { return Notice.CountSheets; } }
        public int IdFile { get { return Notice.IdFile; } }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }
        public ApplicabilityN ApplicabilityN { get; set; }
        public ObservableCollection<DetailModel> ApplicabilityDetails{ get; set; }

        public NoticeModel(int idNotice, bool isNotice)
        {
            _dataManager = new DataManager();
            this.Notice = _dataManager.GetNoticeById(idNotice);

            if (isNotice)
            {
                ApplicabilityDetails = new ObservableCollection<DetailModel>();
                GetApplicabilityNotice(Id);
                GetAddress(Id);
                Title = String.Format("Извещение № {0}", Number);
            }
        }

        public void DeleteApplicabilityNotice(ApplicabilityN appN)
        {
            _dataManager.DeleteApplicabilityNotice(appN.IdDetail, appN.IdNotice);

            DetailModel dm = ApplicabilityDetails.SingleOrDefault(apd => apd.ApplicabilityN == appN);
            ApplicabilityDetails.Remove(dm);
        }

        public Task OpenFileAssync(int id)
        {
            return Task.Run(() => OpenFile(id));
        }

        private void OpenFile(int id)
        {

            ProgressIsIndeterminate = true;
            int idFile = IdFile;
            string fileName = String.Empty;

            byte[] fileData = _dataManager.DataFile(idFile, ref fileName);

            OpenFile(fileData, fileName);

            ProgressIsIndeterminate = false;
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

        private void GetApplicabilityNotice(int id)
        {
            List<ApplicabilityNotice> result = _dataManager.GetApplicabilityNoticeByIdNotice(id);
            foreach (var applicabilityNotice in result)
            {
                ApplicabilityDetails.Add(new DetailModel(applicabilityNotice.Detail.Id) { ApplicabilityN = new ApplicabilityN(applicabilityNotice.Detail.Id, id), DescriptionNotice = applicabilityNotice.Description });
            }
        }

        private void GetAddress(int idNotice)
        {
            string txtWithSubscription = "С подпиской: ";
            string txtWithoutSubscription = "Без подписки: ";

            List<AddressNotice> address = _dataManager.GetAdressNotices(idNotice);
            foreach (var item in address)
            {
                if (item.Subscription) txtWithSubscription += String.Format("{0}; ", item.Address1); else txtWithoutSubscription += String.Format("{0}; ", item.Address1);
            }
            if (txtWithSubscription.Length > 13) Adress += txtWithSubscription;
            if (txtWithoutSubscription.Length > 14) Adress += txtWithoutSubscription;
        }
    }
}
