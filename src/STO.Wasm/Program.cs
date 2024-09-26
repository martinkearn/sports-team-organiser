using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


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
            if (user.Identity is null || !user.Identity.IsAuthenticated) return false;
            // User is authenticated
            var authName = user.Identity.Name;
            return authName == "Martin Kearn";
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
builder.Services.AddSingleton<IPlayerEntityService, PlayerEntityService>();
builder.Services.AddSingleton<IGameEntityService, GameEntityService>();
builder.Services.AddSingleton<ITransactionEntityService, TransactionEntityService>();
builder.Services.AddSingleton<IRatingEntityService, RatingEntityService>();

// Add Blazored.LocalStorage
builder.Services.AddBlazoredLocalStorageAsSingleton();

// Add Auth
builder.Services.AddCascadingAuthenticationState();

// Bypass Auth for local host if the "?bypassauth" query string is present
if (builder.HostEnvironment.Environment.Equals("LocalhostBypassAuth", StringComparison.CurrentCultureIgnoreCase))
    builder.Services.AddSingleton<IAuthorizationHandler, BypassAuthService>();

// Build
var host = builder.Build();

// Initialise data
var cachedDataService = host.Services.GetRequiredService<IDataService>();
await cachedDataService.LoadDataAsync(false, false);
        
// Run app
await host.RunAsync();