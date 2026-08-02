using FluentAssertions;
using RentalPipeline.Entities.Enums;
using RentalPipeline.Exceptions;
using RentalPipeline.Services;

namespace RentalPipeline.Tests
{
    public class PropostaStateMachineTests
    {
        [Theory]
        [InlineData(StatusProposta.Nova, StatusProposta.AnaliseCredito)]
        [InlineData(StatusProposta.AnaliseCredito, StatusProposta.ContratoEmitido)]
        [InlineData(StatusProposta.ContratoEmitido, StatusProposta.Assinado)]
        [InlineData(StatusProposta.Assinado, StatusProposta.Ativo)]
        [InlineData(StatusProposta.Nova, StatusProposta.Reprovada)]
        [InlineData(StatusProposta.Nova, StatusProposta.Cancelada)]
        [InlineData(StatusProposta.Assinado, StatusProposta.Cancelada)]
        public void ValidarTransicao_DevePermitirTransicoesValidas(StatusProposta atual, StatusProposta novo)
        {
            Action act = () => PropostaStateMachine.ValidarTransicao(atual, novo);
            act.Should().NotThrow();
        }

        [Theory]
        [InlineData(StatusProposta.Nova, StatusProposta.Assinado)]
        [InlineData(StatusProposta.Nova, StatusProposta.Ativo)]
        [InlineData(StatusProposta.AnaliseCredito, StatusProposta.Ativo)]
        [InlineData(StatusProposta.Ativo, StatusProposta.Cancelada)]
        [InlineData(StatusProposta.Reprovada, StatusProposta.AnaliseCredito)]
        public void ValidarTransicao_DeveLancarExcecao_QuandoTransicaoForInvalida(StatusProposta atual, StatusProposta novo)
        {
            Action act = () => PropostaStateMachine.ValidarTransicao(atual, novo);
            act.Should().Throw<RegraDeNegocioException>()
               .WithMessage($"Transição inválida: Não é permitido alterar o status da proposta de '{atual}' para '{novo}'.");
        }

        [Fact]
        public void ValidarTransicao_DeveLancarExcecao_QuandoStatusForIdentico()
        {
            Action act = () => PropostaStateMachine.ValidarTransicao(StatusProposta.Nova, StatusProposta.Nova);
            act.Should().Throw<RegraDeNegocioException>()
               .WithMessage("A proposta já está no status 'Nova'.");
        }
    }
}