# GitHub Actions Pipeline - FastTechFoods

## Sobre a Pipeline

Esta pipeline do GitHub Actions foi configurada para automatizar o processo de build e teste do projeto FastTechFoods.

### O que a pipeline faz:

1. **Checkout do código**: Baixa o código do repositório
2. **Setup do .NET**: Configura o ambiente .NET 8.0
3. **Restore de dependências**: Restaura todos os pacotes NuGet
4. **Build da solução**: Compila toda a solução em modo Release
5. **Execução de testes**: Roda todos os testes unitários
6. **Build individual dos serviços**: Compila cada serviço individualmente

### Triggers (Quando a pipeline executa):

- Push para as branches `main` ou `develop`
- Pull Request para as branches `main` ou `develop`

## Como usar:

### 1. Criar a pipeline no GitHub:

1. Vá para o seu repositório no GitHub
2. Clique na aba "Actions"
3. Clique em "New workflow"
4. Escolha "set up a workflow yourself"
5. Substitua o conteúdo pelo arquivo `build.yml` que criamos
6. Clique em "Commit changes"

### 2. Executar a pipeline manualmente:

1. Vá para a aba "Actions" no GitHub
2. Clique no workflow "Build and Test FastTechFoods"
3. Clique no botão "Run workflow"
4. Selecione a branch e clique em "Run workflow"

### 3. Verificar o status:

- Vá para a aba "Actions" para ver o histórico de execuções
- Clique em uma execução para ver os logs detalhados
- A pipeline mostrará ✅ (sucesso) ou ❌ (falha)

## Estrutura dos arquivos:

```
.github/
└── workflows/
    ├── build.yml          # Pipeline principal
    └── README.md          # Esta documentação
```

## Comandos locais equivalentes:

Se quiser executar os mesmos comandos localmente:

```bash
# Restaurar dependências
dotnet restore FastTechFoods.sln

# Build da solução
dotnet build FastTechFoods.sln --configuration Release

# Executar testes
dotnet test FastTechFoods.sln --configuration Release

# Build individual dos serviços
dotnet build AuthService/AuthService.csproj --configuration Release
dotnet build MenuService/MenuService.csproj --configuration Release
dotnet build OrderService/OrderService.csproj --configuration Release
dotnet build KitchenService/KitchenService.csproj --configuration Release
```

## Troubleshooting:

- **Erro de dependências**: Verifique se todos os pacotes NuGet estão corretos
- **Erro de build**: Verifique se o código compila localmente
- **Erro de testes**: Verifique se os testes passam localmente
- **Timeout**: A pipeline pode demorar alguns minutos para executar 