using System.Net;

namespace ApiPeliculas.Modelos
{
    public class RespuestaAPI
    {
        public RespuestaAPI()
        {
            MensajesError = new List<string>();
        }

        public HttpStatusCode CodigoEstado { get; set; }
        public bool EsExitosa { get; set; } = true;
        public List<string> MensajesError { get; set; }
        public object Resultado { get; set; }  

    }
}
