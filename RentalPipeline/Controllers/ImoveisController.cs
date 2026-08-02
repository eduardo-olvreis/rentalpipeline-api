using Microsoft.AspNetCore.Mvc;
using RentalPipeline.DTOs.Imoveis;
using RentalPipeline.Services;

namespace RentalPipeline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImoveisController : ControllerBase
    {
        private readonly IImovelService _imovelService;
        public ImoveisController(IImovelService imovelService)
        {
            _imovelService = imovelService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImovelResponseDto>>> ObterTodos()
        {
            var imoveis = await _imovelService.ObterTodosAsync();
            return Ok(imoveis);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ImovelResponseDto>> ObterPorId(Guid id)
        {
            var imovel = await _imovelService.ObterPorIdAsync(id);
            return Ok(imovel);
        }

        [HttpPost]
        public async Task<ActionResult<ImovelResponseDto>> Criar([FromBody] ImovelCreateDto dto)
        {
            var imovel = await _imovelService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = imovel.Id }, imovel);
        }
    }
}
