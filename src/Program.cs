global using STO.Services;
global using STO.Interfaces;
global using STO.Models;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Authentication.OpenIdConnect;
global using Microsoft.Identity.Web;
global using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authorization;
using STO.Policies;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

/* builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
}); */
builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("IsAdminEmail", policy => policy.Requirements.Add(new IsAdminEmailRequirement("martinkearn@live.co.uk")));
});

builder.Services.AddSingleton<IAuthorizationHandler, IsAdminEmailHandler>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
    options.DisconnectedCircuitMaxRetained = 100;
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5);
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
    options.MaxBufferedUnacknowledgedRenderBatches = 10;
})
    .AddMicrosoftIdentityConsentHandler();
builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddOptions<StorageConfiguration>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection(nameof(StorageConfiguration)).Bind(settings);
    });
var configuration = builder.Configuration;

builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
        // Handling SameSite cookie according to https://learn.microsoft.com/aspnet/core/security/samesite?view=aspnetcore-3.1
        options.HandleSameSiteCookieCompatibility();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}");
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Redirect default built-in signed out page to root
app.UseRewriter(new RewriteOptions().Add(
    context =>
    {
        if (context.HttpContext.Request.Path == "/MicrosoftIdentity/Account/SignedOut")
        {
            context.HttpContext.Response.Redirect("/");
        }
    }));

app.Run();
