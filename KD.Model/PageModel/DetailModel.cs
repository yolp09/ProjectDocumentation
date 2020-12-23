using KD.Data;
using KD.Model.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Media;

namespace KD.Model.PageModel
{
    public class DetailModel : ModelBase
    {
        private DataManager _dataManager;
        private DescriptionChangeDetail _descriptionChangeDetail;
        private string _descriptionFile;
        private bool _progressIsIndeterminate;

        internal Detail Detail;

        public string Title { get; set; }
        public string Number { get { return Detail.Number; } }
        public string Name { get { return Detail.Name; } }
        public string Developer { get { return Detail.Developer; } }
        public string ArchiveNotice { get { return Detail.ArchiveNotice; } }
        public string NumberAndName { get { return String.Format("{0} {1}", Number, Name); } }
        public string DescriptionFile { get { return _descriptionFile; } set { _descriptionFile = value; OnPropertyChanged("DescriptionFile"); } }
        public string DescriptionNotice { get; set; }
        public string ContentRadioButtonAdd { get; set; }
        public string ContentRadioButtonDelete { get; set; }
        public string SelectedRadioButton { get; set; }
        public string Adress { get; set; }
        public string InfoScan { get { return Detail.IdFile == null ? "Без скана" : "Актуальный скан"; } }
        public string GetResultRadioButton
        {
            get
            {
                switch (DescriptionChange)
                {
                    case DescriptionChangeDetail.Sacn:
                        return "(Изменение скана)";
                    case DescriptionChangeDetail.AddApplicability:
                        return ContentRadioButtonAdd;
                    case DescriptionChangeDetail.DeleteApplicability:
                        return ContentRadioButtonDelete;
                    case DescriptionChangeDetail.Archive:
                        return "(Аннулировано)";
                }
                return "";
            }
        }
        public int? IdFile { get { return Detail.IdFile; } }
        public int? SetIdFile { get; set; }
        public int Id { get { return Detail.Id; } }
        public bool Archive { get { return Detail.Archive; } }
        public bool SetArchive { get; set; }
        public bool IsEnabledFile { get { return Detail.IdFile == null ? false : true; } }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }
        public bool IsScan
        {
            get { return DescriptionChange == DescriptionChangeDetail.Sacn; }
            set { DescriptionChange = value ? DescriptionChangeDetail.Sacn : DescriptionChange; }
        }
        public bool IsAddApplicability
        {
            get { return DescriptionChange == DescriptionChangeDetail.AddApplicability; }
            set { DescriptionChange = value ? DescriptionChangeDetail.AddApplicability : DescriptionChange; }
        }
        public bool IsDeleteApplicability
        {
            get { return DescriptionChange == DescriptionChangeDetail.DeleteApplicability; }
            set { DescriptionChange = value ? DescriptionChangeDetail.DeleteApplicability : DescriptionChange; }
        }
        public bool IsArchive
        {
            get { return DescriptionChange == DescriptionChangeDetail.Archive; }
            set { DescriptionChange = value ? DescriptionChangeDetail.Archive : DescriptionChange; }
        }
        public TypeDetail? TypeDetaill { get { foreach (Model.TypeDetail typeDetail in Enum.GetValues(typeof(Model.TypeDetail))) { if ((int)typeDetail == Detail.TypeDetail) return typeDetail; } return null; } }
        public ApplicabilityN ApplicabilityN { get; set; }
        public SolidColorBrush TextColor { get { return Archive ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black); } }
        public DescriptionChangeDetail DescriptionChange
        {
            get { return _descriptionChangeDetail; }
            set
            {
                if (_descriptionChangeDetail == value)
                    return;

                _descriptionChangeDetail = value;
                OnPropertyChanged("IsScan");
                OnPropertyChanged("IsAddApplicability");
                OnPropertyChanged("IsDeleteApplicability");
                OnPropertyChanged("IsArchive");
            }
        }
        public ObservableCollection<NoticeModel> ApplicabilityNotices { get; set; }
        public ObservableCollection<ApplicabilityModel> Applicabilities { get; set; }
        public ObservableCollection<ApplicabilityFileModel> ApplicabilityFs { get; set; }

        public DetailModel(int idDetail)
        {
            this._dataManager = new DataManager();
            this.Detail = _dataManager.ReturnDetail(idDetail);
            switch (TypeDetaill)
            {
                case TypeDetail.Detail: { Title = String.Format("Деталь: {0}", NumberAndName); break; }
                case TypeDetail.Document: { Title = String.Format("Документ: {0}", NumberAndName); break; }
                case TypeDetail.StandardProduct: { Title = String.Format("Стандартное изделие: {0}", NumberAndName); break; }
                case TypeDetail.OthresProduct: { Title = String.Format("Прочее изделие: {0}", NumberAndName); break; }
                case TypeDetail.Material: { Title = String.Format("Материал: {0}", NumberAndName); break; }
            }
            if (Archive)
                Title += String.Format(" (Аннулирована: {0})", ArchiveNotice);

            DescriptionFile = "Без изменения";
            GetAddress();

            ApplicabilityNotices = new ObservableCollection<NoticeModel>();
            GetApplicabilityNotice(Id);

            Applicabilities = new ObservableCollection<ApplicabilityModel>();
            GetApplicability();

            ApplicabilityFs = new ObservableCollection<ApplicabilityFileModel>();
            GetApplicabilityFs(Id);
        }

        public void DeleteApplicability(ApplicabilitySBOrNotSb applicabil)
        {
            if (applicabil.IsSB) _dataManager.DeleteApplicabilitySB(applicabil.Id); else _dataManager.DeleteApplicability(applicabil.Id);
            ApplicabilityModel am = Applicabilities.SingleOrDefault(ap => ap.ApplicabilitySbOrNotSb == applicabil);
            Applicabilities.Remove(am);
        }

        public void DeleteApplicabilityNotice(ApplicabilityN appN)
        {
            _dataManager.DeleteApplicabilityNotice(appN.IdDetail, appN.IdNotice);
            NoticeModel nm = ApplicabilityNotices.SingleOrDefault(apn => apn.ApplicabilityN == appN);
            ApplicabilityNotices.Remove(nm);
        }

        public void GetApplicability()
        {
            Applicabilities.Clear();
            List<Applicability> resultProduct = _dataManager.GetaApplicabilityProduct(Id);
            foreach (var applicability in resultProduct)
            {
                Applicabilities.Add(new ApplicabilityModel(applicability, null));
            }

            List<ApplicabilitySB> resultSB = _dataManager.GetaApplicabilitySB(Id);
            foreach (var applicabilitySB in resultSB)
            {
                Applicabilities.Add(new ApplicabilityModel(null, applicabilitySB));
            }
        }

        public void ChangeProgressApplicability(int id, bool value)
        {
            ApplicabilityFs.SingleOrDefault(da => da.IdFile == id).ProgressIsIndeterminate = value;
        }

        public Task OpenFileAssync(int id)
        {
            return Task.Run(() => OpenFile(id));
        }

        public Task OpenFileApplicabilityAssync(int id)
        {
            return Task.Run(() => OpenFileApplicability(id));
        }

        private void GetAddress()
        {
            Adress = String.Empty;
            string txtWithSubscription = "С подпиской: ";
            string txtWithoutSubscription = "Без подписки: ";

            List<Address> address = _dataManager.GetAdress(Id);
            foreach(var item in address)
            {
                if (item.Subscription) txtWithSubscription += String.Format("{0}; ", item.Address1); else txtWithoutSubscription += String.Format("{0}; ", item.Address1);
            }
            if (txtWithSubscription.Length > 13) Adress += txtWithSubscription;
            if (txtWithoutSubscription.Length > 14) Adress += txtWithoutSubscription;
        }

        private void GetApplicabilityFs(int id)
        {
            List<ApplicabilityFile> result = _dataManager.GetApplicabilityFiles(id);

            foreach(var applicabilityFile in result)
            {
                ApplicabilityFs.Add(new ApplicabilityFileModel(applicabilityFile));
            }
        }

        private void GetApplicabilityNotice(int id)
        {
            List<ApplicabilityNotice> result = _dataManager.GetApplicabilityNotice(id);
            foreach (var notice in result)
            {
                ApplicabilityNotices.Add(new NoticeModel(notice.Notice.Id, false) { ApplicabilityN = new ApplicabilityN(id, notice.Notice.Id), DescriptionDetail = notice.Description });
            }
        }

        private void OpenFile(int id)
        {
            string fileName = String.Empty;
            Thread.Sleep(2000);
            byte[] fileData = _dataManager.DataFile(IdFile.Value, ref fileName);

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

        private void OpenFileApplicability(int id)
        {
            ApplicabilityFileModel applicabilityFileModel = ApplicabilityFs.SingleOrDefault(da => da.IdFile == id);

            applicabilityFileModel.ProgressIsIndeterminate = true;

            int idFile = applicabilityFileModel.IdFile.Value;
            string fileName = String.Empty;

            byte[] fileData = _dataManager.DataFile(idFile, ref fileName);

            OpenFile(fileData, fileName);
            applicabilityFileModel.ProgressIsIndeterminate = false;
        }
    }
}
