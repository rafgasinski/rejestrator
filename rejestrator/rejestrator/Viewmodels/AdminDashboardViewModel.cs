namespace rejestrator.Viewmodels
{
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System.Windows.Input;
    using rejestrator.Models;
    using System.Collections.ObjectModel;
    using MaterialDesignThemes.Wpf;
    using System.Windows.Data;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Linq;

    public class AdminDashboardViewModel : ViewModelBase, IPageViewModel
    {
        private static AdminDashboardViewModel _instance = new AdminDashboardViewModel();
        public static AdminDashboardViewModel Instance { get { return _instance; } }
        public static EmployeeListingViewModel EmployeeListingViewModel { get; set; }
        public static ObservableCollection<string> employeesList = new ObservableCollection<string>();
        public static ObservableCollection<string> workItems = new ObservableCollection<string> { "Dzienny", "Nocny" };

        private ICommand _addCommand;

        public ICommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(x =>
                {
                    List<string> employeeList = new List<string>();

                    employeesList.Clear();

                    adminModel.GetEmployeesFullNamesandID(employeeList);

                    foreach (var employee in employeeList)
                        employeesList.Add(employee);

                    Employee.Queries = new ObservableCollection<string>(employeesList);

                    Employee.TwoWays = new ObservableCollection<string>(workItems);

                    OnAdd();           
                }));
            }
        }

        #region Singleton
        private AdminModel adminModel = null;

        public AdminDashboardViewModel()
        {
            adminModel = AdminModel.Instance;
            Mediator.Subscribe(Token.GO_TO_ADMIN_DASHBOARD, FillListOnViewchanged);
        }
        #endregion

        private void FillListOnViewchanged(object obj)
        {
            EmployeeListingViewModel = new EmployeeListingViewModel();
        }
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

        private ICommand _reload;

        public ICommand Reload
        {
            get
            {
                return _reload ?? (_reload = new RelayCommand(x =>
                {
                    EmployeeListingViewModel._employeeCollectionView.Clear();
                    foreach (AllLogsModel employeeViewModel in EmployeeListingViewModel.GetEmployeeViewModels())
                    {
                        EmployeeListingViewModel._employeeCollectionView.Add(employeeViewModel);
                    }
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
                            AdminEmployeesViewModel.IsDialogAdminOpen = false;
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

                    if (!adminModel.SameTaskAdded(words[0]))
                        adminModel.InsertTask(words[0], item.Task);
                    else
                        MessageBox.Show("To zadanie jest juz przydzielone temu pracownikowi.");
                }
            }
            IsDialogAddOpen = false;
            AdminRaportViewModel.IsDialogAddOpen = false;
            AdminEmployeesViewModel.IsDialogAddOpen = false;
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

        public static event PropertyChangedEventHandler PropertyChanged1;

        public static void OnPropertyChanged1(string propertyName)
        {
            if (PropertyChanged1 != null)
            {
                PropertyChanged1(Instance, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public class Employee : INotifyPropertyChanged
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;

        public string AdminName { get; set; } = string.Empty;
        public string AdminSurname { get; set; } = string.Empty;
        public string IDadmin { get; set; } = string.Empty;
        public string AdminUsername { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;

        public string Task { get; set; } = string.Empty;
        public static ObservableCollection<string> Queries { get; set; }
        public static ObservableCollection<string> TwoWays { get; set; }

        public Employee()
        {
            _myCommand = new MyCommand(FuncToCall, FuncToEvaluate);
            _myCommand2 = new MyCommand(FuncToCall2, FuncToEvaluate);
            _myCommand3 = new MyCommand(FuncToCall3, FuncToEvaluate);
            _closeDialogAdd = new MyCommand(CloseDialogHostAdd, FuncToEvaluate);
            _closeDialogEdit = new MyCommand(CloseDialogHostEdit, FuncToEvaluate);
            _closeDialogDelete = new MyCommand(CloseDialogHostDelete, FuncToEvaluate);
            _closeDialogAddAdmin = new MyCommand(CloseDialogHostAddAdmin, FuncToEvaluate);
            _closeDialogAddAndAddToDatabase = new MyCommand(CloseDialogAddAndAdd, FuncToEvaluate);
            _closeDialogEditAndEditDatabase = new MyCommand(CloseDialogEditAndEdit, FuncToEvaluate);
            _closeDialogDeleteAndDeleteFromDatabase = new MyCommand(CloseDialogDeleteAndDelete, FuncToEvaluate);         
            _closeDialogAddAdminAndAdd = new MyCommand(CloseDialogAdminAndAdd, FuncToEvaluate);
        }

        private static string _selectedEmployee;
        public static string SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
            }
        }

        private static string _selectedShift;
        public static string SelectedShift
        {
            get { return _selectedShift; }
            set
            {
                _selectedShift = value;
            }
        }

        private string _passwordConfirm = string.Empty;
        public string PasswordConfirm
        {
            get { return _passwordConfirm; }
            set
            {
                _passwordConfirm = value;
            }
        }

        private ICommand _closeDialogAdd;

        public ICommand CloseDialogAdd
        {
            get { return _closeDialogAdd; }
            set { _closeDialogAdd = value; }
        }

        private ICommand _closeDialogEdit;

        public ICommand CloseDialogEdit
        {
            get { return _closeDialogEdit; }
            set { _closeDialogEdit = value; }
        }

        private ICommand _closeDialogDelete;

        public ICommand CloseDialogDelete
        {
            get { return _closeDialogDelete; }
            set { _closeDialogDelete = value; }
        }

        private ICommand _closeDialogAddAdmin;

        public ICommand CloseDialogAddAdmin
        {
            get { return _closeDialogAddAdmin; }
            set { _closeDialogAddAdmin = value; }
        }

        private ICommand _closeDialogAddAdminAndAdd;

        public ICommand CloseDialogAddAdminAndAdd
        {
            get { return _closeDialogAddAdminAndAdd; }
            set { _closeDialogAddAdminAndAdd = value; }
        }

        private ICommand _closeDialogAddAndAddToDatabase;

        public ICommand CloseDialogAddAndAddToDatabase
        {
            get { return _closeDialogAddAndAddToDatabase; }
            set { _closeDialogAddAndAddToDatabase = value; }
        }

        private ICommand _closeDialogEditAndEditDatabase;

        public ICommand CloseDialogEditAndEditDatabase
        {
            get { return _closeDialogEditAndEditDatabase; }
            set { _closeDialogEditAndEditDatabase = value; }
        }

        private ICommand _closeDialogDeleteAndDeleteFromDatabase;

        public ICommand CloseDialogDeleteAndDeleteFromDatabase
        {
            get { return _closeDialogDeleteAndDeleteFromDatabase; }
            set { _closeDialogDeleteAndDeleteFromDatabase = value; }
        }

        private void CloseDialogHostAdd(object context)
        {          
            DialogHost.Close("AddDialogHost");
            AdminEmployeesViewModel.IsDialogAddOpen = false;
            AdminDashboardViewModel.IsDialogAddOpen = false;
            AdminRaportViewModel.IsDialogAddOpen = false;
        }

        private void CloseDialogHostAddAdmin(object context)
        {
            DialogHost.Close("AddAdminDialogHost");
            AdminEmployeesViewModel.IsDialogAdminOpen = false;
            AdminDashboardViewModel.IsDialogAdminOpen = false;
            AdminRaportViewModel.IsDialogAdminOpen = false;
        }

        private void CloseDialogHostEdit(object context)
        {          
            DialogHost.Close("EditDialogHost");
            AdminEmployeesViewModel.IsDialogEditOpen = false;
        }

        private void CloseDialogHostDelete(object context)
        {
            DialogHost.Close("DeleteDialogHost");
            AdminEmployeesViewModel.IsDialogDeleteOpen = false;
        }

        private void CloseDialogAddAndAdd(object context)
        {
            DialogHost.Close("AddDialogHost");
            AdminEmployeesViewModel.IsDialogAddOpen = true;
            AdminDashboardViewModel.IsDialogAddOpen = true;
            AdminRaportViewModel.IsDialogAddOpen = true;
        }

        private void CloseDialogEditAndEdit(object context)
        {
            DialogHost.Close("EditDialogHost");
            AdminEmployeesViewModel.IsDialogEditOpen = true;
        }

        private void CloseDialogDeleteAndDelete(object context)
        {
            DialogHost.Close("DeleteDialogHost");
            AdminEmployeesViewModel.IsDialogDeleteOpen = true;
        }

        private void CloseDialogAdminAndAdd(object context)
        {
            DialogHost.Close("AddAdminDialogHost");
            AdminEmployeesViewModel.IsDialogAdminOpen = true;
            AdminDashboardViewModel.IsDialogAdminOpen = true;
            AdminRaportViewModel.IsDialogAdminOpen = true;
        }

        private ICommand _myCommand;

        public ICommand EmployeeAddViewCommand
        {
            get { return _myCommand; }
            set { _myCommand = value; }
        }


        private ICommand _myCommand2;

        public ICommand TaskAddViewCommand
        {
            get { return _myCommand2; }
            set { _myCommand2 = value; }
        }

        private ICommand _myCommand3;

        public ICommand AddAdmindCommand
        {
            get { return _myCommand3; }
            set { _myCommand3 = value; }
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
            if (this.ChangeControlVisibility3 == Visibility.Collapsed && this.ChangeControlVisibility2 == Visibility.Visible)
            {
                this.ChangeControlVisibility3 = Visibility.Visible;
                this.ChangeControlVisibility2 = Visibility.Collapsed;
            }
            else
            {
                this.ChangeControlVisibility = Visibility.Collapsed;
                this.ChangeControlVisibility2 = Visibility.Visible;
            }
        }

        private void FuncToCall3(object context)
        {
            if (this.ChangeControlVisibility4 == Visibility.Collapsed && this.ChangeControlVisibility2 == Visibility.Visible)
            {
                this.ChangeControlVisibility4 = Visibility.Visible;
                this.ChangeControlVisibility2 = Visibility.Collapsed;
            }
            else
            {
                this.ChangeControlVisibility4 = Visibility.Collapsed;
                this.ChangeControlVisibility2 = Visibility.Visible;
            }
        }

        private bool FuncToEvaluate(object context)
        {
            return true;
        }

        private Visibility _visibility = Visibility.Collapsed;

        public Visibility ChangeControlVisibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                this.OnPropertyChanged(nameof(ChangeControlVisibility));
            }
        }

        private Visibility _visibility2 = Visibility.Visible;

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

        private Visibility _visibility4 = Visibility.Collapsed;

        public Visibility ChangeControlVisibility4
        {
            get { return _visibility4; }
            set
            {
                _visibility4 = value;
                this.OnPropertyChanged(nameof(ChangeControlVisibility4));
            }
        }

        private Visibility _visibility5 = Visibility.Collapsed;

        public Visibility ChangeControlVisibility5
        {
            get { return _visibility5; }
            set
            {
                _visibility5 = value;
                this.OnPropertyChanged(nameof(ChangeControlVisibility5));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    class MyCommand : ICommand
    {
        public delegate void ICommandOnExecute(object parameter);
        public delegate bool ICommandOnCanExecute(object parameter);
        private ICommandOnExecute _execute;
        private ICommandOnCanExecute _canExecute;

        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke(parameter);
        }

        public MyCommand(ICommandOnExecute onExecuteMethod, ICommandOnCanExecute onCanExecuteMethod)
        {
            _execute = onExecuteMethod;
            _canExecute = onCanExecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }
    }

    public static class Bindings
    {
        public static bool GetVisibilityToEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(VisibilityToEnabledProperty);
        }

        public static void SetVisibilityToEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(VisibilityToEnabledProperty, value);
        }
        public static readonly DependencyProperty VisibilityToEnabledProperty =
            DependencyProperty.RegisterAttached("VisibilityToEnabled", typeof(bool), typeof(Bindings), new PropertyMetadata(false, OnVisibilityToEnabledChanged));

        private static void OnVisibilityToEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is FrameworkElement element)
            {
                if ((bool)args.NewValue)
                {
                    Binding b = new Binding
                    {
                        Source = element,
                        Path = new PropertyPath(nameof(FrameworkElement.IsEnabled)),
                        Converter = new BooleanToVisibilityConverter()
                    };
                    element.SetBinding(UIElement.VisibilityProperty, b);
                }
                else
                {
                    BindingOperations.ClearBinding(element, UIElement.VisibilityProperty);
                }
            }
        }
    }
}
