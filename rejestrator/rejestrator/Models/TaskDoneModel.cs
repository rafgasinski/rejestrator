using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rejestrator.Models
{
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    public class TaskDoneModel : ViewModelBase, IPageViewModel
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

        private string _datestart;
        public string DateStart
        {
            get
            {
                return _datestart;
            }
            private set
            {
                _datestart = value;
                OnPropertyChanged(nameof(DateStart));
            }
        }

        private string _dateend;
        public string DateEnd
        {
            get
            {
                return _dateend;
            }
            private set
            {
                _dateend = value;
                OnPropertyChanged(nameof(DateEnd));
            }
        }

        private string _time;
        public string Time
        {
            get
            {
                return _time;
            }
            private set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }

        public TaskDoneModel(int id, string task, string datestart, string dateend, string time)
        {
            ID = id;
            Task = task;
            DateStart = datestart;
            DateEnd = dateend;
            Time = time;
        }
    }
}
