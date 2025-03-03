using ApiPeliculas.Datos;
using ApiPeliculas.PeliculasMappers;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ContextoAplicacionBD>(opciones =>
        opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));

//SOPORTE PARA LA AUTENTICACIÓN CON.NET IDENTITY
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ContextoAplicacionBD>();

//SOPORTE PARA CACHÉ Y VERSIONAMIENTO
builder.Services.AddResponseCaching();

//SOPORTE PARA CACHÉ GLOBAL (PARA TODA LA APLICACIÓN)
builder.Services.AddControllers(opcion =>
{
    //CACHÉ PROFILE: UN CACHÉ GLOBAL PARA NO TENER QUE COLOCARLO EN CADA CONTROLADOR
    opcion.CacheProfiles.Add("perfil20Segundos", new CacheProfile() { Duration = 20 });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//IMPLEMENTAR AUTENTICACIÓN DENTRO DE LA DOCUMENTACIÓN DE LA API
builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Autenticación JWT usando el esquema Bearer. \r\n\r\n" +
                "Ingresa la palabra 'Bearer' seguido de un [espacio] y después su token en el campo de abajo. \r\n\r\n" +
                "Ejemplo: \"Bearer asfdasfki1234oaks23\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "Bearer"
            });
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
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
            //SOPORTE PARA DOCUMENTACIÓN SWAGGER
            options.SwaggerDoc("v1", new OpenApiInfo
                { 
                    Version = "v1.0",
                    Title = "Peliculas Api V1",
                    Description = "API de películas Versión 1",
                    TermsOfService = new Uri("https://google.co.uk"),
                    Contact = new OpenApiContact
                    {
                        Name = "Google",
                        Url = new Uri("https://google.co.uk")
                    },
                    License = new OpenApiLicense//opcional
                    {
                        Name = "Licencia personal",
                        Url = new Uri("https://google.co.uk")
                    }
                }
            
            );
            //SOPORTE DOCUMENTACIÓN MÚLTIPLE VERSIÓN API
            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Version = "v2.0",
                Title = "Peliculas Api V2",
                Description = "API de películas Versión 2",
                TermsOfService = new Uri("https://google.co.uk"),
                Contact = new OpenApiContact
                {
                    Name = "Google",
                    Url = new Uri("https://google.co.uk")
                },
                License = new OpenApiLicense//opcional
                {
                    Name = "Licencia personal",
                    Url = new Uri("https://google.co.uk")
                }
            }

            );
        }
    );

//SOPORTE CORS
/*
    - Se pueden habilitar: 1-dominio, 2-multiples dominios
    - Se usa (*) para todos los dominios **AQUI HAY QUE TENER CUIDADO, YA QUE CON ESTO CUALQUIER DOMINIO
    PODRÍA ACCEDER
    - Si se quiere usar dominios especificos, se deben de colocar en "WithOrigins" y separados por 
    comas (,). Ejemplo: build.WithOrigins("http://localhost/, http://localhost:3223")
 */
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build => 
    build.WithOrigins("http://localhost/").AllowAnyMethod().AllowAnyHeader()    
));

//AQUÍ SE AGREGARÁN LOS REPOSITORIOS
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

//OBTENER LLAVE TOKEN
var llave = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

//SOPORTE PARA VERSIONAMIENTO
builder.Services.AddApiVersioning();

var apiVersioningBuilder = builder.Services.AddApiVersioning((opcion =>
{
    opcion.AssumeDefaultVersionWhenUnspecified = true;
    opcion.DefaultApiVersion = new ApiVersion(1, 0);
    opcion.ReportApiVersions = true;
    //opcion.ApiVersionReader = ApiVersionReader.Combine(
    //        new QueryStringApiVersionReader("api-version")//?api-version=1.0
    //        //new HeaderApiVersionReader("X-Version"), //LEE LA VERSIÓN DESDE EL ENCABEZADO X-VERSION
    //        //new MediaTypeApiVersionReader("ver") //LEE DESDE EL TIPO DE MEDIO (MEDIA TYPE) DE LA SOLICITUD
    //    );
}));

apiVersioningBuilder.AddApiExplorer(
        opciones =>
        {
            opciones.GroupNameFormat = "'v'VVV";
            opciones.SubstituteApiVersionInUrl = true;
        }
    );

//AGREGAMOS EL AUTOMAPPER
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

//AQUÍ SE CONFIGURA LA AUTENTICACIÓN
builder.Services.AddAuthentication(
        x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    ).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false; //EN UNA API PRODUCTIVA, ESTO SE CAMBIA POR 'true'
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(llave)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    //SOPORTE PARA DOCUMENTACIÓN DE VERSIÓN EN SWAGGER
    app.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiPeliculasV1");
        //DOCUMENTACIÓN MÚLTIPLE VERSIÓN
        opciones.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiPeliculasV2");
    });
}

//SOPORTE CORS
app.UseCors("PoliticaCors");

app.UseHttpsRedirection();

//EL USE AUTHENTICATION ES EL SOPORTE PARA AUTENTICACIÓN
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
