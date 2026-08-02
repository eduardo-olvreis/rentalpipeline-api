# Rental Pipeline API — Auxiliadora Predial

API REST para gerenciamento da esteira de contratos de locação de imóveis, desenvolvida para o Desafio Técnico da Auxiliadora Predial. O sistema garante o controle estrito do fluxo da proposta e a sincronização do estado dos imóveis.

---

## Arquitetura e Decisões de Design

A aplicação foi desenvolvida utilizando .NET 10 (Web API) e PostgreSQL.

### Estrutura do Projeto
- **Entities:** Contém os modelos de domínio e a máquina de estados pura (`PropostaStateMachine`), isolada de dependências externas.
- **Services:** Camada de aplicação responsável pela orquestração dos casos de uso, controle de transações no banco de dados e disparo de eventos.
- **Data (EF Core):** Mapeamento relacional e contexto de banco de dados (`AppDbContext`).
- **DTOs:** Objetos de transferência de dados usados para validação de contratos e isolamento das entidades no transporte público da API.
- **Middlewares:** Tratamento global e centralizado de exceções (`GlobalExceptionMiddleware`), garantindo respostas HTTP padronizadas.

---

## Fluxo da Esteira e Regras de Negócio

A evolução da proposta de locação segue uma sequência imutável de estados:

[ NOVA ] ---> [ ANALISE_CREDITO ] ---> [ CONTRATO_EMITIDO ] ---> [ ASSINADO ] ---> [ ATIVO ]
|                  |                       |                      |
+------------------+-----------------------+----------------------+
|
v
[ REPROVADA / CANCELADA ]


### Regras de Transição e Integridade:
1. **Atribuição do Imóvel:** Uma proposta só pode ser criada para imóveis no status `Disponivel`. No momento da criação, o status do imóvel passa para `EmNegociacao`.
2. **Máquina de Estados:** Pulos de etapas na esteira são proibidos. Caso ocorra uma tentativa de transição inválida, a API retorna código `422 Unprocessable Entity`.
3. **Conclusão:** Ao atingir o status `Ativo`, o imóvel passa permanentemente para `Alugado`.
4. **Reversão:** Se a proposta for `Reprovada` ou `Cancelada`, o imóvel reverte automaticamente para `Disponivel`, permitindo novas negociações.

---

## Soluções para Requisitos Avançados

### 1. Prevenção de Concorrência (Race Conditions)
Para impedir que requisições simultâneas no mesmo milissegundo criem múltiplas propostas para o mesmo imóvel disponível, a criação de propostas é executada dentro de uma transação com isolamento no banco de dados. A verificação do status do imóvel e a alteração para `EmNegociacao` ocorrem de forma atômica no PostgreSQL.

### 2. Trilha de Auditoria (Audit Trail)
Todas as alterações de status de uma proposta persistem um registro na entidade `HistoricoStatusProposta`. O histórico contém:
- Identificador da proposta;
- Status anterior e novo status;
- Carimbo de data/hora em UTC.

O histórico pode ser consultado via endpoint de auditoria.

### 3. Arquitetura Orientada a Eventos
A aplicação simula uma arquitetura de eventos através da interface `INotificadorCondominioService`. Ao transicionar uma proposta para o status `Ativo`, o serviço dispara uma notificação desacoplada simulando o envio de dados para o sistema financeiro do condomínio.

---

## Endpoints Principais

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| `POST` | `/api/clientes` | Cadastro de novos clientes |
| `POST` | `/api/imoveis` | Cadastro de imóveis (iniciam como `Disponivel`) |
| `POST` | `/api/propostas` | Criação de proposta (altera imóvel para `EmNegociacao`) |
| `PUT` | `/api/propostas/{id}/status` | Atualiza status da proposta na máquina de estados |
| `GET` | `/api/propostas/{id}/historico` | Consulta a trilha de auditoria da proposta |

---

## Instruções de Configuração e Execução

### Pré-requisitos
- Docker Engine e Docker Compose instalados; ou
- SDK do .NET 10.0 e PostgreSQL local.

---

### Opção 1: Execução via Docker Compose (Recomendado)

O projeto possui containerização completa para a API e o banco de dados PostgreSQL.

1. Na raiz do projeto, execute o comando de orquestração:
docker-compose up --build

A API iniciará na porta 8080.

A documentação OpenAPI / Scalar estará disponível em:

http://localhost:8080/scalar/v1

---

### Opção 2: Execução Local via Visual Studio / IIS Express

Certifique-se de ter um banco PostgreSQL rodando e ajuste a string de conexão no appsettings.json.

Execute as migrações do banco de dados:

dotnet ef database update --project RentalPipeline

Inicie a aplicação via Visual Studio (F5) ou via terminal:

dotnet run --project RentalPipeline

A documentação no ambiente local estará disponível na porta configurada pelo perfil de desenvolvimento:

Plaintext
https://localhost:7026/scalar/v1

---

### Execução dos Testes Automatizados
A suíte de testes unitários e de integração utiliza xUnit, FluentAssertions e Moq. Os testes cobrem a validação das regras da máquina de estados, transições de status do imóvel e concorrência.

Para executar os testes, rode o comando abaixo no terminal na raiz do projeto:

dotnet test

Cobertura dos Testes:
PropostaStateMachineTests: Garante que apenas transições permitidas sejam aceitas e valida mensagens de exceção de regras de negócio.

PropostaServiceTests: Valida a alteração de status do imóvel, revertibilidade ao cancelar/reprovar e o disparo da notificação de evento.

ConcorrenciaPropostaTests: Simula requisições simultâneas para garantir que o isolamento no banco impeça concorrência.