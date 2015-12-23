using Nancy;
using Nancy.Authentication.WorldDomination;
using Nancy.Cookies;

namespace DemoNancy
{
    public class SocialAuthenticationCallbackProvider : IAuthenticationCallbackProvider
    {
        public dynamic Process(NancyModule nancyModule, AuthenticateCallbackData callbackData)
        {
            nancyModule.Context.CurrentUser = new User
            {
                UserName = callbackData.AuthenticatedClient.UserInformation.UserName,
            };

            return nancyModule.Response.AsRedirect("/")
                .WithCookie(new NancyCookie("todoUser",
                    new TokenService().GetToken(nancyModule.Context.CurrentUser.UserName)));
        }

        public dynamic OnRedirectToAuthenticationProviderError(NancyModule nancyModule, string errorMessage)
        {
            return "login failed: " + errorMessage;
        }
    }
}