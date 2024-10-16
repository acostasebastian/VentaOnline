using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VentaOnline.Data;
using VentaOnline.DataAccess.Data.Repository.IRepository;
using VentaOnline.DataAccess.Data.Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("String de Conexión 'ConexionSQL' no encontrada.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

//Agregar contenedor de trabajo al contenedor IoC de inyección de dependencias
builder.Services.AddScoped<IContenedorTrabajo, ContenedorTrabajo>();


//AGREGAR EN EL PAQUETE NUGGET Serilog.Extensions.Logging Y Serilog.Sinks.File PARA PODER USARLO
//Injectamos el logger para poder guardar en un archivo el log
Log.Logger = new LoggerConfiguration().WriteTo.File("logs/Logs.txt", rollingInterval: RollingInterval.Day).CreateLogger();
//borra otros proveedores de loggers si es que los hay
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Cliente}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
