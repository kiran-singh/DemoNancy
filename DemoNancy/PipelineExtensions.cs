using Nancy.Bootstrapper;
using NLog;

namespace DemoNancy
{
    public static class PipelinesExtensions
    {
        public const string KeyTodoUser = "todoUser";

        public const string FormatLogRequest = "Handling request {0} '{1}'";

        public const string FormatLogResponseCode = "Responding {0} to {1} '{2}'";
        public const string FormatLogFailedRequest = "Request {0} '{1}' failed";

        public static IPipelines LogAllRequests(this IPipelines pipelines, Logger logger)
        {
            pipelines.BeforeRequest += ctx =>
            {
                logger.Info(FormatLogRequest, ctx.Request.Method, ctx.Request.Path);
                return null;
            };

            return pipelines;
        }

        public static IPipelines LogAllResponseCodes(this IPipelines pipelines, Logger logger)
        {
            pipelines.AfterRequest +=
                ctx =>
                {
                    var infoToLog = string.Format(FormatLogResponseCode, ctx.Response.StatusCode, ctx.Request.Method,
                        ctx.Request.Path);
                    logger.Info(infoToLog);
                };

            return pipelines;
        }

        public static IPipelines LogUnhandledExceptions(this IPipelines pipelines, Logger logger)
        {
            pipelines.OnError.AddItemToStartOfPipeline((ctx, error) =>
            {
                logger.Error(error, FormatLogFailedRequest, ctx.Request.Method, ctx.Request.Path);
                return null;
            });

            return pipelines;
        }

        public static IPipelines SetCurrentUserWhenLoggedIn(this IPipelines pipelines)
        {
            pipelines.BeforeRequest += ctx =>
            {
                ctx.CurrentUser = ctx.Request.Cookies.ContainsKey(KeyTodoUser)
                    ? new TokenService().GetUserFromToken(ctx.Request.Cookies[KeyTodoUser])
                    : User.Anonymous;
                return null;
            };

            return pipelines;
        }
    }
}