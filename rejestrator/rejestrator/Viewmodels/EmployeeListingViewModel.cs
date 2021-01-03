namespace rejestrator.Viewmodels
{
    using rejestrator.Models;
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Data;

    public class EmployeeListingViewModel : ViewModelBase, IPageViewModel
    {
        private readonly List<EmployeeViewModel> _employeeViewModels;

        public List<string> logsDatesList = new List<string>();

        public List<string> logsNamesList = new List<string>();

        public ICollectionView EmployeesCollectionView { get; }

        private string _employeesFilter = string.Empty;
        public string EmployeesFilter
        {
            get
            {
                return _employeesFilter;
            }
            set
            {
                _employeesFilter = value;
                OnPropertyChanged(nameof(EmployeesFilter));
                EmployeesCollectionView.Refresh();
            }
        }

       private AdminModel adminModel = null;

        public EmployeeListingViewModel()
        {
            adminModel = AdminModel.Instance;

            _employeeViewModels = new List<EmployeeViewModel>();

            foreach (EmployeeViewModel employeeViewModel in GetEmployeeViewModels())
            {
                _employeeViewModels.Add(employeeViewModel);
            }

            EmployeesCollectionView = CollectionViewSource.GetDefaultView(_employeeViewModels);

            EmployeesCollectionView.Filter = FilterEmployees;
            EmployeesCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(EmployeeViewModel.Date)));
            EmployeesCollectionView.SortDescriptions.Add(new SortDescription(nameof(EmployeeViewModel.Name), ListSortDirection.Ascending));
        }

        private bool FilterEmployees(object obj)
        {
            if (obj is EmployeeViewModel employeeViewModel)
            {
                return employeeViewModel.Name.IndexOf(EmployeesFilter, StringComparison.OrdinalIgnoreCase) >= 0 || employeeViewModel.Date.IndexOf(EmployeesFilter, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return false;
        }

        private IEnumerable<EmployeeViewModel> GetEmployeeViewModels()
        {
            adminModel.GetLogsDates(logsDatesList);
            adminModel.GetLogsNames(logsNamesList);

            List<EmployeeViewModel> listOfModels = new List<EmployeeViewModel>();
            
            if(logsDatesList.Count == logsNamesList.Count)
            {
                for(int i = 0; i<logsDatesList.Count; i++)
                {
                    yield return new EmployeeViewModel(logsNamesList[i], logsDatesList[i]);
                }
            }
        }
    }
}
