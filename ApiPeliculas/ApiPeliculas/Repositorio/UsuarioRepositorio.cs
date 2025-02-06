using ApiPeliculas.Datos;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
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

        public UsuarioRepositorio(ContextoAplicacionBD bd, IConfiguration config)
        {
            _bd = bd;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
        }

        public bool ExisteUsuario(string Usuario)
        {
            var usuarioBd = _bd.Usuario.FirstOrDefault(u => u.Nombre == Usuario);
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
            var contraseñaEncriptada = obtenerMD5(usuarioLoginDto.Contraseña);

            var usuario = _bd.Usuario.FirstOrDefault(
                    u => u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
                        && u.Contraseña == contraseñaEncriptada
                    );
            //VALIDAMOS SI EL USUARIO NO EXISTE CON LA COMBINACIÓN DE USUARIO/CONTRASEÑA
            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    Usuario = null
                };
            }

            //AQUI EXISTE EL USUARIO, ENTONCES PODEMOS PROCESAR EL LOGIN
            var manejadorToken = new JwtSecurityTokenHandler();
            var llave = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(llave), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = manejadorToken.CreateToken(tokenDescriptor);

            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            { 
                Token = manejadorToken.WriteToken(token),
                Usuario = usuario
            };

            return usuarioLoginRespuestaDto;

        }

        public async Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            var contraseñaEncriptada = obtenerMD5(usuarioRegistroDto.Contraseña);

            Usuario usuario = new Usuario()
            {
                Nombre = usuarioRegistroDto.Nombre,
                NombreUsuario = usuarioRegistroDto.NombreUsuario,
                Contraseña = contraseñaEncriptada,
                Rol = usuarioRegistroDto.Rol
            };

            _bd.Usuario.Add(usuario);
            await _bd.SaveChangesAsync();
            usuario.Contraseña = contraseñaEncriptada;
            return usuario;
        }

        //MÉTODO ENCRIPTAR CONTRASEÑA
        public static string obtenerMD5(string valor)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] datos = System.Text.Encoding.UTF8.GetBytes(valor);
            datos = x.ComputeHash(datos);
            string respuesta = "";
            for (int i = 0; i < datos.Length; i++)
                respuesta += datos[i].ToString("x2").ToLower();
            return respuesta;
        }

    }
}
