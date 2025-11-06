using Afterpelago.Services;
using ApexCharts;
using BlazorWorker.Core;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using static System.Net.WebRequestMethods;

namespace Afterpelago
{
    public class Program
    {
        public static string BlobEndpoint = @"https://afterpelagodata.blob.core.windows.net/web/";
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMudServices();

            builder.Services.AddWorkerFactory();

            builder.Services.AddApexCharts();

            builder.Services.AddSingleton<SearchService>();

            await builder.Build().RunAsync();
        }
    }
}
