using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Zonosis.Web;
using Zonosis.Web.Repositories;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var uriBack = "https://localhost:7084/";
//builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(uriBack) });
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(uriBack) });
builder.Services.AddScoped<IRepository, Repository>();

await builder.Build().RunAsync();
