# AI Integration - Ollama Task Suggestions

## 🤖 Visão Geral

O TaskManager agora inclui integração com **Ollama** para gerar sugestões automáticas de descrição de tarefas usando o modelo de linguagem **Mistral**.

## ✨ Recursos

### Frontend (Angular)
- Novo botão **"✨ AI"** próximo ao campo de título de tarefas
- Dialog elegante para geração de sugestões
- Preenchimento automático da descrição com a sugestão gerada
- Verificação de disponibilidade do serviço de IA
- Interface responsiva e amigável

### Backend (.NET 8)
- Novo serviço `IAiSuggestionService` na Application layer
- Implementação com Ollama no Infrastructure layer
- Endpoints REST:
  - `POST /api/tasks/suggestions/description` - Gera sugestão
  - `GET /api/tasks/suggestions/available` - Verifica disponibilidade
- Tratamento robusto de erros
- Timeout de 30 segundos para requisições

## 🏗️ Arquitetura

```
Controller (TasksController)
    ↓
Application Service (IAiSuggestionService)
    ↓
Infrastructure (OllamaAiSuggestionService)
    ↓
Ollama HTTP API (localhost:11434)
```

## 🔧 Como Usar

### 1. Iniciar a Aplicação

```bash
# Na raiz do projeto
docker-compose up --build
```

Na primeira execução:
- Ollama faz download do modelo Mistral (~4GB)
- Pode levar alguns minutos
- Monitore com: `docker-compose logs ollama`

### 2. Usar a Sugestão de IA

1. Acesse http://localhost:4200
2. Faça login com:
   - Email: `demo@taskmanager.com`
   - Senha: `Demo@123456`

3. Na tela de tarefas:
   - Digite o título da tarefa no campo "Title"
   - Clique no botão **"✨ AI"**
   - Digite o título novamente no dialog (se necessário)
   - Clique **"Generate"**
   - Revise a sugestão gerada
   - Clique **"Use This"** para preencher a descrição

## 📁 Estrutura de Arquivos

```
backend/
├── TaskManager.Application/
│   ├── Abstractions/
│   │   └── IAiSuggestionService.cs      # Interface do serviço
│   └── AI/
│       └── AiSuggestionDtos.cs          # DTOs de resposta
├── TaskManager.Infrastructure/
│   ├── AI/
│   │   └── OllamaAiSuggestionService.cs # Implementação com Ollama
│   └── DependencyInjection.cs           # Registro do serviço
└── TaskManager.Api/
    └── Controllers/
        └── TasksController.cs            # Endpoints de IA

frontend/
└── taskmanager-web/
    └── src/app/
        ├── core/services/
        │   └── ai-suggestion.service.ts  # Serviço Angular
        └── features/tasks/
            ├── task-list.component.ts    # Componente principal
            └── ai-suggestion-dialog.component.ts # Dialog de sugestão

docker/
├── ollama-entrypoint.sh                  # Script de inicialização
└── README.md                             # Documentação
```

## 🔌 Endpoints da API

### Gerar Sugestão de Descrição

```http
POST /api/tasks/suggestions/description?title=Learn%20TypeScript
Authorization: Bearer {token}

Response:
{
  "suggestion": "Master the fundamentals of TypeScript including types, interfaces, generics, and async programming patterns."
}
```

### Verificar Disponibilidade de IA

```http
GET /api/tasks/suggestions/available
Authorization: Bearer {token}

Response:
{
  "available": true
}
```

## 🛠️ Configuração

### Variáveis do Ollama

No `docker-compose.yml`:
```yaml
environment:
  OLLAMA_HOST: "0.0.0.0:11434"
```

### Timeout do Serviço

No `DependencyInjection.cs`:
```csharp
services.AddHttpClient<IAiSuggestionService, OllamaAiSuggestionService>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    });
```

## 📊 Fluxo de Dados

```
1. Usuário clica "✨ AI" no frontend
   ↓
2. Dialog abre solicitando o título
   ↓
3. Usuário digita título e clica "Generate"
   ↓
4. Frontend chama POST /api/tasks/suggestions/description
   ↓
5. Backend chama OllamaAiSuggestionService.SuggestDescriptionAsync()
   ↓
6. Ollama gera sugestão usando modelo Mistral
   ↓
7. Sugestão retorna ao frontend
   ↓
8. Usuário revisa e clica "Use This" para preencher descrição
```

## 🎯 Modelo Mistral

- **Nome**: Mistral 7B
- **Tamanho**: ~4GB
- **Tipo**: Large Language Model (LLM)
- **Qualidade**: Otimizado para instruções
- **Download**: Automático na primeira execução

## 🚀 Próximas Melhorias (Roadmap)

- [ ] Cache de sugestões já geradas
- [ ] Sugestões de datas de entrega
- [ ] Sugestão de prioridade de tarefas
- [ ] Integração com múltiplos modelos de IA
- [ ] Histórico de sugestões
- [ ] Ajuste de detalhamento (breve/detalhado)
- [ ] Geração em português
- [ ] Suporte a OpenAI como alternativa

## 🐛 Troubleshooting

### "AI service is unavailable"

**Causa**: Ollama não está rodando ou o modelo não foi baixado

**Solução**:
```bash
# Verifique os logs
docker-compose logs ollama

# Reinicie o serviço
docker-compose restart ollama

# Aguarde ~5 minutos para o modelo ser baixado
docker-compose logs ollama -f
```

### Timeout na geração de sugestão

**Causa**: Modelo Mistral ainda está sendo baixado ou requisição muito lenta

**Solução**:
- Aguarde o download completo
- Verifique sua conexão de internet
- Aumente timeout em `DependencyInjection.cs` se necessário

### Sugestão de baixa qualidade

**Causa**: Modelo Mistral é ótimo mas não perfeito

**Solução**:
- Refine o título da tarefa (seja mais específico)
- Regenere a sugestão
- Considere usar outro modelo no futuro

## 📚 Referências

- [Ollama - Local LLMs](https://ollama.ai/)
- [Mistral 7B - Model Card](https://mistral.ai/)
- [Ollama API](https://github.com/ollama/ollama/blob/main/docs/api.md)

## 📝 Notas de Implementação

### Por que Ollama?

1. ✅ Totalmente local - sem custo
2. ✅ Sem dependência de serviços externos
3. ✅ Privado - dados não saem do servidor
4. ✅ Fácil de integrar
5. ✅ Comunidade ativa

### Por que Mistral?

1. ✅ Modelo leve (~7B parâmetros)
2. ✅ Ótimo custo-benefício
3. ✅ Bom desempenho em tarefas de instrução
4. ✅ Totalmente open source
5. ✅ Funciona bem em hardware modesto

### Padrão de Timeout

Configurado para 30 segundos para:
- Permitir processamento do modelo
- Evitar requisições "travadas"
- Balancear entre qualidade e responsividade

## 🔐 Segurança

- ✅ Requisições via HTTPS em produção (recomendado)
- ✅ Ollama em rede isolada (taskmanager-network)
- ✅ Autenticação JWT necessária para usar sugestões
- ✅ Sem armazenamento de dados sensíveis
- ✅ Tratamento robusto de erros

