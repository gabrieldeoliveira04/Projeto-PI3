# Gestão de Férias (PI3) — API (.NET 8 + EF Core + PostgreSQL)

API para cadastro de usuários/setores e **gestão de solicitações de férias** com fluxo de aprovação, validação de períodos e controle de **limite de férias simultâneas por setor**.

## Stack
- .NET 8
- EF Core 8
- PostgreSQL (Npgsql)
- JWT Auth + Roles (`Gestor`, `Colaborador`, etc.)
- Swagger

---

## Regras de Negócio (importante pro front)

### 1) Usuário e Matrícula
- Usuários são identificados por **Matrícula** (string `"0001"` até `"9999"`).
- No `register`, a matrícula é **gerada automaticamente**.
- Se não informar setor no cadastro, cai no setor padrão **"Sem Setor"**:
  - `SetorId`: `11111111-1111-1111-1111-111111111111`

### 2) Setor e Limite de férias simultâneas
- Cada setor tem `limiteFeriasSimultaneas` (int).
- Esse limite define **quantas férias aprovadas** podem acontecer no mesmo dia dentro do setor.
- O gestor pode atualizar via:
  - `PATCH /api/setores/{id}/limite-ferias`
- Regras:
  - `limiteFeriasSimultaneas >= 1`

### 3) Solicitação de Férias (períodos)
Ao solicitar férias:
- Deve enviar **1 ou mais períodos**
- Cada período precisa ter `inicio` e `fim`
- `inicio <= fim`
- **Períodos não podem se sobrepor** dentro do mesmo pedido
- A soma total deve ser **EXATAMENTE 30 dias corridos**
  - dias corridos = `fim - inicio + 1`
- Capacidade do setor:
  - Se já atingiu o limite em algum dia por **férias aprovadas**, a solicitação é bloqueada (erro)
  - Se existir **pendência** de outros pedidos em algum dia, o pedido pode ser criado, mas volta com **avisos** (não bloqueia)

### 4) Status das férias (enum)
Fluxo típico:
- `Pendente` → `Aprovada` ou `Negada`
- `Pendente` → `Cancelada` (somente o próprio usuário)
- Regras:
  - **Somente `Pendente` pode ser aprovada, negada ou cancelada**
  - Aprovar/Negar exige `AprovadoPorId`/`NegadoPorId` válido (precisa existir na tabela Usuarios)

### 5) Calendário do Setor
Endpoint retorna ocupação por dia (não por período):
- `GET /api/Ferias/calendario/setor/{setorId}?inicio=YYYY-MM-DD&fim=YYYY-MM-DD`
- Se `inicio` e `fim` não forem enviados:
  - `inicio = hoje (UTC)`
  - `fim = inicio + 9 dias` (por padrão do controller atual)

---

## Autenticação (JWT)

### Header obrigatório (endpoints protegidos)