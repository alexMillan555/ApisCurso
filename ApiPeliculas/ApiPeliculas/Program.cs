using ApiPeliculas.Datos;
using ApiPeliculas.PeliculasMappers;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ContextoAplicacionBD>(opciones =>
        opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//IMPLEMENTAR AUTENTICACIÓN DENTRO DE LA DOCUMENTACIÓN DE LA API
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "Autenticación JWT usando el esquema Bearer. \r\n\r\n " +
        "Ingresa la palabra 'Bearer' seguida de un [espacio] y despues su token en el campo de abajo \r\n\r\n" +
        "Ejemplo: \"Bearer tkdknkdllskd\"",
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
});
//builder.Services.AddSwaggerGen(options =>
//        {
//            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//            {
//                Description = "Autenticación JWT usando el esquema Bearer. \r\n\r\n" + 
//                "Ingresa la palabra 'Bearer' seguido de un [espacio] y después su token en el campo de abajo. \r\n\r\n" + 
//                "Ejemplo: \"Bearer asfdasfki1234oaks23\"",
//                Name = "Authorization",
//                In = ParameterLocation.Header,
//                Scheme = "Bearer"
//            });
//            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
//            {
//                {
//                    new OpenApiSecurityScheme
//                    {
//                        Reference = new OpenApiReference
//                        {
//                            Type = ReferenceType.SecurityScheme,
//                            Id = "Bearer"
//                        },
//                        Scheme = "oauth2",
//                        Name = "Bearer",
//                        In = ParameterLocation.Header
//                    },
//                    new List<string>()
//                }                
//            });
//        }
//    );

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
    app.UseSwaggerUI();
}

//SOPORTE CORS
app.UseCors("PoliticaCors");

app.UseHttpsRedirection();

//EL USE AUTHENTICATION ES EL SOPORTE PARA AUTENTICACIÓN
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
