namespace RentalPipeline.Services
{
    public interface INotificadorCondominioService
    {
        Task NotificarAtivacaoContratoAsync(Guid propostaId, Guid imovelId);
    }
}
