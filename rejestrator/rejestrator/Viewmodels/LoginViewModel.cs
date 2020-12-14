namespace rejestrator.Viewmodels
{
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System.Windows.Input;

    public class LoginViewModel : ViewModelBase, IPageViewModel
    {
        private ICommand _goToDashboard;

        public ICommand GoToDashboard
        {
            get
            {
                return _goToDashboard ?? (_goToDashboard = new RelayCommand(x =>
                {
                    Mediator.Notify(Token.GO_TO_DASHBOARD);
                }));
            }
        }
    }
}
