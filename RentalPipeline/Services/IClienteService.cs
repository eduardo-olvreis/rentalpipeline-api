using RentalPipeline.DTOs.Clientes;

namespace RentalPipeline.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteResponseDto>> ObterTodosAsync();
        Task<ClienteResponseDto> ObterPorIdAsync(Guid id);
        Task<ClienteResponseDto> CriarAsync(ClienteCreateDto dto);
    }
}
