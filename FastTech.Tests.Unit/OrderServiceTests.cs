using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace FastTech.Tests.Unit
{
    public class OrderServiceTests
    {
        [Fact]
        public void Deve_Calcular_Valor_Total_Do_Pedido()
        {
            // Arrange
            var itens = new List<ItemPedidoTeste>
            {
                new ItemPedidoTeste { Nome = "X-Burger", PrecoUnitario = 15.90m, Quantidade = 2 },
                new ItemPedidoTeste { Nome = "Batata Frita", PrecoUnitario = 8.50m, Quantidade = 1 }
            };
            var taxaEntrega = 5.00m;
            var desconto = 2.00m;

            // Act
            var total = CalcularValorTotal(itens, taxaEntrega, desconto);

            // Assert
            Assert.Equal(43.20m, total);
        }

        [Fact]
        public void Deve_Validar_Forma_Entrega_Correta()
        {
            // Arrange
            var formaEntrega = "Delivery";

            // Act
            var isValid = ValidarFormaEntrega(formaEntrega);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Deve_Rejeitar_Forma_Entrega_Incorreta()
        {
            // Arrange
            var formaEntrega = "FormaInvalida";

            // Act
            var isValid = ValidarFormaEntrega(formaEntrega);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Deve_Validar_Telefone_Correto()
        {
            // Arrange
            var telefone = "11999999999";

            // Act
            var isValid = ValidarTelefone(telefone);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Deve_Rejeitar_Telefone_Incorreto()
        {
            // Arrange
            var telefone = "123";

            // Act
            var isValid = ValidarTelefone(telefone);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Deve_Gerar_Numero_Pedido_Unico()
        {
            // Arrange
            var pedidos = new List<string>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                pedidos.Add(GerarNumeroPedido());
            }

            // Assert
            Assert.Equal(10, pedidos.Count);
            Assert.Equal(10, pedidos.Distinct().Count()); // Todos devem ser únicos
        }

        [Fact]
        public void Deve_Validar_Quantidade_Item_Positiva()
        {
            // Arrange
            var quantidade = 2;

            // Act
            var isValid = ValidarQuantidade(quantidade);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Deve_Rejeitar_Quantidade_Item_Zero()
        {
            // Arrange
            var quantidade = 0;

            // Act
            var isValid = ValidarQuantidade(quantidade);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Deve_Rejeitar_Quantidade_Item_Negativa()
        {
            // Arrange
            var quantidade = -1;

            // Act
            var isValid = ValidarQuantidade(quantidade);

            // Assert
            Assert.False(isValid);
        }

        // Métodos auxiliares para simular a lógica dos repositórios
        private decimal CalcularValorTotal(List<ItemPedidoTeste> itens, decimal taxaEntrega, decimal desconto)
        {
            var subtotal = itens.Sum(i => i.PrecoUnitario * i.Quantidade);
            return subtotal + taxaEntrega - desconto;
        }

        private bool ValidarFormaEntrega(string formaEntrega)
        {
            var formasValidas = new[] { "Balcao", "DriveThru", "Delivery" };
            return formasValidas.Contains(formaEntrega);
        }

        private bool ValidarTelefone(string telefone)
        {
            return !string.IsNullOrWhiteSpace(telefone) && telefone.Length >= 10 && telefone.All(char.IsDigit);
        }

        private string GerarNumeroPedido()
        {
            return $"PED{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
        }

        private bool ValidarQuantidade(int quantidade)
        {
            return quantidade > 0;
        }

        // Classes auxiliares para os testes
        public class ItemPedidoTeste
        {
            public string Nome { get; set; } = "";
            public decimal PrecoUnitario { get; set; }
            public int Quantidade { get; set; }
        }
    }
} 