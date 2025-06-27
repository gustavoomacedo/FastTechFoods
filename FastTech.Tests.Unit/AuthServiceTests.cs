using Xunit;
using System.Security.Cryptography;
using System.Text;

namespace FastTech.Tests.Unit
{
    public class AuthServiceTests
    {
        [Fact]
        public void Deve_Hashear_Senha_Corretamente()
        {
            // Arrange
            var senha = "123456";
            
            // Act
            var hash = HashSenha(senha);
            
            // Assert
            Assert.NotNull(hash);
            Assert.NotEqual(senha, hash);
            Assert.Equal(hash, HashSenha(senha)); // Mesma senha deve gerar mesmo hash
        }

        [Fact]
        public void Deve_Validar_Email_Correto()
        {
            // Arrange
            var email = "teste@fasttech.com";
            
            // Act
            var isValid = IsValidEmail(email);
            
            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Deve_Rejeitar_Email_Incorreto()
        {
            // Arrange
            var email = "email_invalido";
            
            // Act
            var isValid = IsValidEmail(email);
            
            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Deve_Validar_CPF_Correto()
        {
            // Arrange
            var cpf = "12345678901";
            
            // Act
            var isValid = IsValidCPF(cpf);
            
            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Deve_Rejeitar_CPF_Incorreto()
        {
            // Arrange
            var cpf = "123";
            
            // Act
            var isValid = IsValidCPF(cpf);
            
            // Assert
            Assert.False(isValid);
        }

        // Métodos auxiliares para simular a lógica dos repositórios
        private string HashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidCPF(string cpf)
        {
            return !string.IsNullOrWhiteSpace(cpf) && cpf.Length == 11 && cpf.All(char.IsDigit);
        }
    }
} 