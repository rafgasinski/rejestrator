namespace rejestrator.Viewmodels
{
    using MaterialDesignThemes.Wpf;
    using rejestrator.Models;
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    public class AdminEmployeesViewModel : ViewModelBase, IPageViewModel
    {
        private static AdminEmployeesViewModel _instance = new AdminEmployeesViewModel();
        public static AdminEmployeesViewModel Instance { get { return _instance; } }

        private ICommand _addCommand;

        public ICommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(x =>
                {
                    OnAdd();
                }));
            }
        }

        #region Singleton
        private AdminModel adminModel = null;

        public AdminEmployeesViewModel()
        {
            adminModel = AdminModel.Instance;
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
                            adminModel.InsertEmployee(item.ID, item.Pin, item.Name, item.Surname);
                            AdminDashboardViewModel.employeesList.Add(item.ID + " " + item.Name + " " + item.Surname);
                            Employee.Queries = new CollectionView(AdminDashboardViewModel.employeesList);
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

                        if (!adminModel.TaskAlreadyIn(words[0]))
                        {
                            adminModel.InsertTask(words[0], words[1], words[2], item.Task);
                        }
                        else
                        {
                            MessageBox.Show("To zadanie zostało już przypisane!");
                        }
                    }
                }
            }
        }

        public static void queries_CurrentChanged(object sender, EventArgs e)
        {
            var currentQuery = (string)Employee.Queries.CurrentItem;
        }

        public string getCurrentListItem()
        {
            var currentQuery = (string)Employee.Queries.CurrentItem;
            return currentQuery;
        }
    }
}