using KD.Data;
using KD.Model.PageModel;

namespace KD.Model
{
    public class Manager
    {
        private readonly DataManager _dataManager;
        public ModelOfPageProduct ModelOfPageProduct { get; set; }
        public ModelOfPageDetail ModelOfPageDetails { get; set; }
        public ModelOfPageDetail ModelOfPageDocuments { get; set; }
        public ModelOfPageDetail ModelOfPageAssemblyUnits { get; set; }
        public ModelOfPageDetail ModelOfPageStandardProducts { get; set; }
        public ModelOfPageDetail ModelOfPageOtherProducts { get; set; }
        public ModelOfPageDetail ModelOfPageComplexs { get; set; }
        public ModelOfPageDetail ModelOfPageKomplekts { get; set; }
        public ModelOfPageDetail ModelOfPageMaterials { get; set; }
        public ModelOfPageUnusedFiles ModelOfPageUnusedFiles { get; set; }
        public ModelOfPageNotice ModelOfPageNotice { get; set; }
        public ModelOfPageDetailsInProduct ModelOfPageDetailsInProduct { get; set; }
        public ModelOfPageDetail ModelOfPageArchive { get; set; }

        public Manager()
        {
            this._dataManager = new DataManager();
            ModelOfPageProduct = new ModelOfPageProduct();
            ModelOfPageDetails = new ModelOfPageDetail(TypeDetail.Detail);
            ModelOfPageDocuments = new ModelOfPageDetail(TypeDetail.Document);
            ModelOfPageAssemblyUnits = new ModelOfPageDetail(TypeDetail.AssemblyUnit);
            ModelOfPageStandardProducts = new ModelOfPageDetail(TypeDetail.StandardProduct);
            ModelOfPageOtherProducts = new ModelOfPageDetail(TypeDetail.OthresProduct);
            ModelOfPageComplexs = new ModelOfPageDetail(TypeDetail.Complex);
            ModelOfPageKomplekts = new ModelOfPageDetail(TypeDetail.Komplect);
            ModelOfPageMaterials = new ModelOfPageDetail(TypeDetail.Material);
            ModelOfPageNotice = new ModelOfPageNotice();
            ModelOfPageUnusedFiles = new ModelOfPageUnusedFiles();
            ModelOfPageArchive = new ModelOfPageDetail(TypeDetail.Archive);
        }

        public void CollectionUpdate(TypeDetail typeDetail)
        {
            switch (typeDetail)
            {
                case TypeDetail.Detail: { ModelOfPageDetails.GetCollectionStart(); break; }
                case TypeDetail.Document: { ModelOfPageDocuments.GetCollectionStart(); break; }
                case TypeDetail.AssemblyUnit: { ModelOfPageAssemblyUnits.GetCollectionStart(); break; }
                case TypeDetail.StandardProduct: { ModelOfPageStandardProducts.GetCollectionStart(); break; }
                case TypeDetail.OthresProduct: { ModelOfPageOtherProducts.GetCollectionStart(); break; }
                case TypeDetail.Complex: { ModelOfPageComplexs.GetCollectionStart(); break; }
                case TypeDetail.Komplect: { ModelOfPageKomplekts.GetCollectionStart(); break; }
                case TypeDetail.Material: { ModelOfPageMaterials.GetCollectionStart(); break; }
                case TypeDetail.Product: { CollectionsUpdate(); break; }
            }
        }

        private void CollectionsUpdate()
        {
            ModelOfPageDetails.GetCollectionStart();
            ModelOfPageDocuments.GetCollectionStart();
            ModelOfPageAssemblyUnits.GetCollectionStart();
            ModelOfPageStandardProducts.GetCollectionStart();
            ModelOfPageOtherProducts.GetCollectionStart();
            ModelOfPageComplexs.GetCollectionStart();
            ModelOfPageKomplekts.GetCollectionStart();
            ModelOfPageMaterials.GetCollectionStart();
        }
    }
}
