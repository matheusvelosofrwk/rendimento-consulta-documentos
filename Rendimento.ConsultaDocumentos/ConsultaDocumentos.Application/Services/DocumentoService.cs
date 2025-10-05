using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class DocumentoService : BaseService<DocumentoDTO, Documento>, IDocumentoService
    {
        public DocumentoService(IDocumentoRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
