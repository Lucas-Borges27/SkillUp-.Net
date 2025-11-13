# SkillUp API (ASP.NET Core 8 + EF Core)

API de trilhas de aprendizado com Oracle/EF Core, construída seguindo arquitetura limpa e requisitos da FIAP GS.

## Arquitetura & Domínio
- **Domínio (SkillUp.Domain)**: Entidades ricas (`Usuario`, `Curso`, `Progresso`) com invariantes (email e senha fortes, carga horária positiva, porcentagem 0-100, status coerente com datas) + exceção de domínio.
- **Aplicação (SkillUp.Application)**: Casos de uso (`UsuarioAppService`, `CursoAppService`, `ProgressoAppService`), DTOs de entrada/saída, busca paginada com filtros/ordenação e geração de links HATEOAS.
- **Infraestrutura (SkillUp.Infrastructure)**: `SkillUpDbContext`, mapeamentos Oracle (`SKILLUP_*`), repositórios concretos e `SkillUpDbSeeder` com dados iniciais. Migração `Initial` espelha o `CREATE_TABLE.sql` enviado.
- **Web (SkillUp.Api)**: Controllers REST com ProblemDetails, middleware de exceções, Swagger, endpoint `/health` e payloads com links (`self`, `update`, `delete`, `next`, `prev`).

## Como rodar
1. **Pré-requisitos**
   - .NET 8 SDK
   - Oracle Database / Oracle XE (ou outro alvo compatível com o provider `Oracle.EntityFrameworkCore`)
   - Ferramenta `dotnet-ef`: `dotnet tool install --global dotnet-ef`

2. **Restaurar & compilar**
   ```bash
   cd skillup-api
   dotnet restore
   dotnet build
   ```
3. **Executar**
   ```bash
   dotnet run --project src/SkillUp.Api/SkillUp.Api.csproj
   ```
   - Swagger/OpenAPI: `http://localhost:5000/swagger`
   - Health check: `GET /health`

## Endpoints principais
| Recurso | Operações |
| --- | --- |
| `/api/usuarios` | `POST`, `PUT /{id}`, `DELETE /{id}`, `GET /{id}` |
| `/api/usuarios/search` | `GET` com `filter`, `areaInteresse`, `sortBy`, `sortDirection`, `page`, `size` |
| `/api/cursos` | CRUD completo |
| `/api/cursos/search` | `GET` com `categoria`, `dificuldade`, ordenação/paginação |
| `/api/progresso` | CRUD completo (associa usuário ↔ curso) |
| `/api/progresso/search` | `GET` com `usuarioId`, `cursoId`, `status`, filtros gerais |

Todas as rotas `/search` retornam `PagedResponse` com `metadata` + `links` (`self`, `first`, `last`, `next`, `prev`). Recursos individuais retornam `ResourceResponse` com links de navegação (`self`, `update`, `delete`).

## Exemplos rápidos
```bash
# Buscar usuários com filtro e paginação
curl "https://localhost:5001/api/usuarios/search?filter=ana&page=1&size=5&sortBy=dataCadastro&sortDirection=desc"

# Criar novo curso
curl -X POST https://localhost:5001/api/cursos \
  -H "Content-Type: application/json" \
  -d '{
        "nome": "Kubernetes para Devs",
        "categoria": "DevOps",
        "cargaHoraria": 24,
        "dificuldade": "Intermediario",
        "descricao": "Hands-on com clusters"
      }'

# Atualizar progresso de um usuário
curl -X PUT https://localhost:5001/api/progresso/1 \
  -H "Content-Type: application/json" \
  -d '{
        "usuarioId": 1,
        "cursoId": 2,
        "status": "EmAndamento",
        "porcentagem": 55
      }'
```

## Tratamento de erros & validações
- Middleware converte exceções em `application/problem+json` com `traceId`.
- Model binding inválido gera `ValidationProblemDetails` (400).
- Domínio verifica: emails válidos, senhas ≥8 chars, status coerente com datas, carga horária > 0, porcentagem 0-100. Violações retornam 400 com mensagem clara.

## Próximos passos sugeridos
- Adicionar autenticação/JWT.
- Criar coleção Postman/HTTP file.
- Cobrir regras críticas com testes automatizados.

## Artefatos úteis
- Coleção Postman: [`postman/SkillUp.postman_collection.json`](postman/SkillUp.postman_collection.json) (usa `{{baseUrl}}` apontando para sua porta/host)
