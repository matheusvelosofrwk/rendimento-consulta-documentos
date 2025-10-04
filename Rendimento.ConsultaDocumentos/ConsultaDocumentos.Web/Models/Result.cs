namespace ConsultaDocumentos.Web.Models
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public List<string> Notifications { get; set; } = new List<string>();
    }
}
