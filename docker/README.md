# Docker Configuration - TaskManager

Esta pasta contém todos os recursos Docker necessários para executar a aplicação TaskManager.

## 📁 Estrutura

```
docker/
├── backend/
│   └── Dockerfile          # .NET 8 API container
├── frontend/
│   └── Dockerfile          # Angular 17+ container
└── README.md               # Este arquivo
```

## 🚀 Como Usar

### Iniciar todos os serviços

```bash
# Na raiz do projeto
docker-compose up --build
```

### Serviços disponíveis

| Serviço     | URL                          | Descrição                    |
|-------------|------------------------------|------------------------------|
| Frontend    | http://localhost:4200        | Angular Web App              |
| API         | http://localhost:8080        | .NET 8 REST API + Swagger    |
| SQL Server  | localhost:14333              | Banco de dados               |

### Comandos úteis

```bash
# Ver logs de todos os serviços
docker-compose logs -f

# Ver logs de um serviço específico
docker-compose logs -f api
docker-compose logs -f frontend

# Parar todos os serviços
docker-compose down

# Parar e remover volumes (limpa o banco de dados)
docker-compose down -v

# Reconstruir containers
docker-compose build

# Reiniciar um serviço específico
docker-compose restart api
```

## 🔧 Configuração

### Variáveis de Ambiente da API

As seguintes variáveis de ambiente são configuradas no `docker-compose.yml`:

- `ASPNETCORE_ENVIRONMENT`: Development
- `ConnectionStrings__DefaultConnection`: String de conexão com SQL Server
- `Jwt__Issuer`: Emissor do token JWT
- `Jwt__Audience`: Audiência do token JWT
- `Jwt__Secret`: Chave secreta para assinatura JWT

### Rede Docker

Todos os serviços estão conectados à rede `taskmanager-network` (bridge), permitindo comunicação interna entre os containers.

### Volumes

- `sqlserver_data`: Volume persistente para dados do SQL Server

## 🔒 Segurança

**Importante**: As credenciais e segredos configurados são para desenvolvimento. Para produção:

1. Altere a senha do SQL Server
2. Use um segredo JWT forte gerado aleatoriamente
3. Utilize variáveis de ambiente ou Azure Key Vault para segredos
4. Não faça commit de senhas reais no repositório

## 🐛 Troubleshooting

### O banco de dados não inicializa

```bash
# Verifique os logs do serviço de init
docker-compose logs sqlserver-init

# Reinicie o serviço
docker-compose restart sqlserver sqlserver-init
```

### API não conecta ao banco

Verifique se a string de conexão no `docker-compose.yml` está correta:
- Server: `sqlserver` (nome do serviço)
- Porta: `1433` (porta interna)

### Frontend não acessa a API

O nginx está configurado para proxy de `/api` para `http://api:8080`. Verifique os logs:

```bash
docker-compose logs frontend
```

## 📝 Notas

- Os Dockerfiles originais nas pastas `backend/` e `frontend/` foram movidos para esta estrutura organizada
- O `docker-compose.yml` foi atualizado para apontar para os novos caminhos
- A rede Docker isolada melhora a segurança e organização dos serviços
