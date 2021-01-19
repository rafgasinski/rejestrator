namespace rejestrator.Viewmodels
{
    using LiveCharts;
    using LiveCharts.Defaults;
    using MaterialDesignThemes.Wpf;
    using rejestrator.Models;
    using rejestrator.Viewmodels.BaseViewModel;
    using rejestrator.Viewmodels.Navigator;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using MigraDoc;
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.Rendering;
    using PdfSharp.Pdf;
    using System.Diagnostics;
    using System.Globalization;

    public class AdminRaportViewModel : ViewModelBase, IPageViewModel, INotifyPropertyChanged
    {
        private static AdminRaportViewModel _instance = new AdminRaportViewModel();
        public static AdminRaportViewModel Instance { get { return _instance; } }
        List<EmployeeChartModel> employees = new List<EmployeeChartModel>();
        public int leftEmployeeNumber, rightEmployeeNumber;

        #region Singleton
        private AdminModel adminModel = null;

        public AdminRaportViewModel()
        {
            adminModel = AdminModel.Instance;

            EmployeeLogsCounts = new ChartValues<int>();
            EmployeeTaskCounts = new ChartValues<int>();
            EmployeeTasksInProgressCounts = new ChartValues<int>();
            EmployeeTasksDoneCounts = new ChartValues<int>();
            EmployeeNames = new ObservableCollection<string>();

            Mediator.Subscribe(Token.GO_TO_ADMIN_RAPORT, RefreshChart);
        }
        #endregion

        private void RefreshChart(object obj)
        {
            Reload.Execute(null);
            Page = 1;
        }

        #region Properties

        public static string Name { get; set; }
        public static string Username { get; set; }

        private int _page = 1;
        public int Page
        {
            get => _page;
            set
            {
                _page = value;
                OnPropertyChanged(nameof(Page));
            }
        }

        private static bool _IsDialogAdminOpen;
        public static bool IsDialogAdminOpen
        {
            get { return _IsDialogAdminOpen; }
            set
            {
                _IsDialogAdminOpen = value;
                OnPropertyChanged1(nameof(IsDialogAdminOpen));
            }
        }

        private static bool _IsDialogAddOpen;
        public static bool IsDialogAddOpen
        {
            get { return _IsDialogAddOpen; }
            set
            {
                _IsDialogAddOpen = value;
                OnPropertyChanged1(nameof(IsDialogAddOpen));
            }
        }

        private bool _checkedBool = false;
        public bool CheckedBool
        {
            get => _checkedBool;
            set
            {
                _checkedBool = value;
                OnPropertyChanged(nameof(CheckedBool));
            }
        }

        private static ChartValues<int> _employeeLogsCounts;
        public static ChartValues<int> EmployeeLogsCounts
        {
            get
            {
                return _employeeLogsCounts;
            }
            private set
            {
                _employeeLogsCounts = value;
                OnPropertyChanged1(nameof(EmployeeLogsCounts));
            }
        }

        private static ChartValues<int> _employeeTaskCounts;
        public static ChartValues<int> EmployeeTaskCounts
        {
            get
            {
                return _employeeTaskCounts;
            }
            private set
            {
                _employeeTaskCounts = value;
                OnPropertyChanged1(nameof(EmployeeTaskCounts));
            }
        }

        private static ChartValues<int> _employeeTasksInProgressCounts;
        public static ChartValues<int> EmployeeTasksInProgressCounts
        {
            get
            {
                return _employeeTasksInProgressCounts;
            }
            private set
            {
                _employeeTasksInProgressCounts = value;
                OnPropertyChanged1(nameof(EmployeeTasksInProgressCounts));
            }
        }

        private static ChartValues<int> _employeeTasksDoneCounts;
        public static ChartValues<int> EmployeeTasksDoneCounts
        {
            get
            {
                return _employeeTasksDoneCounts;
            }
            private set
            {
                _employeeTasksDoneCounts = value;
                OnPropertyChanged1(nameof(EmployeeTasksDoneCounts));
            }
        }

        public static ObservableCollection<string> EmployeeNames { get; set; }

        #endregion

        #region Commands

        private ICommand _checked;

        public ICommand Checked
        {
            get
            {
                return _checked ?? (_checked = new RelayCommand(x =>
                {
                    CheckedBool = true;

                    employees.Clear();
                    Page = 1;
                    List<string> ids = new List<string>();

                    rightEmployeeNumber = 0;
                    leftEmployeeNumber = 0;

                    adminModel.GetEmployeesIDs(ids);

                    for (int i = 0; i < adminModel.GetEmployeeCount(); i++)
                    {
                        var temp = ids[i].Split(' ');
                        employees.Add(new EmployeeChartModel(ids[i], adminModel.GetEmployeeLogsCountToday(temp[0]), adminModel.GetEmployeeTasksCount(temp[0]), adminModel.GetEmployeeTasksInProgressCount(temp[0]), adminModel.GetEmployeeTasksDoneCountToday(temp[0])));
                    }

                    EmployeeNames.Clear();
                    EmployeeLogsCounts.Clear();
                    EmployeeTaskCounts.Clear();
                    EmployeeTasksInProgressCounts.Clear();
                    EmployeeTasksDoneCounts.Clear();

                    for (int i = 0; i < 4; i++)
                    {
                        if (i < employees.Count())
                        {
                            EmployeeLogsCounts.Add(employees[i].NumberofLogins);
                            EmployeeTaskCounts.Add(employees[i].NumberofTasks);
                            EmployeeTasksInProgressCounts.Add(employees[i].NumberofTasksInProgress);
                            EmployeeTasksDoneCounts.Add(employees[i].NumberofTasksDone);
                            EmployeeNames.Add(employees[i].Name);
                        }
                        rightEmployeeNumber++;
                    }

                }));
            }
        }

        private ICommand _unchecked;

        public ICommand Unchecked
        {
            get
            {
                return _unchecked ?? (_unchecked = new RelayCommand(x =>
                {
                    Reload.Execute(null);

                }));
            }
        }

        private ICommand _generateRaport;

        public ICommand GenerateRaport
        {
            get
            {
                return _generateRaport ?? (_generateRaport = new RelayCommand(x =>
                {
                DateTime dateTime = DateTime.Now;
                string fileDate = dateTime.ToString("yyyy-MM-dd_HH-mm");
                string date = dateTime.ToString("dd.MM.yyyy HH:mm");
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                List<string> ids = new List<string>();
                adminModel.GetEmployeesIDs(ids);
                Document document = new Document();
                document.DefaultPageSetup.LeftMargin = document.DefaultPageSetup.LeftMargin / 2;
                document.DefaultPageSetup.LeftMargin = document.DefaultPageSetup.RightMargin / 2;
                Section section = document.AddSection();
                Paragraph paragraph = section.AddParagraph("Raport");
                paragraph.Format.Font.Size = 48;
                paragraph.Format.SpaceAfter = 6;

                paragraph = section.AddParagraph(date);
                paragraph.Format.Font.Size = 24;
                paragraph.Format.SpaceAfter = 24;
                /*int recordsOnPage = 2;*/
                foreach (string id in ids)
                {
                    var temp = id.Split(' ');
                    string fullNameShift = string.Empty;
                    string tasksCount = string.Empty;
                    string tasksInProgress = string.Empty;
                    string tasksDoneCount = string.Empty;
                    string logsCount = string.Empty;
                    string tasksDoneCountToday = string.Empty;
                    string logsCountToday = string.Empty;
                    List<TaskDoneModel> tasks = new List<TaskDoneModel>();
                    adminModel.GetReportData(temp[0], ref fullNameShift, ref tasksCount, ref tasksInProgress, ref tasksDoneCount, ref logsCount, ref tasksDoneCountToday, ref logsCountToday, tasks);

                    paragraph = section.AddParagraph(fullNameShift);
                    paragraph.Format.LeftIndent = 6;
                    paragraph.Format.Font.Size = 18;
                    paragraph.Format.TabStops.Clear();
                    paragraph.Format.TabStops.AddTabStop((document.DefaultPageSetup.PageWidth - document.DefaultPageSetup.LeftMargin - document.DefaultPageSetup.RightMargin) / 4);
                    paragraph.Format.TabStops.AddTabStop((document.DefaultPageSetup.PageWidth - document.DefaultPageSetup.LeftMargin - document.DefaultPageSetup.RightMargin) / 7 * 5);
                    paragraph.Format.SpaceAfter = 3;

                    paragraph = section.AddParagraph($"Przydzielone: {tasksCount}\tW trakcie: {tasksInProgress}\tZakończone: {tasksDoneCountToday}");
                    paragraph.Format.TabStops.Clear();
                    paragraph.Format.TabStops.AddTabStop((document.DefaultPageSetup.PageWidth - document.DefaultPageSetup.LeftMargin - document.DefaultPageSetup.RightMargin) / 10 * 3);
                    paragraph.Format.TabStops.AddTabStop((document.DefaultPageSetup.PageWidth - document.DefaultPageSetup.LeftMargin - document.DefaultPageSetup.RightMargin) / 3 * 2);
                    paragraph.Format.Font.Size = 13.5;
                    paragraph.AddText($"\nLogowania: {logsCountToday}\tLogowania(ogółem): {logsCount}\tZakończone(ogółem): {tasksDoneCount}");

                    paragraph = section.AddParagraph();
                    if(tasks.Count != 0)
                    {
                        paragraph.AddFormattedText("Ukończone dzisiejsze zadania:", TextFormat.Bold);
                    }
                    paragraph.Format.Font.Size = 13.5;
                    paragraph.Format.SpaceAfter = 6;
                    foreach (TaskDoneModel task in tasks)
                    {
                        paragraph = section.AddParagraph($"{task.Task}:\n");
                        paragraph.Format.Font.Size = 13.5;
                        paragraph = section.AddParagraph($"\"{task.DateStart}\" - \"{task.DateEnd}\"\tCzas: {task.Time}");
                        paragraph.Format.Font.Size = 13.5;
                        paragraph.Format.LeftIndent = 6;
                        paragraph.Format.TabStops.Clear();
                        paragraph.Format.TabStops.AddTabStop((document.DefaultPageSetup.PageWidth - document.DefaultPageSetup.LeftMargin - document.DefaultPageSetup.RightMargin) / 4 * 3);

                    }

                    paragraph.Format.SpaceAfter = 24;
                    /*if(++recordsOnPage == 9)
                    {
                        section.AddPageBreak();
                        recordsOnPage = 0;
                    }*/
                    }
                    


                    PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
                    pdfRenderer.Document = document;
                    pdfRenderer.RenderDocument();


                    string filename = $"Report_{fileDate}.pdf";
                    string fullpath = $"{path}\\{filename}";
                    try
                    {
                        pdfRenderer.PdfDocument.Save(fullpath);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Nie można nadpisać raportu.\nPlik:\n{fullpath}\njest wykorzystywany przez inny proces");
                        return;
                    }
                    
                    Process.Start(fullpath);
                }));
            }
        }

        private ICommand _addCommand;

        public ICommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(x =>
                {
                    List<string> employeeList = new List<string>();

                    AdminDashboardViewModel.employeesList.Clear();

                    adminModel.GetEmployeesFullNamesandID(employeeList);

                    foreach (var employee in employeeList)
                        AdminDashboardViewModel.employeesList.Add(employee);

                    Employee.Queries = new ObservableCollection<string>(AdminDashboardViewModel.employeesList);

                    Employee.TwoWays = new ObservableCollection<string>(AdminDashboardViewModel.workItems);

                    OnAdd();
                }));
            }
        }

        private ICommand _reload;

        public ICommand Reload
        {
            get
            {
                return _reload ?? (_reload = new RelayCommand(x =>
                {
                    List<string> ids = new List<string>();

                    employees.Clear();

                    adminModel.GetEmployeesIDs(ids);

                    leftEmployeeNumber = 0;
                    rightEmployeeNumber = 0;

                    for (int i = 0; i < adminModel.GetEmployeeCount(); i++)
                    {
                        var temp = ids[i].Split(' ');
                        employees.Add(new EmployeeChartModel(ids[i], adminModel.GetEmployeeLogsCount(temp[0]), adminModel.GetEmployeeTasksCount(temp[0]), adminModel.GetEmployeeTasksInProgressCount(temp[0]), adminModel.GetEmployeeTasksDoneCount(temp[0])));
                    }

                    EmployeeNames.Clear();
                    EmployeeLogsCounts.Clear();
                    EmployeeTaskCounts.Clear();
                    EmployeeTasksInProgressCounts.Clear();
                    EmployeeTasksDoneCounts.Clear();

                    for (int i = 0; i < 4; i++)
                    {
                        if (i < employees.Count())
                        {
                            EmployeeLogsCounts.Add(employees[i].NumberofLogins);
                            EmployeeTaskCounts.Add(employees[i].NumberofTasks);
                            EmployeeTasksInProgressCounts.Add(employees[i].NumberofTasksInProgress);
                            EmployeeTasksDoneCounts.Add(employees[i].NumberofTasksDone);
                            EmployeeNames.Add(employees[i].Name);

                            if (rightEmployeeNumber < 4)
                                rightEmployeeNumber++;
                        }
                    }

                    CheckedBool = false;

                    Page = 1;

                }));
            }
        }

        private ICommand _nextOnClick;

        public ICommand NextOnClick
        {
            get
            {
                return _nextOnClick ?? (_nextOnClick = new RelayCommand(x =>
                {
                    if(rightEmployeeNumber == 0)
                        rightEmployeeNumber = EmployeeNames.Count();

                    if ((rightEmployeeNumber < adminModel.GetEmployeeCount()))
                    {
                        if(CheckedBool == false)
                        {
                            List<string> ids = new List<string>();
                            employees.Clear();

                            adminModel.GetEmployeesIDs(ids);

                            for (int i = 0; i < adminModel.GetEmployeeCount(); i++)
                            {
                                var temp = ids[i].Split(' ');
                                employees.Add(new EmployeeChartModel(ids[i], adminModel.GetEmployeeLogsCount(temp[0]), adminModel.GetEmployeeTasksCount(temp[0]), adminModel.GetEmployeeTasksInProgressCount(temp[0]), adminModel.GetEmployeeTasksDoneCount(temp[0])));
                            }

                            Page++;
                            EmployeeNames.Clear();
                            EmployeeLogsCounts.Clear();
                            EmployeeTaskCounts.Clear();
                            EmployeeTasksInProgressCounts.Clear();
                            EmployeeTasksDoneCounts.Clear();

                            for (int i = leftEmployeeNumber + 4; i < rightEmployeeNumber + 4; i++)
                            {
                                if (i < employees.Count())
                                {
                                    EmployeeLogsCounts.Add(employees[i].NumberofLogins);
                                    EmployeeTaskCounts.Add(employees[i].NumberofTasks);
                                    EmployeeTasksInProgressCounts.Add(employees[i].NumberofTasksInProgress);
                                    EmployeeTasksDoneCounts.Add(employees[i].NumberofTasksDone);
                                    EmployeeNames.Add(employees[i].Name);
                                }
                            }
                            leftEmployeeNumber += 4;
                            rightEmployeeNumber += 4;
                        }
                        else
                        {
                            List<string> ids = new List<string>();
                            employees.Clear();

                            adminModel.GetEmployeesIDs(ids);

                            for (int i = 0; i < adminModel.GetEmployeeCount(); i++)
                            {
                                var temp = ids[i].Split(' ');
                                employees.Add(new EmployeeChartModel(ids[i], adminModel.GetEmployeeLogsCountToday(temp[0]), adminModel.GetEmployeeTasksCount(temp[0]), adminModel.GetEmployeeTasksInProgressCount(temp[0]), adminModel.GetEmployeeTasksDoneCountToday(temp[0])));
                            }

                            Page++;
                            EmployeeNames.Clear();
                            EmployeeLogsCounts.Clear();
                            EmployeeTaskCounts.Clear();
                            EmployeeTasksInProgressCounts.Clear();
                            EmployeeTasksDoneCounts.Clear();

                            for (int i = leftEmployeeNumber + 4; i < rightEmployeeNumber + 4; i++)
                            {
                                if (i < employees.Count())
                                {
                                    EmployeeLogsCounts.Add(employees[i].NumberofLogins);
                                    EmployeeTaskCounts.Add(employees[i].NumberofTasks);
                                    EmployeeTasksInProgressCounts.Add(employees[i].NumberofTasksInProgress);
                                    EmployeeTasksDoneCounts.Add(employees[i].NumberofTasksDone);
                                    EmployeeNames.Add(employees[i].Name);
                                }
                            }
                            leftEmployeeNumber += 4;
                            rightEmployeeNumber += 4;
                        }


                    }
                }));
            }
        }

        private ICommand _prevOnClick;

        public ICommand PrevOnClick
        {
            get
            {
                return _prevOnClick ?? (_prevOnClick = new RelayCommand(x =>
                {
                    if (leftEmployeeNumber != 0)
                    {
                        if (CheckedBool == false)
                        {
                            List<string> ids = new List<string>();
                            employees.Clear();

                            adminModel.GetEmployeesIDs(ids);

                            for (int i = 0; i < adminModel.GetEmployeeCount(); i++)
                            {
                                var temp = ids[i].Split(' ');
                                employees.Add(new EmployeeChartModel(ids[i], adminModel.GetEmployeeLogsCount(temp[0]), adminModel.GetEmployeeTasksCount(temp[0]), adminModel.GetEmployeeTasksInProgressCount(temp[0]), adminModel.GetEmployeeTasksDoneCount(temp[0])));
                            }

                            Page--;
                            EmployeeNames.Clear();
                            EmployeeLogsCounts.Clear();
                            EmployeeTaskCounts.Clear();
                            EmployeeTasksInProgressCounts.Clear();
                            EmployeeTasksDoneCounts.Clear();

                            for (int i = leftEmployeeNumber - 4; i < rightEmployeeNumber - 4; i++)
                            {
                                if (i < employees.Count())
                                {
                                    EmployeeLogsCounts.Add(employees[i].NumberofLogins);
                                    EmployeeTaskCounts.Add(employees[i].NumberofTasks);
                                    EmployeeTasksInProgressCounts.Add(employees[i].NumberofTasksInProgress);
                                    EmployeeTasksDoneCounts.Add(employees[i].NumberofTasksDone);
                                    EmployeeNames.Add(employees[i].Name);
                                }
                            }

                            leftEmployeeNumber -= 4;
                            rightEmployeeNumber -= 4;
                        }
                        else
                        {
                            List<string> ids = new List<string>();
                            employees.Clear();

                            adminModel.GetEmployeesIDs(ids);

                            for (int i = 0; i < adminModel.GetEmployeeCount(); i++)
                            {
                                var temp = ids[i].Split(' ');
                                employees.Add(new EmployeeChartModel(ids[i], adminModel.GetEmployeeLogsCountToday(temp[0]), adminModel.GetEmployeeTasksCount(temp[0]), adminModel.GetEmployeeTasksInProgressCount(temp[0]), adminModel.GetEmployeeTasksDoneCountToday(temp[0])));
                            }

                            Page--;
                            EmployeeNames.Clear();
                            EmployeeLogsCounts.Clear();
                            EmployeeTaskCounts.Clear();
                            EmployeeTasksInProgressCounts.Clear();
                            EmployeeTasksDoneCounts.Clear();

                            for (int i = leftEmployeeNumber - 4; i < rightEmployeeNumber - 4; i++)
                            {
                                if (i < employees.Count())
                                {
                                    EmployeeLogsCounts.Add(employees[i].NumberofLogins);
                                    EmployeeTaskCounts.Add(employees[i].NumberofTasks);
                                    EmployeeTasksInProgressCounts.Add(employees[i].NumberofTasksInProgress);
                                    EmployeeTasksDoneCounts.Add(employees[i].NumberofTasksDone);
                                    EmployeeNames.Add(employees[i].Name);
                                }
                            }

                            leftEmployeeNumber -= 4;
                            rightEmployeeNumber -= 4;
                        }
                            
                    }
                }));
            }
        }

        private ICommand _goToLogin;
        public ICommand GoToLogin
        {
            get
            {
                return _goToLogin ?? (_goToLogin = new RelayCommand(x =>
                {
                    Reload.Execute(null);
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
                    Reload.Execute(null);
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
                    Reload.Execute(null);
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

                }));
            }
        } 
        


        #endregion

        #region Methods

        public static event PropertyChangedEventHandler PropertyChanged1;

        public static void OnPropertyChanged1(string propertyName)
        {
            if (PropertyChanged1 != null)
            {
                PropertyChanged1(Instance, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async void OnAdd()
        {
            var item = new Employee();
            await DialogHost.Show(item, "AddDialogHost");
            if (IsDialogAddOpen == true)
            {
                if (item.Task == string.Empty)
                {
                    if (item.ID != string.Empty && item.Pin != string.Empty && item.Name != string.Empty && item.Surname != string.Empty)
                    {
                        if (!item.ID.All(char.IsDigit) && !item.Pin.All(char.IsDigit))
                        {
                            MessageBox.Show("Id i pin nie składają się tylko z cyfr!");
                        }
                        else if (!item.ID.All(char.IsDigit))
                        {
                            MessageBox.Show("Id nie składa się tylko z cyfr!");
                        }
                        else if (!item.Pin.All(char.IsDigit))
                        {
                            MessageBox.Show("Pin nie składa się tylko z cyfr!");
                        }
                        else if (item.Pin.Length != 4 && item.ID.Length != 4)
                        {
                            MessageBox.Show("Id oraz pin są za krótkie!");
                        }
                        else if (item.ID.Length != 4)
                        {
                            MessageBox.Show("Id jest za krótkie!");
                        }
                        else if (item.Pin.Length != 4)
                        {
                            MessageBox.Show("Pin jest za krótki!");
                        }
                        else if (!adminModel.EmployeeIDUsed(item.ID))
                        {
                            string shift = getCurrentItemEmployee();
                            adminModel.InsertEmployee(item.ID, item.Pin, item.Name, item.Surname, shift);

                            if (EmployeeNames.Count != 4)
                            {
                                var tempInt = EmployeeNames.Count;
                                EmployeeNames.Add($"{item.ID} {item.Name} {item.Surname}");
                                EmployeeLogsCounts.Add(0);
                                EmployeeTaskCounts.Add(0);
                                EmployeeTasksInProgressCounts.Add(0);
                                EmployeeTasksDoneCounts.Add(0);
                            }
                        }
                        else
                        {
                            MessageBox.Show("To id zostało już przypisane!");
                        }
                    }
                    else if (item.IDadmin != string.Empty && item.AdminUsername != string.Empty && item.AdminPassword != string.Empty && item.AdminName != string.Empty && item.AdminSurname != string.Empty)
                    {
                        if (!item.IDadmin.All(char.IsDigit))
                        {
                            MessageBox.Show("Id nie składa się tylko z cyfr!");
                        }
                        else if (item.IDadmin.Length != 4)
                        {
                            MessageBox.Show("Id jest za krótkie!");
                        }
                        else if (adminModel.AdminIDUsed(item.IDadmin) && adminModel.AdminUsernameUsed(item.AdminUsername))
                        {
                            MessageBox.Show("Id oraz nazwa użytkoniwka została już przypisana!");
                        }
                        else if (adminModel.AdminIDUsed(item.IDadmin))
                        {
                            MessageBox.Show("To id zostało już przypisane!");
                        }
                        else if (adminModel.AdminUsernameUsed(item.AdminUsername))
                        {
                            MessageBox.Show("Nazwa użytkownka została już użyta!");
                        }
                        else
                        {
                            IsDialogAddOpen = false;
                            await DialogHost.Show(item, "AddAdminDialogHost");
                            if (IsDialogAdminOpen == true)
                            {
                                if (item.PasswordConfirm == adminModel.CanAdminDeleteemployee(Username) && item.PasswordConfirm != string.Empty)
                                {
                                    adminModel.InsertAdmin(item.IDadmin, item.AdminUsername, item.AdminPassword, item.AdminName, item.AdminSurname);
                                }
                                else if (item.PasswordConfirm == string.Empty)
                                {
                                    MessageBox.Show("Nie wpisano hasła.");
                                }
                                else
                                {
                                    MessageBox.Show("Niepoprawne hasło, nie dodano administratora.");
                                }
                            }

                            IsDialogAdminOpen = false;
                            AdminEmployeesViewModel.IsDialogAdminOpen = false;
                            AdminDashboardViewModel.IsDialogAdminOpen = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Pozostawiono puste pola.");
                    }
                }
                else
                {
                    string temp = getCurrentListItem();
                    string[] words = temp.Split(' ');

                    if (!adminModel.SameTaskAdded(words[0], item.Task))
                    {
                        adminModel.InsertTask(words[0], item.Task);

                        if (EmployeeNames.Contains(temp))
                        {
                            for (int i = 0; i < EmployeeNames.Count; i++)
                            {
                                if (EmployeeNames[i] == temp)
                                {
                                    EmployeeTaskCounts[i]++;
                                }
                            }
                        }
                    }
                    else
                        MessageBox.Show("To zadanie jest juz przydzielone temu pracownikowi.");
                }
            }
            IsDialogAddOpen = false;
            AdminEmployeesViewModel.IsDialogAddOpen = false;
            AdminDashboardViewModel.IsDialogAddOpen = false;
        }


        public string getCurrentListItem()
        {
            var currentQuery = (string)Employee.SelectedEmployee;
            return currentQuery;
        }

        public string getCurrentItemEmployee()
        {
            var currentQuery = (string)Employee.SelectedShift;
            return currentQuery;
        }
        #endregion 
    }
}
