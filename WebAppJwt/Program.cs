using AuhtService;
using AuhtService.Interfaces;
using AuthData;
using AuthData.Interfaces;
using AuthData.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Crear una instancia del constructor de la aplicaci�n
var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de servicios
// Agrega servicios al contenedor de servicios de la aplicaci�n
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITasksService, TasksService>();
builder.Services.AddScoped<ITaskInformation, TaskInformation>();

// Agrega los controladores al contenedor de servicios
builder.Services.AddControllers();

// Configuraci�n de protecci�n contra CSRF (Cross-Site Request Forgery)
builder.Services.AddAntiforgery(options =>
{    
    options.HeaderName = "X-XSRF-TOKEN"; // Nombre del encabezado para el token CSRF  
    options.Cookie.Name = "XSRF-TOKEN"; // Nombre de la cookie    
    options.Cookie.HttpOnly = false; // Solo accesible por el servidor
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Solo se env�a sobre HTTPS
    options.Cookie.SameSite = SameSiteMode.None; // Protecci�n contra CSRF    
    options.Cookie.Expiration = TimeSpan.FromMinutes(60); // Expiraci�n del token
});

// Configuraci�n de JWT (JSON Web Token) para autenticaci�n
// Obtiene la secci�n de configuraci�n para JWT
IConfigurationSection jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    // Define el esquema de autenticaci�n por defecto
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // Define el esquema de desaf�o por defecto
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    // Requiere HTTPS para la transmisi�n del token
    options.RequireHttpsMetadata = true;
    // Guarda el token en la respuesta
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Valida el emisor del token
        ValidateAudience = true, // Valida el destinatario del token
        ValidateLifetime = true, // Valida la fecha de expiraci�n del token
        ValidateIssuerSigningKey = true, // Valida la clave de firma del token
        ValidIssuer = jwtSettings["Issuer"], // Define el emisor v�lido
        ValidAudience = jwtSettings["Audience"], // Define el destinatario v�lido
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

// Configura la autorizaci�n en la aplicaci�n
builder.Services.AddAuthorization();

// Configuraci�n de CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",// Nombre de la pol�tica CORS
        policy => policy.WithOrigins("http://localhost:4200") // Permite solicitudes desde el origen especificado
                        .AllowAnyHeader() // Permite cualquier encabezado
                        .AllowAnyMethod() // Permite cualquier m�todo HTTP
                        .AllowCredentials()); // Para manejar cookies de sesi�n si es necesario
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();  // Habilita la exploraci�n de los puntos finales
builder.Services.AddSwaggerGen();// Agrega el generador de Swagger para la documentaci�n

// Crear una instancia de la aplicaci�n
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Configura Swagger en el entorno de desarrollo
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usa la pol�tica CORS configurada
app.UseCors("AllowAngularApp");

// Redirige las solicitudes HTTP a HTTPS
app.UseHttpsRedirection();

// Registra el middleware personalizado para validaci�n de tokens XSRF
app.UseMiddleware<XsrfTokenValidationMiddleware>();

// Agregar el middleware de autenticaci�n
app.UseAuthentication();
app.UseAuthorization();

// Mapea los controladores en la aplicaci�n
app.MapControllers();

// Ejecuta la aplicaci�n
app.Run();

public partial class Program();