using AutoMapper;
using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Application.Results;
using ConsultaDocumentos.Domain.Entities;
using ConsultaDocumentos.Domain.Intefaces;

namespace ConsultaDocumentos.Application.Services
{
    public class SituacaoCadastralService : BaseService<SituacaoCadastralDTO, SituacaoCadastral>, ISituacaoCadastralService
    {
        private readonly ISituacaoCadastralRepository _situacaoCadastralRepository;

        public SituacaoCadastralService(ISituacaoCadastralRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _situacaoCadastralRepository = repository;
        }

        public async Task<Result<IEnumerable<SituacaoCadastralDTO>>> GetByTipoPessoaAsync(char tipoPessoa)
        {
            var result = new Result<IEnumerable<SituacaoCadastralDTO>>();

            try
            {
                var entities = await _situacaoCadastralRepository.GetByTipoPessoaAsync(tipoPessoa);
                result.Data = _mapper.Map<IEnumerable<SituacaoCadastralDTO>>(entities);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Notifications.Add($"Erro ao buscar situações cadastrais por tipo de pessoa: {ex.Message}");
            }

            return result;
        }
    }
}
