using Microsoft.EntityFrameworkCore;
using RentalPipeline.Data;
using RentalPipeline.DTOs.Clientes;
using RentalPipeline.Entities;
using RentalPipeline.Exceptions;

namespace RentalPipeline.Services
{
    public class ClienteService : IClienteService
    {
        private readonly AppDbContext _context;
        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClienteResponseDto>> ObterTodosAsync()
        {
            var clientes = await _context.Clientes.AsNoTracking().ToListAsync();
            return clientes.Select(MapearParaDto);
        }

        public async Task<ClienteResponseDto> ObterPorIdAsync(Guid id)
        {
            var cliente = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null)
                throw new NaoEncontradoException("Cliente não encontrado.");

            return MapearParaDto(cliente);
        }

        public async Task<ClienteResponseDto> CriarAsync(ClienteCreateDto dto)
        {
            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome,
                Email = dto.Email,
                Cpf = dto.Cpf,
                CriadoEm = DateTime.UtcNow
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return MapearParaDto(cliente);
        }

        private static ClienteResponseDto MapearParaDto(Cliente cliente)
        {
            return new ClienteResponseDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Cpf = cliente.Cpf,
                CriadoEm = cliente.CriadoEm
            };
        }
    }
}
