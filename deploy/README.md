# Publicação na DeLimaIT

## Arquitetura e estado desta preparação

Frontend: `dictionary.delimait.com.br` → Cloudflare Access → Tunnel → Caddy → entrada privada do K3s → `dictionary-web:8080`.
Backend: `dictionary-api:8080`, Service ClusterIP, sem Ingress, NodePort ou hostname externo. O Blazor Server chama a API dentro do cluster. SQL Server permanece na infraestrutura existente.

Os manifests canônicos estão em `C:\dev\homelab-infra\kubernetes\applications\dictionary`, com overlay em `overlays/production`. A Application está em `kubernetes/platform/argocd/dictionary-application.yaml` e a rota do Caddy já foi preparada no Git. A entrada segue Traefik em `192.168.15.11:32492`. Ainda é necessário verificar os servidores, preencher as imagens e Secrets, aplicar a Application e configurar Access/Tunnel.

## Como funciona o pipeline

1. Um pull request executa os testes unitários, publica o projeto Web para validar a compilação e constrói as duas imagens sem enviá-las ao registry.
2. Um push em `master` ou uma execução manual também envia as imagens ao GitHub Container Registry (GHCR).
3. As imagens são `ghcr.io/denismasterclass/delimait.dictionary-api:sha-<commit>` e `ghcr.io/denismasterclass/delimait.dictionary-web:sha-<commit>`. O commit é o SHA completo de 40 caracteres. As duas imagens suportam amd64 e arm64.
4. A promoção para produção deve atualizar as duas referências no repositório da infraestrutura. Após a sincronização manual da Application `dictionary-production`, o Argo CD reconcilia essa alteração no cluster.

O workflow deste repositório faz CI e publicação de imagens. A Application preparada no `homelab-infra` usa sincronização manual, conforme o padrão atual. O workflow não tem credenciais do cluster e não publica diretamente em produção.

O workflow usa `GITHUB_TOKEN` com `packages: write`. Não precisa salvar um PAT para publicar imagens. O cluster precisa de acesso de leitura aos pacotes privados por meio do Secret `ghcr-pull`; use o mecanismo de secrets já adotado pela infraestrutura. Não torne os pacotes públicos sem decidir explicitamente por isso.

## Preparação inicial da infraestrutura

- Confirmar o contexto Kubernetes e o padrão atual de GitOps antes de aplicar qualquer recurso.
- Conferir o namespace existente `delimait-production` e o Secret `ghcr-pull` nele, pelo mecanismo já existente.
- Criar `dictionary-sql` com a chave `connection-string`, com credencial de banco dedicada à aplicação. Nunca colocar a string real em YAML versionado, argumentos do terminal ou logs de CI.
- Confirmar o database e executar `database/schema.sql` nele, após revisar o alvo e o procedimento de backup. Não usar os testes de integração no banco de produção.
- Permitir a conectividade dos pods com SQL Server; conferir NetworkPolicies existentes. ClusterIP evita exposição externa pelo Service, mas não restringe por si só outros pods.
- Usar o Ingress Traefik já preparado no `homelab-infra`, acessado pelo Caddy via `192.168.15.11:32492`. Confirmar o funcionamento no cluster antes de ativar a rota pública.
- Configurar Caddy para encaminhar somente `dictionary.delimait.com.br` para essa entrada. Preservar o Host e habilitar o percurso de WebSockets; não adicionar rota para `dictionary-api`.
- Criar a aplicação Cloudflare Access para o hostname, com a política da equipe, antes de habilitar a rota pública. Não criar bypass geral para `/_blazor`: a conexão WebSocket deve usar a sessão autenticada.
- Adicionar a rota do Tunnel para o Caddy existente e confirmar o DNS, seguindo o padrão atual. Não publicar diretamente o NodePort/Ingress na Internet.

O frontend usa uma réplica e estratégia Recreate para evitar circuitos Blazor divididos entre pods. Uma atualização interrompe sessões abertas brevemente; recarregar a página reconecta. Antes de escalar, configurar afinidade e persistência de Data Protection conforme a arquitetura adotada.

## Próximas versões: passo a passo

1. Faça a alteração, rode `dotnet test test/DeLimaIt.Dictionary.Tests/DeLimaIt.Dictionary.Tests.csproj -c Release` e envie um pull request.
2. Depois da revisão, faça merge em `master`.
3. No GitHub, abra **Actions → Test and publish containers**. Aguarde o job de testes e as duas imagens terminarem com sucesso.
4. Copie o SHA completo do commit publicado. Confirme que as duas tags `sha-<commit>` existem no GHCR.
5. No checkout de `homelab-infra`, atualize as duas imagens da aplicação Dictionary para essa mesma versão. Use `kubernetes/applications/dictionary/overlays/production/kustomization.yaml`. Prefira os digests exibidos no resumo do workflow, conforme o README da aplicação na infraestrutura. Abra/revise o diff: ele deve alterar apenas as versões esperadas.
6. Faça commit e push da promoção. No Argo CD, confira o sync e a saúde da aplicação conforme a política já adotada.
7. Confira os deployments e os endpoints abaixo. Uma imagem publicada no GHCR só entra em produção depois da promoção da infraestrutura.

Exemplo de atualização do Kustomize no diretório da aplicação no repositório da infraestrutura (substitua o SHA):

```bash
RELEASE_SHA=COLE_O_SHA_COMPLETO
kustomize edit set image dictionary-api=ghcr.io/denismasterclass/delimait.dictionary-api:sha-$RELEASE_SHA
kustomize edit set image dictionary-web=ghcr.io/denismasterclass/delimait.dictionary-web:sha-$RELEASE_SHA
kubectl kustomize .
git diff --check
git diff
```

Os exemplos usam `kustomize` standalone para editar e `kubectl kustomize` para renderizar. Sem o binário standalone, edite os dois `newTag` no YAML diretamente. O overlay da infraestrutura fixa os digests da versão aprovada; substitua ambos ao promover uma release. Não use `latest`.

## Validação da publicação

No contexto correto do cluster:

```bash
kubectl -n delimait-production rollout status deployment/dictionary-api --timeout=180s
kubectl -n delimait-production rollout status deployment/dictionary-web --timeout=180s
kubectl -n delimait-production get pods,services
```

API: `/health/live` verifica o processo e `/health/ready` verifica a conexão SQL com `SELECT 1`. A readiness SQL não confirma a existência das tabelas; validar também uma consulta real de módulos.

Frontend: abrir `https://dictionary.delimait.com.br`, confirmar o login do Access, listar os módulos e conferir a conexão WebSocket `/_blazor` no navegador. Confirmar que não existe rota pública para a API e que o frontend não pode ser acessado pela Internet contornando o Access. Validar as políticas existentes antes de declarar a publicação concluída.

## Rollback

Reverta o commit que promoveu as duas imagens em `homelab-infra` e sincronize o Argo CD. Confirme os dois rollouts e o acesso pelo domínio novamente. Não usar apenas `kubectl rollout undo` em recursos geridos por GitOps: a reconciliação pode reaplicar a versão indesejada. Mudanças de schema exigem um plano próprio de compatibilidade e recuperação; rollback de imagem não desfaz migração de banco.

## Referências

- [GitHub: publicação de imagens com Actions](https://docs.github.com/en/actions/tutorials/publish-packages/publish-docker-images)
- [Kubernetes: Kustomize](https://kubernetes.io/docs/tasks/manage-kubernetes-objects/kustomization/)
- [Microsoft: publicação de Blazor Server](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server?view=aspnetcore-8.0)
