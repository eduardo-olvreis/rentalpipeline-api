namespace RentalPipeline.Services
{
    public class NotificadorCondominioService : INotificadorCondominioService
    {
        private readonly ILogger<NotificadorCondominioService> _logger;

        public NotificadorCondominioService(ILogger<NotificadorCondominioService> logger)
        {
            _logger = logger;
        }

        public Task NotificarAtivacaoContratoAsync(Guid propostaId, Guid imovelId)
        {
            _logger.LogInformation("Evento emitido: Proposta {PropostaId} do imóvel {ImovelId} ativada. Notificando sistema financeiro do condomínio.", propostaId, imovelId);
            return Task.CompletedTask;
        }
    }
}
