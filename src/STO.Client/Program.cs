global using STO.Models;
global using STO.Models.Interfaces;
global using STO.Services;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();

await builder.Build().RunAsync();
