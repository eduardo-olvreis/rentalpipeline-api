using Microsoft.AspNetCore.Mvc;
using RentalPipeline.DTOs.Propostas;
using RentalPipeline.Services;

namespace RentalPipeline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropostasController : ControllerBase
    {
        private readonly IPropostaService _propostaService;
        public PropostasController(IPropostaService propostaService)
        {
            _propostaService = propostaService;
        }

        [HttpPost]
        public async Task<ActionResult<PropostaResponseDto>> Criar([FromBody] PropostaCreateDto dto)
        {
            var proposta = await _propostaService.CriarPropostaAsync(dto);
            return CreatedAtAction(nameof(ObterHistorico), new { id = proposta.Id }, proposta);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<PropostaResponseDto>> AtualizarStatus(Guid id, [FromBody] PropostaUpdateStatusDto dto)
        {
            var proposta = await _propostaService.AtualizarStatusAsync(id, dto);
            return Ok(proposta);
        }

        [HttpGet("{id:guid}/historico")]
        public async Task<ActionResult<IEnumerable<HistoricoPropostaResponseDto>>> ObterHistorico(Guid id)
        {
            var historico = await _propostaService.ObterHistoricoAsync(id);
            return Ok(historico);
        }
    }
}
