using static CustomCADSolutions.Infrastructure.Data.DataConstants.RoleConstants;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Database, Identity and MVC
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddApplicationIdentity();
builder.Services.AddInbuiltServices();

// Localizer
System.Globalization.CultureInfo[] cultures = [new("en-US"), new("bg-BG")];
builder.Services.AddLocalizer(cultures);

// Roles, Stripe and SignalR
string[] roles = [Admin, Designer, Contributor, Client];
builder.Services.AddRoles(roles);
builder.Services.AddStripe(builder.Configuration);
builder.Services.AddSignalRAndHubs();

// Abstractions and App Cookie
builder.Services.AddAbstractions();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Home/Unauthorized";
});

WebApplication app = builder.Build();

app.UseLocalizion("en-US", cultures);
if (app.Environment.IsProduction())
{
    app.UseHsts();
}
app.UseAnimalErrors("/Error/StatusCodeHandler");

app.UseHttpsRedirection();
app.UseStaticFilesWithGltf();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
await app.Services.UseRolesAsync(roles);

Dictionary<string, string> users = new()
{
    ["Administrator"] = "NinjataBG",
    ["Designer"] = "Designer",
    ["Contributor"] = "Contributor",
    ["Client"] = "Client",
};
await app.Services.UseAppUsers(app.Configuration, users);

app.MapRoutes();
await app.RunAsync();