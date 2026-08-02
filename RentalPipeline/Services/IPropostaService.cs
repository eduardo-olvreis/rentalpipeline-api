using RentalPipeline.DTOs.Propostas;

namespace RentalPipeline.Services
{
    public interface IPropostaService
    {
        Task<PropostaResponseDto> CriarPropostaAsync(PropostaCreateDto propostaDto);
        Task<PropostaResponseDto> AtualizarStatusAsync(Guid id, PropostaUpdateStatusDto propostaDto);
        Task<IEnumerable<HistoricoPropostaResponseDto>> ObterHistoricoAsync(Guid id);
    }
}
