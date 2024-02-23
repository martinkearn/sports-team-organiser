global using STO.Models;
global using STO.Models.Interfaces;
global using STO.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using STO.WasmPwa;
using Microsoft.Extensions.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();

await builder.Build().RunAsync();
