using Microsoft.AspNetCore.Localization;
using System.Globalization;
using final_project.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("es-CO");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Configurar Npgsql para manejar DateTime sin zona horaria
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar DbContext con PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

var supportedCultures = new[] { cultureInfo };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultureInfo),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Crear la base de datos automáticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        // Esto crea la BD y todas las tablas basadas en tu DbContext
        context.Database.EnsureCreated();
        Console.WriteLine("✓ Conexión exitosa a PostgreSQL");
        Console.WriteLine("✓ Base de datos 'final_project' lista");
    }
    catch (Npgsql.PostgresException ex)
    {
        Console.WriteLine($"✗ Error de PostgreSQL: {ex.Message}");
        Console.WriteLine($"   Verifica que PostgreSQL esté corriendo y las credenciales sean correctas");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Error al conectar con la base de datos: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();