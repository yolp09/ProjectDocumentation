using KD.Model;
using KD.ViewModel.Common;
using KD.ViewModel.Pages;

namespace KD.ViewModel
{
    public class ManagerViewModel
    {
        public DetailViewModel DetailsViewModel;
        public DetailViewModel DocomentsViewModel;
        public DetailViewModel AssemblyUnitsViewModel;
        public DetailViewModel StandardProductsViewModel;
        public DetailViewModel OtherProductsViewModel;
        public DetailViewModel ComplexsViewModel;
        public DetailViewModel KomplectsViewModel;
        public DetailViewModel MaterialsViewModel;
        public NoticeViewModel NoticesViewModel;
        public DetailViewModel ArchiveViewModel;
        public ApplicationMain ApplicationMain;
        public Manager Manager;

        public ManagerViewModel()
        {
            this.ApplicationMain = new ApplicationMain();
            this.Manager = new Model.Manager();
            DetailsViewModel = new DetailViewModel(this.ApplicationMain, this.Manager, "Детали", TypeDetail.Detail);
            DocomentsViewModel = new DetailViewModel(this.ApplicationMain, this.Manager, "Документы", TypeDetail.Document);
            AssemblyUnitsViewModel = new DetailViewModel(this.ApplicationMain, this.Manager, "Сборочные единицы", TypeDetail.AssemblyUnit);
            StandardProductsViewModel = new DetailViewModel(this.ApplicationMain, this.Manager, "Стандартные изделия", TypeDetail.StandardProduct);
            OtherProductsViewModel = new DetailViewModel(this.ApplicationMain, this.Manager, "Прочие изделия", TypeDetail.OthresProduct);
            ComplexsViewModel = new DetailViewModel(this.ApplicationMain, this.Manager, "Комплексы", TypeDetail.Complex);
            KomplectsViewModel = new DetailViewModel(this.ApplicationMain, this.Manager, "Комплекты", TypeDetail.Komplect);
            MaterialsViewModel = new DetailViewModel(this.ApplicationMain, this.Manager, "Материалы", TypeDetail.Material);
            NoticesViewModel = new NoticeViewModel(this.ApplicationMain, this.Manager);
            ArchiveViewModel = new DetailViewModel(this.ApplicationMain, this.Manager, "Архив", TypeDetail.Archive);
        }
    }
}
