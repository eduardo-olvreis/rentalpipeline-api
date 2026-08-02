using RentalPipeline.Entities.Enums;

namespace RentalPipeline.Entities
{
    public class HistoricoProposta
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PropostaId { get; set; }
        public StatusProposta StatusAnterior { get; set; }
        public StatusProposta StatusNovo { get; set; }
        public DateTime CriadoEm { get; set; }

        public Proposta Proposta { get; set; } = null!;
    }
}
