using KD.Data;
using KD.Model.Common;
using KD.Model.PageModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KD.Model.ModelsForAdd
{
    public class ModelOfDeleteAddress : ModelBase
    {
        private DataManager _dataManager;
        private bool _isDetail;

        public ObservableCollection<AddressModel> Adresses { get; set; }

        public ModelOfDeleteAddress(int idDetail, bool isDetail)
        {
            _dataManager = new DataManager();
            this._isDetail = isDetail;
            Adresses = new ObservableCollection<AddressModel>();
            Initializ(idDetail);
        }

        public void DeleteAdress()
        {
            foreach (var item in Adresses)
            {
                if(item.IsDelete == true)
                {
                    if (_isDetail == true) _dataManager.DeleteAddress(item.AdressId);
                    else _dataManager.DeleteAddressNotice(item.AdressId);
                }
            }
        }

        private void Initializ(int idDetail)
        {

            if (_isDetail == true)
            {
                List<Address> result = _dataManager.GetAdress(idDetail);
                foreach (var item in result) { Adresses.Add(new AddressModel(item, null)); }
            }
            else
            {
                List<AddressNotice> result = _dataManager.GetAdressNotices(idDetail);
                foreach (var item in result) { Adresses.Add(new AddressModel(null, item)); }
            }
        }
    }
}
