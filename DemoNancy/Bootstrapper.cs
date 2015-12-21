﻿using DemoNancy.Data;
using DemoNancy.Model;
using Nancy;
using Nancy.Conventions;
using Nancy.TinyIoc;
using Nancy.ViewEngines.Razor;

namespace DemoNancy
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private RazorViewEngine _ensureRazorIsLoaded;

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IDataStore<Todo>>(new TodoDataStore(new FilePathProvider()));
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/docs", "Docs"));
        }
    }
}