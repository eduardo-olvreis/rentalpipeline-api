using RentalPipeline.Entities.Enums;

namespace RentalPipeline.DTOs.Propostas
{
    public class HistoricoPropostaResponseDto
    {
        public Guid Id { get; set; }
        public Guid PropostaId { get; set; }
        public StatusProposta StatusAnterior { get; set; }
        public StatusProposta StatusNovo { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
