using System.Text;
using FunkoRP.Infraestructures;
using Serilog;

Log.Logger= SerilogConfig.Configure().CreateLogger();
Console.OutputEncoding = Encoding.UTF8; 
var builder = WebApplication.CreateBuilder(args);
//configuracion log
builder.Host.UseSerilog();

// Add services to the container.cd
builder.Services.AddRazorPages();
builder.Services.AddDatabase();
// repositorios
builder.Services.AddRepositories();
// servicios
builder.Services.AddServices();
builder.Services.AddStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
app.InitializeDatabaseAsync();
app.InitializeStorage();
app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();
app.UseStatusCodePagesWithReExecute("/NotFound", "?code={0}");
app.Run();