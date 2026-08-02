using Microsoft.EntityFrameworkCore;
using RentalPipeline.Data;
using RentalPipeline.DTOs.Propostas;
using RentalPipeline.Entities;
using RentalPipeline.Entities.Enums;
using RentalPipeline.Exceptions;

namespace RentalPipeline.Services
{
    public class PropostaService : IPropostaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PropostaService> _logger;
        private readonly INotificadorCondominioService _notificadorCondominio;

        public PropostaService(AppDbContext context, ILogger<PropostaService> logger, INotificadorCondominioService notificadorCondominio)
        {
            _context = context;
            _logger = logger;
            _notificadorCondominio = notificadorCondominio;
        }

        public async Task<PropostaResponseDto> CriarPropostaAsync(PropostaCreateDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var imovel = await _context.Imoveis.FirstOrDefaultAsync(i => i.Id == dto.ImovelId);
                if (imovel == null)
                {
                    throw new NaoEncontradoException("Imóvel não encontrado.");
                }

                if (imovel.Status != StatusImovel.Disponivel)
                {
                    throw new RegraDeNegocioException("O imóvel não está disponível para receber novas propostas.");
                }

                var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == dto.ClienteId);
                if (!clienteExiste)
                {
                    throw new NaoEncontradoException("Cliente não encontrado.");
                }

                var proposta = new Proposta
                {
                    Id = Guid.NewGuid(),
                    ImovelId = dto.ImovelId,
                    ClienteId = dto.ClienteId,
                    Status = StatusProposta.Nova,
                    CriadoEm = DateTime.UtcNow,
                    AtualizadoEm = DateTime.UtcNow
                };

                imovel.Status = StatusImovel.EmNegociacao;
                var historico = new HistoricoProposta
                {
                    Id = Guid.NewGuid(),
                    PropostaId = proposta.Id,
                    StatusAnterior = StatusProposta.Nova,
                    StatusNovo = StatusProposta.Nova,
                    CriadoEm = DateTime.UtcNow
                };

                _context.Propostas.Add(proposta);
                _context.HistoricoPropostas.Add(historico);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return MapearParaDto(proposta);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Conflito de concorrência detectado ao criar proposta para o imóvel {ImovelId}", dto.ImovelId);
                throw new ConflitoConcorrenciaException("Conflito ao processar a requisição. O imóvel pode ter sido alterado por outra operação.");
            }
        }

        public async Task<PropostaResponseDto> AtualizarStatusAsync(Guid id, PropostaUpdateStatusDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var proposta = await _context.Propostas.Include(p => p.Imovel).FirstOrDefaultAsync(p => p.Id == id);
            if (proposta == null)
            {
                throw new NaoEncontradoException("Proposta não encontrada.");
            }

            PropostaStateMachine.ValidarTransicao(proposta.Status, dto.NovoStatus);
            var statusAnterior = proposta.Status;
            proposta.Status = dto.NovoStatus;
            proposta.AtualizadoEm = DateTime.UtcNow;

            if (dto.NovoStatus == StatusProposta.Ativo)
            {
                proposta.Imovel.Status = StatusImovel.Alugado;
                await _notificadorCondominio.NotificarAtivacaoContratoAsync(proposta.Id, proposta.ImovelId);
            }
            else if (dto.NovoStatus is StatusProposta.Reprovada or StatusProposta.Cancelada)
            {
                proposta.Imovel.Status = StatusImovel.Disponivel;
            }

            var historico = new HistoricoProposta
            {
                Id = Guid.NewGuid(),
                PropostaId = proposta.Id,
                StatusAnterior = statusAnterior,
                StatusNovo = dto.NovoStatus,
                CriadoEm = DateTime.UtcNow
            };

            _context.HistoricoPropostas.Add(historico);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return MapearParaDto(proposta);
        }

        public async Task<IEnumerable<HistoricoPropostaResponseDto>> ObterHistoricoAsync(Guid propostaId)
        {
            var propostaExiste = await _context.Propostas.AnyAsync(p => p.Id == propostaId);
            if (!propostaExiste)
                throw new NaoEncontradoException("Proposta não encontrada.");

            return await _context.HistoricoPropostas.Where(h => h.PropostaId == propostaId).OrderBy(h => h.CriadoEm).Select(h => new HistoricoPropostaResponseDto
            {
                Id = h.Id,
                PropostaId = h.PropostaId,
                StatusAnterior = h.StatusAnterior,
                StatusNovo = h.StatusNovo,
                CriadoEm = h.CriadoEm
            }).ToListAsync();
        }

        private static PropostaResponseDto MapearParaDto(Proposta proposta)
        {
            return new PropostaResponseDto
            {
                Id = proposta.Id,
                ImovelId = proposta.ImovelId,
                ClienteId = proposta.ClienteId,
                Status = proposta.Status,
                CriadoEm = proposta.CriadoEm,
                AtualizadoEm = proposta.AtualizadoEm
            };
        }
    }
}