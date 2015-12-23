using DemoNancy.Data;
using DemoNancy.Model;
using Nancy;
using Nancy.Authentication.WorldDomination;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using Nancy.ViewEngines.Razor;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace DemoNancy
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        public const string LoggerName = "RequestLogger";

        private readonly Logger _logger = LogManager.GetLogger(LoggerName);

        private RazorViewEngine _ensureRazorIsLoaded;

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            SimpleConfigurator.ConfigureForTargetLogging(new AsyncTargetWrapper(new EventLogTarget()));

            pipelines
                .SetCurrentUserWhenLoggedIn()
                .LogAllRequests(_logger)
                .LogAllResponseCodes(_logger)
                .LogUnhandledExceptions(_logger);
        }


        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IFilePathProvider, FilePathProvider>();
            container.Register<IDataStore<Todo>, TodoDataStore>();
            container.Register<IAuthenticationCallbackProvider, SocialAuthenticationCallbackProvider>();
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/docs", "Docs"));
        }
    }
}