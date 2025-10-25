namespace ConsultaDocumentos.Domain.Enums
{
    public enum OrigemConsulta
    {
        RepositorioEHubs = 1,  // Padrão: tenta cache/BD primeiro, depois hubs
        ApenasRepositorio = 2, // Busca apenas no cache/BD local
        ApenasHubs = 3         // Força consulta nos provedores externos
    }
}
