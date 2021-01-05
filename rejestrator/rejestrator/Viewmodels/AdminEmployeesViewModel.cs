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

        private ObservableCollection<string> _dates = new ObservableCollection<string>();
        public ObservableCollection<string> Dates
        {
            get { return _dates; }
            set
            {
                _dates = value;
                OnPropertyChanged("Dates");
            }
        }

        private ObservableCollection<string> _tasks = new ObservableCollection<string>();
        public ObservableCollection<string> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnPropertyChanged("Tasks");
            }
        }

        private ObservableCollection<TaskInProgressModel> _tasksInProgress = new ObservableCollection<TaskInProgressModel>();
        public ObservableCollection<TaskInProgressModel> TasksInProgress
        {
            get { return _tasksInProgress; }
            set
            {
                _tasksInProgress = value;
                OnPropertyChanged("TasksInProgress");
            }
        }

        private ObservableCollection<TaskDoneModel> _tasksDone = new ObservableCollection<TaskDoneModel>();
        public ObservableCollection<TaskDoneModel> TasksDone
        {
            get { return _tasksDone; }
            set
            {
                _tasksDone = value;
                OnPropertyChanged("TasksDone");
            }
        }

        private string _selectedEmployee;
        public string SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                PopulateLists();
            }
        }

        private void PopulateLists()
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

                adminModel.GetLogsDatesForEmployee(datesToAdd, words[0]);
                adminModel.GetLogsTasksForEmployee(tasksToAdd, words[0]);
                adminModel.GetLogsTasksInProgressForEmployee(taskInProgressToAdd, words[0]);
                adminModel.GetLogsTasksDoneForEmployee(tasksDoneToAdd, words[0]);

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

        private void PopulateTaskLists()
        {
            Tasks.Clear();

            if (_selectedEmployee != null)
            {
                string[] words = _selectedEmployee.Split(' ');

                List<string> tasksToAdd = new List<string>();

                adminModel.GetLogsTasksForEmployee(tasksToAdd, words[0]);

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

                    Employee.Queries = new CollectionView(AdminDashboardViewModel.employeesList);
                    Employee.Queries.CurrentChanged += new EventHandler(AdminDashboardViewModel.queries_CurrentChanged);

                    Employee.TwoWays = new CollectionView(AdminDashboardViewModel.workItems);

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
        public static string AdminName { get; set; }
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
                            ListOfEmployees.Add(item.ID + " " + item.Name + " " + item.Surname);
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

        public static void queries_CurrentChanged(object sender, EventArgs e)
        {
            var currentQuery = (string)Employee.Queries.CurrentItem;
        }

        public static void twoWays_CurrentChanged(object sender, EventArgs e)
        {
            var currentQuery = (string)Employee.TwoWays.CurrentItem;
        }

        public string getCurrentListItem()
        {
            var currentQuery = (string)Employee.Queries.CurrentItem;
            return currentQuery;
        }

        public string getCurrentItemEmployee()
        {
            var currentQuery = (string)Employee.TwoWays.CurrentItem;
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