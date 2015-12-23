using DemoNancy.Data;
using DemoNancy.Model;
using Moq;
using Nancy;
using Nancy.Testing;
using Nancy.TinyIoc;

namespace DemoNancy.UnitTests
{
    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class DocumentationFixture
    {
        [Test, Ignore("Todo: fix")]
        public void Get_Overview_ReturnsStaticFile()
        {
            // Arrange
            var expected = HttpStatusCode.OK;
            //var configurableBootstrapper = new ConfigurableBootstrapper(with =>
            //{
            //    with.Dependency(new Mock<IDataStore<Todo>>().Object);
            //    with.RootPathProvider(new TestRootPathProvider("Docs"));
            //});

            var bootstrapper = new Bootstrapper();
            
            var browser = new Browser(bootstrapper);

            // Act
            var actual = browser.Get("/docs/overview.htm", with => with.Accept("text/html"));

            // Assert
            actual.StatusCode.Should().Be(expected);
        }
    }
}