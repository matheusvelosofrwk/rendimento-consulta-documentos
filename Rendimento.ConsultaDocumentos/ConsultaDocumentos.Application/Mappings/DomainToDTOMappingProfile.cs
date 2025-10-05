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
            CreateMap<AplicacaoDTO, Aplicacao>().ReverseMap();
            CreateMap<ProvedorDTO, Provedor>().ReverseMap();
            CreateMap<NacionalidadeDTO, Nacionalidade>().ReverseMap();
            CreateMap<SituacaoCadastralDTO, SituacaoCadastral>().ReverseMap();
            CreateMap<DocumentoDTO, Documento>().ReverseMap();
            CreateMap<EnderecoDTO, Endereco>().ReverseMap();
            CreateMap<TelefoneDTO, Telefone>().ReverseMap();
            CreateMap<EmailDTO, Email>().ReverseMap();
            CreateMap<QuadroSocietarioDTO, QuadroSocietario>().ReverseMap();
            CreateMap<AplicacaoProvedorDTO, AplicacaoProvedor>().ReverseMap();
            CreateMap<LogAuditoriaDTO, LogAuditoria>().ReverseMap();
            CreateMap<LogErroDTO, LogErro>().ReverseMap();
        }
    }
}
