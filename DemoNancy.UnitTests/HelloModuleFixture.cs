using FluentAssertions;
using Nancy;
using Nancy.Testing;
using NUnit.Framework;

namespace DemoNancy.UnitTests
{
    [TestFixture]
    public class HelloModuleFixture
    {
        [Test]
        public void Get_RootPath_ReturnsStatusCodeOk()
        {
            // Arrange
            var expected = HttpStatusCode.OK;
            var browser = new Browser(new ConfigurableBootstrapper(with => with.Module<HelloModule>()));

            // Act
            var actual = browser.Get("/");

            // Assert
            actual.StatusCode.Should().Be(expected);
        }
    }
}