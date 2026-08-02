using System.ComponentModel.DataAnnotations;

namespace RentalPipeline.DTOs.Propostas
{
    public class PropostaCreateDto
    {
        [Required(ErrorMessage = "O ID do imóvel é obrigatório.")]
        public Guid ImovelId { get; set; }

        [Required(ErrorMessage = "O ID do cliente é obrigatório.")]
        public Guid ClienteId { get; set; }
    }
}
