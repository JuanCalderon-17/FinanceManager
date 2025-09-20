using System.Text;
using FinanceManager.API.Data;
using FinanceManager.API.Interfaces;
using FinanceManager.API.Models;
using FinanceManager.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// --- INICIO DE NUESTRO C�DIGO DE CONFIGURACI�N ---

// 1. Configuraci�n de la Base de Datos
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 2. Configuraci�n de ASP.NET Core Identity
builder.Services.AddIdentityCore<AppUser>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager<SignInManager<AppUser>>();

// 3. Configuraci�n de la Autenticaci�n con JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

// 4. Registro de nuestro servicio de tokens
builder.Services.AddScoped<ITokenService, TokenService>();


// 5. Le damos permiso a nuestra app de Angular para que hable con la API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // La direcci�n de nuestra app de Angular
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


// 6. Configuraci�n de Swagger para que entienda JWT
builder.Services.AddSwaggerGen(options =>
{
    // Definimos el esquema de seguridad "Bearer"
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // A�adimos el requisito de que se puede usar este esquema de seguridad
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});






// --- FIN DE NUESTRO C�DIGO DE CONFIGURACI�N ---


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware de Autenticaci�n y Autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();