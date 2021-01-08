namespace rejestrator.Viewmodels
{
    using LiveCharts;
    using LiveCharts.Defaults;
    using MaterialDesignThemes.Wpf;
    using rejestrator.Models;
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    public class AdminRaportViewModel : ViewModelBase, IPageViewModel, INotifyPropertyChanged
    {
        private static AdminRaportViewModel _instance = new AdminRaportViewModel();
        public static AdminRaportViewModel Instance { get { return _instance; } }
        List<EmployeeChartModel> employees = new List<EmployeeChartModel>();
        public int k, l = 0;

        #region Singleton
        private AdminModel adminModel = null;

        public AdminRaportViewModel()
        {
            adminModel = AdminModel.Instance;

            EmployeeLogsCounts = new ChartValues<int>();
            EmployeeTaskCounts = new ChartValues<int>();
            EmployeeTasksInProgressCounts = new ChartValues<int>();
            EmployeeTasksDoneCounts = new ChartValues<int>();
            EmployeeNames = new ObservableCollection<string>();

            List<string> ids = new List<string>();

            adminModel.GetEmployeesIDs(ids);

            for (int i = 0; i < adminModel.GetEmployeeCount(); i++)
            {
                var temp = ids[i].Split(' ');
                employees.Add(new EmployeeChartModel(ids[i], adminModel.GetEmployeeLogsCount(temp[0]), adminModel.GetEmployeeTasksCount(temp[0]), adminModel.GetEmployeeTasksInProgressCount(temp[0]), adminModel.GetEmployeeTasksDoneCount(temp[0])));
            }

            EmployeeNames.Clear();
            EmployeeLogsCounts.Clear();
            EmployeeTaskCounts.Clear();
            EmployeeTasksInProgressCounts.Clear();
            EmployeeTasksDoneCounts.Clear();

            for (int i = k; i < 3; i++)
            {
                if (i < employees.Count())
                {
                    EmployeeLogsCounts.Add(employees[i].NumberofLogins);
                    EmployeeTaskCounts.Add(employees[i].NumberofTasks);
                    EmployeeTasksInProgressCounts.Add(employees[i].NumberofTasksInProgress);
                    EmployeeTasksDoneCounts.Add(employees[i].NumberofTasksDone);
                    EmployeeNames.Add(employees[i].Name);
                }
                k++;
            }

            Page = 1;
        }
        #endregion

        private bool _checkedBool = false;
        public bool CheckedBool
        {
            get => _checkedBool;
            set
            {
                _checkedBool = value;
                OnPropertyChanged(nameof(CheckedBool));
            }
        }

        private ICommand _checked;

        public ICommand Checked
        {
            get
            {
                return _checked ?? (_checked = new RelayCommand(x =>
                {
                    employees.Clear();
                    Page = 1;
                    List<string> ids = new List<string>();

                    k = 0;
                    l = 0;

                    adminModel.GetEmployeesIDs(ids);

                    for (int i = 0; i < adminModel.GetEmployeeCount(); i++)
                    {
                        var temp = ids[i].Split(' ');
                        employees.Add(new EmployeeChartModel(ids[i], adminModel.GetEmployeeLogsCountToday(temp[0]), adminModel.GetEmployeeTasksCount(temp[0]), adminModel.GetEmployeeTasksInProgressCount(temp[0]), adminModel.GetEmployeeTasksDoneCountToday(temp[0])));
                    }

                    EmployeeNames.Clear();
                    EmployeeLogsCounts.Clear();
                    EmployeeTaskCounts.Clear();
                    EmployeeTasksInProgressCounts.Clear();
                    EmployeeTasksDoneCounts.Clear();

                    for (int i = 0; i < 3; i++)
                    {
                        if (i < employees.Count())
                        {
                            EmployeeLogsCounts.Add(employees[i].NumberofLogins);
                            EmployeeTaskCounts.Add(employees[i].NumberofTasks);
                            EmployeeTasksInProgressCounts.Add(employees[i].NumberofTasksInProgress);
                            EmployeeTasksDoneCounts.Add(employees[i].NumberofTasksDone);
                            EmployeeNames.Add(employees[i].Name);
                        }
                        k++;
                    }

                }));
            }
        }

        private ICommand _unchecked;

        public ICommand Unchecked
        {
            get
            {
                return _unchecked ?? (_unchecked = new RelayCommand(x =>
                {
                    Reload.Execute(null);
                }));
            }
        }


        #region ChartRelated

        private ICommand _nextOnClick;

        public ICommand NextOnClick
        {
            get
            {
                return _nextOnClick ?? (_nextOnClick = new RelayCommand(x =>
                {
                    if ((k < adminModel.GetEmployeeCount()))
                    {
                        List<string> ids = new List<string>();
                        employees.Clear();

                        adminModel.GetEmployeesIDs(ids);

                        for (int i = 0; i < adminModel.GetEmployeeCount(); i++)
                        {
                            var temp = ids[i].Split(' ');
                            employees.Add(new EmployeeChartModel(ids[i], adminModel.GetEmployeeLogsCount(temp[0]), adminModel.GetEmployeeTasksCount(temp[0]), adminModel.GetEmployeeTasksInProgressCount(temp[0]), adminModel.GetEmployeeTasksDoneCount(temp[0])));
                        }

                        Page++;
                        EmployeeNames.Clear();
                        EmployeeLogsCounts.Clear();
                        EmployeeTaskCounts.Clear();
                        EmployeeTasksInProgressCounts.Clear();
                        EmployeeTasksDoneCounts.Clear();

                        for (int i = l + 3; i < k + 3; i++)
                        {
                            if (i < employees.Count())
                            {
                                EmployeeLogsCounts.Add(employees[i].NumberofLogins);
                                EmployeeTaskCounts.Add(employees[i].NumberofTasks);
                                EmployeeTasksInProgressCounts.Add(employees[i].NumberofTasksInProgress);
                                EmployeeTasksDoneCounts.Add(employees[i].NumberofTasksDone);
                                EmployeeNames.Add(employees[i].Name);
                            }
                        }
                        l += 3;
                        k += 3;

                    }
                }));
            }
        }

        private ICommand _prevOnClick;

        public ICommand PrevOnClick
        {
            get
            {
                return _prevOnClick ?? (_prevOnClick = new RelayCommand(x =>
                {
                    if (l != 0)
                    {
                        List<string> ids = new List<string>();
                        employees.Clear();

                        adminModel.GetEmployeesIDs(ids);

                        for (int i = 0; i < adminModel.GetEmployeeCount(); i++)
                        {
                            var temp = ids[i].Split(' ');
                            employees.Add(new EmployeeChartModel(ids[i], adminModel.GetEmployeeLogsCount(temp[0]), adminModel.GetEmployeeTasksCount(temp[0]), adminModel.GetEmployeeTasksInProgressCount(temp[0]), adminModel.GetEmployeeTasksDoneCount(temp[0])));
                        }

                        Page--;
                        EmployeeNames.Clear();
                        EmployeeLogsCounts.Clear();
                        EmployeeTaskCounts.Clear();
                        EmployeeTasksInProgressCounts.Clear();
                        EmployeeTasksDoneCounts.Clear();

                        for (int i = l - 3; i < k - 3; i++)
                        {
                            if (i < employees.Count())
                            {
                                EmployeeLogsCounts.Add(employees[i].NumberofLogins);
                                EmployeeTaskCounts.Add(employees[i].NumberofTasks);
                                EmployeeTasksInProgressCounts.Add(employees[i].NumberofTasksInProgress);
                                EmployeeTasksDoneCounts.Add(employees[i].NumberofTasksDone);
                                EmployeeNames.Add(employees[i].Name);
                            }
                        }
                        l -= 3;
                        k -= 3;
                    }
                }));
            }
        }

        public static event PropertyChangedEventHandler PropertyChanged1;

        public static void OnPropertyChanged1(string propertyName)
        {
            if (PropertyChanged1 != null)
            {
                PropertyChanged1(Instance, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static ChartValues<int> _employeeLogsCounts;
        public static ChartValues<int> EmployeeLogsCounts
        {
            get
            {
                return _employeeLogsCounts;
            }
            private set
            {
                _employeeLogsCounts = value;
                OnPropertyChanged1(nameof(EmployeeLogsCounts));
            }
        }

        private static ChartValues<int> _employeeTaskCounts;
        public static ChartValues<int> EmployeeTaskCounts
        {
            get
            {
                return _employeeTaskCounts;
            }
            private set
            {
                _employeeTaskCounts = value;
                OnPropertyChanged1(nameof(EmployeeTaskCounts));
            }
        }

        private static ChartValues<int> _employeeTasksInProgressCounts;
        public static ChartValues<int> EmployeeTasksInProgressCounts
        {
            get
            {
                return _employeeTasksInProgressCounts;
            }
            private set
            {
                _employeeTasksInProgressCounts = value;
                OnPropertyChanged1(nameof(EmployeeTasksInProgressCounts));
            }
        }

        private static ChartValues<int> _employeeTasksDoneCounts;
        public static ChartValues<int> EmployeeTasksDoneCounts
        {
            get
            {
                return _employeeTasksDoneCounts;
            }
            private set
            {
                _employeeTasksDoneCounts = value;
                OnPropertyChanged1(nameof(EmployeeTasksDoneCounts));
            }
        }

        public static ObservableCollection<string> EmployeeNames { get; set; }
        #endregion

        #region Properties
        public static string Name { get; set; }

        private int _page = 1;
        public int Page
        {
            get => _page;
            set
            {
                _page = value;
                OnPropertyChanged(nameof(Page));
            }
        }
        #endregion

        private ICommand _addCommand;

        public ICommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(x =>
                {
                    List<string> employeeList = new List<string>();

                    AdminDashboardViewModel.employeesList.Clear();

                    adminModel.GetEmployeesFullNamesandID(employeeList);

                    foreach (var employee in employeeList)
                        AdminDashboardViewModel.employeesList.Add(employee);

                    Employee.Queries = new ObservableCollection<string>(AdminDashboardViewModel.employeesList);

                    Employee.TwoWays = new ObservableCollection<string>(AdminDashboardViewModel.workItems);

                    OnAdd();
                }));
            }
        }

        private ICommand _reload;

        public ICommand Reload
        {
            get
            {
                return _reload ?? (_reload = new RelayCommand(x =>
                {
                    employees.Clear();

                    List<string> ids = new List<string>();

                    k = 0;
                    l = 0;

                    adminModel.GetEmployeesIDs(ids);

                    for (int i = 0; i < adminModel.GetEmployeeCount(); i++)
                    {
                        var temp = ids[i].Split(' ');
                        employees.Add(new EmployeeChartModel(ids[i], adminModel.GetEmployeeLogsCount(temp[0]), adminModel.GetEmployeeTasksCount(temp[0]), adminModel.GetEmployeeTasksInProgressCount(temp[0]), adminModel.GetEmployeeTasksDoneCount(temp[0])));
                    }

                    EmployeeNames.Clear();
                    EmployeeLogsCounts.Clear();
                    EmployeeTaskCounts.Clear();
                    EmployeeTasksInProgressCounts.Clear();
                    EmployeeTasksDoneCounts.Clear();

                    for (int i = 0; i < 3; i++)
                    {
                        if (i < employees.Count())
                        {
                            EmployeeLogsCounts.Add(employees[i].NumberofLogins);
                            EmployeeTaskCounts.Add(employees[i].NumberofTasks);
                            EmployeeTasksInProgressCounts.Add(employees[i].NumberofTasksInProgress);
                            EmployeeTasksDoneCounts.Add(employees[i].NumberofTasksDone);
                            EmployeeNames.Add(employees[i].Name);
                        }
                        k++;
                    }

                    Page = 1;
                    CheckedBool = false;
                }));
            }
        }

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

        private ICommand _goToAdminDashboard;

        public ICommand GoToAdminDashboard
        {
            get
            {
                return _goToAdminDashboard ?? (_goToAdminDashboard = new RelayCommand(x =>
                {
                    AdminDashboardViewModel.EmployeeListingViewModel = new EmployeeListingViewModel();
                    Mediator.Notify(Token.GO_TO_ADMIN_DASHBOARD);
                }));
            }
        }

        private ICommand _goToAdminEmployees;

        public ICommand GoToAdminEmployees
        {
            get
            {
                return _goToAdminEmployees ?? (_goToAdminEmployees = new RelayCommand(x =>
                {
                    List<string> employeeList = new List<string>();

                    AdminEmployeesViewModel.ListOfEmployees.Clear();

                    adminModel.GetEmployeesFullNamesandID(employeeList);

                    foreach (var employee in employeeList)
                        AdminEmployeesViewModel.ListOfEmployees.Add(employee);

                    AdminEmployeesViewModel.PopulateLists();

                    Mediator.Notify(Token.GO_TO_ADMIN_EMPLOYEES);
                }));
            }
        }

        private ICommand _goToAdminRaport;

        public ICommand GoToAdminRaport
        {
            get
            {
                return _goToAdminRaport ?? (_goToAdminRaport = new RelayCommand(x =>
                {
                    Mediator.Notify(Token.GO_TO_ADMIN_RAPORT);
                }));
            }
        }

        private async void OnAdd()
        {
            if (await DialogHost.Show(new Employee()) is Employee item)
            {
                if (item.Task == string.Empty)
                {
                    if (item.ID != string.Empty && item.Pin != string.Empty && item.Name != string.Empty && item.Surname != string.Empty)
                    {
                        if (!item.ID.All(char.IsDigit) && !item.Pin.All(char.IsDigit))
                        {
                            MessageBox.Show("Id i pin nie składają się tylko z cyfr!");
                        }
                        else if (!item.ID.All(char.IsDigit))
                        {
                            MessageBox.Show("Id nie składa się tylko z cyfr!");
                        }
                        else if (!item.Pin.All(char.IsDigit))
                        {
                            MessageBox.Show("Pin nie składa się tylko z cyfr!");
                        }
                        else if (item.Pin.Length != 4 && item.ID.Length != 4)
                        {
                            MessageBox.Show("Id oraz pin są za krótkie!");
                        }
                        else if (item.ID.Length != 4)
                        {
                            MessageBox.Show("Id jest za krótkie!");
                        }
                        else if (item.Pin.Length != 4)
                        {
                            MessageBox.Show("Pin jest za krótki!");
                        }
                        else if (!adminModel.EmployeeIDUsed(item.ID))
                        {
                            string shift = getCurrentItemEmployee();
                            adminModel.InsertEmployee(item.ID, item.Pin, item.Name, item.Surname, shift);
                            employees.Add(new EmployeeChartModel($"{item.ID} {item.Name} {item.Surname}", adminModel.GetEmployeeLogsCount(item.ID), adminModel.GetEmployeeTasksCount(item.ID), adminModel.GetEmployeeTasksInProgressCount(item.ID), adminModel.GetEmployeeTasksDoneCount(item.ID)));
                            AdminEmployeesViewModel.ListOfEmployees.Add($"{item.ID} {item.Name} {item.Surname}");

                            if (EmployeeNames.Count != 3)
                            {
                                var tempInt = EmployeeNames.Count;
                                EmployeeNames.Add($"{item.ID} {item.Name} {item.Surname}");
                                EmployeeLogsCounts.Add(0);
                                EmployeeTaskCounts.Add(0);
                                EmployeeTasksInProgressCounts.Add(0);
                                EmployeeTasksDoneCounts.Add(0);
                            }
                        }
                        else
                        {
                            MessageBox.Show("To id zostało już przypisane!");
                        }
                    }
                    else if (item.IDadmin != string.Empty && item.AdminUsername != string.Empty && item.AdminPassword != string.Empty && item.AdminName != string.Empty && item.AdminSurname != string.Empty)
                    {
                        if (!item.IDadmin.All(char.IsDigit))
                        {
                            MessageBox.Show("Id nie składa się tylko z cyfr!");
                        }
                        else if (item.IDadmin.Length != 4)
                        {
                            MessageBox.Show("Id jest za krótkie!");
                        }
                        else if (!adminModel.AdminIDUsed(item.IDadmin))
                        {
                            adminModel.InsertAdmin(item.IDadmin, item.AdminUsername, item.AdminPassword, item.AdminName, item.AdminSurname);
                        }
                        else
                        {
                            MessageBox.Show("To id zostało już przypisane!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Pozostawiono puste pola.");
                    }
                }               
                else
                {
                    string temp = getCurrentListItem();
                    string[] words = temp.Split(' ');

                    adminModel.InsertTask(words[0], item.Task);
                    AdminEmployeesViewModel.PopulateTaskLists();

                    if (EmployeeNames.Contains(temp))
                    {
                        for (int i = 0; i < EmployeeNames.Count; i++)
                        {
                            if (EmployeeNames[i] == temp)
                            {
                                EmployeeTaskCounts[i]++;
                            }
                        }
                    }
                }
            }
        }


        public string getCurrentListItem()
        {
            var currentQuery = (string)Employee.SelectedEmployee;
            return currentQuery;
        }

        public string getCurrentItemEmployee()
        {
            var currentQuery = (string)Employee.SelectedShift;
            return currentQuery;
        }
    }
}
