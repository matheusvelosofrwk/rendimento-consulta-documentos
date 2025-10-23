using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Application.Results;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class AplicacaoProvedorService : BaseService<AplicacaoProvedorDTO, AplicacaoProvedor>, IAplicacaoProvedorService
    {
        private readonly IAplicacaoProvedorRepository _aplicacaoProvedorRepository;

        public AplicacaoProvedorService(IAplicacaoProvedorRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _aplicacaoProvedorRepository = repository;
        }

        public async Task<Result<IList<AplicacaoProvedorDTO>>> GetByAplicacaoIdAsync(Guid aplicacaoId)
        {
            var entities = await _aplicacaoProvedorRepository.GetByAplicacaoIdAsync(aplicacaoId);
            var dtos = _mapper.Map<IList<AplicacaoProvedorDTO>>(entities);

            // Popular nomes de Aplicação e Provedor
            foreach (var dto in dtos)
            {
                var entity = entities.FirstOrDefault(e => e.Id == dto.Id);
                if (entity != null)
                {
                    dto.NomeAplicacao = entity.Aplicacao?.Nome;
                    dto.NomeProvedor = entity.Provedor?.Nome;
                }
            }

            return Result<IList<AplicacaoProvedorDTO>>.SuccessResult(dtos);
        }
    }
}
