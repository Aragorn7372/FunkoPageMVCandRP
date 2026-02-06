using System.Text;
using CommonServices.Database;
using CommonServices.model;
using FunkoRP.Infraestructures;
using Microsoft.AspNetCore.Identity;
using Serilog;

Log.Logger= SerilogConfig.Configure().CreateLogger();
Console.OutputEncoding = Encoding.UTF8; 
var builder = WebApplication.CreateBuilder(args);
//configuracion log
builder.Host.UseSerilog();

// Add services to the container.cd
builder.Services.AddRazorPages();
builder.Services.AddDatabase();

// Configurar Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 4;
    })
    .AddEntityFrameworkStores<FunkoDbContext>()
    .AddDefaultTokenProviders();
// configuro la cookie de verga
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
    options.LogoutPath = "/Logout";
});
// repositorios
builder.Services.AddRepositories();
// servicios
builder.Services.AddServices();
builder.Services.AddStorage();

var app = builder.Build();

await app.SeedIdentityAsync();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/NotFound", "?code={404}");
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.InitializeDatabaseAsync();
app.InitializeStorage();
app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();