using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class AplicacaoProvedorService : BaseService<AplicacaoProvedorDTO, AplicacaoProvedor>, IAplicacaoProvedorService
    {
        public AplicacaoProvedorService(IAplicacaoProvedorRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
