namespace rejestrator.Viewmodels
{
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System.Windows.Input;
    using rejestrator.Models;
    using MaterialDesignThemes.Wpf;
    using System.Windows.Data;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.ComponentModel;

    public class AdminDashboardViewModel : ViewModelBase, IPageViewModel
    {
        private static AdminDashboardViewModel _instance = new AdminDashboardViewModel();
        public static AdminDashboardViewModel Instance { get { return _instance; } }
        public static EmployeeListingViewModel EmployeeListingViewModel { get; set; }
        public static ObservableCollection<string> employeesList = new ObservableCollection<string>();
        public static ObservableCollection<string> workItems = new ObservableCollection<string> { "Dzienny, Nocny" };

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

                    Employee.Queries = new CollectionView(employeesList);
                    Employee.Queries.CurrentChanged += new EventHandler(queries_CurrentChanged);

                    Employee.TwoWays = new CollectionView(workItems);

                    OnAdd();           
                }));
            }
        }

        #region Singleton
        private AdminModel adminModel = null;

        public AdminDashboardViewModel()
        {
            adminModel = AdminModel.Instance;
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
                    foreach (EmployeeModel employeeViewModel in EmployeeListingViewModel.GetEmployeeViewModels())
                    {
                        EmployeeListingViewModel._employeeCollectionView.Add(employeeViewModel);
                    }
                }));
            }
        }

        private async void OnAdd()
        {
            if (await DialogHost.Show(new Employee()) is Employee item)
            {
                if(item.Task == null)
                {
                    if(item.ID != null && item.Pin != null && item.Name != null && item.Surname != null)
                    {
                        if(!adminModel.EmployeeIDUsed(item.ID))
                        {
                            string shift = getCurrentItemEmployee();
                            adminModel.InsertEmployee(item.ID, item.Pin, item.Name, item.Surname, shift);
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

    }

    public class Employee : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ID { get; set; }
        public string Pin { get; set; }
        public string Task { get; set; }
        public static CollectionView Queries { get; set; }
        public static CollectionView TwoWays { get; set; }

        public Employee()
        {
            _myCommand = new MyCommand(FuncToCall, FuncToEvaluate);
            _myCommand2 = new MyCommand(FuncToCall2, FuncToEvaluate);
        }

        private ICommand _myCommand;

        public ICommand MyButtonClickCommand
        {
            get { return _myCommand; }
            set { _myCommand = value; }
        }

        private ICommand _myCommand2;

        public ICommand MyButtonClickCommand2
        {
            get { return _myCommand2; }
            set { _myCommand2 = value; }
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
                this.OnPropertyChanged("ChangeControlVisibility");
            }
        }

        private Visibility _visibility2 = Visibility.Visible;

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
