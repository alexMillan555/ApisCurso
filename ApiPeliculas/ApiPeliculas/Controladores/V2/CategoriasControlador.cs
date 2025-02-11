using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controladores.V2
{
    //[Route("api/[controller]")] //Opción estática
    //SOPORTE PARA CACHÉ A NIVEL CONTROLADOR. SE UTILIZA PARA MEJORAR LA EFICIENCIA DEL SERVIDOR
    //[ResponseCache(Duration = 20)] //LA DURACIÓN ES EN SEGUNDOS
    //[Route("api/categorias")] //Opción dinámica
    [Route("api/v{version:apiVersion}/categorias")] //Opción dinámica
    [Authorize(Roles = "Admin")] //SI SE COLOCA ESTA INSTRUCCIÓN A NIVEL DE CLASE, SE PROTEGERÁ TODO EL CONTROLADOR. SE PUEDE DEFINIR ROLES
    [ApiController]
    //[EnableCors("PoliticaCors")] //AQUÍ SE PROTEGEN TODOS LOS MÉTODOS CON CORS A NIVEL CONTROLADOR
    ////SOPORTE VERSIONAMIENTO API A CONTROLADOR: ATRIBUTO DE VERSIÓN A NIVEL CONTROLADOR
    //[ApiVersion("1.0")]
    //SE LE PUEDE ESPECIFICAR VARIAS VERSIONES CON LAS QUE DEBE TRABAJAR (NO SE RECOMIENDA TENER VARIAS VERSIONES EN UN CONTROLADOR)
    //SE RECOMIENDA ORGANIZAR Y SEPARAR CADA VERSIÓN
    [ApiVersion("2.0")]
    public class CategoriasControlador : ControllerBase
    {
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;

        public CategoriasControlador(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("GetString")]
        //[MapToApiVersion("2.0")]
        public IEnumerable<string> Get()
        {
            return new string[] { "alex", "andrea" };
        }

    }
}
