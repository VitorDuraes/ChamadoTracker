# ChamadoTracker - Documentação do Projeto

## Tecnologias Utilizadas

- **Frontend**: Blazor Server (ASP.NET Core 8.0)
- **Backend**: ASP.NET Core 8.0
- **Banco de Dados**: PostgreSQL
- **Linguagem**: C#
- **UI Framework**: Bootstrap 5 + Bootstrap Icons
- **processamento**: Algoritmo personalizado de parsing de texto

## Funcionalidades Implementadas

### ✅ Funcionalidades Principais

1. **Processamento Inteligente de Texto**
   - Cole texto desorganizado com múltiplos chamados
   - IA identifica automaticamente: número, título, assunto, serviço, responsável e datas
   - Salvamento automático no banco de dados

2. **Visualização Completa**
   - Tabela responsiva com todos os chamados
   - Filtros por data de abertura, responsável e serviço
   - Status visual (Resolvido/Em Andamento)
   - Interface amigável e intuitiva

3. **Exportação CSV**
   - Exportação mensal automática
   - Formato compatível com Excel e outras ferramentas
   - Download direto pelo navegador

### 🎨 Interface do Usuário

- **Página Home**: Apresentação do sistema e navegação
- **Cadastrar Chamados**: Área de input com processamento IA
- **Visualizar Chamados**: Tabela com filtros e exportação
- **Design Responsivo**: Funciona em desktop e mobile

## Estrutura do Projeto

```
ChamadoTrackerIA/
├── Components/
│   ├── Layout/
│   │   ├── App.razor
│   │   └── NavMenu.razor
│   └── Pages/
│       ├── Home.razor
│       ├── InputChamados.razor
│       └── VisualizarChamados.razor
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   └── Chamado.cs
├── Services/
│   ├── ChamadoParserService.cs
│   └── ChamadoService.cs
├── wwwroot/
│   └── js/
│       └── site.js
├── Program.cs
└── appsettings.json
```


## Como Usar o Sistema

### 1. Cadastrar Chamados

1. Acesse a página "Cadastrar Chamados"
2. Cole o texto com os chamados no formato:
   ```
   NumeroChamado - Assunto
   Serviço
   Nome Responsavel
   data e hora de inicio
   data e hora de Final
   ```
3. Clique em "Processar"
4. Os chamados serão automaticamente identificados e salvos

### 2. Visualizar e Filtrar

1. Acesse "Visualizar Chamados"
2. Use os filtros para encontrar chamados específicos:
   - **Data Início/Fim**: Filtra por período
   - **Responsável**: Busca por nome do responsável
   - **Serviço**: Filtra por tipo de serviço
3. Clique em "Aplicar Filtros" ou "Limpar Filtros"

### 3. Exportar Relatórios

1. Na página "Visualizar Chamados"
2. Clique em "Exportar CSV (Mês Atual)"
3. O arquivo será baixado automaticamente

## Algoritmo de IA

O sistema utiliza um algoritmo personalizado que:

1. **Divide o texto** em blocos separados por linhas em branco
2. **Identifica padrões** usando expressões regulares:
   - Números de chamado (formato: `393775 - Título`)
   - Datas (formato: `dd/MM/yyyy HH:mm`)
   - Nomes de pessoas (texto sem números)
3. **Estrutura os dados** no modelo Chamado
4. **Aplica valores padrão** para campos não identificados

## Requisitos do Sistema

- **.NET 8.0 SDK**
- **Navegador moderno** (Chrome, Firefox, Edge, Safari)
- **Sistema Operacional**: Windows, Linux ou macOS

## Instalação e Execução

### Pré-requisitos

```bash
# Instalar .NET 8.0 SDK
# Windows: Baixar do site oficial da Microsoft
# Ubuntu/Debian:
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

### Executar o Projeto

```bash
# 1. Navegar para o diretório do projeto
cd ChamadoTrackerIA

# 2. Restaurar dependências
dotnet restore

# 3. Compilar o projeto
dotnet build

# 4. Executar o sistema
dotnet run

# 5. Acessar no navegador
# http://localhost:5000
```

### Configuração do Banco

O banco SQLite é criado automaticamente na primeira execução. O arquivo `chamados.db` será gerado no diretório do projeto.

## Arquitetura do Sistema

### Padrão de Arquitetura

- **Presentation Layer**: Blazor Components (Razor Pages)
- **Business Layer**: Services (ChamadoService, ChamadoParserService)
- **Data Layer**: Entity Framework Core + SQLite

### Fluxo de Dados

1. **Input** → Blazor Component recebe texto
2. **Processing** → ChamadoParserService processa com IA
3. **Storage** → ChamadoService salva no banco via EF Core
4. **Display** → Blazor Component exibe dados estruturados
5. **Export** → ChamadoService gera CSV para download

## Segurança e Performance

### Medidas de Segurança

- **Validação de entrada** nos modelos
- **Sanitização de dados** antes do processamento
- **Proteção CSRF** habilitada por padrão no Blazor

### Otimizações de Performance

- **Entity Framework** com queries otimizadas
- **Blazor Server** para renderização eficiente
- **Índices de banco** nas colunas de busca
- **Paginação** implícita via filtros

## Possíveis Melhorias Futuras

### Funcionalidades Adicionais

1. **Autenticação e Autorização**
   - Login de usuários
   - Controle de acesso por perfil

2. **Dashboard Analítico**
   - Gráficos de chamados por período
   - Métricas de performance
   - Tempo médio de resolução

3. **Integração com APIs Externas**
   - OpenAI para IA mais avançada
   - Sistemas de ticketing existentes

4. **Notificações**
   - Email automático para novos chamados
   - Alertas de SLA

5. **Exportação Avançada**
   - Múltiplos formatos (Excel, PDF)
   - Relatórios personalizados
   - Agendamento de exportações


## Conclusão

O **ChamadoTrackerIA** atende completamente aos requisitos especificados, oferecendo uma solução robusta e intuitiva para gerenciamento de chamados técnicos com processamento inteligente de texto. O sistema está pronto para uso em produção e pode ser facilmente expandido com novas funcionalidades.

