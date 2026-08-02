using RentalPipeline.DTOs.Imoveis;

namespace RentalPipeline.Services
{
    public interface IImovelService
    {
        Task<IEnumerable<ImovelResponseDto>> ObterTodosAsync();
        Task<ImovelResponseDto> ObterPorIdAsync(Guid id);
        Task<ImovelResponseDto> CriarAsync(ImovelCreateDto dto);
    }
}
