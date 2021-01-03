namespace rejestrator.Viewmodels
{
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using rejestrator.Models;
    using rejestrator.Utils;
    using System.Windows.Input;
    using rejestrator.Properties;
    using System;

    public class LoginViewModel : ViewModelBase, IPageViewModel
    {
        #region Singleton
        private LoginModel loginModel = null;

        public LoginViewModel()
        {
            loginModel = LoginModel.Instance;
        }
        #endregion

        #region Properties
        private bool _leftAvailable = true;
        public bool LeftAvailable
        {
            get => _leftAvailable;
            set
            {
                _leftAvailable = value;
                OnPropertyChanged(nameof(LeftAvailable));
            }
        }

        private bool _rightAvailable = false;
        public bool RightAvailable
        {
            get => _rightAvailable;
            set
            {
                _rightAvailable = value;
                OnPropertyChanged(nameof(RightAvailable));
            }
        }

        private string _id = string.Empty;
        public string ID
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(ID));
            }
        }

        private string _pin = string.Empty;
        public string Pin
        {
            get => _pin;
            set
            {
                _pin = value;
                OnPropertyChanged(nameof(Pin));
            }
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string _error = string.Empty;
        public string Error
        {
            get => _error;
            set
            {
                _error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        #region Dots
        private bool _dot1 = false;
        public bool Dot1
        {
            get => _dot1;
            set
            {
                _dot1 = value;
                OnPropertyChanged(nameof(Dot1));
            }
        }

        private bool _dot2 = false;
        public bool Dot2
        {
            get => _dot2;
            set
            {
                _dot2 = value;
                OnPropertyChanged(nameof(Dot2));
            }
        }

        private bool _dot3 = false;
        public bool Dot3
        {
            get => _dot3;
            set
            {
                _dot3 = value;
                OnPropertyChanged(nameof(Dot3));
            }
        }

        private bool _dot4 = false;
        public bool Dot4
        {
            get => _dot4;
            set
            {
                _dot4 = value;
                OnPropertyChanged(nameof(Dot4));
            }
        }
        #endregion

        #endregion

        #region Commands
        private ICommand _goToDashboard;

        public ICommand GoToDashboard
        {
            get
            {
                return _goToDashboard ?? (_goToDashboard = new RelayCommand(x =>
                {
                    Mediator.Notify(Token.GO_TO_DASHBOARD);
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

        private ICommand _loginAdmin;
    
        public ICommand LoginAdmin
        {
            get
            {
                return _loginAdmin ?? (_loginAdmin = new RelayCommand(x =>
                {
                    if (loginModel.LoginAdmin(Username, Password))
                    {
                        AdminDashboardViewModel.AdminName = loginModel.GetAdminName(loginModel.GetAdminID(Username, Password));
                        AdminDashboardViewModel.EmployeeListingViewModel = new EmployeeListingViewModel();
                        LeftAvailable ^= true;
                        RightAvailable ^= true;
                        ClearAllFields.Execute(null);
                        GoToAdminDashboard.Execute(null);
                    }
                    else
                        DisplayError.Execute(null);
                }));
            }
        }

        private ICommand _addDigit;

        public ICommand AddDigit
        {
            get
            {
                return _addDigit ?? (_addDigit = new RelayCommand(x =>
                {
                    if(ID.Length < ProgramInfo.ID_LENGTH && string.IsNullOrEmpty(Pin))
                        ID = $"{ID}{x}";
                    else if (Pin.Length < ProgramInfo.PIN_LENGTH && ID.Length == ProgramInfo.ID_LENGTH)
                    {
                        Pin = $"{Pin}{x}";

                        if (Pin.Length == 1)
                            Dot1 = true;
                        else if (Pin.Length == 2)
                            Dot2 = true;
                        else if (Pin.Length == 3)
                            Dot3 = true;
                        else if (Pin.Length == 4)
                            Dot4 = true;

                        if (Pin.Length == ProgramInfo.PIN_LENGTH)
                        {
                            if (loginModel.LoginEmployee(ID, Pin))
                            {
                                loginModel.InsertLoginDate(ID, loginModel.GetEmployeeName(ID), loginModel.GetEmployeeSurname(ID), DateTime.Now.ToString("MM/dd/yyyy H:mm"));
                                LeftAvailable ^= true;
                                RightAvailable ^= true;
                                ClearAllFields.Execute(null);
                                GoToDashboard.Execute(null);
                            }
                            else
                            {
                                DisplayError.Execute(null);
                            }
                        }    
                    } 
                }));
            }
        }

        private ICommand _eraseDigit;

        public ICommand EraseDigit
        {
            get
            {
                return _eraseDigit ?? (_eraseDigit = new RelayCommand(x =>
                {
                    if (Utils.Between(ID.Length, 0, ProgramInfo.ID_LENGTH) && string.IsNullOrEmpty(Pin))
                        ID = ID.Substring(0, ID.Length - 1);
                    else if(Utils.Between(Pin.Length, 0, ProgramInfo.PIN_LENGTH) && ID.Length == ProgramInfo.ID_LENGTH)
                    {
                        Pin = Pin.Substring(0, Pin.Length - 1);
                        if (Pin.Length == 0)
                            Dot1 = false;
                        else if (Pin.Length == 1)
                            Dot2 = false;
                        else if (Pin.Length == 2)
                            Dot3 = false;
                        else if (Pin.Length == 3)
                            Dot4 = false;
                    }
                        
                }));
            }
        }

        private ICommand _changeLoginMethod;

        public ICommand ChangeLoginMethod
        {
            get
            {
                return _changeLoginMethod ?? (_changeLoginMethod = new RelayCommand(x =>
                {
                    LeftAvailable ^= true;
                    RightAvailable ^= true;

                    ClearAllFields.Execute(null);
                }));
            }
        }

        private ICommand _clearAllFields;

        public ICommand ClearAllFields
        {
            get
            {
                return _clearAllFields ?? (_clearAllFields = new RelayCommand(x =>
                {
                    ID = string.Empty;
                    Pin = string.Empty;
                    Username = string.Empty;
                    Password = string.Empty;
                    Error = string.Empty;
                    Dot1 = false;
                    Dot2 = false;
                    Dot3 = false;
                    Dot4 = false;
                }));
            }
        }

        private ICommand _displayError;

        public ICommand DisplayError
        {
            get
            {
                return _displayError ?? (_displayError = new RelayCommand(x =>
                {
                    ClearAllFields.Execute(null);
                    Error = ResourcesLogin.Error;
                }));
            }
        }
        #endregion
    }
}
