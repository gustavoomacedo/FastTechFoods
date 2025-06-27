using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace FastTech.Tests.Unit
{
    public class MenuServiceTests
    {
        [Fact]
        public void Deve_Calcular_Preco_Total_Corretamente()
        {
            // Arrange
            var itens = new List<ItemTeste>
            {
                new ItemTeste { Nome = "X-Burger", Preco = 15.90m, Quantidade = 2 },
                new ItemTeste { Nome = "Batata Frita", Preco = 8.50m, Quantidade = 1 },
                new ItemTeste { Nome = "Refrigerante", Preco = 5.00m, Quantidade = 2 }
            };

            // Act
            var total = CalcularTotal(itens);

            // Assert
            Assert.Equal(50.30m, total);
        }

        [Fact]
        public void Deve_Filtrar_Produtos_Por_Categoria()
        {
            // Arrange
            var produtos = new List<ProdutoTeste>
            {
                new ProdutoTeste { Nome = "X-Burger", Categoria = "Pratos", Disponivel = true },
                new ProdutoTeste { Nome = "Coca-Cola", Categoria = "Bebidas", Disponivel = true },
                new ProdutoTeste { Nome = "Batata Frita", Categoria = "Acompanhamentos", Disponivel = true },
                new ProdutoTeste { Nome = "Sorvete", Categoria = "Sobremesas", Disponivel = false }
            };

            // Act
            var pratos = FiltrarPorCategoria(produtos, "Pratos");

            // Assert
            Assert.Single(pratos);
            Assert.Equal("X-Burger", pratos.First().Nome);
        }

        [Fact]
        public void Deve_Filtrar_Produtos_Disponiveis()
        {
            // Arrange
            var produtos = new List<ProdutoTeste>
            {
                new ProdutoTeste { Nome = "X-Burger", Categoria = "Pratos", Disponivel = true },
                new ProdutoTeste { Nome = "Coca-Cola", Categoria = "Bebidas", Disponivel = true },
                new ProdutoTeste { Nome = "Batata Frita", Categoria = "Acompanhamentos", Disponivel = false },
                new ProdutoTeste { Nome = "Sorvete", Categoria = "Sobremesas", Disponivel = false }
            };

            // Act
            var disponiveis = FiltrarDisponiveis(produtos);

            // Assert
            Assert.Equal(2, disponiveis.Count);
            Assert.All(disponiveis, p => Assert.True(p.Disponivel));
        }

        [Fact]
        public void Deve_Validar_Preco_Positivo()
        {
            // Arrange
            var preco = 15.90m;

            // Act
            var isValid = ValidarPreco(preco);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Deve_Rejeitar_Preco_Negativo()
        {
            // Arrange
            var preco = -5.00m;

            // Act
            var isValid = ValidarPreco(preco);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Deve_Rejeitar_Preco_Zero()
        {
            // Arrange
            var preco = 0m;

            // Act
            var isValid = ValidarPreco(preco);

            // Assert
            Assert.False(isValid);
        }

        // Métodos auxiliares para simular a lógica dos repositórios
        private decimal CalcularTotal(List<ItemTeste> itens)
        {
            return itens.Sum(i => i.Preco * i.Quantidade);
        }

        private List<ProdutoTeste> FiltrarPorCategoria(List<ProdutoTeste> produtos, string categoria)
        {
            return produtos.Where(p => p.Categoria == categoria).ToList();
        }

        private List<ProdutoTeste> FiltrarDisponiveis(List<ProdutoTeste> produtos)
        {
            return produtos.Where(p => p.Disponivel).ToList();
        }

        private bool ValidarPreco(decimal preco)
        {
            return preco > 0;
        }

        // Classes auxiliares para os testes
        public class ItemTeste
        {
            public string Nome { get; set; } = "";
            public decimal Preco { get; set; }
            public int Quantidade { get; set; }
        }

        public class ProdutoTeste
        {
            public string Nome { get; set; } = "";
            public string Categoria { get; set; } = "";
            public bool Disponivel { get; set; }
        }
    }
} 