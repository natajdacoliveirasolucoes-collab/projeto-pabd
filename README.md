# EventHub API

API RESTful para gestao de eventos baseada no documento de software EventHub.

## Recursos implementados

- Cadastro e autenticacao de usuarios com JWT.
- Perfil de organizador vinculado ao usuario autenticado.
- CRUD de categorias, locais e palestrantes.
- CRUD de eventos com filtros por categoria, cidade, data e status.
- Validacao de conflito de datas por local.
- Programacao de eventos com validacao de conflito por sala e horario.
- Tipos de ingresso por evento.
- Compra de ingressos com baixa automatica da quantidade disponivel.
- Inscricao de usuarios em eventos publicados.
- Vinculo N:N entre eventos e palestrantes.
- Avaliacao de eventos ja realizados por usuarios inscritos.

## Banco de dados

O projeto usa MySQL via EF Core/Pomelo. A string padrao esta em `appsettings.json`:

```json
"mysql": "Server=localhost;Database=eventhub_db;User=root;Password=root;Port=3360;"
```

Como este repositorio nao possuia migrations, o script SQL completo esta em:

```text
Database/EventHub.sql
```

## Principais rotas

- `POST /api/usuarios`
- `POST /api/auth/login`
- `POST /api/organizadores`
- `GET|POST|PUT|DELETE /api/categorias`
- `GET|POST|PUT|DELETE /api/locais`
- `GET|POST|PUT|DELETE /api/palestrantes`
- `GET|POST|PUT|DELETE /api/eventos`
- `POST /api/eventos/{id}/programacao`
- `POST /api/eventos/{id}/tipos-ingresso`
- `POST /api/eventos/{id}/palestrantes`
- `POST /api/eventos/{id}/inscricoes`
- `POST /api/eventos/{id}/avaliacoes`
- `GET|POST /api/compras`
