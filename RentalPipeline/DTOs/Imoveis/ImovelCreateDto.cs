using System.ComponentModel.DataAnnotations;

namespace RentalPipeline.DTOs.Imoveis
{
    public class ImovelCreateDto
    {
        [Required(ErrorMessage = "O endereço é obrigatório.")]
        public string Endereco { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do aluguel deve ser maior que zero.")]
        public decimal ValorAluguel { get; set; }
    }
}
