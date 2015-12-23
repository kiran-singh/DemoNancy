using System.Linq;
using Nancy;
using Nancy.Authentication.WorldDomination;
using Nancy.Testing;
using WorldDomination.Web.Authentication;

namespace DemoNancy.UnitTests
{
    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class SocialAuthenticationCallbackProviderFixture
    {
        private AuthenticateCallbackData _callBackData;
        private SocialAuthenticationCallbackProvider _socialAuthenticationCallbackProvider;

        [SetUp]
        public void SetUp()
        {
            _callBackData = new AuthenticateCallbackData
            {
                AuthenticatedClient = new AuthenticatedClient("")
                {
                    UserInformation = new UserInformation
                    {
                        UserName = TestUserName
                    }
                }
            };

            new Browser(new TestBootstrapper()).Get("/testing");
            _socialAuthenticationCallbackProvider = new SocialAuthenticationCallbackProvider();
        }

        [Test]
        public void BrowserGet_CookieSet_SetsUserIdentity()
        {
            // Arrange
            var userNameToken = new TokenService().GetToken(TestUserName);

            var brower = new Browser(new TestBootstrapper());

            // Act
            brower.Get(TestingModule.ModulePath, with => with.Cookie(PipelinesExtensions.KeyTodoUser, userNameToken));

            // Assert
            TestingModule.ActualUser.UserName.Should().Be(TestUserName);
        }

        [Test]
        public void Process_StandardCall_RedirectsToRootOnCallback()
        {
            // Arrange
            // Act
            var actual = (Response)_socialAuthenticationCallbackProvider.Process(TestingModule.ActualModule, _callBackData);

            // Assert
            actual.StatusCode.Should().Be(HttpStatusCode.SeeOther);
        }

        [Test]
        public void Process_SocialAuthCallback_SetsUserCookie()
        {
            // Arrange
            var expected = new TokenService().GetToken("chr_horsdal");

            // Act
            var actual = (Response)_socialAuthenticationCallbackProvider.Process(TestingModule.ActualModule, _callBackData);

            // Assert
            actual.Cookies.First(x => x.Name == "todoUser").Value.Should().Be(expected);
        }

        public const string TestUserName = "chr_horsdal";
    }
}