global using STO.Server;
global using STO.Server.Policies;
global using STO.Server.Components;
global using STO.Server.Services;
global using STO.Models;
global using STO.Models.Interfaces;
global using STO.Services;
global using STO.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.Circuits;

//Provided by template
var builder = WebApplication.CreateBuilder(args);

//Reads the app settings for AzureAdB2C and registers an authentication service
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));

//AddMicrosoftIdentityUI adds the razor pages used for account management, login etc
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

//Adds CascadingAuthenticationState to service collection to avoid having it in razor.
//CascadingAuthenticationState exposes authentication data to razor pages and enables things like AuthorizeView
builder.Services.AddCascadingAuthenticationState();

//Required for Microsoft.Identity.Web.UI
//Based on https://github.com/dotnet/aspnetcore/issues/52245
builder.Services.AddRazorPages();

//Provided by template
// Add services to the container.
// Timeout settings from: https://stackoverflow.com/questions/77425476/how-to-prevent-a-blazor-server-webapp-from-needing-refresh-after-device-standby
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => {
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(30);
    })
    .AddInteractiveWebAssemblyComponents();

// Add custom auth policy
builder.Services.AddAuthorization(config =>
    {
        config.AddPolicy("IsAdminEmail", policy => policy.Requirements.Add(new IsAdminEmailRequirement("martinkearn@live.co.uk")));
    });
builder.Services.AddSingleton<IAuthorizationHandler, IsAdminEmailHandler>();

// Httpclient
builder.Services.AddHttpClient();

// Add custom services to the container
builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddSingleton<IApiStorageService, ApiStorageService>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<IRatingService, RatingService>();
builder.Services.AddOptions<StorageConfiguration>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection(nameof(StorageConfiguration)).Bind(settings);
    });

// Blazor circuitpersisster from https://github.com/SteveSandersonMS/CircuitPersisterExample
builder.Services.AddScoped<PersistingCircuitHandler>();
builder.Services.AddSingleton<CircuitStateStore>();
builder.Services.AddScoped<CircuitHandler>(s => s.GetRequiredService<PersistingCircuitHandler>());

//Provided by template
var app = builder.Build();

//Provided by template
//Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Add Antiforgery Middleware to the request processing pipeline after the call to app.UseRouting. If there are calls to app.UseRouting and app.UseEndpoints, the call to app.UseAntiforgery must go between them. A call to app.UseAntiforgery must be placed after calls to app.UseAuthentication and app.UseAuthorization.
//Based on https://learn.microsoft.com/en-us/aspnet/core/migration/70-80?view=aspnetcore-8.0&tabs=visual-studio
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.UseAntiforgery();

//Needed for authentication provider routing
app.MapControllers();

//Provided by template
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

//Provided by template
app.Run();
