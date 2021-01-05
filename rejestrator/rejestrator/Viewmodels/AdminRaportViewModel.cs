namespace rejestrator.Viewmodels
{
    using rejestrator.Models;
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System.Windows.Input;

    public class AdminRaportViewModel : ViewModelBase, IPageViewModel
    {
        private static AdminRaportViewModel _instance = new AdminRaportViewModel();
        public static AdminRaportViewModel Instance { get { return _instance; } }

        #region Singleton
        private AdminModel adminModel = null;

        public AdminRaportViewModel()
        {
            adminModel = AdminModel.Instance;
        }
        #endregion

        #region Properties
        public static string Name { get; set; }
        #endregion
    }
}
