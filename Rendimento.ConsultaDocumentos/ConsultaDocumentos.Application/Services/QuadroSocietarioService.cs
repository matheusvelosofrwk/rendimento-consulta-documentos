using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class QuadroSocietarioService : BaseService<QuadroSocietarioDTO, QuadroSocietario>, IQuadroSocietarioService
    {
        public QuadroSocietarioService(IQuadroSocietarioRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
