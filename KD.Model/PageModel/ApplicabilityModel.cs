
using KD.Data;
namespace KD.Model.PageModel
{
    public class ApplicabilityModel
    {
        internal Applicability Applicability { get; set; }
        internal ApplicabilitySB ApplicabilitySB { get; set; }

        public string Number { get; private set; }
        public string Name { get; private set; }
        public int Id { get; private set; }
        public ApplicabilitySBOrNotSb ApplicabilitySbOrNotSb { get; private set; }

        public ApplicabilityModel(Applicability applicability, ApplicabilitySB applicabilitySB)
        {
            if (applicability != null || applicabilitySB != null)
            {
                Id = applicability == null ? applicabilitySB.Id : applicability.Id;
                Number = applicability == null ? applicabilitySB.DetailSB.Number : applicability.Product.Number;
                Name = applicability == null ? applicabilitySB.DetailSB.Name : applicability.Product.Name;
                ApplicabilitySbOrNotSb = applicability == null ? new ApplicabilitySBOrNotSb(Id, true, applicabilitySB.DetailSB.Id) : new ApplicabilitySBOrNotSb(Id, false, applicability.Product.Id);
            }
        }
    }

    public class ApplicabilitySBOrNotSb
    {
        public int Id { get; private set; }
        public int IdProduct { get; private set; }
        public bool IsSB { get; private set; }

        public ApplicabilitySBOrNotSb(int id, bool isSb, int idProduct)
        {
            this.Id = id;
            this.IsSB = isSb;
            this.IdProduct = idProduct;
        }
    }
}
