using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controladores
{
    //[Route("api/[controller]")] //Opción estática
    //SOPORTE PARA CACHÉ A NIVEL CONTROLADOR. SE UTILIZA PARA MEJORAR LA EFICIENCIA DEL SERVIDOR
    //[ResponseCache(Duration = 20)] //LA DURACIÓN ES EN SEGUNDOS
    [Route("api/categorias")] //Opción dinámica
    [Authorize(Roles = "Admin")] //SI SE COLOCA ESTA INSTRUCCIÓN A NIVEL DE CLASE, SE PROTEGERÁ TODO EL CONTROLADOR. SE PUEDE DEFINIR ROLES
    [ApiController]
    //[EnableCors("PoliticaCors")] //AQUÍ SE PROTEGEN TODOS LOS MÉTODOS CON CORS A NIVEL CONTROLADOR
    public class CategoriasControlador : ControllerBase
    {
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;

        public CategoriasControlador(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo; 
            _mapper = mapper;
        }

        [AllowAnonymous]//SI PROTEJO A NIVEL DE CLASE, CON ESTA INSTRUCCIÓN SE ESPECIFICA CUALES QUIERO QUE SEAN PÚBLICOS AL 100%
        [HttpGet]
        //[ResponseCache(Duration = 20)] //SOPORTE CACHÉ A NIVEL DE MÉTODO. NO SE RECOMIENDA USAR DONDE LA INFO. SE ACTUALIZA CONSTANTEMENTE
        [ResponseCache(CacheProfileName = "perfil20Segundos")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[EnableCors("PoliticaCors")] //AQUÍ SOLO SE PROTEGE ESTE MÉTODO
        public IActionResult GetCategorias()
        {

            var listaCategorias = _ctRepo.GetCategorias();

            var listaCategoriasDto = new List<CategoriaDto>();

            foreach (var lista in listaCategorias)
            {
                listaCategoriasDto.Add(_mapper.Map<CategoriaDto>(lista));
            }

            return Ok(listaCategoriasDto);

        }

        [AllowAnonymous]
        [HttpGet("{CategoriaId:int}", Name = "GetCategoria")]
        /*
            QUIZÁ PARA BÚSQUEDAS INDIVIDUALES O ESPECÍFICAS, SÍ SE PODRÍA ACTIVAR CACHE. AL DETECTAR OTRA BÚSQUEDA A LA GUARDADA EN CACHE,
            VOLVERÁ A ACTUALIZAR EL CACHÉ

            LAS INSTRUCCIONES "Location = ResponseCacheLocation.None, NoStore = true", PERMITEN QUE LA INFORMACIÓN NO SE GUARDE NI EN LA
            CACHÉ NI EN EL SERVIDOR O CLIENTE.
            
            ESTA INSTRUCCIÓN SE RECOMIENDA PARA PROCESOS QUE MANEJEN INFORMACIÓN SENSIBLE O DATOS CONFIDENCIALES.
            TAMBIÉN ESTA INSTRUCCIÓN AYUDA A QUE LOS CLIENTES RECIBAN RESPUESTAS DEL SERVIDOR EN TIEMPO REAL, CON INFORMACIÓN ACTUALIZADA SIEMPRE.
            LA ÚNICA DESVENTAJA DE ESTA INSTRUCCIÓN, ES QUE AL NO UTILIZAR CACHÉ, SI EL SERVIDOR LLEGA A TENER MUCHAS RESPUESTAS SIMULTÁNEAS, 
            PUEDE AFECTAR NOTORIAMENTE SU RENDIMIENTO Y EFICIENCIA.

            USAR LA CACHÉ O NO PERMITIRLA DEPENDE DE CADA CASO Y SE DEBE TOMAR SIEMPRE EN CUENTA Y SOBRE TODO PRIORIZAR EL RENDIMIENTO Y 
            EFICIENCIA DEL SERVIDOR ANTES DE USAR O NO USAR LA CACHÉ
            
        */
        //[ResponseCache(Duration = 40)] 
        //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [ResponseCache(CacheProfileName = "perfil20Segundos")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategoria(int CategoriaId)
        {

            var itemCategoria = _ctRepo.GetCategoria(CategoriaId);

            if (itemCategoria == null)
                return NotFound();

            var itemCategoriaDto = _mapper.Map<CategoriaDto>(itemCategoria);

            return Ok(itemCategoriaDto);

        }

        //[Authorize]//SI SE UTILIZA ESTA INSTRUCCIÓN A NIVEL MÉTODO, SOLO SE PROTEGERÁ EL MÉTODO DONDE SE ENCUENTRE ESTA INSTRUCCIÓN
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CrearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto)
        {

            if(!ModelState.IsValid)
                return BadRequest();

            if(crearCategoriaDto == null)
            {
                return BadRequest(ModelState);
            }

            if(_ctRepo.ExisteCategoria(crearCategoriaDto.Nombre))
            {
                ModelState.AddModelError("", $"La categoría ya existe");
                return StatusCode(404, ModelState);
            }

            var categoria = _mapper.Map<Categoria>(crearCategoriaDto);

            if(!_ctRepo.CrearCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salió mal guardando el registro: {categoria.Nombre}"); //El '$' es para ponerle variables y personalizar el msg
                return StatusCode(404, ModelState);
            }

            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria);

        }

        //[Authorize(Roles = "Admin")]
        [HttpPatch("{CategoriaId:int}", Name = "ActualizarPatchCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPatchCategoria(int CategoriaId, [FromBody] CategoriaDto categoriaDto)
        {

            if (!ModelState.IsValid)
                return BadRequest();

            if (categoriaDto == null || CategoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro: {categoria.Nombre}"); //El '$' es para ponerle variables y personalizar el msg
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("{CategoriaId:int}", Name = "ActualizarPutCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPutCategoria(int CategoriaId, [FromBody] CategoriaDto categoriaDto)
        {

            if (!ModelState.IsValid)
                return BadRequest();

            if (categoriaDto == null || CategoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var categoriaExistente = _ctRepo.GetCategoria(CategoriaId);
            if (categoriaExistente == null)
                return NotFound($"No se encontró la categoría con ID: {CategoriaId}");

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro: {categoria.Nombre}"); //El '$' es para ponerle variables y personalizar el msg
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete("{CategoriaId:int}", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarCategoria(int CategoriaId)
        {
            
            if (!_ctRepo.ExisteCategoria(CategoriaId))
                return NotFound();

            var categoria = _ctRepo.GetCategoria(CategoriaId);

            if (!_ctRepo.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salió mal borrando el registro: {categoria.Nombre}"); //El '$' es para ponerle variables y personalizar el msg
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

    }
}
