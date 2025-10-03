using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class AplicacaoService : BaseService<AplicacaoDTO, Aplicacao>, IAplicacaoService
    {
        public AplicacaoService(IAplicacaoRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
