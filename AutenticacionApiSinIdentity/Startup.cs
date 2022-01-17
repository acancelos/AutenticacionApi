using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Datos;
using AutenticacionApiSinIdentity.Interfaces;
using AutenticacionApiSinIdentity.Modelos;
using AutenticacionApiSinIdentity.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace AutenticacionApiSinIdentity
{
    public class Startup
    {
        //inyecto Iconfiguration para obtener la llave que est� en el appsettings.Json
        public Startup(IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
           

            services.AddDbContext<ApplicationDbContext>(
               options => options.UseSqlServer(Configuration.GetConnectionString("Conexion")));

            //configuracion de Swagger para usar autenticacion
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "Servicio REST de autenticaci�n �nico, reutilizable, gen�rico y extensible", 
                    Version = "v1",
                    Description= "Dise�o de un componente gen�rico que contemple distintos m�todos de autenticaci�n y su consecuente generaci�n y refresh de tokens.",
                    

                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                 Type= ReferenceType.SecurityScheme,
                            Id= "Bearer"
                            }

                        },
                        new string[]{}
                    }
                });
                var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXML);
                c.IncludeXmlComments(rutaXML, includeControllerXmlComments: true);
               
            });
            //Agrego el servicio de autenticacion JWTBearer
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
                    ClockSkew = TimeSpan.Zero
                });
            //Agrego el servicio de autenticacion indicandole que tiene que instanciar AutenticarJWT en caso de llamar a la Interface
           
            
            services.AddScoped<IToken, TokenJWT>();

            var metodo = Configuration["MetodoAutenticacion"];

            switch (metodo)
            {
                case "CustomAuthentication":
                    services.AddScoped<IAutenticar, CustomAuthentication>();
                    break;
            }
            

            services.Configure<Usuario>(Configuration.GetSection(Usuario.Key));

            //Autorizacion por medio de los Claims
            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("Admin", politica => politica.RequireClaim("Admin"));
            });

            services.AddDataProtection();

            services.AddTransient<Encriptacion>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutenticacionApiSinIdentity v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
