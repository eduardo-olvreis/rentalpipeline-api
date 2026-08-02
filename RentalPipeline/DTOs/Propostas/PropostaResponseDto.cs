using RentalPipeline.Entities.Enums;

namespace RentalPipeline.DTOs.Propostas
{
    public class PropostaResponseDto
    {
        public Guid Id { get; set; }
        public Guid ImovelId { get; set; }
        public Guid ClienteId { get; set; }
        public StatusProposta Status { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime AtualizadoEm { get; set; }
    }
}
