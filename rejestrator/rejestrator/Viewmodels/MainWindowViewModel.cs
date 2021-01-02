namespace rejestrator.Viewmodels
{
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System.Collections.Generic;
    using System.Linq;

    public class MainWindowViewModel : ViewModelBase
    {
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                _currentPageViewModel = value;
                OnPropertyChanged("CurrentPageViewModel");
            }
        }

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }

        private void OnGoLoginScreen(object obj)
        {
            ChangeViewModel(PageViewModels[0]);
        }

        private void OnGoDashboard(object obj)
        {
            ChangeViewModel(PageViewModels[1]);
        }

        private void OnGoAdminDashboard(object obj)
        {
            ChangeViewModel(PageViewModels[2]);
        }

        private void OnGoAdminEmployees(object obj)
        {
            ChangeViewModel(PageViewModels[3]);
        }

        private void OnGoAdminRaport(object obj)
        {
            ChangeViewModel(PageViewModels[4]);
        }

        public MainWindowViewModel()
        {
            PageViewModels.Add(new LoginViewModel());
            PageViewModels.Add(new DashboardViewModel());
            PageViewModels.Add(new AdminDashboardViewModel());
            PageViewModels.Add(new AdminEmployeesViewModel());
            PageViewModels.Add(new AdminRaportViewModel());

            CurrentPageViewModel = PageViewModels[0];

            Mediator.Subscribe(Token.GO_TO_LOGIN, OnGoLoginScreen);
            Mediator.Subscribe(Token.GO_TO_DASHBOARD, OnGoDashboard);
            Mediator.Subscribe(Token.GO_TO_ADMIN_DASHBOARD, OnGoAdminDashboard);
            Mediator.Subscribe(Token.GO_TO_ADMIN_EMPLOYEES, OnGoAdminEmployees);
            Mediator.Subscribe(Token.GO_TO_ADMIN_RAPORT, OnGoAdminRaport);
        }
    }
}
