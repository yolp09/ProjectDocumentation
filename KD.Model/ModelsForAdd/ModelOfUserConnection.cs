using KD.Data;
using KD.Model.Common;
using System;
using System.Threading.Tasks;

namespace KD.Model.ModelsForAdd
{
    public class ModelOfUserConnection : ModelBase
    {
        private DataManager _dataManager;
        private string _userPassword;

        public string UserName { get; set; }
        public string UserPassword { get { return _userPassword; } set { _userPassword = value; OnPropertyChanged("UserPassword"); } }

        public ModelOfUserConnection()
        {
            this._dataManager = new DataManager();
        }

        public Task<int> CheckUserAssync()
        {
            return Task.Run(() => CheckUser());
        }

        public void SaveHistory()
        {
            _dataManager.SaveHistory(DateTime.Now.ToString(), UserName);
        }

        private int CheckUser()
        {
            return _dataManager.CheckUser(UserName, UserPassword);
        }
    }
}
