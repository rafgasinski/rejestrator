namespace rejestrator.Models
{
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    public class TaskInProgressModel : ViewModelBase, IPageViewModel
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        private string _task;
        public string Task
        {
            get
            {
                return _task;
            }
            private set
            {
                _task = value;
                OnPropertyChanged(nameof(Task));
            }
        }

        private string _date;
        public string Date
        {
            get
            {
                return _date;
            }
            private set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public TaskInProgressModel(string task, string date)
        {
            Task = task;
            Date = date;
        }
    }
}
