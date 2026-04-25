# Good Hamburger - Sistema de Gestão de Pedidos

Esta é a solução para o Desafio Técnico (C# Developer Júnior) da construção de um sistema de registro de pedidos para a lanchonete "Good Hamburger".

O projeto implementa uma API RESTful em ASP.NET Core para o backend e um client rico em Blazor WebAssembly para o frontend, cumprindo todos os requisitos obrigatórios e os diferenciais.

---

## 🚀 Tecnologias Utilizadas

* **Backend:** C#, .NET 10, ASP.NET Core Web API
* **Frontend:** C#, Blazor WebAssembly, Bootstrap
* **Banco de Dados:** Entity Framework Core + SQL Server
* **Testes:** xUnit
* **Documentação de API:** Swagger / OpenAPI

---

## 🛠️ Como Executar o Projeto

O projeto utiliza o .NET 10 e um banco de dados SQL Server. 

### Pré-requisitos
* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* SQL Server (LocalDB ou instância rodando)

### Passo a Passo

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/VitorGH/GoodHambuguerApp.git
   cd GoodHambuguerApp
   ```

2. **Configuração do Banco de Dados:**
   A API usa a *Connection String* `DefaultConnection` configurada no arquivo `appsettings.json` do projeto `GoodHamburger.Api`. Por padrão, ela aponta para `localhost` usando Windows Authentication. Modifique-a se necessário.

3. **Executando a API (Backend):**
   A API aplica as *migrations* e popula o banco de dados (seed) automaticamente ao ser iniciada.
   ```bash
   dotnet run --project GoodHamburger.Api/GoodHamburger.Api.csproj
   ```
   * A API estará rodando em `http://localhost:5062` e `https://localhost:7087`.
   * Você pode testar os endpoints interativamente acessando a interface do **Swagger** em `http://localhost:5062`.

4. **Executando o Client (Frontend Blazor):**
   Em um **novo terminal**, execute:
   ```bash
   dotnet run --project GoodHamburger.Client/GoodHamburger.Client.csproj
   ```
   * O Frontend estará disponível em `http://localhost:5190`.

5. **Executando os Testes Automatizados:**
   A solução possui 19 testes unitários. Para executá-los:
   ```bash
   dotnet test
   ```

---

## 🏗️ Decisões de Arquitetura

1. **Separação em Projetos:**
   A *Solution* foi dividida em três projetos distintos:
   * `GoodHamburger.Api`: Contém a API REST, regras de negócio e acesso a dados.
   * `GoodHamburger.Client`: Interface de usuário em Blazor WebAssembly.
   * `GoodHamburger.Tests`: Testes unitários focados nas lógicas de negócios.

2. **Padrão Controller-Service-Repository (via EF):**
   Utilizou-se uma arquitetura baseada em serviços para manter os `Controllers` limpos. Toda a regra de negócio (cálculo de descontos, validação de limites) fica isolada em `OrderService`, facilitando muito a criação dos testes unitários. O Entity Framework atua como a camada de abstração de dados (Repository pattern embutido no `DbContext`).

3. **Global Exception Handling (Middleware):**
   Ao invés de encher os Controllers com blocos `try-catch`, foi implementado o `ExceptionHandlingMiddleware`. Exceções customizadas (como `DuplicateItemException` ou `InvalidOrderException`) são capturadas globalmente e transformadas em respostas HTTP padronizadas no formato `ProblemDetails` (ex: 400 Bad Request, 409 Conflict), melhorando a clareza para os clientes da API.

4. **Models vs DTOs:**
   Houve uma separação clara entre as entidades de banco de dados (`Order`, `MenuItem`) e os objetos de transferência de dados (`OrderResponse`, `CreateOrderRequest`). Isso evita o vazamento de detalhes do banco de dados (over-posting) para o frontend.

5. **Persistência Relacional com Seed Automático:**
   Foi utilizado o SQL Server com EF Core. Ao iniciar a aplicação, a API executa o `db.Database.Migrate()` para garantir que o banco existe e injeta o cardápio padrão via *Seed Data*. 

6. **Validação Híbrida (Frontend e Backend):**
   As regras de negócio foram implementadas robustamente na API (backend), mas o Frontend em Blazor também aplica as mesmas regras (ex: bloqueia o botão de adicionar um segundo sanduíche e calcula a prévia de desconto localmente) para melhorar a UX e reduzir requisições desnecessárias.

---

## ❌ O Que Ficou Fora do Escopo

* **Autenticação e Autorização:** Não foi implementado sistema de login (JWT ou Identity). Todos os endpoints são públicos, assumindo que é uma versão simples de caixa de lanchonete.
* **Paginação de Pedidos:** O endpoint `GET /api/orders` retorna todos os pedidos do banco. Em um cenário real de produção com milhares de pedidos, seria essencial adicionar paginação (ex: `?page=1&pageSize=20`).
* **Padrão Repository Explícito:** O `AppDbContext` foi injetado diretamente nos serviços. Como a aplicação é simples, criar interfaces formais como `IOrderRepository` traria uma complexidade desnecessária (*over-engineering*). O `DbSet` do EF já é uma implementação do padrão Repository.
* **Testes de Integração:** O projeto foca em testes unitários das regras de negócio (serviços) usando `InMemoryDatabase`. Testes de integração (E2E) chamando os endpoints com HttpClient ficaram de fora.

---

## 📜 Regras de Negócio e Descontos (Conforme Requisitos)

* **Regra 1 (20% de desconto)**: Pedidos contendo Sanduíche + batata + refrigerante.
* **Regra 2 (15% de desconto)**: Pedidos contendo Sanduíche + refrigerante.
* **Regra 3 (10% de desconto)**: Pedidos contendo Sanduíche + batata.
* **Validação de Quantidade**: Cada pedido pode conter no máximo um sanduíche, uma batata e um refrigerante.
* **Validação de Duplicidade**: A tentativa de adicionar itens duplicados retorna um erro de conflito amigável.