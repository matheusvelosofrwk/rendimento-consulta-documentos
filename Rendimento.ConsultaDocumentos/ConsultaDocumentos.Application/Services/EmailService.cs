using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class EmailService : BaseService<EmailDTO, Email>, IEmailService
    {
        public EmailService(IEmailRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
