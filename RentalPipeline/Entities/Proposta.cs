using RentalPipeline.Entities.Enums;

namespace RentalPipeline.Entities
{
    public class Proposta
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ImovelId { get; set; }
        public Guid ClienteId { get; set; }
        public StatusProposta Status { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

        public Imovel Imovel { get; set; } = null!;
        public Cliente Cliente { get; set; } = null!;
    }
}
