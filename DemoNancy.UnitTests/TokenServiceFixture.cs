using System;
using FluentAssertions;
using NUnit.Framework;

namespace DemoNancy.UnitTests
{
    [TestFixture]
    public class TokenServiceFixture
    {
        private Random _random;
        private TokenService _tokenService;

        [SetUp]
        public void SetUp()
        {
            _random = new Random();
            _tokenService = new TokenService();
        }

        [Test]
        public void GetToken_UserNameProvided_ReturnsUserName()
        {
            // Arrange
            var expected = _random.Next().ToString();

            // Act
            var actual = _tokenService.GetToken(expected);

            // Assert
            actual.Should().Be(expected);
        }

        [Test]
        public void GetUserFromToken_TokenProvided_ReturnsUserWithTokenAsUserName()
        {
            // Arrange
            var expected = _random.Next().ToString();

            // Act
            var actual = _tokenService.GetUserFromToken(expected);

            // Assert
            actual.Should().BeOfType<User>();
            actual.UserName.Should().Be(expected);
        }
    }
}