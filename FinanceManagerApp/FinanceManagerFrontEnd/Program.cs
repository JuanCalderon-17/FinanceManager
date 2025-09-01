using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://localhost:7133"); // Asegúrate de que este puerto sea el de tu API
});

// Le decimos al sistema que use autenticación de Cookies.
// Este es el sistema de "pase de visitante" nativo de ASP.NET.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Si un guardia detiene a un usuario sin pase, lo envía a esta dirección.
        options.LoginPath = "/Account/Login";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Contratamos a los guardias en el orden correcto.
app.UseAuthentication(); // Primero, el guardia revisa la identidad.
app.UseAuthorization();  // Segundo, el guardia revisa los permisos.

app.MapRazorPages();

app.Run();
