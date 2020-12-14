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

        public MainWindowViewModel()
        {
            PageViewModels.Add(new LoginViewModel());
            PageViewModels.Add(new DashboardViewModel());
            PageViewModels.Add(new AdminDashboardViewModel());

            CurrentPageViewModel = PageViewModels[0];

            Mediator.Subscribe(Token.GO_TO_LOGIN, OnGoLoginScreen);
            Mediator.Subscribe(Token.GO_TO_DASHBOARD, OnGoDashboard);
            Mediator.Subscribe(Token.GO_TO_ADMIN_DASHBOARD, OnGoAdminDashboard);
        }
    }
}
