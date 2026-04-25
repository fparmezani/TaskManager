# Docker Configuration - TaskManager

Esta pasta contém todos os recursos Docker necessários para executar a aplicação TaskManager.

## 📁 Estrutura

```
docker/
├── backend/
│   └── Dockerfile          # .NET 8 API container
├── frontend/
│   └── Dockerfile          # Angular 17+ container
├── ollama-entrypoint.sh    # Script para inicializar Ollama com modelo
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
| Ollama      | http://localhost:11434       | Serviço de IA (Mistral)      |

## 🤖 Integração com Ollama (IA)

O TaskManager inclui integração com **Ollama** para gerar sugestões de descrição de tarefas usando o modelo **Mistral**.

### Recursos de IA:
- ✨ Botão "AI" na criação de tarefas
- 🧠 Gera descrições automáticas baseadas no título da tarefa
- 🔄 Chat com o modelo Mistral via Ollama
- 🚀 Totalmente local - nenhuma chamada externa a serviços de IA

### Primeiras execuções:
Na primeira execução, o Ollama:
1. Iniciará o serviço
2. Fará download do modelo Mistral (~4GB)
3. Estará pronto para gerar sugestões

Isso pode levar alguns minutos. Você pode monitorar o progresso com:

```bash
docker-compose logs ollama
```

### Usar o AI:
1. Vá para http://localhost:4200
2. Login com credenciais demo
3. Clique no botão "✨ AI" ao lado do título da tarefa
4. Digite o título da tarefa
5. Clique "Generate" para obter uma sugestão de descrição

## 📋 Comandos úteis

### Ver logs de todos os serviços

```bash
docker-compose logs -f
```

### Ver logs de um serviço específico

```bash
docker-compose logs -f api
docker-compose logs -f frontend
docker-compose logs -f ollama
```

### Parar todos os serviços

```bash
docker-compose down
```

### Parar e remover volumes (limpa o banco de dados e modelo Ollama)

```bash
docker-compose down -v
```

### Reconstruir containers

```bash
docker-compose build
```

### Reiniciar um serviço específico

```bash
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
- `ollama_data`: Volume persistente para modelos de IA do Ollama

## 🔒 Segurança

**Importante**: As credenciais e segredos configurados são para desenvolvimento. Para produção:

1. Altere a senha do SQL Server
2. Use um segredo JWT forte gerado aleatoriamente
3. Utilize variáveis de ambiente ou Azure Key Vault para segredos
4. Não faça commit de senhas reais no repositório
5. Configure HTTPS para API e Frontend

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

### Ollama não está respondendo

```bash
# Verifique se o serviço está rodando
docker-compose ps ollama

# Veja os logs do Ollama (pode estar baixando o modelo)
docker-compose logs ollama

# Teste a conexão manualmente
curl http://localhost:11434/api/tags
```

### "AI service unavailable" no frontend

1. Verifique se o Ollama iniciou corretamente:
   ```bash
   docker-compose logs ollama
   ```

2. Verifique se o modelo Mistral foi baixado:
   ```bash
   curl http://localhost:11434/api/tags
   ```

3. Se o modelo está sendo baixado, aguarde a conclusão
4. Reinicie o Ollama:
   ```bash
   docker-compose restart ollama
   ```

## 📝 Notas

- Os Dockerfiles originais nas pastas `backend/` e `frontend/` foram movidos para esta estrutura organizada
- O `docker-compose.yml` foi atualizado para apontar para os novos caminhos
- A rede Docker isolada melhora a segurança e organização dos serviços
- Ollama roda localmente sem dependência de serviços externos de IA
