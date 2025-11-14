using Afterpelago.Models;
using ApexCharts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Afterpelago.Pages.Report
{
    public partial class PlayerStats
    {
        private ApexChart<BasicStat> checkChart;

        private Slot CurrentPlayer { get; set; }

        private ApexChartOptions<BasicStat> checkPercentOptions { get; set; } = new();

        public PlayerStats()
        {
            CurrentPlayer = Archipelago.Slots.Values.First();
        }

        protected override void OnInitialized()
        {
            checkPercentOptions.PlotOptions = new PlotOptions
            {
                Pie = new PlotOptionsPie
                {
                    Donut = new PlotOptionsDonut
                    {
                        Labels = new DonutLabels
                        {
                            Total = new DonutLabelTotal 
                            { 
                                FontSize = "16px", 
                                Color = "#D807B8",
                            }
                        }
                    }
                }
            };
            checkPercentOptions.Legend = new Legend
            {
                Position = LegendPosition.Bottom
            };
        }

        private async Task OnPlayerChanged(Slot newPlayer)
        {
            CurrentPlayer = newPlayer;
            // Add any additional logic you want to run when the player changes
            await InvokeAsync(StateHasChanged);
            await checkChart.RenderAsync();
        }
    }
}
