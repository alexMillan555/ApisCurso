using ApiPeliculas.Datos;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiPeliculas.Controladores
{
    //[Route("api/usuarios")]
    [Route("api/v{version:apiVersion}/usuarios")] //Opción dinámica
    [Authorize(Roles = "Admin")] //SI SE COLOCA ESTA INSTRUCCIÓN A NIVEL DE CLASE, SE PROTEGERÁ TODO EL CONTROLADOR
    [ApiController]
    //[ApiVersion("1.0")]
    //AQUÍ SE INDICA QUE ESTE CONTROLADOR, UTILIZA LA VERSIÓN NEUTRAL DE LA API, YA QUE HAY PROCESOS QUE NUNCA O CASI NUNCA CAMBIARÁN
    //ESTO SE HACE PARA QUE SEA INDEPENDIENTE DE CADA VERSIÓN QUE SE LANCE DE LA API
    [ApiVersionNeutral]
    public class UsuariosControlador : ControllerBase
    {
        private readonly IUsuarioRepositorio _usRepo;
        private readonly IMapper _mapper;
        protected RespuestaAPI _respuestaAPI;

        public UsuariosControlador(IUsuarioRepositorio usRepo, IMapper mapper)
        {
            _usRepo = usRepo;
            _mapper = mapper;
            _respuestaAPI = new();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ResponseCache(CacheProfileName = "perfil20Segundos")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuarios()
        {

            var listaUsuarios = _usRepo.GetUsuarios();

            var listaUsuariosDto = new List<UsuarioDto>();

            foreach (var lista in listaUsuarios)
            {
                listaUsuariosDto.Add(_mapper.Map<UsuarioDto>(lista));
            }

            return Ok(listaUsuariosDto);

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{UsuarioId:int}", Name = "GetUsuario")]
        [ResponseCache(CacheProfileName = "perfil20Segundos")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUsuario(int UsuarioId)
        {

            var itemUsuario = _usRepo.GetUsuario(UsuarioId);

            if (itemUsuario == null)
                return NotFound();

            var itemUsuarioDto = _mapper.Map<UsuarioDto>(itemUsuario);

            return Ok(itemUsuarioDto);

        }

        [AllowAnonymous]
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto)
        {

            bool validarNombreUsuarioUnico = _usRepo.ExisteUsuario(usuarioRegistroDto.NombreUsuario);
            if (!validarNombreUsuarioUnico)
            {
                _respuestaAPI.CodigoEstado = HttpStatusCode.BadRequest;
                _respuestaAPI.EsExitosa = false;
                _respuestaAPI.MensajesError.Add("El nombre de usuario ya existe");
                return BadRequest(_respuestaAPI);
            }

            var usuario = await _usRepo.Registro(usuarioRegistroDto);
            if (usuario == null)
            {
                _respuestaAPI.CodigoEstado = HttpStatusCode.BadRequest;
                _respuestaAPI.EsExitosa = false;
                _respuestaAPI.MensajesError.Add("Error en el registro de usuario");
                return BadRequest(_respuestaAPI);
            }

            _respuestaAPI.CodigoEstado = HttpStatusCode.OK;
            _respuestaAPI.EsExitosa = true;
            return Ok(_respuestaAPI);

        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuarioLoginDto)
        {

            var respuestaLogin = await _usRepo.Login(usuarioLoginDto);

            if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
            {
                _respuestaAPI.CodigoEstado = HttpStatusCode.BadRequest;
                _respuestaAPI.EsExitosa = false;
                _respuestaAPI.MensajesError.Add("El nombre de usuario o contraseña son incorrectos");
                return BadRequest(_respuestaAPI);
            }

            _respuestaAPI.CodigoEstado = HttpStatusCode.OK;
            _respuestaAPI.EsExitosa = true;
            _respuestaAPI.Resultado = respuestaLogin;
            return BadRequest(_respuestaAPI);

        }

    }
}
