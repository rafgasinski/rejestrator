using LiveCharts.Defaults;

namespace rejestrator.Models
{
    public class EmployeeChartModel
    {
        public string Name { get; set; }
        public int NumberofLogins { get; set; }
        public int NumberofTasks { get; set; }
        public int NumberofTasksInProgress { get; set; }
        public int NumberofTasksDone { get; set; }

        public EmployeeChartModel(string name, int logs, int tasks, int progress, int dones)
        {
            Name = name;
            NumberofLogins = logs;
            NumberofTasks = tasks;
            NumberofTasksInProgress = progress;
            NumberofTasksDone = dones;
        }
    }
}
