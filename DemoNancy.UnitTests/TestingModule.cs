using Nancy;
using Nancy.Security;

namespace DemoNancy.UnitTests
{
    public class TestingModule : NancyModule
    {
        public new const string ModulePath = "/testing";
        public static IUserIdentity ActualUser;
        public static TestingModule ActualModule;

        public TestingModule()
        {
            Get[ModulePath] = _ =>
            {
                ActualUser = Context.CurrentUser;
                ActualModule = this;
                return HttpStatusCode.OK;
            };
        }
    }
}