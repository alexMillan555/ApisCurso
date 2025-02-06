using ApiPeliculas.Datos;
using ApiPeliculas.PeliculasMappers;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ContextoAplicacionBD>(opciones =>
        opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

//AGREGAMOS EL AUTOMAPPER
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

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

app.UseAuthorization();

app.MapControllers();

app.Run();
