using Afterpelago.Models;
using Afterpelago.Serializers;
using Afterpelago.Services;
using ApexCharts;
using BlazorWorker.BackgroundServiceFactory;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static MudBlazor.Icons;

namespace Afterpelago.Components
{
    public partial class ChecksChart
    {
        [Parameter]
        public required CheckObtainedLogEntry[] Checks { get; set; }

        private List<Check> Data { get; set; } = new();

        public bool HasLoaded { get; private set; } = true;

        public ApexChartOptions<Check> ChartOptions = new ApexChartOptions<CheckObtainedLogEntry>
        {
            Chart = new Chart
            {
                Type = ApexCharts.ChartType.Line,
                Height = 350
            },
            Xaxis = new XAxis
            {
                Type = XAxisType.Datetime,
                Title = new AxisTitle
                {
                    Text = "Time Obtained"
                }
            }
        };

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Last value is invalid, usually (@todo fix this? it's faster than where though)
            Data = Checks.SkipLast(1).ToList();
        }

        public MarkupString ShowTooltip(Check check)
        {
            return (MarkupString)$"<b>{check.SenderName}</b> found <b>{check.ItemName}</b> for <b>{check.ReceiverName}</b><br/>{check.LocationName}\n<br/>{check.Timestamp.ToShortDateString()} {check.Timestamp.ToShortTimeString()}";
        }

        /// <summary>
        /// When the Component Mounts, build the Chart Data
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            //// Create the WebWorker for Async (I wish we had WASM Threads working...)
            //var worker = await workerFactory.CreateAsync();

            //// Create the service reference for the WebWorker
            //var service = await worker.CreateBackgroundServiceAsync<ChartBuilderWebWorkerService>(options => options.UseCustomExpressionSerializer(typeof(CustomCheckSerializer)));

            //// Build the chart series (and make a local reference to get around some scope shenanigans...)
            //var scopeSafeChecks = this.Checks;
            //var result = await service.RunAsync(s => s.BuildChartDataFromChecks(scopeSafeChecks));

            //// If this worked, store it locally and force a refresh
            //if(result != null)
            //{
            //    Data = result;
            //    HasLoaded = true;
            //    StateHasChanged();
            //}

            // Dispose of the WW
            //await service.DisposeAsync();
            //await worker.DisposeAsync();
        }
    }
}
