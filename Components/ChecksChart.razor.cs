using Afterpelago.Models;
using Afterpelago.Serializers;
using Afterpelago.Services;
using BlazorWorker.BackgroundServiceFactory;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Afterpelago.Components
{
    public partial class ChecksChart
    {
        [Parameter]
        public required CheckObtainedLogEntry[] Checks { get; set; }

        public Dictionary<int, string> tooltipMap { get; set; }

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

        private List<TimeSeriesChartSeries> _series = new();

        private readonly Random _random = new Random();

        private bool _roundedLabelSpacing = false;
        private bool _roundedLabelSpacingPadSeries = false;

        private string _width = "100%";
        private string _height = "100%";

        /// <summary>
        /// When the Component Mounts, build the Chart Data
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            // Create the WebWorker for Async (I wish we had WASM Threads working...)
            var worker = await workerFactory.CreateAsync();

            // Create the service reference for the WebWorker
            var service = await worker.CreateBackgroundServiceAsync<ChartBuilderWebWorkerService>(options => options.UseCustomExpressionSerializer(typeof(CustomCheckSerializer)));

            // Build the chart series (and make a local reference to get around some scope shenanigans...)
            var scopeSafeChecks = this.Checks;
            var result = await service.RunAsync(s => s.BuildChartDataFromChecks(scopeSafeChecks));

            // If this worked, store it locally and force a refresh
            if(result != null)
            {
                _series.Add(new TimeSeriesChartSeries
                {
                    Index = 0,
                    Name = "All Checks Obtained",
                    Data = result,
                    IsVisible = true,
                    LineDisplayType = LineDisplayType.Line,
                    DataMarkerTooltipTitleFormat = $"hey",
                    DataMarkerTooltipSubtitleFormat = "{{X_VALUE}}"
                });
                HasLoaded = true;
                StateHasChanged();
            }

            // Dispose of the WW
            await service.DisposeAsync();
            await worker.DisposeAsync();
        }
    }
}
