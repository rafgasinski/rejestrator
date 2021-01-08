namespace rejestrator.Models
{
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;

    public class TaskAvailableModel : ViewModelBase, IPageViewModel
    {
        private int _id;
        public int ID
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(ID));
            }
        }

        private string _task = null;
        public string Task
        {
            get => _task;
            set
            {
                _task = value;
                OnPropertyChanged(nameof(Task));
            }
        }

        public TaskAvailableModel(int id, string task)
        {
            ID = id;
            Task = task;
        }
    }
}
