
using KD.Data;
namespace KD.Model.PageModel
{
    public class ProductModel
    {
        private Product _product;

        public string NumberProduct { get { return _product.Number; } }
        public string NameProduct { get { return _product.Name; } }
        public int IdProduct { get { return _product.Id; } }

        public ProductModel(Product product)
        {
            this._product = product;
        }
    }
}
