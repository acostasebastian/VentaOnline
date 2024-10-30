using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VentaOnline.Data;
using VentaOnline.DataAccess.Data.Repository.IRepository;
using VentaOnline.DataAccess.Data.Repository;
using Serilog;
using VentaOnline.Models;
using VentaOnline.DataAccess.Data.Initialiser;
using VentaOnline.DataAccess.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("String de Conexión 'ConexionSQL' no encontrada.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Se corrige esta linea al empezar a usar roles >> builder.Services.AddDefaultIdentity<ApplicationUser> y se agrega .AddDefaultUI()
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI().AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();

//Agregar contenedor de trabajo al contenedor IoC de inyección de dependencias
builder.Services.AddScoped<IContenedorTrabajo, ContenedorTrabajo>();

//Siembra de datos - Paso 1
builder.Services.AddScoped<IInicializadorBD, InicializadorBD>();


//AGREGAR EN EL PAQUETE NUGGET Serilog.Extensions.Logging Y Serilog.Sinks.File PARA PODER USARLO
//Injectamos el logger para poder guardar en un archivo el log
Log.Logger = new LoggerConfiguration().WriteTo.File("logs/Logs.txt", rollingInterval: RollingInterval.Day).CreateLogger();
//borra otros proveedores de loggers si es que los hay
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

//CARRITO

builder.Services.AddSession(); // Habilitar sesión
builder.Services.AddScoped<ShoppingCart>(); // Añadir ShoppingCart como un servicio

//CARACTERISTICAS DE LA CONTRASEÑA
//builder.Services.Configure<IdentityOptions>(options =>
//{
//    // Default Password settings.
//    options.Password.RequireDigit = true;
//    options.Password.RequireLowercase = true;
//    options.Password.RequireNonAlphanumeric = true;
//    options.Password.RequireUppercase = true;
//    options.Password.RequiredLength = 6;
//    options.Password.RequiredUniqueChars = 1;
//});


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

//Siembra de datos - Paso 2 Metodo que ejecuta la siembra de datos
SiembraDatos();


//CARRITO
app.UseSession(); // Usar sesión

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Cliente}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();


//Funcionalidad método SiembraDeDatos();
void SiembraDatos()
{
    using (var scope = app.Services.CreateScope())
    {
        var inicializadorBD = scope.ServiceProvider.GetRequiredService<IInicializadorBD>();
        inicializadorBD.Inicializar();
    }
}