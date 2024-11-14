using AuhtService;
using AuhtService.Interfaces;
using AuthData;
using AuthData.Interfaces;
using AuthData.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Crear una instancia del constructor de la aplicación
var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
// Agrega servicios al contenedor de servicios de la aplicación
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITasksService, TasksService>();
builder.Services.AddScoped<ITaskInformation, TaskInformation>();

// Agrega los controladores al contenedor de servicios
builder.Services.AddControllers();

// Configuración de protección contra CSRF (Cross-Site Request Forgery)
builder.Services.AddAntiforgery(options =>
{    
    options.HeaderName = "X-XSRF-TOKEN"; // Nombre del encabezado para el token CSRF  
    options.Cookie.Name = "XSRF-TOKEN"; // Nombre de la cookie    
    options.Cookie.HttpOnly = false; // Solo accesible por el servidor
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Solo se envía sobre HTTPS
    options.Cookie.SameSite = SameSiteMode.None; // Protección contra CSRF    
    options.Cookie.Expiration = TimeSpan.FromMinutes(60); // Expiración del token
});

// Configuración de JWT (JSON Web Token) para autenticación
// Obtiene la sección de configuración para JWT
IConfigurationSection jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    // Define el esquema de autenticación por defecto
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // Define el esquema de desafío por defecto
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    // Requiere HTTPS para la transmisión del token
    options.RequireHttpsMetadata = true;
    // Guarda el token en la respuesta
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Valida el emisor del token
        ValidateAudience = true, // Valida el destinatario del token
        ValidateLifetime = true, // Valida la fecha de expiración del token
        ValidateIssuerSigningKey = true, // Valida la clave de firma del token
        ValidIssuer = jwtSettings["Issuer"], // Define el emisor válido
        ValidAudience = jwtSettings["Audience"], // Define el destinatario válido
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))// Define la clave de firma del token
    };

    // Configura el manejo de tokens desde cookies
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Leer el token JWT desde una cookie
            var token = context.Request.Cookies["JWT-TOKEN"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;// Asigna el token al contexto de la solicitud
            }
            return Task.CompletedTask;// Completa la tarea
        }
    };
});

// Configura la autorización en la aplicación
builder.Services.AddAuthorization();

// Configuración de CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",// Nombre de la política CORS
        policy => policy.WithOrigins("http://localhost:4200") // Permite solicitudes desde el origen especificado
                        .AllowAnyHeader() // Permite cualquier encabezado
                        .AllowAnyMethod() // Permite cualquier método HTTP
                        .AllowCredentials()); // Para manejar cookies de sesión si es necesario
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();  // Habilita la exploración de los puntos finales
builder.Services.AddSwaggerGen();// Agrega el generador de Swagger para la documentación

// Crear una instancia de la aplicación
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Configura Swagger en el entorno de desarrollo
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usa la política CORS configurada
app.UseCors("AllowAngularApp");

// Redirige las solicitudes HTTP a HTTPS
app.UseHttpsRedirection();

// Registra el middleware personalizado para validación de tokens XSRF
app.UseMiddleware<XsrfTokenValidationMiddleware>();

// Agregar el middleware de autenticación
app.UseAuthentication();
app.UseAuthorization();

// Mapea los controladores en la aplicación
app.MapControllers();

// Ejecuta la aplicación
app.Run();

public partial class Program();