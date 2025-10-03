using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Domain.Entities;

namespace ConsultaDocumentos.Application.Mappings
{
    public class DomainToDTOMappingProfile : Profile
    {
        public DomainToDTOMappingProfile()
        {
            CreateMap<ClienteDTO, Cliente>().ReverseMap();
        }
    }
}
