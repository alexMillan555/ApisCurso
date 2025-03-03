namespace ApiPeliculas.Modelos.Dtos
{
    public class UsuarioLoginRespuestaDto
    {
        public UsuarioDatosDto Usuario { get; set; }
        public string Rol { get; set; }
        public string Token { get; set; }
    }
}
