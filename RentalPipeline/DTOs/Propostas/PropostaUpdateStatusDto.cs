using RentalPipeline.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace RentalPipeline.DTOs.Propostas
{
    public class PropostaUpdateStatusDto
    {
        [Required(ErrorMessage = "O novo status é obrigatório.")]
        public StatusProposta NovoStatus { get; set; }
    }
}
