using Afterpelago.Models;
using Afterpelago.Serializers;
using Afterpelago.Services;
using ApexCharts;
using BlazorWorker.BackgroundServiceFactory;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using static MudBlazor.Icons;

namespace Afterpelago.Components
{
    public partial class ChecksChart
    {
        [Parameter]
        public required CheckObtainedLogEntry[] Checks { get; set; }

        private ApexChart<Check> ChartRef;

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
            },
            Annotations = new Annotations
            {
                Images = new List<AnnotationsImage>
                {
                    new AnnotationsImage
                    {
                        Path = "https://afterpelagodata.blob.core.windows.net/web/banjotooie/images/items/jiggy.webp",
                        Width = 12,
                        Height = 12,
                        X = 200,
                        Y = 200
                    }
                }
            }
        };

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Last value is invalid, usually (@todo fix this? it's faster than Where(<dt valid>) though)
            var checks = Checks.SkipLast(1).ToList();

            // Store Data
            Data = checks;
        }

        protected async Task OnRender()
        {
            //// Get the data
            //if(ChartRef != null)
            //{
            //    if(ChartRef.Series.Count > 0)
            //    {
            //        var points = ChartRef.Series[0].GenerateDataPoints(Data);
            //        foreach(var point in points)
            //        {
            //            ChartOptions.Annotations.Images.Add(new AnnotationsImage
            //            {
            //                Path = "https://afterpelagodata.blob.core.windows.net/web/banjotooie/images/items/jiggy.webp",
            //                Width = 12,
            //                Height = 12,
            //                X = 200,
            //                Y = 200
            //            })
            //        }
            //    }
            //}
        }

        public MarkupString ShowTooltip(Check check)
        {
            return (MarkupString)$"<b>{check.SenderName}</b> found <b>{check.ItemName}</b> for <b>{check.ReceiverName}</b><br/>{$"(Found at {check.LocationName})"}\n<br/>{check.Timestamp.ToShortDateString()} {check.Timestamp.ToShortTimeString()}";
        }
    }
}
