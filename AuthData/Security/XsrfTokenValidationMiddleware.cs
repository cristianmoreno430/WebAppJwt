using Microsoft.AspNetCore.Http;

namespace AuthData.Security
{
    public class XsrfTokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public XsrfTokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Omitir la validación para el endpoint de login
            var path = context.Request.Path.Value.ToLower();
            if (path.Contains("/login"))
            {
                await _next(context);
                return;
            }

            // Solo aplicar la validación en métodos POST, PUT y DELETE
            if (context.Request.Method == HttpMethods.Post ||
                context.Request.Method == HttpMethods.Put ||
                context.Request.Method == HttpMethods.Delete)
            {
                // Leer el token desde la cookie (usualmente llamado 'XSRF-TOKEN')
                var cookieToken = context.Request.Cookies["XSRF-TOKEN"];

                // Leer el token desde los encabezados de la solicitud (por ejemplo, 'X-XSRF-TOKEN')
                var requestToken = context.Request.Headers["X-XSRF-TOKEN"].FirstOrDefault();

                // Validar si ambos tokens existen
                if (string.IsNullOrEmpty(cookieToken) || string.IsNullOrEmpty(requestToken) ||
                    cookieToken != requestToken)
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync("Antiforgery token is missing.");
                    return;
                }
                
            }

            // Continuar si los tokens son válidos o si la solicitud es de otro tipo
            await _next(context);
        }
    }
}
