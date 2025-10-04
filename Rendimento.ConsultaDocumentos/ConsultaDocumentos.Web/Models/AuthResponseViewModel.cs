namespace ConsultaDocumentos.Web.Models
{
    public class AuthResponseViewModel
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public DateTime Expiration { get; set; }
    }
}
