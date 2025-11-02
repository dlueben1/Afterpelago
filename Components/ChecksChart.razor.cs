using Afterpelago.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Afterpelago.Components
{
    public partial class ChecksChart
    {
        [Parameter]
        public required CheckObtainedLogEntry[] Checks { get; set; }

        public bool HasLoaded { get; private set; } = false;

        private int _index = -1; //default value cannot be 0 -> first selectedindex is 0.
        private ChartOptions _options = new ChartOptions
        {
            YAxisLines = false,
            YAxisTicks = 500,
            MaxNumYAxisTicks = 10,
            YAxisRequireZeroPoint = true,
            XAxisLines = false,
            LineStrokeWidth = 1,
        };

        private AxisChartOptions _axisChartOptions = new() { };

        private TimeSeriesChartSeries dataAsSeries = new();

        private List<TimeSeriesChartSeries> _series = new();

        private readonly Random _random = new Random();

        private bool _roundedLabelSpacing = false;
        private bool _roundedLabelSpacingPadSeries = false;

        private string _width = "650px";
        private string _height = "350px";

        /// <summary>
        /// When the Component Mounts, build the Chart Data
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            dataAsSeries = new TimeSeriesChartSeries
            {
                Index = 0,
                Name = "All Checks Obtained",
                Data = Checks.Select((check, index) => new TimeSeriesChartSeries.TimeValue(check.Timestamp, index)).ToList(),
                IsVisible = true,
                LineDisplayType = LineDisplayType.Line,
                DataMarkerTooltipTitleFormat = "{{X_VALUE}}",
                DataMarkerTooltipSubtitleFormat = "{{Y_VALUE}}"
            };

            _series.Add(dataAsSeries);

            StateHasChanged();
        }
    }
}
