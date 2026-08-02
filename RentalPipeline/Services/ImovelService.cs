using Microsoft.EntityFrameworkCore;
using RentalPipeline.Data;
using RentalPipeline.DTOs.Imoveis;
using RentalPipeline.Entities;
using RentalPipeline.Entities.Enums;
using RentalPipeline.Exceptions;

namespace RentalPipeline.Services
{
    public class ImovelService : IImovelService
    {
        private readonly AppDbContext _context;
        public ImovelService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ImovelResponseDto>> ObterTodosAsync()
        {
            var imoveis = await _context.Imoveis.AsNoTracking().ToListAsync();
            return imoveis.Select(MapearParaDto);
        }

        public async Task<ImovelResponseDto> ObterPorIdAsync(Guid id)
        {
            var imovel = await _context.Imoveis.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            if (imovel == null)
                throw new NaoEncontradoException("Imóvel não encontrado.");

            return MapearParaDto(imovel);
        }

        public async Task<ImovelResponseDto> CriarAsync(ImovelCreateDto dto)
        {
            var imovel = new Imovel
            {
                Id = Guid.NewGuid(),
                Endereco = dto.Endereco,
                ValorAluguel = dto.ValorAluguel,
                Status = StatusImovel.Disponivel,
                CriadoEm = DateTime.UtcNow
            };

            _context.Imoveis.Add(imovel);
            await _context.SaveChangesAsync();
            return MapearParaDto(imovel);
        }

        private static ImovelResponseDto MapearParaDto(Imovel imovel)
        {
            return new ImovelResponseDto
            {
                Id = imovel.Id,
                Endereco = imovel.Endereco,
                ValorAluguel = imovel.ValorAluguel,
                Status = imovel.Status,
                CriadoEm = imovel.CriadoEm
            };
        }
    }
}
