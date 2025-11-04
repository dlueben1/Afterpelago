using Afterpelago.Components;
using Afterpelago.Models;
using MudBlazor;

namespace Afterpelago.Services
{
    /// <summary>
    /// To use WebWorkers, we need to store CPU-intensive functions in a separate service class.
    /// This service class is for crunching lots of numbers
    /// </summary>
    public class ChartBuilderWebWorkerService
    {
        //public List<ChecksChart.CheckChartData> BuildChartDataFromChecks(CheckObtainedLogEntry[] checks)
        //{
        //    return checks.Select((check, index) => new ChecksChart.CheckChartData(check.Timestamp, index, "Test")).ToList();
        //} 
    }
}
