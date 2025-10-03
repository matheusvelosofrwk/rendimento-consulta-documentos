using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class ClienteService : BaseService<ClienteDTO, Cliente>, IClienteService
    {
        public ClienteService(IClienteRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
