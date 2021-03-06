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
    using System.Linq;
    using System.Windows;
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

        private static ObservableCollection<TaskAvailableModel> _tasks = new ObservableCollection<TaskAvailableModel>();
        public static ObservableCollection<TaskAvailableModel> Tasks
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

        private static string _selectedEmployee = string.Empty;
        public static string SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                PopulateLists();
            }
        }

        private Int32 _zindex;
        public Int32  Zindex
        {
            get { return _zindex; }
            set
            {
                _zindex = value;
                OnPropertyChanged(nameof(Zindex));
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
                List<TaskAvailableModel> tasksToAdd = new List<TaskAvailableModel>();
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

        public void PopulateListsOnViewChanged(object obj)
        {
            Reload.Execute(null);

            Dates.Clear();
            Tasks.Clear();
            TasksInProgress.Clear();
            TasksDone.Clear();

            if (_selectedEmployee != null)
            {
                string[] words = _selectedEmployee.Split(' ');

                List<string> datesToAdd = new List<string>();
                List<TaskAvailableModel> tasksToAdd = new List<TaskAvailableModel>();
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

                List<TaskAvailableModel> tasksToAdd = new List<TaskAvailableModel>();

                Instance.adminModel.GetLogsTasksForEmployee(tasksToAdd, words[0]);

                foreach (var task in tasksToAdd)
                    Tasks.Add(task);
            }
        }

        private ICommand _editEmployee;

        public ICommand EditEmployee
        {
            get
            {
                return _editEmployee ?? (_editEmployee = new RelayCommand(x =>
                {
                    Zindex = 5;
                    OnEdition();
                }));
            }
        }

        private ICommand _deleteEmployee;

        public ICommand DeleteEmployee
        {
            get
            {
                return _deleteEmployee ?? (_deleteEmployee = new RelayCommand(x =>
                {                
                    Delete();
                }));
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

        private ICommand _deleteTask = null;
        public ICommand DeleteTask
        {
            get
            {
                return _deleteTask ?? (_deleteTask = new RelayCommand(x =>
                {
                    TaskAvailableModel task = x as TaskAvailableModel;
                    adminModel.DeleteTask(task.ID.ToString());
                    PopulateLists();
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
                    ListOfEmployees.Clear();

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

            Mediator.Subscribe(Token.GO_TO_ADMIN_EMPLOYEES, PopulateListsOnViewChanged);
        }
        #endregion

        #region Properties
        public static string Name { get; set; }
        public static string Username { get; set; }
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
            var item = new Employee();
            await DialogHost.Show(item, "AddDialogHost");
            if (IsDialogAddOpen == true)
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
                            ListOfEmployees.Add($"{item.ID} {item.Name} {item.Surname}");
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
                        else if (adminModel.AdminIDUsed(item.IDadmin) && adminModel.AdminUsernameUsed(item.AdminUsername))
                        {
                            MessageBox.Show("Id oraz nazwa użytkoniwka została już przypisana!");
                        }
                        else if (adminModel.AdminIDUsed(item.IDadmin))
                        {
                            MessageBox.Show("To id zostało już przypisane!");
                        }
                        else if (adminModel.AdminUsernameUsed(item.AdminUsername))
                        {
                            MessageBox.Show("Nazwa użytkownka została już użyta!");
                        }
                        else
                        {
                            IsDialogAddOpen = false;
                            await DialogHost.Show(item, "AddAdminDialogHost");
                            if (IsDialogAdminOpen == true)
                            {
                                if (item.PasswordConfirm == adminModel.CanAdminDeleteemployee(Username) && item.PasswordConfirm != string.Empty)
                                {
                                    adminModel.InsertAdmin(item.IDadmin, item.AdminUsername, item.AdminPassword, item.AdminName, item.AdminSurname);
                                }
                                else if (item.PasswordConfirm == string.Empty)
                                {
                                    MessageBox.Show("Nie wpisano hasła.");
                                }
                                else
                                {
                                    MessageBox.Show("Niepoprawne hasło, nie dodano administratora.");
                                }                          
                            }

                            IsDialogAdminOpen = false;
                            AdminRaportViewModel.IsDialogAdminOpen = false;
                            AdminDashboardViewModel.IsDialogAdminOpen = false;
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

                    if (!adminModel.SameTaskAdded(words[0], item.Task))
                        adminModel.InsertTask(words[0], item.Task);
                    else
                        MessageBox.Show("To zadanie jest juz przydzielone temu pracownikowi.");

                    PopulateTaskLists();
                }
            }
            IsDialogAddOpen = false;
            AdminRaportViewModel.IsDialogAddOpen = false;
            AdminDashboardViewModel.IsDialogAddOpen = false;

        }

        private async void OnEdition()
        {
            var item = new Employee();           

            if(SelectedEmployee != string.Empty)
            {
                var selectedEmployee = SelectedEmployee.Split(' ');
                var tempEmployee = adminModel.GetEmployeeFullNameShift(selectedEmployee[0]);
                var tempEmployeeTable = tempEmployee.Split(' ');

                item.ID = selectedEmployee[0];
                item.Pin = adminModel.GetEmployeePin(item.ID);
                item.Name = tempEmployeeTable[0];
                item.Surname = tempEmployeeTable[1];
                Employee.TwoWays = new ObservableCollection<string> { "Dzienny", "Nocny" };
                Employee.SelectedShift = tempEmployeeTable[2];

                await DialogHost.Show(item, "EditDialogHost");
                if (IsDialogEditOpen == true)
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
                        else if (!adminModel.EmployeeIDUsed(item.ID) || item.ID == selectedEmployee[0])
                        {
                            adminModel.EditEmployeeUpdate(selectedEmployee[0], item.ID, item.Pin, item.Name, item.Surname, getCurrentItemEmployee());

                            for (int i = 0; i < ListOfEmployees.Count; i++)
                            {
                                if (ListOfEmployees[i] == SelectedEmployee)
                                {
                                    ListOfEmployees[i] = $"{item.ID} {item.Name} {item.Surname}";
                                    break;
                                }
                            }
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
            }        

            Zindex = -1;
            IsDialogEditOpen = false;
        }

        private async void Delete()
        {
            var item = new Employee();

            if (SelectedEmployee != string.Empty)
            {
                await DialogHost.Show(item, "DeleteDialogHost");
                if (IsDialogDeleteOpen == true)
                {
                    if (item.PasswordConfirm == adminModel.CanAdminDeleteemployee(Username) && item.PasswordConfirm != string.Empty)
                    {
                        var tempEmployee = SelectedEmployee.Split(' ');
                        adminModel.DeleteEmployee(tempEmployee[0]);
                        ListOfEmployees.Remove(SelectedEmployee);
                    }
                    else if(item.PasswordConfirm == string.Empty)
                    {
                        MessageBox.Show("Nie wpisano hasła.");
                    }
                    else
                    {
                        MessageBox.Show("Niepoprawne hasło, nie usunięto pracownika.");
                    }
                }
            }

            IsDialogDeleteOpen = false;
        }     

        private string _passwordHint = "Hasło";
        public string PasswordHint
        {
            get { return _passwordHint; }
            set
            {
                _passwordHint = value;
                OnPropertyChanged1(nameof(PasswordHint));
            }
        }

        private static bool _IsDialogEditOpen;
        public static bool IsDialogEditOpen
        {
            get { return _IsDialogEditOpen; }
            set
            {
                _IsDialogEditOpen = value;
                OnPropertyChanged1(nameof(IsDialogEditOpen));
            }
        }

        private static bool _IsDialogDeleteOpen;
        public static bool IsDialogDeleteOpen
        {
            get { return _IsDialogDeleteOpen; }
            set
            {
                _IsDialogDeleteOpen = value;
                OnPropertyChanged1(nameof(IsDialogDeleteOpen));
            }
        }

        private static bool _IsDialogAdminOpen;
        public static bool IsDialogAdminOpen
        {
            get { return _IsDialogAdminOpen; }
            set
            {
                _IsDialogAdminOpen = value;
                OnPropertyChanged1(nameof(IsDialogAdminOpen));
            }
        }

        private static bool _IsDialogAddOpen;
        public static bool IsDialogAddOpen
        {
            get { return _IsDialogAddOpen; }
            set
            {
                _IsDialogAddOpen = value;
                OnPropertyChanged1(nameof(IsDialogAddOpen));
            }
        }

        public ICommand ShowDialogCommand { get; }

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
                this.OnPropertyChanged(nameof(ChangeControlVisibility));
            }
        }

        private Visibility _visibility2 = Visibility.Collapsed;

        public Visibility ChangeControlVisibility2
        {
            get { return _visibility2; }
            set
            {
                _visibility2 = value;
                this.OnPropertyChanged(nameof(ChangeControlVisibility2));
            }
        }

        private Visibility _visibility3 = Visibility.Collapsed;

        public Visibility ChangeControlVisibility3
        {
            get { return _visibility3; }
            set
            {
                _visibility3 = value;
                this.OnPropertyChanged(nameof(ChangeControlVisibility3));
            }
        }
    }
}