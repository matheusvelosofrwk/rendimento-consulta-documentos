using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class TelefoneService : BaseService<TelefoneDTO, Telefone>, ITelefoneService
    {
        public TelefoneService(ITelefoneRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
