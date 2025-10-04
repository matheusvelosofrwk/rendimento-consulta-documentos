using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class ProvedorService : BaseService<ProvedorDTO, Provedor>, IProvedorService
    {
        public ProvedorService(IProvedorRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
