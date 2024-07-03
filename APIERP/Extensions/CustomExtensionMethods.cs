using APIERP;
using APIERP.Entidades;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;

namespace ERP.Extensions
{
    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddCustomOpenApi(this IServiceCollection services)
        {
            // Cache API
            services.AddOutputCache();
            // Configure Open API
            services.AddEndpointsApiExplorer();

            // Add framework services.
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TipeSoft - API ERP",
                    Version = "v1",
                    Description = "The ERP API CRUD"
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services, string policyName)
        {
            // Configure Open API
            services.AddCors(options =>
            {
                options.AddPolicy(policyName,
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:5203")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });

                options.AddPolicy("libre", configuracion =>
                {
                    configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            return services;
        }
    }


    public static class CustomMiddlewareExtensionMethods
    {

        public static void DatabaseInit(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            bool dbCreated = false;

            try
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbCreated = context.Database.EnsureCreated();
                //if (dbCreated) DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }

        public static void ConfigureSwagger(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.Map("/", () => Results.Redirect("/swagger"));
            }
            app.UseOutputCache();
        }

        public static void UseExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
            {
                var exceptionHandleFeature = context.Features.Get<IExceptionHandlerFeature>();
                var excepcion = exceptionHandleFeature?.Error;

                var error = new Error
                {
                    Fecha = DateTime.UtcNow,
                    MensajeDeError = excepcion?.Message ?? "Error sin mensaje",
                    StackTrace = excepcion?.StackTrace
                };

                var db = context.RequestServices.GetRequiredService<ApplicationDbContext>();
                db.Add(error);
                await db.SaveChangesAsync();

                await TypedResults.BadRequest(
                    new { tipo = "Error", mensaje = "Ha ocurrido un error inesperado", estado = 500 })
                .ExecuteAsync(context);
            }));
        }
    }
}
