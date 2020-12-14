namespace rejestrator.Viewmodels
{
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using rejestrator.Models;
    using rejestrator.Utils;
    using System.Windows.Input;

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
                        Pin = $"{Pin}{x}";
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
                        Pin = Pin.Substring(0, Pin.Length - 1);
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
                }));
            }
        }
        #endregion
    }
}
