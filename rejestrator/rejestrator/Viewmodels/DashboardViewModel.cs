namespace rejestrator.Viewmodels
{
    using rejestrator.Models;
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using rejestrator.Utils;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using System.Collections.Generic;
    using System;
    using System.Windows.Threading;
    using MaterialDesignThemes.Wpf;
    using System.Windows;
    using System.Windows.Media;

    public class DashboardViewModel : ViewModelBase, IPageViewModel
    {
        #region Singleton

        private static EmployeeModel employeeModel = null;
        public DashboardViewModel()
        {
            employeeModel = EmployeeModel.Instance;
            Mediator.Subscribe(Token.GO_TO_DASHBOARD, ViewChanged);
        }

        #endregion

        #region Properties
        private string _id = null;
        public string ID 
        { 
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(ID));
            }
        }

        private string _name = null;
        public string Name 
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _shift = null;
        public string Shift
        {
            get => _shift;
            set
            {
                _shift = value;
                OnPropertyChanged(nameof(Shift));
            }
        }

        private int _time;
        public int Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }

        private string _passwordConfirm = string.Empty;
        public string PinConfirm
        {
            get { return _passwordConfirm; }
            set
            {
                _passwordConfirm = value;
                OnPropertyChanged(nameof(PinConfirm));
            }
        }

        private bool _okButtonEnabled = false;
        public bool OkButtonEnabled
        {
            get { return _okButtonEnabled; }
            set
            {
                _okButtonEnabled = value;
                OnPropertyChanged(nameof(OkButtonEnabled));
            }
        }

        private Brush _foregroundColor;
        public Brush ForegroundColor
        {
            get { return _foregroundColor; }
            set
            {
                _foregroundColor = value;
                OnPropertyChanged(nameof(ForegroundColor));
            }
        }

        private ICommand _resetTimer;

        public ICommand ResetTimer
        {
            get
            {
                return _resetTimer ?? (_resetTimer = new RelayCommand(x =>
                {
                    StopTimer();
                    ForegroundColor = Brushes.White;
                    StartTimer();

                }));
            }
        }

        public ObservableCollection<TaskAvailableModel> TasksAvailable { get; set; } = new ObservableCollection<TaskAvailableModel>();

        public ObservableCollection<TaskInProgressModel> TasksInProgress { get; set; } = new ObservableCollection<TaskInProgressModel>();

        public ObservableCollection<TaskDoneModel> TasksDone { get; set; } = new ObservableCollection<TaskDoneModel>();

        private DispatcherTimer timer = null;

        #endregion

        #region Commands

        private ICommand _goToLogin;
        public ICommand GoToLogin
        {
            get
            {
                return _goToLogin ?? (_goToLogin = new RelayCommand(x =>
                {
                    StopTimer();
                    Mediator.Notify(Token.GO_TO_LOGIN);
                }));
            }
        }

        private ICommand _startTask = null;
        public ICommand StartTask
        {
            get
            {
                return _startTask ?? (_startTask = new RelayCommand(x =>
                {
                    TaskAvailableModel task = x as TaskAvailableModel;
                    StartTaskDialog(task);
                }));
            }
        }

        private ICommand _endTask = null;
        public ICommand EndTask
        {
            get
            {
                return _endTask ?? (_endTask = new RelayCommand(x =>
                {
                    TaskInProgressModel task = x as TaskInProgressModel;
                    EndTaskDialog(task);
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
                    if (PinConfirm.Length < ProgramInfo.PIN_LENGTH)
                        PinConfirm = $"{PinConfirm}{x}";
                    if (PinConfirm.Length == ProgramInfo.PIN_LENGTH)
                        OkButtonEnabled = true;

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
                    if (Utils.Between(PinConfirm.Length, 0, ProgramInfo.PIN_LENGTH))
                        PinConfirm = PinConfirm.Substring(0, PinConfirm.Length - 1);

                    OkButtonEnabled = false;
                }));
            }
        }

        #endregion

        #region Methods

        private void ViewChanged(object obj)
        {
            ID = employeeModel.ID;
            Name = employeeModel.Name;
            Shift = employeeModel.Shift;
            ForegroundColor = Brushes.White;
            StartTimer();
            FillLists();
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += new EventHandler(TimerTick);
            Time = 60;

            timer.Start();
        }

        private void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Time--;
            if(Time <= 10)
            {
                ForegroundColor = Brushes.Red;
            }

            if (Time <= 0)
            {
                StopTimer();
                GoToLogin.Execute(null);
            }
        }

        private void FillLists()
        {
            FillAvailable();
            FillInProgress();
            FillDone();
        }

        private void FillAvailable()
        {
            TasksAvailable.Clear();

            List<TaskAvailableModel> tasksAvailable = new List<TaskAvailableModel>();
            employeeModel.GetTasksAvailable(ref tasksAvailable, ID);

            foreach (var taskAvailable in tasksAvailable)
                TasksAvailable.Add(taskAvailable);
        }

        private void FillInProgress()
        {
            TasksInProgress.Clear();

            List<TaskInProgressModel> tasksInProgress = new List<TaskInProgressModel>();
            employeeModel.GetTasksInProgress(ref tasksInProgress, ID);

            foreach (var taskInProgress in tasksInProgress)
                TasksInProgress.Add(taskInProgress);
        }

        private void FillDone()
        {
            TasksDone.Clear();

            List<TaskDoneModel> tasksDone = new List<TaskDoneModel>();
            employeeModel.GetTasksDone(ref tasksDone, ID, DateTime.Now.ToString("dd/MM/yyyy"));

            foreach (var taskDone in tasksDone)
                TasksDone.Add(taskDone);
        }

        private async void StartTaskDialog(TaskAvailableModel task)
        {
            if (await DialogHost.Show(new DashboardViewModel()) is DashboardViewModel item)
            {
                if (item.PinConfirm == employeeModel.pin && item.PinConfirm != string.Empty)
                {                
                    employeeModel.StartTask(task, ID);
                    FillAvailable();
                    FillInProgress();
                }
                else if (item.PinConfirm == string.Empty)
                {
                    MessageBox.Show("Nie wpisano pinu.");
                }
                else
                {
                    MessageBox.Show("Niepoprawny pin.");
                }
            }
        }

        private async void EndTaskDialog(TaskInProgressModel task)
        {
            if (await DialogHost.Show(new DashboardViewModel()) is DashboardViewModel item)
            {
                if (item.PinConfirm == employeeModel.pin && item.PinConfirm != string.Empty)
                {
                    employeeModel.EndTask(task, ID);
                    FillInProgress();
                    FillDone();
                }
                else if (item.PinConfirm == string.Empty)
                {
                    MessageBox.Show("Nie wpisano pinu.");
                }
                else
                {
                    MessageBox.Show("Niepoprawny pin.");
                }
            }
        }

        #endregion
    }
}
