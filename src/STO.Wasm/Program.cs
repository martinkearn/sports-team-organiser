global using STO.Models;
global using STO.Wasm.Services;
global using STO.Wasm.Models;
global using STO.Wasm.Interfaces;
global using Blazored.LocalStorage;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace STO.Wasm;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Httpclient
        builder.Services.AddHttpClient();

        // Refer to https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/standalone-with-azure-active-directory-b2c?view=aspnetcore-8.0
        builder.Services.AddMsalAuthentication(options =>
        {
            builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
            options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
            options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
            options.ProviderOptions.LoginMode = "redirect";
        });

        // Add custom auth policy
        _ = builder.Services.AddAuthorizationCore(config =>
            {
                config.AddPolicy("IsAdminEmail", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var user = context.User;
                        if (user.Identity is not null && user.Identity.IsAuthenticated)
                        {
                            // User is authenticated
                            var email = user.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
                            var authName = user.Identity.Name;
                            if (authName == "Martin Kearn")
                            {
                                return true;
                            }
                        }

                        return false;
                    })
                );
            });

        // Add custom app settings
        builder.Services.AddOptions<ApiConfiguration>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(nameof(ApiConfiguration)).Bind(settings);
            });

        // Add custom services
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IDataService, DataService>();
		builder.Services.AddSingleton<ICachedDataService, CachedDataService>();
		builder.Services.AddSingleton<IPlayerService, PlayerService>();
		builder.Services.AddSingleton<IPlayerEntityService, PlayerEntityService>();
		builder.Services.AddSingleton<IGameService, GameService>();
        builder.Services.AddSingleton<ITransactionService, TransactionService>();
		builder.Services.AddSingleton<ITransactionEntityService, TransactionEntityService>();
		builder.Services.AddSingleton<IRatingService, RatingService>();

        // Add Blazored.LocalStorage
        builder.Services.AddBlazoredLocalStorageAsSingleton();

        builder.Services.AddCascadingAuthenticationState();

        Console.WriteLine($"Client Hosting Environment: {builder.HostEnvironment.Environment}");

        var host = builder.Build();

        // Initialise data
        //var dataService = host.Services.GetRequiredService<IDataService>();
        //await dataService.LoadData();

		var cachedDataService = host.Services.GetRequiredService<ICachedDataService>();
		await cachedDataService.LoadData(false);

		//await builder.Build().RunAsync();
		await host.RunAsync();
    }
}
