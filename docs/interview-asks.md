# Simulado de Entrevista Técnica — Avaliador vs. Candidato
## BLOCO 1 — Clean Architecture
### P1: Explique como você organizou as camadas do projeto e por que essa estrutura segue Clean Architecture.

R: O projeto tem quatro projetos separados com dependências unidirecionais:

TaskManager.Domain — entidades, enums, exceções. Sem dependências externas.
TaskManager.Application — serviços, DTOs, interfaces de repositório. Depende apenas do Domain.
TaskManager.Infrastructure — repositórios SQL, JWT, hash de senha. Depende do Application e Domain.
TaskManager.Api — controllers, middleware. Depende do Application.
A Infrastructure nunca é referenciada pelo Domain ou Application — a inversão de dependência é feita via interfaces: ITaskRepository, IUserRepository, IPasswordHasher, IJwtTokenGenerator ficam no Application, e as implementações concretas ficam na Infrastructure. O Domain não conhece nenhuma camada acima dele.

### P2: Por que as interfaces dos repositórios estão na camada Application e não na Infrastructure?

R: Porque a Application precisa depender da abstração, não da implementação. Se ITaskRepository estivesse na Infrastructure, a Application teria que referenciar a Infrastructure para usar a interface — quebrando o Dependency Inversion Principle. Com a interface no Application, o TaskService declara sua dependência sem saber nada sobre SQL Server ou ADO.NET. O contêiner de DI em Program.cs faz o binding concreto em runtime.

### P3: O Domain poderia referenciar qualquer NuGet externo como System.Text.Json ou FluentValidation?

R: Não. O Domain deve ser o núcleo completamente independente. No projeto, o TaskManager.Domain.csproj não tem nenhum PackageReference — só o SDK base do .NET. Qualquer biblioteca externa introduzida no Domain criaria um acoplamento que viola o princípio de independência. Validações de negócio usam apenas exceções do próprio domínio (DomainValidationException).

### P4: Como você garante que os controllers não contêm regras de negócio?

R: O TasksController delega tudo ao TaskService. Por exemplo, em Create, o controller apenas chama await _taskService.CreateAsync(User.GetUserId(), request, cancellationToken) e retorna o resultado. Não há if, nenhuma validação de campo e nenhum acesso a repositório direto no controller. A única responsabilidade do controller é HTTP: receber o request, extrair o userId do JWT via ClaimsPrincipalExtensions, chamar o serviço e mapear o resultado para o status code correto.

# BLOCO 2 — Testes e TDD
### P5: Você tem três tipos de teste. Qual a responsabilidade de cada um?

R:

Unit tests (TaskManager.UnitTests): testam regras de negócio puras sem I/O. TaskItemTests valida todas as regras do Domain como título vazio, data passada, limite de caracteres. TaskServiceTests usa NSubstitute para mockar o repositório e verifica que o serviço chama os métodos certos com os argumentos certos.
Integration tests (TaskManager.IntegrationTests): usam Testcontainers para subir um SQL Server real em Docker durante o teste. SqlTaskRepositoryTests verifica insert, update, delete e isolamento de dados por usuário contra o banco real.
API tests (TaskManager.ApiTests): usam WebApplicationFactory<Program> para subir o pipeline HTTP completo em memória. TaskCrudEndpointTests faz chamadas HTTP reais e verifica status codes e bodies. UserIsolationTests e UnauthorizedAccessTests verificam segurança e autorização end-to-end.
P6: No UserIsolationTests, o teste User_A_Should_Not_Delete_Task_From_User_B tem um comportamento inesperado — o delete retorna NoContent. Por que isso acontece e é um problema?

R: Sim, é uma fragilidade de design. O DELETE no repositório faz DELETE FROM dbo.Tasks WHERE Id = @Id AND UserId = @UserId. Quando o User B tenta deletar a task do User A, o SQL executa sem erros mas afeta 0 linhas — o banco simplesmente não encontra a combinação. O controller chama ExecuteNonQueryAsync e retorna NoContent sem verificar rows affected. O resultado é que o User B recebe 204 (success) mas a task do User A continua intacta — verificado no teste pelo GET seguinte. Para produção, o ideal seria verificar as linhas afetadas e retornar 404 quando zero. Para a entrevista, o comportamento está documentado e testado, e os dados do usuário permanecem protegidos.

###P7: Por que você usou NSubstitute e não Moq para os mocks?

R: NSubstitute tem uma API mais fluente e menos verbosa para .NET moderno. Substitute.For<ITaskRepository>() é mais limpo que new Mock<ITaskRepository>(). A verificação de chamadas é direta: await _repository.Received(1).CreateAsync(Arg.Is<TaskItem>(x => x.UserId == userId), ...). Ambos resolvem o mesmo problema — é uma preferência de estilo, e o projeto documenta essa escolha no docs/testing.md.

### P8: Como os testes de integração funcionam sem precisar de um SQL Server instalado localmente?

R: O SqlServerTestFixture usa Testcontainers for .NET. Quando o test runner inicia, o Testcontainers sobe um container Docker com mcr.microsoft.com/mssql/server:2022-latest, aguarda o health check, executa os scripts de schema e roda os testes contra esse banco real. Quando os testes terminam, o container é destruído. Isso garante que os testes de integração são idempotentes e rodam em qualquer CI que tenha Docker disponível.

# BLOCO 3 — Segurança
### P9: Como as senhas são armazenadas e por que não usar BCrypt?

R: O projeto usa PBKDF2-SHA256 via Rfc2898DeriveBytes.Pbkdf2 da BCL do .NET, com 100.000 iterações e salt de 16 bytes aleatórios por RandomNumberGenerator. O formato armazenado é PBKDF2-SHA256$100000$<salt_base64>$<hash_base64>. BCrypt é uma alternativa igualmente válida, mas PBKDF2 é o algoritmo recomendado pelo NIST SP 800-132 e já está na biblioteca padrão do .NET sem dependência externa. A comparação usa CryptographicOperations.FixedTimeEquals para evitar timing attacks.

### P10: Como o projeto previne SQL Injection?

R: Todo acesso ao banco usa SqlParameter explícito — nunca concatenação de string. Por exemplo, em GetByIdAsync: command.Parameters.Add(new SqlParameter("@Id", id)). Não existe SELECT * ou interpolação de string com dados do usuário em nenhuma query do projeto. Isso foi um item explícito da checklist de validação do código gerado por AI no docs/code-generation.md.

### P11: O que o JWT valida e como o userId chega no controller?

R: O JWT é validado pelo middleware do ASP.NET com ValidateIssuer, ValidateAudience, ValidateLifetime e ValidateIssuerSigningKey — todos true. O token expira em 2 horas. No payload, o userId é armazenado como claim ClaimTypes.NameIdentifier. A classe ClaimsPrincipalExtensions tem o método GetUserId() que extrai esse claim do User (ClaimsPrincipal) do controller — assim nenhum endpoint de task precisa receber userId no body ou na query string. O usuário não pode falsificar o userId sem a chave secreta JWT.

# BLOCO 4 — ADO.NET e Dados
### P12: Por que ADO.NET e não Entity Framework ou Dapper?

R: Era um requisito do assignment. Mas além disso, ADO.NET demonstra controle total sobre SQL, parâmetros, mapeamento manual e conexões. O SqlTaskRepository tem await connection.OpenAsync(cancellationToken) explícito, await using para garantir dispose assíncrono, e mapeamento manual via reader.GetGuid(0), reader.GetString(2) etc. Isso mostra que o desenvolvedor entende o que os ORMs abstraem. A desvantagem é mais código boilerplate de mapeamento, mas para este escopo CRUD é totalmente gerenciável.

### P13: O que é ISqlConnectionFactory e por que ele existe?

R: É uma abstração que encapsula a criação de SqlConnection. Registrado como Singleton no DI com a connection string. Isso permite que os repositórios recebam a factory via injeção sem conhecer a connection string diretamente. Também facilita substituição em testes — se necessário, a factory pode ser trocada por uma versão de teste. Os repositórios fazem await using var connection = _connectionFactory.CreateConnection() e abrem a conexão por operação, o que é o padrão correto para ADO.NET (sem connection pooling manual — o SQL Client já faz pooling internamente).

# BLOCO 5 — Frontend Angular
### P14: Por que Angular standalone components e não NgModules?

R: Angular 17+ adota standalone como padrão. Standalone elimina a necessidade de NgModule como intermediário — cada componente declara suas próprias dependências no array imports. É mais simples, mais tree-shakable e alinhado com a direção do framework. O TaskListComponent por exemplo importa [ReactiveFormsModule, NgFor, NgIf, NgClass, DatePipe, AiSuggestionDialogComponent] diretamente no decorator.

### P15: Como o frontend lida com autenticação? Onde o token é armazenado e como vai para as requisições?

R: O token é armazenado no localStorage após login. Um HttpInterceptor funcional (não classe) intercepta todas as requisições HTTP e adiciona o header Authorization: Bearer <token> automaticamente. Um functional guard (authGuard) protege as rotas autenticadas verificando se há token válido. Assim nenhum componente precisa gerenciar o token manualmente — toda a lógica de auth é centralizada no interceptor e no guard.

### P16: Como o frontend se comunica com o backend em Docker sem CORS issues?

R: O nginx do container frontend tem um proxy_pass: requisições para /api/ são proxiadas para http://api:8080/api/. Do ponto de vista do browser, não há cross-origin — todas as requisições vão para localhost:4200. O nginx resolve o roteamento internamente na rede Docker. O environment.prod.ts usa apiBaseUrl: '/api' (caminho relativo) para aproveitar esse proxy.

# BLOCO 6 — GenAI e Pensamento Crítico
### P17: Que prompt você usou para gerar o projeto e como validou o output?

R: O prompt está documentado em docs/code-generation.md. As constraints principais foram explicitadas no próprio prompt: sem EF, sem Dapper, sem Mediator, ADO.NET, Clean Architecture, JWT, testes com xUnit/NSubstitute, Docker. A validação seguiu 10 regras de aceitação — por exemplo: "Repository interfaces must be in Application", "SQL commands must use parameters", "Passwords must never be stored as plain text". Cada regra foi verificada manualmente antes de aceitar o código gerado.

### P18: Quais correções você precisou fazer no código gerado por AI?

R: Documentadas em "Typical AI Corrections" no docs/code-generation.md: AI tipicamente gerava com EF e foi corrigido para ADO.NET; queries sem parâmetros foram parametrizadas; validações que estavam nos controllers foram movidas para o Domain; senhas em plain text foram substituídas por PBKDF2; queries sem WHERE UserId = @UserId foram corrigidas para isolar dados por usuário; faltavam CancellationToken nos métodos assíncronos. Cada uma dessas correções é verificável no código atual.

### P19: Como você garante que a AI não introduziu uma vulnerabilidade de segurança?

R: Pela checklist de segurança em docs/security.md e docs/review.md: verificar SQL sem concatenação, verificar hash de senha, verificar que o JWT valida issuer/audience/lifetime/key, verificar que queries de task sempre filtram por UserId. Os testes de UserIsolationTests são a prova automatizada de que um usuário não acessa dados de outro — se a AI tivesse gerado uma query sem o filtro de usuário, esses testes falhariam.

### P20: O assignment diz "use of TDD is preferable". Como você aplicou TDD?

R: As regras de domínio foram escritas com testes primeiro — Constructor_Should_Fail_When_DueDate_Is_In_The_Past define o comportamento esperado antes da implementação do Validate() no TaskItem. O mesmo padrão foi aplicado para title vazio, description vazia, userId inválido. Os testes de serviço definiram a interface do TaskService antes da implementação final. A naming convention MethodName_Should_ExpectedBehavior_When_Condition foi seguida consistentemente e está documentada em docs/testing.md.

# BLOCO 7 — Perguntas de Aprofundamento (Wildcards)
### P21: Se precisasse adicionar paginação na listagem de tasks, como faria sem quebrar a arquitetura?

R: Adicionaria PaginationQuery (page, pageSize) e PagedResult<T> no Application layer. ITaskRepository.GetByUserIdAsync receberia os parâmetros de paginação. A query SQL usaria OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY. O endpoint GET retornaria um envelope com { items: [], total: N, page: N, pageSize: N }. O Domain não seria alterado. O frontend consumiria os metadados de paginação para renderizar controles de navegação.

### P22: Por que o Result<T> existe em vez de lançar exceções diretamente nos serviços?

R: Erros esperados de negócio (usuário não encontrado, email já cadastrado) não são exceções — são outcomes válidos do fluxo. Result<T> torna explícito no tipo de retorno que a operação pode falhar. O controller lê result.IsSuccess e decide o status HTTP sem precisar de try/catch. Exceções de domínio (DomainValidationException) são para invariantes quebradas — casos que não deveriam ocorrer se o input foi validado. O middleware ExceptionHandlingMiddleware captura essas como último recurso e retorna 400.

### P23: O que acontece se o SQL Server não estiver disponível quando a API inicializa?

R: O DatabaseInitializer executa em Program.cs antes de app.Run(). Se o SQL Server não responder, uma SqlException é lançada e a aplicação falha na inicialização — o que é o comportamento correto: é melhor falhar rápido do que subir em estado inconsistente. No Docker Compose, a API depende de sqlserver-init: condition: service_completed_successfully, que por sua vez depende do health check do SQL Server. Isso garante que o banco está pronto antes da API tentar conectar.