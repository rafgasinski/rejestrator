namespace rejestrator.Viewmodels
{
    using rejestrator.Models;
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows.Data;

    public class EmployeeListingViewModel : ViewModelBase, IPageViewModel
    { 

        public List<string> logsIdList = new List<string>();

        public List<string> logsDatesList = new List<string>();

        public List<string> logsNamesList = new List<string>();

        public ICollectionView EmployeesCollectionView { get; set; }

        public static ObservableCollection<AllLogsModel> _employeeCollectionView;

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

            _employeeCollectionView = new ObservableCollection<AllLogsModel>();

            foreach (AllLogsModel employeeViewModel in GetEmployeeViewModels())
            {
                _employeeCollectionView.Add(employeeViewModel);
            }

            EmployeesCollectionView = CollectionViewSource.GetDefaultView(_employeeCollectionView);
            EmployeesCollectionView.Filter = FilterEmployees;
            EmployeesCollectionView.SortDescriptions.Add(new SortDescription(nameof(AllLogsModel.Date), ListSortDirection.Descending));
            EmployeesCollectionView.SortDescriptions.Add(new SortDescription(nameof(AllLogsModel.Id), ListSortDirection.Ascending));
            EmployeesCollectionView.SortDescriptions.Add(new SortDescription(nameof(AllLogsModel.Name), ListSortDirection.Ascending));
        }

        private bool FilterEmployees(object obj)
        {
            if (obj is AllLogsModel employeeViewModel)
            {
                return employeeViewModel.Name.IndexOf(EmployeesFilter, StringComparison.OrdinalIgnoreCase) >= 0 || employeeViewModel.Date.IndexOf(EmployeesFilter, StringComparison.OrdinalIgnoreCase) >= 0
                    || employeeViewModel.Id.IndexOf(EmployeesFilter, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return false;
        }

        public IEnumerable<AllLogsModel> GetEmployeeViewModels()
        {
            logsIdList.Clear();
            logsNamesList.Clear();
            logsDatesList.Clear();

            adminModel.GetLogsEmployeeID(logsIdList);
            adminModel.GetLogsDates(logsDatesList);
            adminModel.GetLogsNames(logsNamesList);      

            List<AllLogsModel> listOfModels = new List<AllLogsModel>();
            
            if(logsDatesList.Count == logsNamesList.Count)
            {
                for(int i = 0; i<logsDatesList.Count; i++)
                {
                    yield return new AllLogsModel(logsIdList[i], logsNamesList[i], logsDatesList[i]);
                }
            }
        }
    }
}
