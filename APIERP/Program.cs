using APIERP;
using APIERP.Endpoints;
using APIERP.Servicios;
using ERP.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Inicio de área de los servicios
builder.Services
    .AddDbContext<ApplicationDbContext>(opciones =>
        opciones.UseSqlServer("name=DefaultConnection")
    )
    .AddCustomOpenApi()
    .AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>()
    .AddHttpContextAccessor()
    .AddAutoMapper(typeof(Program));
// Fin de área de los servicios

var app = builder.Build();

// Inicio de área de los middleware
app.DatabaseInit();
app.ConfigureSwagger();
app.UseExceptionHandler(); // Ocultar detalles de los errores al usuario y log

app.UseStatusCodePages();
app.UseStaticFiles();

app.MapGroup("/generate-data").AddDataToDB();
app.MapGroup("/categories").MapCategories();
app.MapGroup("/products").MapProducts();
app.MapGroup("/orders").MapOrders();
app.MapGroup("/carts").MapCartItems();
//app.MapGroup("/stores").MapStores();
//app.MapGroup("/rainchecks").MapRainChecks();

// Fin de área de los middleware

app.Run();