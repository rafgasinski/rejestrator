namespace rejestrator.Viewmodels
{
    using rejestrator.Models;
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System.Windows.Input;

    public class DashboardViewModel : ViewModelBase, IPageViewModel
    {
        #region Singleton

        private EmployeeModel emploeeModel = null;
        public DashboardViewModel()
        {
            emploeeModel = EmployeeModel.Instance;
        }

        #endregion

        #region Properties

        public static string Name { get; set; }

        #endregion

        #region Commands

        private ICommand _goToLogin;
        public ICommand GoToLogin
        {
            get
            {
                return _goToLogin ?? (_goToLogin = new RelayCommand(x =>
                {
                    Mediator.Notify(Token.GO_TO_LOGIN);
                }));
            }
        }

        #endregion
    }
}
