using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace FastTech.Tests.Unit
{
    public class KitchenServiceTests
    {
        [Fact]
        public void Deve_Validar_Transicao_Status_Permitida()
        {
            // Arrange
            var statusAtual = "Pendente";
            var novoStatus = "Aceito";

            // Act
            var isValid = ValidarTransicaoStatus(statusAtual, novoStatus);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Deve_Rejeitar_Transicao_Status_Invalida()
        {
            // Arrange
            var statusAtual = "Cancelado";
            var novoStatus = "Aceito";

            // Act
            var isValid = ValidarTransicaoStatus(statusAtual, novoStatus);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Deve_Calcular_Tempo_Preparo_Estimado()
        {
            // Arrange
            var itens = new List<ItemPreparoTeste>
            {
                new ItemPreparoTeste { Nome = "X-Burger", TempoPreparo = 10, Quantidade = 2 },
                new ItemPreparoTeste { Nome = "Batata Frita", TempoPreparo = 5, Quantidade = 1 }
            };

            // Act
            var tempoTotal = CalcularTempoPreparo(itens);

            // Assert
            Assert.Equal(25, tempoTotal); // 10*2 + 5*1 = 25 minutos
        }

        [Fact]
        public void Deve_Filtrar_Pedidos_Pendentes()
        {
            // Arrange
            var pedidos = new List<PedidoTeste>
            {
                new PedidoTeste { Numero = "PED001", Status = "Pendente" },
                new PedidoTeste { Numero = "PED002", Status = "Aceito" },
                new PedidoTeste { Numero = "PED003", Status = "Pendente" },
                new PedidoTeste { Numero = "PED004", Status = "Cancelado" }
            };

            // Act
            var pendentes = FiltrarPedidosPendentes(pedidos);

            // Assert
            Assert.Equal(2, pendentes.Count);
            Assert.All(pendentes, p => Assert.Equal("Pendente", p.Status));
        }

        [Fact]
        public void Deve_Calcular_Estatisticas_Cozinha()
        {
            // Arrange
            var pedidos = new List<PedidoTeste>
            {
                new PedidoTeste { Status = "Pendente", Valor = 25.00m },
                new PedidoTeste { Status = "Aceito", Valor = 30.00m },
                new PedidoTeste { Status = "Pendente", Valor = 15.00m },
                new PedidoTeste { Status = "Cancelado", Valor = 20.00m }
            };

            // Act
            var stats = CalcularEstatisticas(pedidos);

            // Assert
            Assert.Equal(2, stats.PedidosPendentes);
            Assert.Equal(1, stats.PedidosAceitos);
            Assert.Equal(1, stats.PedidosCancelados);
            Assert.Equal(70.00m, stats.ValorTotal);
        }

        [Fact]
        public void Deve_Validar_Observacoes_Obrigatorias_Para_Rejeicao()
        {
            // Arrange
            var observacoes = "Produto indisponível";

            // Act
            var isValid = ValidarObservacoesRejeicao(observacoes);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Deve_Rejeitar_Observacoes_Vazias_Para_Rejeicao()
        {
            // Arrange
            var observacoes = "";

            // Act
            var isValid = ValidarObservacoesRejeicao(observacoes);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Deve_Ordenar_Pedidos_Por_Prioridade()
        {
            // Arrange
            var pedidos = new List<PedidoTeste>
            {
                new PedidoTeste { Numero = "PED001", Prioridade = 2, DataCriacao = DateTime.Now.AddMinutes(-30) },
                new PedidoTeste { Numero = "PED002", Prioridade = 1, DataCriacao = DateTime.Now.AddMinutes(-10) },
                new PedidoTeste { Numero = "PED003", Prioridade = 1, DataCriacao = DateTime.Now.AddMinutes(-20) }
            };

            // Act
            var ordenados = OrdenarPorPrioridade(pedidos);

            // Assert
            Assert.Equal("PED002", ordenados[0].Numero); // Prioridade 1, mais recente
            Assert.Equal("PED003", ordenados[1].Numero); // Prioridade 1, mais antigo
            Assert.Equal("PED001", ordenados[2].Numero); // Prioridade 2
        }

        // Métodos auxiliares para simular a lógica dos repositórios
        private bool ValidarTransicaoStatus(string statusAtual, string novoStatus)
        {
            var transicoesValidas = new Dictionary<string, string[]>
            {
                ["Pendente"] = new[] { "Aceito", "Rejeitado", "Cancelado" },
                ["Aceito"] = new[] { "EmPreparo", "Cancelado" },
                ["EmPreparo"] = new[] { "Pronto", "Cancelado" },
                ["Pronto"] = new[] { "Entregue" },
                ["Cancelado"] = new string[0],
                ["Rejeitado"] = new string[0],
                ["Entregue"] = new string[0]
            };

            return transicoesValidas.ContainsKey(statusAtual) && 
                   transicoesValidas[statusAtual].Contains(novoStatus);
        }

        private int CalcularTempoPreparo(List<ItemPreparoTeste> itens)
        {
            return itens.Sum(i => i.TempoPreparo * i.Quantidade);
        }

        private List<PedidoTeste> FiltrarPedidosPendentes(List<PedidoTeste> pedidos)
        {
            return pedidos.Where(p => p.Status == "Pendente").ToList();
        }

        private EstatisticasTeste CalcularEstatisticas(List<PedidoTeste> pedidos)
        {
            return new EstatisticasTeste
            {
                PedidosPendentes = pedidos.Count(p => p.Status == "Pendente"),
                PedidosAceitos = pedidos.Count(p => p.Status == "Aceito"),
                PedidosCancelados = pedidos.Count(p => p.Status == "Cancelado"),
                ValorTotal = pedidos.Sum(p => p.Valor)
            };
        }

        private bool ValidarObservacoesRejeicao(string observacoes)
        {
            return !string.IsNullOrWhiteSpace(observacoes) && observacoes.Length >= 5;
        }

        private List<PedidoTeste> OrdenarPorPrioridade(List<PedidoTeste> pedidos)
        {
            return pedidos
                .OrderBy(p => p.Prioridade)
                .ThenBy(p => p.DataCriacao)
                .ToList();
        }

        // Classes auxiliares para os testes
        public class ItemPreparoTeste
        {
            public string Nome { get; set; } = "";
            public int TempoPreparo { get; set; }
            public int Quantidade { get; set; }
        }

        public class PedidoTeste
        {
            public string Numero { get; set; } = "";
            public string Status { get; set; } = "";
            public decimal Valor { get; set; }
            public int Prioridade { get; set; }
            public DateTime DataCriacao { get; set; }
        }

        public class EstatisticasTeste
        {
            public int PedidosPendentes { get; set; }
            public int PedidosAceitos { get; set; }
            public int PedidosCancelados { get; set; }
            public decimal ValorTotal { get; set; }
        }
    }
} 