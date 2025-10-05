using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class NacionalidadeService : BaseService<NacionalidadeDTO, Nacionalidade>, INacionalidadeService
    {
        public NacionalidadeService(INacionalidadeRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
