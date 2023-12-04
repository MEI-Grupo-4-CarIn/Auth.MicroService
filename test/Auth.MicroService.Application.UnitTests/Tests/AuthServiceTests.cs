using Auth.MicroService.Application.JwtUtils;
using Auth.MicroService.Application.Models;
using Auth.MicroService.Application.Services;
using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Enums;
using Auth.MicroService.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.MicroService.Application.UnitTests.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPasswordHasher<User>> _passwordHasherMock;
        private Mock<IJwtProvider> _jwtProviderMock;
        private AuthService _authService;

        [TestInitialize]
        public void Initialize()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _authService = new AuthService(_userRepositoryMock.Object, _passwordHasherMock.Object, _jwtProviderMock.Object);
        }

        [TestMethod]
        public async Task UserRegistration_ShouldThrowArgumentNullException_WhenModelIsNull()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _authService.UserRegistration(null, CancellationToken.None));
        }

        [TestMethod]
        public async Task UserRegistration_ShouldRegisterUser_WhenModelIsValid()
        {
            // Arrange
            var model = new RegisterModel
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "test@email.com",
                Password = "abcd123456",
                BirthDate = new DateTime(1999, 1, 20)
            };

            _userRepositoryMock.Setup(x => x.AddNewUser(It.IsAny<User>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            await _authService.UserRegistration(model, CancellationToken.None);

            // Assert
            _userRepositoryMock.Verify(x => x.AddNewUser(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UserRegistration_ShouldThrowArgumentException_WhenEmailIsInvalid()
        {
            // Arrange
            var model = new RegisterModel
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "test.com", // Invalid email
                Password = "abcd123456",
                BirthDate = new DateTime(1999, 1, 20)
            };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _authService.UserRegistration(model, CancellationToken.None));
        }

        [TestMethod]
        public async Task UserLogin_ShouldThrowArgumentNullException_WhenModelIsNull()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _authService.UserLogin(null, CancellationToken.None));
        }

        [TestMethod]
        public async Task UserLogin_ShouldLoginUser_WhenModelIsValid()
        {
            // Arrange
            var model = new LoginModel
            {
                Email = "test@email.com",
                Password = "abcd123456"
            };

            var user = User.CreateUserForTests(
                "Test", 
                "Test", 
                "test@email.com", 
                "abcd123456", 
                new DateTime(1990, 1, 1),
                Role.Driver,
                true
            );

            _userRepositoryMock.Setup(x => x.GetUserByEmail(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyHashedPassword(user, user.Password, model.Password)).Returns(PasswordVerificationResult.Success);
            _jwtProviderMock.Setup(x => x.GenerateJwt(user)).Returns("token");

            // Act
            var result = await _authService.UserLogin(model, CancellationToken.None);

            // Assert
            Assert.AreEqual("token", result);
        }

        [TestMethod]
        public void ValidateToken_ShouldReturnTrue_WhenTokenIsValid()
        {
            // Arrange
            var token = "validToken";
            _jwtProviderMock.Setup(x => x.ValidateToken(token)).Returns(true);

            // Act
            var result = _authService.ValidateToken(token, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task RefreshToken_ShouldReturnNewToken_WhenTokenIsValid()
        {
            // Arrange
            var token = "validToken";
            
            var user = User.CreateUserForTests(
                "Test", 
                "Test", 
                "test@email.com", 
                "abcd123456", 
                new DateTime(1990, 1, 1)
            );

            _jwtProviderMock.Setup(x => x.ValidateToken(token)).Returns(true);
            _jwtProviderMock.Setup(x => x.GetUserIdFromToken(token)).Returns(user.UserId);
            _userRepositoryMock.Setup(x => x.GetUserById(user.UserId.Value, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _jwtProviderMock.Setup(x => x.GenerateJwt(user)).Returns("newToken");

            // Act
            var result = await _authService.RefreshToken(token, CancellationToken.None);

            // Assert
            Assert.AreEqual("newToken", result);
        }
    }
}