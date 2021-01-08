namespace rejestrator.Viewmodels
{
    using rejestrator.Models;
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using System.Collections.Generic;
    using System;

    public class DashboardViewModel : ViewModelBase, IPageViewModel
    {
        #region Singleton

        private static EmployeeModel emploeeModel = null;
        public DashboardViewModel()
        {
            emploeeModel = EmployeeModel.Instance;
            FillLists();
        }

        #endregion

        #region Properties
        public static string ID { get; set; }

        public static string Name { get; set; }

        public static ObservableCollection<TaskAvailableModel> TasksAvailable { get; set; } = new ObservableCollection<TaskAvailableModel>();

        public static ObservableCollection<TaskInProgressModel> TasksInProgress { get; set; } = new ObservableCollection<TaskInProgressModel>();

        public static ObservableCollection<TaskDoneModel> TasksDone { get; set; } = new ObservableCollection<TaskDoneModel>();

        #endregion

        #region Commands

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

        public static void FillLists()
        {
            FillAvailable();
            FillInProgress();
            FillDone();
        }

        private static void FillAvailable()
        {
            TasksAvailable.Clear();

            List<TaskAvailableModel> tasksAvailable = new List<TaskAvailableModel>();
            emploeeModel.GetTasksAvailable(ref tasksAvailable, ID);

            foreach (var taskAvailable in tasksAvailable)
                TasksAvailable.Add(taskAvailable);
        }

        private static void FillInProgress()
        {
            TasksInProgress.Clear();

            List<TaskInProgressModel> tasksInProgress = new List<TaskInProgressModel>();
            emploeeModel.GetTasksInProgress(ref tasksInProgress, ID);

            foreach (var taskInProgress in tasksInProgress)
                TasksInProgress.Add(taskInProgress);
        }

        private static void FillDone()
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
