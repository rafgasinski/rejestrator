﻿namespace rejestrator.Models
{
<<<<<<< Updated upstream
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;

    public class EmployeeModel : ViewModelBase, IPageViewModel
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

        private string _id;
        public string Id
        {
            get
            {
                return _id;
            }
            private set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            private set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
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

        public EmployeeModel(string id, string name, string date)
        {
            Id = id;
            Name = name;
            Date = date;
        }
=======
    public class EmployeeModel
    {
        #region Singleton
        private static EmployeeModel _instance = null;
        public static EmployeeModel Instance
        {
            get 
            {
                if(_instance == null)
                {
                    _instance = new EmployeeModel();
                }

                return _instance;
            }
        }
        #endregion
>>>>>>> Stashed changes
    }
}
