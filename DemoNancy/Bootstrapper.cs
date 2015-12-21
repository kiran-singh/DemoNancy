using System.Web;
using DemoNancy.Data;
using DemoNancy.Model;
using Nancy;
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

            container.Register<IDataStore<Todo>>(
                new TodoDataStore(HttpContext.Current.Server.MapPath("~/App_Data/todos.xml")));
        }
    }
}