using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class ClienteService : BaseService<Cliente>, IClienteService
    {
        public ClienteService(IClienteRepository repository) : base(repository)
        {
        }
    }
}
