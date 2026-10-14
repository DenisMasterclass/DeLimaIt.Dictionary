# DeLimaIt.Dictionary

Aplicação para cadastrar e consultar configurações organizadas por módulo, dicionário, chave e valor. A solution contém uma API ASP.NET Core, uma interface web em Blazor Server, a camada de aplicação e projetos de testes.

## Como os dados são organizados

Cada módulo agrupa dicionários de uma aplicação ou domínio. Cada dicionário reúne chaves e seus valores:

```text
Módulo (CONFIGURACAO_MODULO)
└── Dicionário/parâmetro (CONFIGURACAO_PARAMETRO)
    └── Chaves e valores (CONFIGURACAO_CHAVE)
```

- **Módulo:** grupo de configuração, como `Checkout` ou `Cadastro`.
- **Dicionário:** conjunto de configurações dentro de um módulo, como `Pagamentos`.
- **Chave:** nome único dentro do dicionário, como `api.timeout`.
- **Valor:** texto associado à chave. Pode conter texto simples ou JSON.

Os nomes de módulos e dicionários aceitam até 100 caracteres; as chaves, até 200. Não é permitido repetir o nome de um módulo, o nome de um dicionário dentro do mesmo módulo ou uma chave dentro do mesmo dicionário.

## Interface web

A interface oferece duas áreas:

1. **Cadastrar dicionários** (`/admin`): criar, editar e excluir módulos e dicionários.
2. **Explorar dicionários** (`/`): selecionar módulo e dicionário, pesquisar chaves e valores, e criar, editar ou excluir entradas.

Ao excluir um módulo, seus dicionários e entradas são removidos. Ao excluir um dicionário, suas entradas são removidas.

## Requisitos

- .NET SDK 8.0.
- SQL Server acessível pela máquina que executa a API.
- Um database criado para a aplicação.

## Preparação do database

Configure a connection string da API na configuração `SQL_DELIMAIT_DICTIONARY_CONN_STRING`. Em desenvolvimento, o arquivo usado pela aplicação é `src/DeLimaIt.Dictionary.Api/appsettings.Development.json`; também é possível fornecer a configuração por variável de ambiente com esse nome. Não inclua credenciais reais em documentação ou em commits.

Execute `database/schema.sql` no database configurado. O script cria, caso ainda não existam, as tabelas `CONFIGURACAO_MODULO`, `CONFIGURACAO_PARAMETRO` e `CONFIGURACAO_CHAVE`, com chaves estrangeiras e restrições de unicidade.

## Executar a aplicação

Na raiz do repositório, abra o PowerShell e execute:

```powershell
.\start.ps1
```

O script inicia a API e o frontend quando ainda não estão respondendo e grava os logs em `%LOCALAPPDATA%\DeLimaIt.Dictionary\logs`.

- Frontend: <http://localhost:5050/>
- Administração: <http://localhost:5050/admin>
- Swagger da API (ambiente Development): <http://localhost:5080/swagger>

Para iniciar os serviços manualmente, em dois terminais na raiz:

```powershell
dotnet run --project src/DeLimaIt.Dictionary.Api/DeLimaIt.Dictionary.Api.csproj --launch-profile http
```

```powershell
dotnet run --project src/DeLimaIt.Dictionary.Web/DeLimaIt.Dictionary.Web.csproj --launch-profile http
```

O frontend usa `DictionaryApi:BaseUrl` para localizar a API; o padrão é `http://localhost:5080/`.

## Utilizar pela API

A API fornece operações para módulos, dicionários e entradas. Os endpoints de consulta e de valores ficam em `ConfigurationController`; o CRUD administrativo de módulos e dicionários fica em `ConfigurationAdminController`.

| Operação | Método e rota |
| --- | --- |
| Listar módulos | `GET /api/Configuration/Modules/0` |
| Listar dicionários de um módulo | `GET /api/Configuration/Dictionaries/{moduleId}` |
| Listar entradas de um dicionário | `GET /api/Configuration/Dictionaries/Values/{dictionaryId}` |
| Criar módulo | `POST /api/Configuration/Modules` |
| Editar módulo | `PUT /api/Configuration/Modules/{moduleId}` |
| Excluir módulo | `DELETE /api/Configuration/Modules/{moduleId}` |
| Criar dicionário | `POST /api/Configuration/Dictionaries` |
| Editar dicionário | `PUT /api/Configuration/Dictionaries/{dictionaryId}` |
| Excluir dicionário | `DELETE /api/Configuration/Dictionaries/{dictionaryId}` |
| Criar entrada | `POST /api/Configuration/Dictionaries/Values/{dictionaryId}` |
| Editar entrada | `PUT /api/Configuration/Dictionaries/Values/{dictionaryId}` |
| Excluir entrada | `DELETE /api/Configuration/Dictionaries/Values/{dictionaryId}/{key}` |

Exemplos de payloads JSON:

```json
{ "name": "Checkout" }
```

```json
{ "moduleId": 1, "name": "Pagamentos" }
```

```json
{ "key": "api.timeout", "value": "30" }
```

Na edição de uma entrada, envie também `oldKey` quando o nome da chave for alterado:

```json
{ "oldKey": "api.timeout", "key": "api.timeout.seconds", "value": "30" }
```

Os endpoints de criação e edição devolvem `409 Conflict` quando o nome já existe em seu escopo. Uma referência a módulo inexistente ao criar dicionário resulta em `400 Bad Request`; identificadores inexistentes em operações de edição ou exclusão resultam em `404 Not Found`.

## Testes

Execute os testes unitários com:

```powershell
dotnet test test/DeLimaIt.Dictionary.Tests/DeLimaIt.Dictionary.Tests.csproj
```

Os testes de integração usam a API hospedada em memória e um SQL Server real. Configure `SQL_DELIMAIT_DICTIONARY_CONN_STRING` para um database com o schema criado antes de executá-los:

```powershell
dotnet test test/DeLimaIt.Dictionary.IntegrationTests/DeLimaIt.Dictionary.IntegrationTests.csproj
```

Os testes de integração criam dados temporários com nomes únicos e removem os registros criados ao final.

## Projetos da solution

- `src/DeLimaIt.Dictionary.Api`: endpoints HTTP e documentação Swagger.
- `src/DeLimaIt.Dictionary.Web`: interface administrativa e de exploração.
- `src/DeLimaIt.Dictionary.Application`: casos de uso, validações e acesso a dados.
- `src/DelimaIt.Core.UseCases`: abstrações compartilhadas de casos de uso e resultados.
- `test/DeLimaIt.Dictionary.Tests`: testes unitários.
- `test/DeLimaIt.Dictionary.IntegrationTests`: testes integrados com API e SQL Server.
- `database/schema.sql`: estrutura inicial do database.
