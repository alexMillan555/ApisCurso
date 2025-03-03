using ApiPeliculas.Datos;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ContextoAplicacionBD _bd;
        private string claveSecreta;
        private readonly UserManager<AppUsuario> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UsuarioRepositorio(ContextoAplicacionBD bd, IConfiguration config, UserManager<AppUsuario> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _bd = bd;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public bool ExisteUsuario(string Usuario)
        {
            var usuarioBd = _bd.AppUsuario.FirstOrDefault(u => u.UserName == Usuario);
            if (usuarioBd == null)
                return true;
            else
                return false;
        }

        public Usuario GetUsuario(int UsuarioId)
        {
            return _bd.Usuario.FirstOrDefault(c => c.Id == UsuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _bd.Usuario.OrderBy(c => c.NombreUsuario).ToList();
        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            //var contraseñaEncriptada = obtenerMD5(usuarioLoginDto.Contraseña);

            var usuario = _bd.AppUsuario.FirstOrDefault(
                    u => u.UserName.ToLower() == usuarioLoginDto.NombreUsuario.ToLower());

            bool esValido = await _userManager.CheckPasswordAsync(usuario, usuarioLoginDto.Contraseña);

            //VALIDAMOS SI EL USUARIO NO EXISTE CON LA COMBINACIÓN DE USUARIO/CONTRASEÑA
            if (usuario == null || !esValido)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    Usuario = null
                };
            }

            //AQUI EXISTE EL USUARIO, ENTONCES PODEMOS PROCESAR EL LOGIN
            var roles = await _userManager.GetRolesAsync(usuario);
            var manejadorToken = new JwtSecurityTokenHandler();
            var llave = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(llave), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = manejadorToken.CreateToken(tokenDescriptor);

            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            { 
                Token = manejadorToken.WriteToken(token),
                Usuario = _mapper.Map<UsuarioDatosDto>(usuario)
            };

            return usuarioLoginRespuestaDto;

        }

        public async Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            //var contraseñaEncriptada = obtenerMD5(usuarioRegistroDto.Contraseña);

            AppUsuario usuario = new AppUsuario()
            {
                UserName = usuarioRegistroDto.NombreUsuario,
                Email = usuarioRegistroDto.NombreUsuario,
                NormalizedEmail = usuarioRegistroDto.NombreUsuario.ToUpper(),
                Nombre = usuarioRegistroDto.Nombre
            };

            var result = await _userManager.CreateAsync(usuario, usuarioRegistroDto.Contraseña);

            if(result.Succeeded)
            {
                //SI NO EXISTE UN ROL, CON ESTE CÓDIGO LO DA DE ALTA
                if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Registrado"));
                }

                await _userManager.AddToRoleAsync(usuario, "Admin");
                var usuarioRetornado = _bd.AppUsuario.FirstOrDefault(u => u.UserName == usuarioRegistroDto.NombreUsuario);

                return _mapper.Map<UsuarioDatosDto>(usuarioRetornado);
            }

            //_bd.Usuario.Add(usuario);
            //await _bd.SaveChangesAsync();
            //usuario.Contraseña = contraseñaEncriptada;
            return new UsuarioDatosDto();
        }

        //MÉTODO ENCRIPTAR CONTRASEÑA
        //public static string obtenerMD5(string valor)
        //{
        //    MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
        //    byte[] datos = System.Text.Encoding.UTF8.GetBytes(valor);
        //    datos = x.ComputeHash(datos);
        //    string respuesta = "";
        //    for (int i = 0; i < datos.Length; i++)
        //        respuesta += datos[i].ToString("x2").ToLower();
        //    return respuesta;
        //}

    }
}
