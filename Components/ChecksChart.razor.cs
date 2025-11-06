using Afterpelago.Models;
using Afterpelago.Serializers;
using Afterpelago.Services;
using Afterpelago.Utilities;
using ApexCharts;
using BlazorWorker.BackgroundServiceFactory;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Linq;
using System.Threading.Tasks;
using static MudBlazor.Icons;

namespace Afterpelago.Components
{
    public partial class ChecksChart
    {
        private ApexChart<Check> ChartRef;

        private List<Check> Data { get; set; } = Archipelago.Checks.SkipLast(1).ToList();

        public bool HasLoaded { get; private set; } = true;

        public ApexChartOptions<Check> ChartOptions = new ApexChartOptions<CheckObtainedLogEntry>
        {
            Chart = new Chart
            {
                Type = ApexCharts.ChartType.Line,
                Height = 350,
                Toolbar = new Toolbar
                {
                    Tools = new Tools
                    {
                        Reset = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24""><path fill=""currentColor"" d=""M12 20q-3.35 0-5.675-2.325T4 12t2.325-5.675T12 4q1.725 0 3.3.712T18 6.75V4h2v7h-7V9h4.2q-.8-1.4-2.187-2.2T12 6Q9.5 6 7.75 7.75T6 12t1.75 4.25T12 18q1.925 0 3.475-1.1T17.65 14h2.1q-.7 2.65-2.85 4.325T12 20""/></svg>"
                    }
                },
                Zoom = new Zoom
                {
                    Enabled = true,
                    AutoScaleYaxis = true
                },
            },
            Xaxis = new XAxis
            {
                Type = XAxisType.Datetime,
                Title = new AxisTitle
                {
                    Text = "Time Obtained"
                }
            },
        };

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Build "Release" Annotations
            List<AnnotationsXAxis> releaseAnnotations = new List<AnnotationsXAxis>();
            for (int i = 0; i < Archipelago.Releases.Length; i++)
            {
                var release = Archipelago.Releases[i];
                releaseAnnotations.Add(new AnnotationsXAxis
                {
                    X = release.Timestamp.ToUnixTimeMilliseconds(),
                    BorderColor = "yellow",
                    Label = new Label
                    {
                        BorderColor = "yellow",
                        Style = new Style
                        {
                            Background = "yellow"
                        },
                        Text = $"{release.SlotName} Cleared!"
                    }
                });
            }

            // Apply Annotations
            var checkPoints = new List<AnnotationsPoint>();
            Archipelago.Checks.SkipLast(1).ToList().ForEach(check =>
            {
                // Is this an item with an image?
                var player = check.Receiver;
                if(player != null && player.Game.Items.ContainsKey(check.ItemName))
                {
                    var item = player.Game.Items[check.ItemName];
                    var annotation = new AnnotationsPoint
                    {
                        X = check.Timestamp.ToUnixTimeMilliseconds(),//(new DateTime(check.Timestamp.Ticks).AddHours(3)).ToUnixTimeMilliseconds(),
                        Y = check.ObtainedOrder
                    };
                    if(item.ImageEndpoint != null)
                    {
                        annotation.Image = new AnnotationsPointImage
                        {
                            Path = player.Game.Items[check.ItemName].ImageEndpoint,
                            Width = 32,
                            Height = 32
                        };
                    }
                    else
                    {
                        annotation.Marker = new AnnotationMarker
                        {
                            Size = 6,
                            FillColor = "blue",
                            StrokeColor = "navy",
                            Shape = AnnotationMarkerShape.Circle
                        };
                    }
                    checkPoints.Add(annotation);
                }
            });
            ChartOptions.Annotations = new Annotations
            {
                Xaxis = releaseAnnotations,
                Points = checkPoints
            };
        }

        public MarkupString ShowTooltip(Check check)
        {
            return (MarkupString)$"<b>{check.SenderName}</b> found <b>{check.ItemName}</b> for <b>{check.ReceiverName}</b><br/>{$"(Found at {check.LocationName})"}\n<br/>{check.Timestamp.ToShortDateString()} {check.Timestamp.ToShortTimeString()}";
        }
    }
}
