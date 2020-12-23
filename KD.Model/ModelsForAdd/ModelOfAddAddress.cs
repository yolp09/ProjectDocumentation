using KD.Data;
using KD.Model.Common;
using System;
using System.Threading.Tasks;

namespace KD.Model.ModelsForAdd
{
    public class ModelOfAddAddress : ModelBase
    {
        private DataManager _dataManager;
        private Manager _manager;
        private int _idDetail;
        private bool _progressIsIndeterminate, _isDetail;

        public string NumberAndNameDetail { get; set; }
        public string Address { get; set; }
        public bool CheckedSubscription { get; set; }
        public bool ProgressIsIndeterminate { get { return _progressIsIndeterminate; } set { _progressIsIndeterminate = value; OnPropertyChanged("ProgressIsIndeterminate"); } }

        public ModelOfAddAddress(int idDetail, bool isDetail, Manager manager)
        {
            this._dataManager = new DataManager();
            this._idDetail = idDetail;
            this._isDetail = isDetail;
            this._manager = manager;

            if (_isDetail)
            {
                Detail detail = _dataManager.GetDetailById(idDetail);
                NumberAndNameDetail = String.Format("{0} {1}", detail.Number, detail.Name);
            }
            else
            {
                Notice notice = _dataManager.GetNoticeById(idDetail);
                NumberAndNameDetail = String.Format("{0}", notice.Number);
            }
        }

        public Task AddAddressAsync()
        {
            return Task.Run(() => AddAddressProduct());
        }

        private void AddAddressProduct()
        {
            ProgressIsIndeterminate = true;
            if (String.IsNullOrWhiteSpace(Address)) throw new Exception("Адрес не может быть пустой строкой или строкой, состоящей только из пробельных символов");
            if (_isDetail)
            {
                _dataManager.AddAddress(_idDetail, Address, CheckedSubscription);
            }
            else
            {
                _dataManager.AddAddressNotice(_idDetail, Address, CheckedSubscription);
            }
            ProgressIsIndeterminate = false;
        }
    }
}
