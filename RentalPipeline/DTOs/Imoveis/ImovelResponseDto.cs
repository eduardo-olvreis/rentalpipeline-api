using RentalPipeline.Entities.Enums;

namespace RentalPipeline.DTOs.Imoveis
{
    public class ImovelResponseDto
    {
        public Guid Id { get; set; }
        public string Endereco { get; set; } = string.Empty;
        public decimal ValorAluguel { get; set; }
        public StatusImovel Status { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
