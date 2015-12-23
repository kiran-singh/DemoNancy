using System.Linq;
using FluentAssertions;
using Nancy;
using Nancy.Testing;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;

namespace DemoNancy.UnitTests
{
    [TestFixture]
    public class RequestLoggingFixture
    {
        private const string InfoRequestlogger =
            "|INFO|" + Bootstrapper.LoggerName;

        private const string ErrorRequestlogger =
            "|ERROR|" + Bootstrapper.LoggerName;

        private Browser _browser;
        private MemoryTarget _actualLog;

        [SetUp]
        public void SetUp()
        {
            _actualLog = new MemoryTarget();
            _actualLog.Layout += "|${exception}";

            _browser = new Browser(new TestBootstrapper());
            SimpleConfigurator.ConfigureForTargetLogging(_actualLog, LogLevel.Info);
        }

        [TestCase("/")]
        [TestCase("/todos/")]
        public void IncomingRequest_StandardCalls_AllLogged_NoErrorLogged(string path)
        {
            // Arrange
            var expected = string.Format(PipelinesExtensions.FormatLogRequest, "GET", path);

            _browser.Get(path, with => with.Accept("application/json"));

            var logs = _actualLog.Logs;

            // Act
            var actual =
                logs.Where(x => x.Contains(InfoRequestlogger)).FirstOrDefault(y => y.Contains(expected));

            // Assert
            actual.Should().NotBeNull($"Expected {string.Join(", ", logs)} to contain {expected}");

            logs.Where(x => x.Contains(ErrorRequestlogger)).Should().BeEmpty();
        }

        public void IncomingRequest_PathIncorrect_RequestAndErrorLogged()
        {
            // Arrange
            var path = "/shouldNotBeFound/";
            var expected = string.Format(PipelinesExtensions.FormatLogRequest, "GET", path);
            _browser.Get(path);
            var logs = _actualLog.Logs;

            // Act
            var actual =
                logs.Where(x => x.Contains(InfoRequestlogger)).FirstOrDefault(y => y.Contains(expected));

            // Assert
            actual.Should().NotBeNull($"Expected {string.Join(", ", logs)} to contain {expected}");

            logs.Count(x => x.Contains(ErrorRequestlogger)).Should().Be(1);
        }

        [TestCase("/", HttpStatusCode.OK)]
        [TestCase("/todos/", HttpStatusCode.OK)]
        [TestCase("/shouldNotBeFound/", HttpStatusCode.NotFound)]
        public void IncomingRequest_StandardCalls_StatusCodeLogged(string path, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var expected = string.Format(PipelinesExtensions.FormatLogResponseCode, expectedStatusCode, "GET", path);
            _browser.Get(path, with =>
                    with.Accept("application/json"));
            var logs = _actualLog.Logs;

            // Act
            var actual =
                logs.Where(x => x.Contains(InfoRequestlogger)).FirstOrDefault(y => y.Contains(expected));

            // Assert
            actual.Should().NotBeNull($"Expected {string.Join(", ", logs)} to contain {expected}");
        }

        [Test]
        public void IncomingRequest_RequestFails_LogsError()
        {
            // Arrange
            const string path = "/todos/illegal_item_id";
            var expected = string.Format(PipelinesExtensions.FormatLogFailedRequest, "DELETE", path);
            const string expectedError = "Input string was not in a correct format";

            // Act
            _browser.Delete(path);

            // Assert
            var logs = _actualLog.Logs;
            var actual =
                logs.Where(x => x.Contains(ErrorRequestlogger)).FirstOrDefault(y => y.Contains(expected));
            actual.Should().NotBeNull($"Expected {string.Join(", ", logs)} to contain {expected}");
            actual.Should().Contain(expectedError);
        }
    }
}