using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using RentalPipeline.Data;
using RentalPipeline.DTOs.Propostas;
using RentalPipeline.Entities;
using RentalPipeline.Entities.Enums;
using RentalPipeline.Exceptions;
using RentalPipeline.Services;

namespace RentalPipeline.Tests
{
    public class ConcorrenciaPropostaTests
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
        public async Task CriarPropostaAsync_RequisicoesSimultaneas_ApenasUmaDeveTerSucesso()
        {
            using var context = ObterDbContextEmMemoria();
            var loggerMock = new Mock<ILogger<PropostaService>>();
            var c1 = new Cliente { Id = Guid.NewGuid(), Nome = "Cliente 1", Email = "c1@test.com", Cpf = "111" };
            var c2 = new Cliente { Id = Guid.NewGuid(), Nome = "Cliente 2", Email = "c2@test.com", Cpf = "222" };
            var imovel = new Imovel { Id = Guid.NewGuid(), Endereco = "Av. Carlos Gomes, 1000", ValorAluguel = 3000, Status = StatusImovel.Disponivel };
            context.Clientes.AddRange(c1, c2);
            context.Imoveis.Add(imovel);
            await context.SaveChangesAsync();
            var notificadorMock = new Mock<INotificadorCondominioService>();
            var service = new PropostaService(context, loggerMock.Object, notificadorMock.Object);
            var task1 = service.CriarPropostaAsync(new PropostaCreateDto { ClienteId = c1.Id, ImovelId = imovel.Id });
            var task2 = service.CriarPropostaAsync(new PropostaCreateDto { ClienteId = c2.Id, ImovelId = imovel.Id });
            var excecoes = 0;
            var sucessos = 0;
            try { await task1; sucessos++; } catch (RegraDeNegocioException) { excecoes++; }
            try { await task2; sucessos++; } catch (RegraDeNegocioException) { excecoes++; }
            sucessos.Should().Be(1);
            excecoes.Should().Be(1);
        }
    }
}