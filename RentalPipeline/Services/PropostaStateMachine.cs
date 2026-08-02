using RentalPipeline.Entities.Enums;
using RentalPipeline.Exceptions;

namespace RentalPipeline.Services
{
    public class PropostaStateMachine
    {
        private static readonly Dictionary<StatusProposta, StatusProposta[]> TransicoesPermitidas = new()
        {
            [StatusProposta.Nova] = [StatusProposta.AnaliseCredito, StatusProposta.Reprovada, StatusProposta.Cancelada],
            [StatusProposta.AnaliseCredito] = [StatusProposta.ContratoEmitido, StatusProposta.Reprovada, StatusProposta.Cancelada],
            [StatusProposta.ContratoEmitido] = [StatusProposta.Assinado, StatusProposta.Reprovada, StatusProposta.Cancelada],
            [StatusProposta.Assinado] = [StatusProposta.Ativo, StatusProposta.Reprovada, StatusProposta.Cancelada],
            [StatusProposta.Ativo] = [],
            [StatusProposta.Reprovada] = [],
            [StatusProposta.Cancelada] = []
        };

        public static void ValidarTransicao(StatusProposta statusAtual, StatusProposta novoStatus)
        {
            if (statusAtual == novoStatus)
            {
                throw new RegraDeNegocioException($"A proposta já está no status '{statusAtual}'.");
            }

            if (!TransicoesPermitidas.TryGetValue(statusAtual, out var permitidos) || !permitidos.Contains(novoStatus))
            {
                throw new RegraDeNegocioException(
                    $"Transição inválida: Não é permitido alterar o status da proposta de '{statusAtual}' para '{novoStatus}'."
                );
            }
        }
    }
}
