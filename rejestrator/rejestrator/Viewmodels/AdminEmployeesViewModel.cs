namespace rejestrator.Viewmodels
{
    using MaterialDesignThemes.Wpf;
    using rejestrator.Models;
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    public class AdminEmployeesViewModel : ViewModelBase, IPageViewModel
    {
        private static AdminEmployeesViewModel _instance = new AdminEmployeesViewModel();
        public static AdminEmployeesViewModel Instance { get { return _instance; } }

        public static ObservableCollection<string> ListOfEmployees { get; set; } = new ObservableCollection<string>();

        private static ObservableCollection<string> _dates = new ObservableCollection<string>();
        public static ObservableCollection<string> Dates
        {
            get { return _dates; }
            set
            {
                _dates = value;
                OnPropertyChanged1("Dates");
            }
        }

        private static ObservableCollection<string> _tasks = new ObservableCollection<string>();
        public static ObservableCollection<string> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnPropertyChanged1("Tasks");
            }
        }

        private static ObservableCollection<TaskInProgressModel> _tasksInProgress = new ObservableCollection<TaskInProgressModel>();
        public static ObservableCollection<TaskInProgressModel> TasksInProgress
        {
            get { return _tasksInProgress; }
            set
            {
                _tasksInProgress = value;
                OnPropertyChanged1("TasksInProgress");
            }
        }

        private static ObservableCollection<TaskDoneModel> _tasksDone = new ObservableCollection<TaskDoneModel>();
        public static ObservableCollection<TaskDoneModel> TasksDone
        {
            get { return _tasksDone; }
            set
            {
                _tasksDone = value;
                OnPropertyChanged1("TasksDone");
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

        private static string _selectedEmployee;
        public static string SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                PopulateLists();
            }
        }

        public static void PopulateLists()
        {
            Dates.Clear();
            Tasks.Clear();
            TasksInProgress.Clear();
            TasksDone.Clear();

            if(_selectedEmployee != null)
            {
                string[] words = _selectedEmployee.Split(' ');

                List<string> datesToAdd = new List<string>();
                List<string> tasksToAdd = new List<string>();
                List<TaskInProgressModel> taskInProgressToAdd = new List<TaskInProgressModel>();
                List<TaskDoneModel> tasksDoneToAdd = new List<TaskDoneModel>();

                Instance.adminModel.GetLogsDatesForEmployee(datesToAdd, words[0]);
                Instance.adminModel.GetLogsTasksForEmployee(tasksToAdd, words[0]);
                Instance.adminModel.GetLogsTasksInProgressForEmployee(taskInProgressToAdd, words[0]);
                Instance.adminModel.GetLogsTasksDoneForEmployee(tasksDoneToAdd, words[0]);

                foreach (var date in datesToAdd)
                    Dates.Add(date);
                foreach (var task in tasksToAdd)
                    Tasks.Add(task);
                foreach (var task in taskInProgressToAdd)
                    TasksInProgress.Add(task);
                foreach (var task in tasksDoneToAdd)
                    TasksDone.Add(task);
            }
        }

        public static void PopulateTaskLists()
        {
            Tasks.Clear();

            if (_selectedEmployee != null)
            {
                string[] words = _selectedEmployee.Split(' ');

                List<string> tasksToAdd = new List<string>();

                Instance.adminModel.GetLogsTasksForEmployee(tasksToAdd, words[0]);

                foreach (var task in tasksToAdd)
                    Tasks.Add(task);
            }
        }

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
                    
                    List<string> employeeList = new List<string>();

                    adminModel.GetEmployeesFullNamesandID(employeeList);

                    foreach (var employee in employeeList)
                    {
                        if(!ListOfEmployees.Contains(employee))
                        {
                            ListOfEmployees.Add(employee);
                        }
                    }

                    PopulateLists();

                }));
            }
        }

        #region Singleton
        private AdminModel adminModel = null;

        public AdminEmployeesViewModel()
        {
            adminModel = AdminModel.Instance;

            _myCommand = new MyCommand(FuncToCall, FuncToEvaluate);
            _myCommand2 = new MyCommand(FuncToCall2, FuncToEvaluate);

        }
        #endregion

        #region Properties
        public static string Name { get; set; }
        #endregion

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
                if (item.Task == null)
                {
                    if (item.ID != null && item.Pin != null && item.Name != null && item.Surname != null)
                    {
                        if (!adminModel.EmployeeIDUsed(item.ID))
                        {
                            string shift = getCurrentItemEmployee();
                            adminModel.InsertEmployee(item.ID, item.Pin, item.Name, item.Surname, shift);
                            ListOfEmployees.Add($"{item.ID} {item.Name} {item.Surname}");
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
                    if (item.ID == null && item.Pin == null && item.Name == null && item.Surname == null)
                    {
                        string temp = getCurrentListItem();
                        string[] words = temp.Split(' ');

                        adminModel.InsertTask(words[0], item.Task);
                        PopulateTaskLists();
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

        private ICommand _myCommand;

        public ICommand SwitchPage
        {
            get { return _myCommand; }
            set { _myCommand = value; }
        }

        private ICommand _myCommand2;

        public ICommand SwitchPage2
        {
            get { return _myCommand2; }
            set { _myCommand2 = value; }
        }

        public ICommand SwitchBackPage2
        {
            get { return _myCommand2; }
            set { _myCommand2 = value; }
        }

        public ICommand SwitchBackPage
        {
            get { return _myCommand; }
            set { _myCommand = value; }
        }


        private void FuncToCall(object context)
        {
            if (this.ChangeControlVisibility == Visibility.Collapsed && this.ChangeControlVisibility2 == Visibility.Visible)
            {
                this.ChangeControlVisibility = Visibility.Visible;
                this.ChangeControlVisibility2 = Visibility.Collapsed;
            }
            else
            {
                this.ChangeControlVisibility = Visibility.Collapsed;
                this.ChangeControlVisibility2 = Visibility.Visible;
            }
        }

        private void FuncToCall2(object context)
        {
            if (this.ChangeControlVisibility2 == Visibility.Collapsed && this.ChangeControlVisibility3 == Visibility.Visible)
            {
                this.ChangeControlVisibility2 = Visibility.Visible;
                this.ChangeControlVisibility3 = Visibility.Collapsed;
            }
            else
            {
                this.ChangeControlVisibility2 = Visibility.Collapsed;
                this.ChangeControlVisibility3 = Visibility.Visible;
            }
        }

        private bool FuncToEvaluate(object context)
        {
            return true;
        }

        private Visibility _visibility = Visibility.Visible;

        public Visibility ChangeControlVisibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                this.OnPropertyChanged("ChangeControlVisibility");
            }
        }

        private Visibility _visibility2 = Visibility.Collapsed;

        public Visibility ChangeControlVisibility2
        {
            get { return _visibility2; }
            set
            {
                _visibility2 = value;
                this.OnPropertyChanged("ChangeControlVisibility2");
            }
        }

        private Visibility _visibility3 = Visibility.Collapsed;

        public Visibility ChangeControlVisibility3
        {
            get { return _visibility3; }
            set
            {
                _visibility3 = value;
                this.OnPropertyChanged("ChangeControlVisibility3");
            }
        }
    }
}