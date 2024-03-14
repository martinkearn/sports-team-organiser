global using STO.Models;
global using STO.Models.Interfaces;
global using STO.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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
                            var email = context?.User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
                            var authName = context?.User?.Identity?.Name;
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
        builder.Services.AddOptions<StorageConfiguration>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(nameof(StorageConfiguration)).Bind(settings);
            });

        // Add custom services
        builder.Services.AddSingleton<IStorageService, ApiStorageService>();
        builder.Services.AddSingleton<IPlayerService, PlayerService>();
        builder.Services.AddSingleton<IGameService, GameService>();
        builder.Services.AddSingleton<ITransactionService, TransactionService>();
        builder.Services.AddSingleton<IRatingService, RatingService>();

        builder.Services.AddCascadingAuthenticationState();

        Console.WriteLine($"Client Hosting Environment: {builder.HostEnvironment.Environment}");

        await builder.Build().RunAsync();
    }
}
