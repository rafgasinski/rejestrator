namespace rejestrator.Viewmodels
{
    using rejestrator.Models;
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using System.Collections.Generic;
    using System;
    using System.Windows.Threading;

    public class DashboardViewModel : ViewModelBase, IPageViewModel
    {
        #region Singleton

        private static EmployeeModel emploeeModel = null;
        public DashboardViewModel()
        {
            emploeeModel = EmployeeModel.Instance;
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
                    emploeeModel.StartTask(task, ID);
                    FillAvailable();
                    FillInProgress();
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
                    emploeeModel.EndTask(task, ID);
                    FillInProgress();
                    FillDone();
                }));
            }
        }

        #endregion

        #region Methods

        private void ViewChanged(object obj)
        {
            ID = emploeeModel.ID;
            Name = emploeeModel.Name;
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
            emploeeModel.GetTasksAvailable(ref tasksAvailable, ID);

            foreach (var taskAvailable in tasksAvailable)
                TasksAvailable.Add(taskAvailable);
        }

        private void FillInProgress()
        {
            TasksInProgress.Clear();

            List<TaskInProgressModel> tasksInProgress = new List<TaskInProgressModel>();
            emploeeModel.GetTasksInProgress(ref tasksInProgress, ID);

            foreach (var taskInProgress in tasksInProgress)
                TasksInProgress.Add(taskInProgress);
        }

        private void FillDone()
        {
            TasksDone.Clear();

            List<TaskDoneModel> tasksDone = new List<TaskDoneModel>();
            emploeeModel.GetTasksDone(ref tasksDone, ID);

            foreach (var taskDone in tasksDone)
                TasksDone.Add(taskDone);
        }

        #endregion
    }
}
