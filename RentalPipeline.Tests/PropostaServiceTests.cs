using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using RentalPipeline.Data;
using RentalPipeline.DTOs.Propostas;
using RentalPipeline.Entities;
using RentalPipeline.Entities.Enums;
using RentalPipeline.Services;

namespace RentalPipeline.Tests
{
    public class PropostaServiceTests
    {
        private AppDbContext ObterDbContextEmMemoria()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CriarPropostaAsync_DeveAtualizarStatusDoImovelParaEmNegociacao()
        {
            using var context = ObterDbContextEmMemoria();
            var loggerMock = new Mock<ILogger<PropostaService>>();
            var notificadorMock = new Mock<INotificadorCondominioService>();
            var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "Eduardo", Email = "eduardo@test.com", Cpf = "12345678900" };
            var imovel = new Imovel { Id = Guid.NewGuid(), Endereco = "Rua A, 100", ValorAluguel = 1500, Status = StatusImovel.Disponivel };
            context.Clientes.Add(cliente);
            context.Imoveis.Add(imovel);
            await context.SaveChangesAsync();
            var service = new PropostaService(context, loggerMock.Object, notificadorMock.Object);
            var resultado = await service.CriarPropostaAsync(new PropostaCreateDto { ClienteId = cliente.Id, ImovelId = imovel.Id });
            resultado.Status.Should().Be(StatusProposta.Nova);
            var imovelAtualizado = await context.Imoveis.FindAsync(imovel.Id);
            imovelAtualizado!.Status.Should().Be(StatusImovel.EmNegociacao);
        }

        [Fact]
        public async Task AtualizarStatusAsync_DeveAlterarImovelParaAlugadoEDispararEvento_QuandoAtivo()
        {
            using var context = ObterDbContextEmMemoria();
            var loggerMock = new Mock<ILogger<PropostaService>>();
            var notificadorMock = new Mock<INotificadorCondominioService>();
            var imovel = new Imovel { Id = Guid.NewGuid(), Endereco = "Rua B, 200", ValorAluguel = 2500, Status = StatusImovel.EmNegociacao };
            var proposta = new Proposta { Id = Guid.NewGuid(), ImovelId = imovel.Id, Status = StatusProposta.Assinado, Imovel = imovel };
            context.Imoveis.Add(imovel);
            context.Propostas.Add(proposta);
            await context.SaveChangesAsync();
            var service = new PropostaService(context, loggerMock.Object, notificadorMock.Object);
            await service.AtualizarStatusAsync(proposta.Id, new PropostaUpdateStatusDto { NovoStatus = StatusProposta.Ativo });
            imovel.Status.Should().Be(StatusImovel.Alugado);
            notificadorMock.Verify(
                x => x.NotificarAtivacaoContratoAsync(proposta.Id, imovel.Id),
                Times.Once);
        }

        [Fact]
        public async Task AtualizarStatusAsync_DeveVoltarImovelParaDisponivel_QuandoCancelado()
        {
            using var context = ObterDbContextEmMemoria();
            var loggerMock = new Mock<ILogger<PropostaService>>();
            var notificadorMock = new Mock<INotificadorCondominioService>();
            var imovel = new Imovel { Id = Guid.NewGuid(), Endereco = "Rua C, 300", ValorAluguel = 1800, Status = StatusImovel.EmNegociacao };
            var proposta = new Proposta { Id = Guid.NewGuid(), ImovelId = imovel.Id, Status = StatusProposta.Nova, Imovel = imovel };
            context.Imoveis.Add(imovel);
            context.Propostas.Add(proposta);
            await context.SaveChangesAsync();
            var service = new PropostaService(context, loggerMock.Object, notificadorMock.Object);
            await service.AtualizarStatusAsync(proposta.Id, new PropostaUpdateStatusDto { NovoStatus = StatusProposta.Cancelada });
            imovel.Status.Should().Be(StatusImovel.Disponivel);
        }
    }
}
