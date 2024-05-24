using Auth.MicroService.Application.JwtUtils;
using Auth.MicroService.Application.Models;
using Auth.MicroService.Application.Services;
using Auth.MicroService.Domain.Entities;
using Auth.MicroService.Domain.Enums;
using Auth.MicroService.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
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
        private Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private Mock<IPasswordHasher<User>> _passwordHasherMock;
        private Mock<IJwtProvider> _jwtProviderMock;
        private Mock<IMemoryCache> _cacheMock;
        private AuthService _authService;

        [TestInitialize]
        public void Initialize()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _cacheMock = new Mock<IMemoryCache>();
            _authService = new AuthService(
                _userRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtProviderMock.Object,
                _cacheMock.Object);
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

            var tokenModel = new TokenModel("token", "refreshToken", 1234567890);
            var authResponseModel = new AuthResponseModel(
                user.UserId.Value,
                user.Email,
                user.FirstName,
                user.LastName,
                user.RoleId,
                tokenModel.Token,
                tokenModel.RefreshToken,
                tokenModel.ExpiresIn);

            _userRepositoryMock.Setup(x => x.GetUserByEmail(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyHashedPassword(user, user.Password, model.Password)).Returns(PasswordVerificationResult.Success);
            _jwtProviderMock.Setup(x => x.GenerateJwt(user, true)).Returns(tokenModel);

            // Act
            var result = await _authService.UserLogin(model, CancellationToken.None);

            // Assert
            Assert.AreEqual(authResponseModel, result);
        }

        [TestMethod]
        public async Task GeneratePasswordResetToken_ShouldReturnToken_WhenEmailIsValid()
        {
            // Arrange
            var email = "test@email.com";

            var user = User.CreateUserForTests(
                "Test", 
                "Test", 
                email, 
                "abcd123456", 
                new DateTime(1990, 1, 1)
            );
            
            var tokenModel = new TokenModel("token", "refreshToken", 1234567890);

            _userRepositoryMock.Setup(x => x.GetUserByEmail(email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _jwtProviderMock.Setup(x => x.GeneratePasswordResetToken(user)).Returns(tokenModel);

            // Act
            var result = await _authService.GeneratePasswordResetToken(email, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(tokenModel, result);
        }

        [TestMethod]
        public async Task ResetPassword_ShouldResetPassword_WhenTokenAndNewPasswordAreValid()
        {
            // Arrange
            var model = new ResetPasswordModel
            {
                Token = "validToken",
                NewPassword = "abcd123456"
            };
            var email = "test@email.com";

            var user = User.CreateUserForTests(
                "Test", 
                "Test", 
                email, 
                "abcd123456",
                new DateTime(1990, 1, 1)
            );

            _jwtProviderMock.Setup(x => x.ValidatePasswordResetToken(model.Token)).Returns(email);
            _userRepositoryMock.Setup(x => x.GetUserByEmail(email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.UpdateUser(It.IsAny<User>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(email));

            // Act
            await _authService.ResetPassword(model, CancellationToken.None);

            // Assert
            _jwtProviderMock.Verify(x => x.ValidatePasswordResetToken(model.Token), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateUser(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
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
            var refreshToken = "validRefreshToken";
            var user = User.CreateUserForTests(
                "Test", 
                "Test", 
                "test@email.com", 
                "abcd123456", 
                new DateTime(1990, 1, 1)
            );
    
            var tokenModel = new TokenModel("token", refreshToken, 1234567890);

            var refreshTokenEntity =
                RefreshToken.CreateRefreshTokenForTests(
                    user.UserId.Value,
                    refreshToken,
                    DateTime.UtcNow.AddDays(30),
                    DateTime.UtcNow,
                    false,
                    null);

            _refreshTokenRepositoryMock.Setup(x => x.GetRefreshToken(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(refreshTokenEntity);
            _userRepositoryMock.Setup(x => x.GetUserById(user.UserId.Value, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _jwtProviderMock.Setup(x => x.GenerateJwt(user, false)).Returns(tokenModel);
            _refreshTokenRepositoryMock.Setup(x => x.UpdateExpiresIn(refreshTokenEntity.RefreshTokenId.Value, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RefreshOneToken(refreshToken, CancellationToken.None);

            // Assert
            Assert.AreEqual(tokenModel.Token, result.Token);
            Assert.AreEqual(tokenModel.ExpiresIn, result.ExpiresIn);
            Assert.AreEqual(refreshTokenEntity.Token, result.RefreshToken);
        }
    }
}