global using STO.Services;
global using STO.Interfaces;
global using STO.Models;
global using STO.Policies;
global using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Authorization;
using STO;

var builder = WebApplication.CreateBuilder(args);

// Add dotnet services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddAuthorization(config =>
    {
        config.AddPolicy("IsAdminEmail", policy => policy.Requirements.Add(new IsAdminEmailRequirement("martinkearn@live.co.uk")));
    });
builder.Services.AddSingleton<IAuthorizationHandler, IsAdminEmailHandler>();
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add custom services to the container
builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<IRatingService, RatingService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddOptions<StorageConfiguration>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection(nameof(StorageConfiguration)).Bind(settings);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

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
