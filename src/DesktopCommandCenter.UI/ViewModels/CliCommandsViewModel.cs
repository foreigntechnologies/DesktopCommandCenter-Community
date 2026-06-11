using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;

namespace DesktopCommandCenter.UI.ViewModels;

public class CliCommandItem
{
    public string Title { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public partial class CliCommandsViewModel : ObservableObject
{
    private readonly ObservableCollection<CliCommandItem> _allCommands = new();
    
    public ObservableCollection<CliCommandItem> FilteredCommands { get; } = new();
    public ObservableCollection<string> Categories { get; } = new();

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = "Todos";

    public CliCommandsViewModel()
    {
        LoadAllCommands();
        ApplyFilters();
    }

    private void AddCmd(string title, string command, string description, string category)
    {
        _allCommands.Add(new CliCommandItem
        {
            Title = title,
            Command = command,
            Description = description,
            Category = category
        });
    }

    private void LoadAllCommands()
    {
        // --- Bash / Zsh (Linux & macOS) ---
        string catBash = "Bash / Zsh";
        AddCmd("Diretório Atual", "pwd", "Exibe o caminho do diretório de trabalho atual.", catBash);
        AddCmd("Listar Arquivos", "ls", "Lista arquivos e pastas do diretório atual.", catBash);
        AddCmd("Listar Detalhado (Ocultos)", "ls -la", "Lista arquivos detalhadamente, incluindo ocultos.", catBash);
        AddCmd("Subir um Diretório", "cd ..", "Muda o diretório de trabalho para o diretório pai.", catBash);
        AddCmd("Ir para Home", "cd ~", "Muda para o diretório pessoal (Home) do usuário.", catBash);
        AddCmd("Criar Pasta", "mkdir pasta", "Cria um novo diretório chamado 'pasta'.", catBash);
        AddCmd("Remover Pasta Vazia", "rmdir pasta", "Remove um diretório vazio.", catBash);
        AddCmd("Remover Arquivo", "rm arquivo", "Remove um arquivo.", catBash);
        AddCmd("Remover Pasta e Conteúdo", "rm -rf pasta", "Remove recursivamente e à força a pasta e todo seu conteúdo.", catBash);
        AddCmd("Criar Arquivo Vazio", "touch arquivo.txt", "Cria um novo arquivo vazio ou atualiza a data de modificação.", catBash);
        AddCmd("Exibir Conteúdo", "cat arquivo.txt", "Exibe o conteúdo completo de um arquivo no terminal.", catBash);
        AddCmd("Visualização Paginada", "less arquivo.txt", "Abre o arquivo para leitura paginada navegável.", catBash);
        AddCmd("Primeiras Linhas", "head arquivo.txt", "Exibe as 10 primeiras linhas do arquivo.", catBash);
        AddCmd("Últimas Linhas", "tail arquivo.txt", "Exibe as 10 últimas linhas do arquivo.", catBash);
        AddCmd("Copiar Arquivo", "cp origem destino", "Copia um arquivo de origem para destino.", catBash);
        AddCmd("Mover/Renomear", "mv origem destino", "Move ou renomeia um arquivo ou pasta.", catBash);
        AddCmd("Buscar por Nome", "find . -name \"*.js\"", "Busca arquivos com extensão .js a partir do diretório atual.", catBash);
        AddCmd("Buscar Texto em Arquivo", "grep \"texto\" arquivo.txt", "Procura pelo termo 'texto' dentro do arquivo.", catBash);
        AddCmd("Buscar Texto Recursivo", "grep -r \"texto\" .", "Procura pelo termo 'texto' recursivamente nos arquivos deste diretório.", catBash);
        AddCmd("Caminho do Executável", "which node", "Mostra o caminho absoluto do executável 'node'.", catBash);
        AddCmd("Localização de Binários/Fontes", "whereis node", "Mostra a localização do binário, fonte e manual de 'node'.", catBash);
        AddCmd("Permissão de Execução", "chmod +x script.sh", "Torna o script executável.", catBash);
        AddCmd("Permissões Completas", "chmod 755 script.sh", "Define permissões: Dono (rwx), Grupo (rx), Outros (rx).", catBash);
        AddCmd("Alterar Dono", "chown user arquivo", "Muda o proprietário do arquivo para 'user'.", catBash);
        AddCmd("Processos Ativos", "ps aux", "Lista todos os processos ativos no sistema.", catBash);
        AddCmd("Monitor de Recursos", "top", "Monitor em tempo real de CPU/Memória e processos ativos.", catBash);
        AddCmd("Monitor de Recursos Avançado", "htop", "Monitor interativo e colorido de processos.", catBash);
        AddCmd("Parar Processo", "kill PID", "Envia um sinal de término suave (SIGTERM) para o processo pelo PID.", catBash);
        AddCmd("Forçar Parada de Processo", "kill -9 PID", "Força o encerramento imediato (SIGKILL) do processo pelo PID.", catBash);
        AddCmd("Espaço em Disco", "df -h", "Exibe o espaço livre/usado nos discos em formato legível.", catBash);
        AddCmd("Uso de Espaço por Arquivos", "du -sh *", "Exibe o tamanho total ocupado por cada arquivo/pasta no diretório atual.", catBash);
        AddCmd("Requisição Web (HTTP)", "curl URL", "Faz uma requisição HTTP para a URL e exibe a resposta.", catBash);
        AddCmd("Download de Arquivos", "wget URL", "Faz o download do arquivo a partir da URL fornecida.", catBash);
        AddCmd("Compactar para ZIP", "zip -r arquivo.zip pasta", "Compacta recursivamente a pasta especificada para um arquivo .zip.", catBash);
        AddCmd("Descompactar ZIP", "unzip arquivo.zip", "Descompacta o arquivo .zip no diretório atual.", catBash);
        AddCmd("Criar Arquivo TAR", "tar -cvf arquivo.tar pasta", "Cria um arquivo .tar da pasta (sem compressão).", catBash);
        AddCmd("Extrair Arquivo TAR", "tar -xvf arquivo.tar", "Extrai os arquivos do arquivo .tar.", catBash);
        AddCmd("Variáveis de Ambiente", "env", "Lista todas as variáveis de ambiente atuais.", catBash);
        AddCmd("Exportar Variável", "export VAR=value", "Define e exporta uma variável de ambiente.", catBash);
        AddCmd("Histórico de Comandos", "history", "Mostra a lista dos comandos executados anteriormente no terminal.", catBash);
        AddCmd("Limpar Tela", "clear", "Limpa o terminal.", catBash);

        // --- Git ---
        string catGit = "Git";
        AddCmd("Inicializar Repositório", "git init", "Cria um novo repositório Git local na pasta atual.", catGit);
        AddCmd("Clonar Repositório", "git clone URL", "Faz o download de um repositório remoto existente.", catGit);
        AddCmd("Configurar Nome de Usuário", "git config --global user.name \"Seu Nome\"", "Configura globalmente o nome de usuário para os commits.", catGit);
        AddCmd("Configurar Email", "git config --global user.email \"seu@email.com\"", "Configura globalmente o email do usuário para os commits.", catGit);
        AddCmd("Status do Repositório", "git status", "Lista os arquivos modificados, adicionados ou não rastreados.", catGit);
        AddCmd("Adicionar Todos Arquivos", "git add .", "Adiciona todos os arquivos modificados ao estágio de commit (stage).", catGit);
        AddCmd("Adicionar Arquivo", "git add arquivo", "Adiciona um arquivo específico ao estágio de commit.", catGit);
        AddCmd("Criar Commit", "git commit -m \"mensagem\"", "Cria um commit com as alterações no stage acompanhado de uma mensagem.", catGit);
        AddCmd("Enviar Alterações", "git push", "Envia os commits locais para o repositório remoto padrão.", catGit);
        AddCmd("Enviar para Branch Remota", "git push origin main", "Envia os commits locais para a branch main no repositório origin.", catGit);
        AddCmd("Puxar Alterações", "git pull", "Baixa e mescla as alterações do repositório remoto na branch local.", catGit);
        AddCmd("Buscar Alterações (Sem Mesclar)", "git fetch", "Baixa referências e objetos do repositório remoto sem realizar merge.", catGit);
        AddCmd("Listar Branches", "git branch", "Lista as branches locais. Se seguido de um nome, cria uma nova branch.", catGit);
        AddCmd("Mudar de Branch", "git checkout branch", "Altera o contexto para a branch especificada.", catGit);
        AddCmd("Alternar Branch (Moderno)", "git switch branch", "Alterna para a branch especificada (alternativa mais clara ao checkout).", catGit);
        AddCmd("Criar e Mudar de Branch", "git checkout -b feature", "Cria uma nova branch chamada 'feature' e muda para ela.", catGit);
        AddCmd("Criar e Mudar Branch (Moderno)", "git switch -c feature", "Cria e alterna para a nova branch chamada 'feature'.", catGit);
        AddCmd("Mesclar Branch", "git merge branch", "Mescla o histórico da branch especificada na branch atual.", catGit);
        AddCmd("Rebase com Main", "git rebase main", "Aplica os commits locais no topo da branch main.", catGit);
        AddCmd("Salvar Modificações Temporariamente", "git stash", "Salva alterações não commitadas e limpa o diretório de trabalho.", catGit);
        AddCmd("Recuperar Modificações Salvas", "git stash pop", "Recupera e remove o último conjunto de alterações salvas com o stash.", catGit);
        AddCmd("Resetar Alterações (Hard)", "git reset --hard", "Descarta todas as alterações locais e commits desde o último commit.", catGit);
        AddCmd("Reverter Commit Específico", "git revert HASH", "Cria um novo commit que desfaz as alterações introduzidas pelo commit (HASH).", catGit);
        AddCmd("Histórico de Commits", "git log", "Mostra o histórico completo de commits do repositório.", catGit);
        AddCmd("Histórico Simplificado", "git log --oneline", "Exibe o histórico de commits de forma compacta (uma linha por commit).", catGit);
        AddCmd("Diferenças nos Arquivos", "git diff", "Mostra as diferenças entre os arquivos modificados e o último commit.", catGit);
        AddCmd("Listar Tags", "git tag", "Lista as tags (marcas de versão) do repositório.", catGit);
        AddCmd("Criar Tag", "git tag v1.0.0", "Cria uma tag leve chamada 'v1.0.0'.", catGit);
        AddCmd("Listar Remotos", "git remote -v", "Mostra as URLs dos repositórios remotos configurados.", catGit);
        AddCmd("Cherry-pick Commit", "git cherry-pick HASH", "Aplica o commit correspondente ao HASH na branch atual.", catGit);

        // --- GitHub CLI (gh) ---
        string catGh = "GitHub CLI (gh)";
        AddCmd("Login GitHub CLI", "gh auth login", "Inicia o processo de autenticação no GitHub.", catGh);
        AddCmd("Status de Autenticação", "gh auth status", "Verifica e exibe as credenciais de autenticação ativas.", catGh);
        AddCmd("Criar Repositório Remoto", "gh repo create", "Cria um novo repositório no GitHub.", catGh);
        AddCmd("Clonar Repositório Remoto", "gh repo clone dono/projeto", "Clona um repositório do GitHub localmente.", catGh);
        AddCmd("Criar Pull Request", "gh pr create", "Cria um novo Pull Request no GitHub a partir da branch atual.", catGh);
        AddCmd("Listar Pull Requests", "gh pr list", "Exibe os Pull Requests abertos para o repositório atual.", catGh);
        AddCmd("Visualizar Pull Request", "gh pr view", "Mostra os detalhes de um Pull Request no terminal.", catGh);
        AddCmd("Mesclar Pull Request", "gh pr merge", "Realiza o merge de um Pull Request ativo.", catGh);
        AddCmd("Criar Issue", "gh issue create", "Cria uma nova Issue no repositório do GitHub.", catGh);
        AddCmd("Listar Issues", "gh issue list", "Exibe a lista de Issues ativas do repositório.", catGh);
        AddCmd("Listar Workflows (Actions)", "gh workflow list", "Lista os workflows do GitHub Actions configurados.", catGh);
        AddCmd("Executar Workflow manualmente", "gh workflow run workflow.yml", "Dispara manualmente a execução de um workflow do Actions.", catGh);
        AddCmd("Criar Nova Release", "gh release create v1.0.0", "Cria uma nova versão (Release) no repositório.", catGh);
        AddCmd("Listar Releases", "gh release list", "Lista as releases existentes no repositório remoto.", catGh);

        // --- GitLab CLI ---
        string catGlab = "GitLab CLI (glab)";
        AddCmd("Login GitLab CLI", "glab auth login", "Inicia a autenticação no GitLab.", catGlab);
        AddCmd("Clonar Repositório GitLab", "glab repo clone dono/projeto", "Clona um repositório do GitLab.", catGlab);
        AddCmd("Criar Repositório GitLab", "glab repo create", "Cria um novo repositório no GitLab.", catGlab);
        AddCmd("Criar Merge Request", "glab mr create", "Cria um Merge Request a partir do branch atual.", catGlab);
        AddCmd("Listar Merge Requests", "glab mr list", "Exibe os Merge Requests ativos no repositório.", catGlab);
        AddCmd("Criar Issue GitLab", "glab issue create", "Cria uma nova issue no GitLab.", catGlab);

        // --- NPM ---
        string catNpm = "NPM";
        AddCmd("Inicializar Pacote Node.js", "npm init", "Guia interativo para criar o arquivo package.json.", catNpm);
        AddCmd("Inicialização Rápida", "npm init -y", "Cria o arquivo package.json com valores padrão automaticamente.", catNpm);
        AddCmd("Instalar Todas Dependências", "npm install", "Instala os pacotes listados no package.json.", catNpm);
        AddCmd("Instalar Dependência de Produção", "npm install pacote", "Instala um pacote e adiciona às dependências de produção.", catNpm);
        AddCmd("Instalar Dependência de Desenvolvimento", "npm install -D pacote", "Instala um pacote e adiciona às dependências de desenvolvimento.", catNpm);
        AddCmd("Desinstalar Pacote", "npm uninstall pacote", "Remove um pacote do projeto e do package.json.", catNpm);
        AddCmd("Atualizar Pacotes", "npm update", "Atualiza os pacotes instalados de acordo com as versões permitidas.", catNpm);
        AddCmd("Verificar Pacotes Desatualizados", "npm outdated", "Mostra quais dependências do projeto estão desatualizadas.", catNpm);
        AddCmd("Auditar Segurança", "npm audit", "Analisa as dependências instaladas em busca de vulnerabilidades.", catNpm);
        AddCmd("Corrigir Vulnerabilidades", "npm audit fix", "Instala automaticamente atualizações compatíveis para sanar vulnerabilidades.", catNpm);
        AddCmd("Executar Script Iniciar", "npm run start", "Executa o script 'start' definido no package.json.", catNpm);
        AddCmd("Compilar Projeto", "npm run build", "Executa o script 'build' definido no package.json.", catNpm);
        AddCmd("Executar Testes", "npm run test", "Executa a suíte de testes do projeto via npm.", catNpm);
        AddCmd("Limpar Cache NPM", "npm cache clean --force", "Limpa à força o cache local do gerenciador de pacotes NPM.", catNpm);
        AddCmd("Listar Dependências Instaladas", "npm ls", "Exibe a árvore de dependências instaladas no diretório atual.", catNpm);
        AddCmd("Publicar Pacote", "npm publish", "Publica o pacote no registro público do NPM.", catNpm);

        // --- PNPM ---
        string catPnpm = "PNPM";
        AddCmd("Instalar Dependências PNPM", "pnpm install", "Instala as dependências usando o cache compartilhado do pnpm.", catPnpm);
        AddCmd("Adicionar Pacote PNPM", "pnpm add pacote", "Adiciona uma nova dependência de produção.", catPnpm);
        AddCmd("Remover Pacote PNPM", "pnpm remove pacote", "Remove um pacote do projeto.", catPnpm);
        AddCmd("Atualizar Dependências PNPM", "pnpm update", "Atualiza os pacotes para as versões mais recentes permitidas.", catPnpm);
        AddCmd("Compilar Projeto PNPM", "pnpm run build", "Executa o script build.", catPnpm);
        AddCmd("Executar Testes PNPM", "pnpm run test", "Executa a suíte de testes do projeto.", catPnpm);
        AddCmd("Limpar Armazenamento PNPM", "pnpm store prune", "Remove pacotes não utilizados do armazenamento central do pnpm.", catPnpm);

        // --- Yarn ---
        string catYarn = "Yarn";
        AddCmd("Instalar Dependências Yarn", "yarn", "Instala todas as dependências listadas no package.json.", catYarn);
        AddCmd("Adicionar Pacote Yarn", "yarn add pacote", "Adiciona uma dependência ao projeto.", catYarn);
        AddCmd("Remover Pacote Yarn", "yarn remove pacote", "Remove uma dependência do projeto.", catYarn);
        AddCmd("Atualizar Dependências Yarn", "yarn upgrade", "Atualiza os pacotes instalados.", catYarn);
        AddCmd("Compilar Projeto Yarn", "yarn build", "Executa o script de build do package.json.", catYarn);
        AddCmd("Executar Testes Yarn", "yarn test", "Executa os testes do projeto.", catYarn);

        // --- Bun ---
        string catBun = "Bun";
        AddCmd("Inicializar Projeto Bun", "bun init", "Inicializa um novo projeto em branco com Bun.", catBun);
        AddCmd("Instalar Dependências Bun", "bun install", "Instala todas as dependências muito rapidamente.", catBun);
        AddCmd("Adicionar Pacote Bun", "bun add pacote", "Adiciona um pacote como dependência.", catBun);
        AddCmd("Remover Pacote Bun", "bun remove pacote", "Remove um pacote do projeto.", catBun);
        AddCmd("Atualizar Dependências Bun", "bun update", "Atualiza todos os pacotes instalados.", catBun);
        AddCmd("Executar Script Bun", "bun run script.ts", "Executa scripts ou comandos definidos muito rapidamente.", catBun);
        AddCmd("Executar Testes Bun", "bun test", "Roda o runner de testes nativo do Bun.", catBun);
        AddCmd("Criar Scaffold Bun", "bun create react app-name", "Cria um projeto usando um template rápido.", catBun);

        // --- Node.js & NPX ---
        string catNode = "Node.js & NPX";
        AddCmd("Iniciar Console Interativo", "node", "Abre o interpretador interativo REPL do Node.js.", catNode);
        AddCmd("Executar Arquivo", "node app.js", "Executa o arquivo JavaScript usando o Node.js.", catNode);
        AddCmd("Executar em Modo Observador", "node --watch app.js", "Executa observando alterações no código (nativo em novas versões).", catNode);
        AddCmd("Roda Teste Nativo", "node --test", "Executa o runner de testes nativo do Node.js.", catNode);
        AddCmd("Versão do Node", "node -v", "Exibe a versão do Node.js instalada.", catNode);
        AddCmd("Versão do NPM", "npm -v", "Exibe a versão do NPM instalada.", catNode);
        AddCmd("Novo Projeto Next.js (NPX)", "npx create-next-app@latest", "Inicia o scaffold de um projeto Next.js moderno.", catNode);
        AddCmd("Novo Projeto React (NPX)", "npx create-react-app app", "Cria uma nova aplicação React (SPA) legada.", catNode);
        AddCmd("Executar Prisma CLI (NPX)", "npx prisma studio", "Executa a CLI do ORM Prisma sem instalá-la globalmente.", catNode);
        AddCmd("Executar ESLint (NPX)", "npx eslint .", "Analisa a qualidade do código com ESLint.", catNode);
        AddCmd("Executar Prettier (NPX)", "npx prettier --write .", "Formata os arquivos do projeto com Prettier.", catNode);

        // --- Angular CLI ---
        string catAngular = "Angular CLI";
        AddCmd("Novo Projeto Angular", "ng new projeto", "Cria um novo projeto Angular com configurações básicas.", catAngular);
        AddCmd("Iniciar Servidor de Dev", "ng serve", "Compila a aplicação e inicia o servidor de desenvolvimento local.", catAngular);
        AddCmd("Compilar para Produção", "ng build", "Compila o aplicativo Angular para o diretório de saída.", catAngular);
        AddCmd("Executar Testes Unitários", "ng test", "Executa testes unitários com Karma/Jasmine.", catAngular);
        AddCmd("Verificar Padrões de Código", "ng lint", "Executa ferramentas de linting para verificar o estilo do código.", catAngular);
        AddCmd("Gerar Componente (Completo)", "ng generate component home", "Gera a estrutura de um novo componente chamado home.", catAngular);
        AddCmd("Gerar Componente (Curto)", "ng g c home", "Forma abreviada para gerar um novo componente.", catAngular);
        AddCmd("Gerar Serviço", "ng generate service auth", "Gera a estrutura de um novo serviço.", catAngular);
        AddCmd("Gerar Rota de Guarda", "ng generate guard auth", "Gera a estrutura de uma rota de guarda (route guard).", catAngular);
        AddCmd("Gerar Módulo", "ng generate module admin", "Gera a estrutura de um novo módulo.", catAngular);
        AddCmd("Adicionar PWA", "ng add @angular/pwa", "Configura a aplicação para rodar como Progressive Web App (PWA).", catAngular);
        AddCmd("Atualizar Angular", "ng update", "Atualiza as dependências do Angular para as versões mais recentes.", catAngular);
        AddCmd("Versão do Angular CLI", "ng version", "Exibe a versão do Angular CLI, Node.js e SO.", catAngular);

        // --- Outros Frameworks Web (Vue, Nuxt, Svelte) ---
        string catOtherWeb = "Vue, Nuxt & Svelte";
        AddCmd("Novo Projeto Vue", "npm create vue@latest", "Inicializa o instalador interativo de projetos Vue.js.", catOtherWeb);
        AddCmd("Executar Dev Server Vue/Svelte", "npm run dev", "Inicia o Vite Development Server.", catOtherWeb);
        AddCmd("Compilar Vue/Svelte", "npm run build", "Compila o projeto para o build de produção.", catOtherWeb);
        AddCmd("Visualizar Produção Local", "npm run preview", "Simula o servidor de produção localmente usando a build gerada.", catOtherWeb);
        AddCmd("Novo Projeto Nuxt", "npx nuxi init app-name", "Inicializa um novo projeto Nuxt.js.", catOtherWeb);
        AddCmd("Executar Dev Server Nuxt", "npx nuxi dev", "Inicia o servidor de desenvolvimento do Nuxt.", catOtherWeb);
        AddCmd("Compilar Nuxt", "npx nuxi build", "Compila a aplicação Nuxt para produção.", catOtherWeb);
        AddCmd("Gerar Estático Nuxt", "npx nuxi generate", "Exporta a aplicação Nuxt como páginas HTML estáticas.", catOtherWeb);
        AddCmd("Novo Projeto Svelte", "npm create svelte@latest app-name", "Inicializa o instalador de projetos Svelte/SvelteKit.", catOtherWeb);

        // --- Docker & Docker Compose ---
        string catDocker = "Docker & Compose";
        AddCmd("Versão do Docker", "docker version", "Exibe informações da versão do Docker instalada.", catDocker);
        AddCmd("Baixar Imagem", "docker pull nginx", "Baixa uma imagem do Docker Hub (ex: nginx).", catDocker);
        AddCmd("Listar Imagens", "docker images", "Lista todas as imagens locais disponíveis.", catDocker);
        AddCmd("Construir Imagem", "docker build -t app .", "Constrói uma imagem Docker a partir do Dockerfile na pasta atual.", catDocker);
        AddCmd("Executar Container", "docker run -p 8080:80 app", "Cria e inicia um novo container ligando a porta 8080 local à 80.", catDocker);
        AddCmd("Listar Containers Ativos", "docker ps", "Lista apenas os containers Docker em execução.", catDocker);
        AddCmd("Listar Todos Containers", "docker ps -a", "Lista todos os containers (ativos e inativos).", catDocker);
        AddCmd("Parar Container", "docker stop CONTAINER", "Para a execução de um container ativo.", catDocker);
        AddCmd("Iniciar Container Parado", "docker start CONTAINER", "Inicia um container parado.", catDocker);
        AddCmd("Reiniciar Container", "docker restart CONTAINER", "Reiniciar o container.", catDocker);
        AddCmd("Remover Container", "docker rm CONTAINER", "Remove um container parado.", catDocker);
        AddCmd("Remover Imagem", "docker rmi IMAGE", "Deleta uma imagem Docker local.", catDocker);
        AddCmd("Logs do Container", "docker logs CONTAINER", "Mostra os logs de saída de um container específico.", catDocker);
        AddCmd("Entrar no Container", "docker exec -it CONTAINER bash", "Abre um terminal interativo dentro de um container rodando.", catDocker);
        AddCmd("Limpeza de Sistema Docker", "docker system prune", "Remove containers parados, redes sem uso, imagens sem tag e cache.", catDocker);
        AddCmd("Subir Serviços Compose", "docker compose up", "Cria e inicia os serviços definidos no docker-compose.yml.", catDocker);
        AddCmd("Subir Compose (Background)", "docker compose up -d", "Inicia os serviços do docker-compose em segundo plano (detached).", catDocker);
        AddCmd("Parar e Remover Compose", "docker compose down", "Para e remove containers, redes e volumes criados pelo compose.", catDocker);
        AddCmd("Compilar Imagens Compose", "docker compose build", "Recompila as imagens do docker-compose.", catDocker);
        AddCmd("Visualizar Logs Compose", "docker compose logs", "Exibe a saída consolidada de logs do compose.", catDocker);
        AddCmd("Status dos Serviços Compose", "docker compose ps", "Lista os containers gerenciados pelo compose atual.", catDocker);

        // --- Kubernetes, Minikube & OpenShift ---
        string catK8s = "Kubernetes & Orchestrators";
        AddCmd("Versão do Kubernetes", "kubectl version", "Exibe informações da versão do Kubernetes.", catK8s);
        AddCmd("Listar Pods", "kubectl get pods", "Lista os Pods ativos no namespace atual.", catK8s);
        AddCmd("Listar Serviços", "kubectl get svc", "Lista os serviços (Services) mapeados.", catK8s);
        AddCmd("Listar Deployments", "kubectl get deployments", "Lista os Deployments do cluster.", catK8s);
        AddCmd("Descrever Recurso", "kubectl describe pod POD_NAME", "Exibe informações detalhadas de status e configuração do Pod.", catK8s);
        AddCmd("Logs do Pod", "kubectl logs POD_NAME", "Mostra os logs emitidos pelos containers no Pod.", catK8s);
        AddCmd("Executar Terminal no Pod", "kubectl exec -it POD_NAME -- bash", "Abre um terminal interativo dentro de um Pod.", catK8s);
        AddCmd("Aplicar Configuração", "kubectl apply -f deployment.yaml", "Cria ou atualiza recursos a partir de um manifesto YAML.", catK8s);
        AddCmd("Deletar por Manifesto", "kubectl delete -f deployment.yaml", "Deleta os recursos descritos no manifesto YAML.", catK8s);
        AddCmd("Escalar Deployment", "kubectl scale deployment app --replicas=5", "Muda o número de réplicas de um deployment para 5.", catK8s);
        AddCmd("Reiniciar Deployment", "kubectl rollout restart deployment app", "Força o reinício de todos os pods de um deployment.", catK8s);
        AddCmd("Iniciar Minikube", "minikube start", "Inicia o cluster local do Kubernetes (Minikube).", catK8s);
        AddCmd("Parar Minikube", "minikube stop", "Para o cluster local do Minikube.", catK8s);
        AddCmd("Minikube Dashboard", "minikube dashboard", "Abre o painel web administrativo do Minikube.", catK8s);
        AddCmd("Minikube Tunnel", "minikube tunnel", "Cria uma ponte de rede para expor serviços LoadBalancer localmente.", catK8s);
        AddCmd("Login OpenShift", "oc login -u developer", "Realiza o login no console da CLI do OpenShift.", catK8s);
        AddCmd("Selecionar Projeto OpenShift", "oc project meu-projeto", "Muda para o projeto OpenShift especificado.", catK8s);
        AddCmd("Listar Projetos OpenShift", "oc projects", "Lista todos os projetos do usuário.", catK8s);

        // --- Terraform, OpenTofu & Pulumi ---
        string catIaC = "Infraestrutura como Código";
        AddCmd("Inicializar Terraform", "terraform init", "Baixa provedores e módulos necessários.", catIaC);
        AddCmd("Planejar Alterações", "terraform plan", "Gera um plano de execução detalhando as alterações.", catIaC);
        AddCmd("Aplicar Plano Terraform", "terraform apply", "Aplica as modificações na infraestrutura real.", catIaC);
        AddCmd("Destruir Infraestrutura", "terraform destroy", "Exclui permanentemente os recursos criados.", catIaC);
        AddCmd("Validar Configurações", "terraform validate", "Verifica a sintaxe dos arquivos de configuração.", catIaC);
        AddCmd("Formatar Arquivos TF", "terraform fmt", "Formata os arquivos .tf no padrão oficial.", catIaC);
        AddCmd("Inicializar OpenTofu", "tofu init", "Inicializa o projeto com a ferramenta open-source OpenTofu.", catIaC);
        AddCmd("Planejar OpenTofu", "tofu plan", "Visualiza o plano de execução no OpenTofu.", catIaC);
        AddCmd("Aplicar OpenTofu", "tofu apply", "Aplica a infraestrutura usando OpenTofu.", catIaC);
        AddCmd("Login Pulumi", "pulumi login", "Realiza a autenticação na plataforma Pulumi.", catIaC);
        AddCmd("Criar Stack Pulumi", "pulumi stack init dev", "Cria um novo ambiente/stack no Pulumi.", catIaC);
        AddCmd("Subir Infraestrutura Pulumi", "pulumi up", "Aplica a infraestrutura definida em código (ex: TS, Python).", catIaC);
        AddCmd("Destruir Pulumi", "pulumi destroy", "Remove toda a infraestrutura controlada pelo stack.", catIaC);

        // --- Ansible & Vagrant ---
        string catAutomation = "Ansible & Vagrant";
        AddCmd("Executar Ad-Hoc Ansible", "ansible all -m ping", "Executa um comando rápido em todos os servidores do inventário.", catAutomation);
        AddCmd("Executar Playbook Ansible", "ansible-playbook -i hosts site.yml", "Executa tarefas de automação descritas em arquivos YAML.", catAutomation);
        AddCmd("Criptografar Vault Ansible", "ansible-vault encrypt segredos.yml", "Criptografa arquivos de variáveis contendo segredos.", catAutomation);
        AddCmd("Inicializar Máquina Vagrant", "vagrant init ubuntu/focal64", "Gera o arquivo Vagrantfile básico de configuração.", catAutomation);
        AddCmd("Subir Máquina Vagrant", "vagrant up", "Cria e configura a máquina virtual descrita no Vagrantfile.", catAutomation);
        AddCmd("Acessar por SSH Vagrant", "vagrant ssh", "Conecta na máquina virtual rodando via SSH.", catAutomation);
        AddCmd("Desligar Vagrant", "vagrant halt", "Desliga graciosamente a VM do Vagrant.", catAutomation);
        AddCmd("Excluir Máquina Vagrant", "vagrant destroy", "Para e deleta todos os discos e arquivos da VM virtual.", catAutomation);

        // --- Cloud Providers (AWS, GCP, Azure) ---
        string catCloud = "Provedores de Nuvem";
        AddCmd("Configurar AWS CLI", "aws configure", "Configura chaves de acesso, região e formato de saída.", catCloud);
        AddCmd("Verificar Identidade AWS", "aws sts get-caller-identity", "Retorna a conta AWS e o usuário autenticado atualmente.", catCloud);
        AddCmd("Listar Buckets S3", "aws s3 ls", "Lista os buckets de armazenamento de arquivos no Amazon S3.", catCloud);
        AddCmd("Copiar Arquivo S3", "aws s3 cp arq.txt s3://bucket/", "Copia um arquivo local para o bucket S3.", catCloud);
        AddCmd("Sincronizar Pasta S3", "aws s3 sync local/ s3://bucket/", "Sincroniza um diretório com o bucket.", catCloud);
        AddCmd("Listar Instâncias EC2", "aws ec2 describe-instances", "Lista e detalha todos os servidores virtuais EC2.", catCloud);
        AddCmd("Inicializar gcloud CLI", "gcloud init", "Inicia o setup interativo do Google Cloud.", catCloud);
        AddCmd("Login Google Cloud", "gcloud auth login", "Realiza a autenticação de login na conta GCP.", catCloud);
        AddCmd("Listar Projetos GCP", "gcloud projects list", "Exibe os projetos do Google Cloud disponíveis.", catCloud);
        AddCmd("Deploy Cloud Run (GCP)", "gcloud run deploy", "Deploy de aplicação containerizada de forma serverless.", catCloud);
        AddCmd("Autenticar Azure CLI", "az login", "Realiza o login de sua conta Microsoft na nuvem Azure.", catCloud);
        AddCmd("Criar Resource Group (Azure)", "az group create --name RG --location eastus", "Cria um grupo de recursos na Azure.", catCloud);
        AddCmd("Listar Contas de Storage Azure", "az storage account list", "Lista as contas de armazenamento criadas na Azure.", catCloud);

        // --- PaaS & Serverless (Vercel, Netlify, Railway, Fly.io) ---
        string catPaas = "PaaS & Deploy Rápido";
        AddCmd("Login Vercel CLI", "vercel login", "Autentica na conta da Vercel.", catPaas);
        AddCmd("Deploy Vercel Preview", "vercel", "Publica o projeto em URL temporária de preview.", catPaas);
        AddCmd("Deploy Vercel Produção", "vercel --prod", "Publica o projeto em ambiente de produção.", catPaas);
        AddCmd("Adicionar Var Vercel", "vercel env add NOME_VAR", "Adiciona uma variável de ambiente no projeto da Vercel.", catPaas);
        AddCmd("Login Netlify CLI", "netlify login", "Abre navegador para autenticar o login do Netlify.", catPaas);
        AddCmd("Deploy Netlify Preview", "netlify deploy", "Gera uma URL temporária de teste no Netlify.", catPaas);
        AddCmd("Deploy Netlify Produção", "netlify deploy --prod", "Publica o site de forma definitiva.", catPaas);
        AddCmd("Login Railway", "railway login", "Autentica a CLI com a conta do Railway.", catPaas);
        AddCmd("Deploy Railway", "railway up", "Faz o build e deploy do diretório atual.", catPaas);
        AddCmd("Deploy Fly.io", "fly deploy", "Faz o deploy do aplicativo configurado na plataforma Fly.io.", catPaas);
        AddCmd("Logs do App Fly.io", "fly logs", "Exibe os logs de execução do app em tempo real no Fly.io.", catPaas);

        // --- Backend BaaS (Firebase, Supabase) ---
        string catBaas = "Firebase & Supabase";
        AddCmd("Autenticar Firebase", "firebase login", "Autentica na CLI do Firebase.", catBaas);
        AddCmd("Inicializar Firebase", "firebase init", "Configura serviços do Firebase no projeto local.", catBaas);
        AddCmd("Publicar Recursos Firebase", "firebase deploy", "Faz deploy de hospedagem, funções e regras configuradas.", catBaas);
        AddCmd("Iniciar Emuladores Firebase", "firebase emulators:start", "Inicia a suite local para testes off-line.", catBaas);
        AddCmd("Login Supabase", "supabase login", "Autentica o desenvolvedor na CLI do Supabase.", catBaas);
        AddCmd("Inicializar Supabase", "supabase init", "Cria configurações de projeto locais para o Supabase.", catBaas);
        AddCmd("Iniciar Supabase Local", "supabase start", "Inicia serviços do Supabase locais utilizando o Docker.", catBaas);
        AddCmd("Parar Supabase Local", "supabase stop", "Para a execução dos containers locais do Supabase.", catBaas);
        AddCmd("Puxar Schema do Banco", "supabase db pull", "Faz download do esquema do banco de dados remoto.", catBaas);
        AddCmd("Criar Migration Supabase", "supabase migration new nome", "Gera um arquivo de migração SQL local.", catBaas);

        // --- Databases (SQL & NoSQL) ---
        string catDb = "Banco de Dados";
        AddCmd("Conectar PostgreSQL", "psql -U usuario -d banco", "Conecta ao terminal interativo psql do Postgres.", catDb);
        AddCmd("Listar Bancos (Postgres)", "\\l", "Lista todos os bancos de dados (dentro do psql).", catDb);
        AddCmd("Listar Tabelas (Postgres)", "\\dt", "Exibe todas as tabelas do banco atual (dentro do psql).", catDb);
        AddCmd("Conectar MySQL", "mysql -u root -p", "Conecta ao console do MySQL com senha.", catDb);
        AddCmd("Listar Bancos (MySQL)", "SHOW DATABASES;", "Lista todos os bancos do servidor MySQL.", catDb);
        AddCmd("Conectar SQL Server", "sqlcmd -S localhost -U SA -P 'Senha'", "Conecta à CLI do SQL Server.", catDb);
        AddCmd("Conectar MongoDB Shell", "mongosh", "Inicia o console interativo do MongoDB.", catDb);
        AddCmd("Listar Bancos (Mongo)", "show dbs", "Lista os bancos de dados dentro do Mongo Shell.", catDb);
        AddCmd("Buscar Registros (Mongo)", "db.collection.find()", "Retorna registros de uma coleção específica no Mongo.", catDb);
        AddCmd("Conectar Redis CLI", "redis-cli", "Inicia o console interativo do Redis.", catDb);
        AddCmd("Definir Chave-Valor Redis", "SET chave valor", "Salva um valor sob uma chave no Redis.", catDb);
        AddCmd("Obter Chave Redis", "GET chave", "Recupera o valor correspondente à chave no Redis.", catDb);
        AddCmd("Abrir Banco SQLite", "sqlite3 banco.db", "Abre a CLI para gerenciar um arquivo de banco de dados SQLite.", catDb);
        AddCmd("Listar Tabelas (SQLite)", ".tables", "Exibe todas as tabelas no terminal do SQLite.", catDb);
        AddCmd("Conectar Cassandra", "cqlsh", "Inicia a CLI interativa para Cassandra.", catDb);

        // --- Java Backend (Maven, Gradle, Spring Boot, Quarkus) ---
        string catJava = "Java & Frameworks";
        AddCmd("Limpar Pasta Target Maven", "mvn clean", "Remove a pasta target de builds anteriores.", catJava);
        AddCmd("Compilar Maven", "mvn compile", "Compila o código-fonte Java do projeto.", catJava);
        AddCmd("Executar Testes Maven", "mvn test", "Executa a suite de testes unitários.", catJava);
        AddCmd("Gerar JAR/WAR Maven", "mvn package", "Compila, testa e gera o arquivo distribuível final.", catJava);
        AddCmd("Instalar Local Maven", "mvn install", "Instala o pacote no repositório Maven local (.m2).", catJava);
        AddCmd("Executar Spring Boot (Maven)", "mvn spring-boot:run", "Inicia o projeto Spring Boot rapidamente.", catJava);
        AddCmd("Limpar Pasta Build Gradle", "gradle clean", "Remove a pasta build gerada.", catJava);
        AddCmd("Compilar e Testar Gradle", "gradle build", "Compila e empacota o projeto usando Gradle.", catJava);
        AddCmd("Executar Spring Boot (Gradle)", "gradle bootRun", "Inicia a aplicação Spring Boot configurada com Gradle.", catJava);
        AddCmd("Executar JAR Empacotado", "java -jar app.jar", "Executa um aplicativo java empacotado nativamente.", catJava);
        AddCmd("Criar App Quarkus", "quarkus create app meu-app", "Gera o scaffold de um projeto Quarkus.", catJava);
        AddCmd("Iniciar Dev Mode Quarkus", "quarkus dev", "Inicia servidor de desenvolvimento com Live Reload do Quarkus.", catJava);

        // --- Outros Frameworks Backend (NestJS, FastAPI, Django, Flask, Laravel) ---
        string catBackends = "Outros Backends (Nest, Python, PHP)";
        AddCmd("Novo Projeto NestJS", "nest new projeto", "Gera a estrutura de um novo projeto NestJS.", catBackends);
        AddCmd("Gerar Módulo NestJS", "nest generate module modulo", "Gera um novo módulo no projeto NestJS.", catBackends);
        AddCmd("Executar Dev Server NestJS", "npm run start:dev", "Inicia o NestJS com monitoramento automático de arquivos.", catBackends);
        AddCmd("Executar FastAPI (Uvicorn)", "uvicorn main:app --reload", "Executa servidor ASGI para FastAPI com reload ativo.", catBackends);
        AddCmd("FastAPI Dev Mode", "fastapi dev main.py", "Inicia o servidor de desenvolvimento nativo do FastAPI.", catBackends);
        AddCmd("Novo Projeto Django", "django-admin startproject nome", "Cria um novo projeto Django.", catBackends);
        AddCmd("Rodar Servidor Django", "python manage.py runserver", "Inicia o servidor de desenvolvimento do Django.", catBackends);
        AddCmd("Aplicar Migrations Django", "python manage.py migrate", "Aplica as migrações de banco pendentes no Django.", catBackends);
        AddCmd("Criar Superuser Django", "python manage.py createsuperuser", "Cria usuário admin para acesso ao painel do Django.", catBackends);
        AddCmd("Executar Servidor Flask", "flask run", "Inicia o servidor de desenvolvimento do Flask.", catBackends);
        AddCmd("Executar Servidor Laravel", "php artisan serve", "Inicia o servidor de desenvolvimento PHP do Laravel.", catBackends);
        AddCmd("Executar Migrations Laravel", "php artisan migrate", "Aplica migrations de banco de dados no Laravel.", catBackends);
        AddCmd("Criar Controller Laravel", "php artisan make:controller Nome", "Gera a estrutura de um controlador no Laravel.", catBackends);

        // --- Mobile (Flutter, Android, React Native, Expo, Ionic/Capacitor) ---
        string catMobile = "Mobile Development";
        AddCmd("Criar App Flutter", "flutter create meu_app", "Cria a estrutura de um aplicativo multiplataforma Flutter.", catMobile);
        AddCmd("Executar App Flutter", "flutter run", "Executa a aplicação no emulador ou dispositivo conectado.", catMobile);
        AddCmd("Build APK Android", "flutter build apk", "Gera o instalador APK de produção para Android.", catMobile);
        AddCmd("Verificar Setup Flutter", "flutter doctor", "Checa o estado das ferramentas de desenvolvimento instaladas.", catMobile);
        AddCmd("Instalar Dependências Flutter", "flutter pub get", "Baixa os pacotes listados no pubspec.yaml.", catMobile);
        AddCmd("Listar Dispositivos ADB", "adb devices", "Exibe a lista de dispositivos/emuladores Android conectados.", catMobile);
        AddCmd("Instalar APK via ADB", "adb install app.apk", "Instala um arquivo APK no dispositivo via linha de comando.", catMobile);
        AddCmd("Iniciar Emulador Android", "emulator -avd NomeEmulador", "Inicializa o emulador do Android Studio pela CLI.", catMobile);
        AddCmd("Novo App React Native", "npx react-native init App", "Gera o esqueleto de um app React Native CLI puro.", catMobile);
        AddCmd("Iniciar Metro Bundler", "npx react-native start", "Inicia o servidor de empacotamento JS do React Native.", catMobile);
        AddCmd("Executar React Native Android", "npx react-native run-android", "Compila e executa o app React Native no Android.", catMobile);
        AddCmd("Iniciar Expo CLI", "npx expo start", "Inicia o servidor Expo para desenvolvimento rápido com QR Code.", catMobile);
        AddCmd("Novo App Ionic", "ionic start", "Inicia o instalador interativo para projetos Ionic.", catMobile);
        AddCmd("Executar Ionic Dev", "ionic serve", "Executa o projeto no navegador local.", catMobile);
        AddCmd("Sincronizar Capacitor", "npx cap sync", "Copia a build web e sincroniza plugins para as pastas nativas.", catMobile);

        // --- IA / LLMs (OpenAI, Ollama, Hugging Face, MLflow, W&B) ---
        string catAi = "IA & LLMs";
        AddCmd("Listar Modelos OpenAI", "openai api models.list", "Retorna a listação de modelos disponíveis na OpenAI CLI.", catAi);
        AddCmd("Baixar Modelo Ollama", "ollama pull llama3", "Faz o download de um LLM local (ex: llama3).", catAi);
        AddCmd("Executar Modelo Ollama", "ollama run llama3", "Inicia um chat interativo com o modelo local.", catAi);
        AddCmd("Listar Modelos Ollama", "ollama list", "Lista todos os modelos de IA instalados no computador.", catAi);
        AddCmd("Modelos Rodando (Ollama)", "ollama ps", "Mostra quais modelos estão carregados na memória do Ollama.", catAi);
        AddCmd("Login Hugging Face", "huggingface-cli login", "Realiza a autenticação na plataforma Hugging Face.", catAi);
        AddCmd("Download Hugging Face", "huggingface-cli download repo-id", "Faz o download de modelos ou datasets.", catAi);
        AddCmd("Iniciar MLflow UI", "mlflow ui", "Inicia o dashboard local para tracking de experimentos de ML.", catAi);
        AddCmd("Servir Modelo MLflow", "mlflow models serve -m modelo", "Cria um endpoint HTTP local para servir previsões do modelo.", catAi);
        AddCmd("Login Weights & Biases", "wandb login", "Autentica no painel de monitoramento de treinamento de ML.", catAi);

        // --- Observabilidade (Grafana, Prometheus, Datadog, New Relic) ---
        string catObs = "Observabilidade";
        AddCmd("Instalar Plugin Grafana", "grafana-cli plugins install id", "Instala um plugin no servidor local do Grafana.", catObs);
        AddCmd("Iniciar Prometheus", "prometheus --config.file=config.yml", "Inicia o servidor Prometheus com arquivo de configuração.", catObs);
        AddCmd("Status Datadog Agent", "datadog-agent status", "Exibe o status atual do agente do Datadog.", catObs);
        AddCmd("Reiniciar Datadog Agent", "datadog-agent restart", "Reinicia o serviço de monitoramento do Datadog.", catObs);
        AddCmd("Login New Relic", "newrelic login", "Autentica as ferramentas da CLI do New Relic.", catObs);
        AddCmd("Listar Entidades New Relic", "newrelic entity list", "Exibe as entidades registradas no painel da conta.", catObs);

        // --- Windows Package Managers (Chocolatey) & PowerShell ---
        string catWin = "Windows (Choco & PowerShell)";
        AddCmd("Instalar Pacote Choco", "choco install nodejs", "Instala um pacote usando o Chocolatey.", catWin);
        AddCmd("Desinstalar Pacote Choco", "choco uninstall nodejs", "Desinstala um pacote.", catWin);
        AddCmd("Atualizar Pacote Choco", "choco upgrade nodejs", "Atualiza um pacote para a versão estável mais recente.", catWin);
        AddCmd("Atualizar Tudo (Choco)", "choco upgrade all", "Atualiza todos os programas instalados via Chocolatey.", catWin);
        AddCmd("Buscar Programas Choco", "choco search chrome", "Pesquisa por pacotes no repositório do Chocolatey.", catWin);
        AddCmd("Verificar Desatualizados Choco", "choco outdated", "Mostra quais pacotes instalados têm atualizações disponíveis.", catWin);
        AddCmd("Listar Processos (PowerShell)", "Get-Process | Sort-Object CPU -Descending | Select-Object -First 10", "Lista os 10 processos que mais consomem CPU.", catWin);
        AddCmd("Reiniciar Adaptador de Rede", "Restart-NetAdapter -Name \"Ethernet\"", "Reinicia o adaptador de rede (Requer Admin).", catWin);
        AddCmd("Ver IP Local (PowerShell)", "Get-NetIPAddress -AddressFamily IPv4 | Select-Object IPAddress", "Mostra o IP local do computador.", catWin);
        AddCmd("Limpar Cache DNS (PowerShell)", "Clear-DnsClientCache", "Limpa o cache do cliente de DNS local.", catWin);
        AddCmd("Procurar PDF no Disco", "Get-ChildItem -Path C:\\ -Filter \"*.pdf\" -Recurse -ErrorAction SilentlyContinue", "Busca todos os PDFs no disco C: ignorando erros de acesso.", catWin);
        AddCmd("Parar Processo Travado", "Stop-Process -Name \"chrome\" -Force", "Força o fechamento de um processo pelo nome.", catWin);

        // --- macOS Essentials (macOS Native Commands) ---
        string catMac = "macOS Essentials";
        AddCmd("Instalar Homebrew", "/bin/bash -c \"$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)\"", "Baixa e instala o gerenciador de pacotes Homebrew no macOS.", catMac);
        AddCmd("Instalar Pacote Brew", "brew install git", "Instala programas/bibliotecas via Homebrew (ex: git).", catMac);
        AddCmd("Atualizar Homebrew", "brew update", "Busca novas definições de fórmulas de pacotes no Homebrew.", catMac);
        AddCmd("Atualizar Programas Brew", "brew upgrade", "Atualiza todos os pacotes instalados via Homebrew.", catMac);
        AddCmd("Desinstalar Pacote Brew", "brew uninstall node", "Remove o programa especificado.", catMac);
        AddCmd("Listar Pacotes Brew", "brew list", "Exibe todos os pacotes instalados localmente pelo Brew.", catMac);
        AddCmd("Ping de Rede", "ping google.com", "Verifica a conectividade e latência com o host.", catMac);
        AddCmd("Visualizar Configuração IP", "ifconfig", "Mostra informações de interfaces de rede (macOS/Linux).", catMac);
        AddCmd("Conexões de Rede", "netstat -an", "Exibe o estado de portas de sockets ativos.", catMac);
        AddCmd("Abrir com Editor Nano", "nano arquivo.txt", "Abre um editor de texto de terminal simples.", catMac);
        AddCmd("Abrir com Editor Vim", "vim arquivo.txt", "Abre o editor de texto avançado Vim.", catMac);

        // --- Linux Essentials ---
        string catLinux = "Linux Essentials";
        AddCmd("Sincronizar Arquivos (rsync)", "rsync -avz pasta/ destino/", "Sincroniza arquivos entre pastas ou de forma remota de forma eficiente.", catLinux);
        AddCmd("Copiar via SSH (scp)", "scp arquivo.txt user@ip:/caminho/", "Copia arquivos entre computadores através de protocolo SSH seguro.", catLinux);
        AddCmd("Multiplexador de Terminal (tmux)", "tmux", "Inicia sessão do tmux que permite dividir a janela e manter processos abertos.", catLinux);
        AddCmd("Processar JSON (jq)", "jq .campo arquivo.json", "Ferramenta de linha de comando para formatar e filtrar JSON.", catLinux);
        AddCmd("Processar YAML (yq)", "yq .campo arquivo.yml", "Processador de arquivos YAML.", catLinux);
        AddCmd("Substituir Texto (sed)", "sed 's/antigo/novo/g' arquivo.txt", "Editor de fluxo para filtrar e transformar textos em arquivos.", catLinux);
        AddCmd("Processar Colunas (awk)", "awk '{print $1}' arquivo.txt", "Linguagem de processamento de texto focada em colunas e dados formatados.", catLinux);

        // --- Categorias Iniciais ---
        Categories.Add("Todos");
        foreach (var category in _allCommands.Select(c => c.Category).Distinct().OrderBy(c => c))
        {
            Categories.Add(category);
        }
    }

    partial void OnSearchQueryChanged(string value) => ApplyFilters();
    partial void OnSelectedCategoryChanged(string value) => ApplyFilters();

    private void ApplyFilters()
    {
        FilteredCommands.Clear();
        var query = SearchQuery?.ToLowerInvariant() ?? "";
        
        foreach (var cmd in _allCommands)
        {
            bool matchCategory = SelectedCategory == "Todos" || cmd.Category == SelectedCategory;
            bool matchQuery = string.IsNullOrEmpty(query) || 
                              cmd.Title.ToLowerInvariant().Contains(query) || 
                              cmd.Command.ToLowerInvariant().Contains(query) || 
                              cmd.Description.ToLowerInvariant().Contains(query);

            if (matchCategory && matchQuery)
            {
                FilteredCommands.Add(cmd);
            }
        }
    }

    [RelayCommand]
    private void CopyCommand(string commandText)
    {
        if (string.IsNullOrEmpty(commandText)) return;
        try
        {
            var package = new DataPackage();
            package.SetText(commandText);
            Clipboard.SetContent(package);
        }
        catch (Exception)
        {
            // Fail-safe
        }
    }
}
