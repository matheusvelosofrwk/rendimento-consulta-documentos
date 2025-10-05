using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class SituacaoCadastralService : BaseService<SituacaoCadastralDTO, SituacaoCadastral>, ISituacaoCadastralService
    {
        public SituacaoCadastralService(ISituacaoCadastralRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
