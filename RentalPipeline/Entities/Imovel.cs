using RentalPipeline.Entities.Enums;

namespace RentalPipeline.Entities
{
    public class Imovel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Endereco { get; set; } = string.Empty;
        public decimal ValorAluguel { get; set; }
        public StatusImovel Status { get; set; } = StatusImovel.Disponivel;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
