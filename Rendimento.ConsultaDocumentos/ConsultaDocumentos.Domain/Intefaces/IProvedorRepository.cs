using ConsultaDocumentos.Domain.Base;
using ConsultaDocumentos.Domain.Entities;

namespace ConsultaDocumentos.Domain.Intefaces
{
    public interface IProvedorRepository : IBaseRepository<Provedor>
    {
        Task<Provedor?> GetByNomeAsync(string nome);
    }
}
