using KD.Data;

namespace KD.Model.PageModel
{
    public class AddressModel
    {
        internal Address Address;
        internal AddressNotice AddressNotice;

        public string AdressText { get { return AddressNotice == null ? this.Address.Address1 : this.AddressNotice.Address1; } }
        public int AdressId { get { return AddressNotice == null ? this.Address.Id : this.AddressNotice.Id; } }
        public bool IsDelete { get; set; }

        public AddressModel(Address adress, AddressNotice adressNotice)
        {
            this.Address = adress;
            this.AddressNotice = adressNotice;
        }
    }
}
