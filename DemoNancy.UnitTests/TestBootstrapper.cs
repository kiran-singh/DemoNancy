using System.IO;
using DemoNancy.Data;
using DemoNancy.Model;
using Moq;
using Nancy.TinyIoc;

namespace DemoNancy.UnitTests
{
    public class TestBootstrapper : Bootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var filePathProvider = new Mock<IFilePathProvider>();
            filePathProvider.Setup(x => x.GetPath()).Returns(Path.Combine(Path.GetTempPath(), "todos.xml"));
            container.Register<IDataStore<Todo>>(new TodoDataStore(filePathProvider.Object));
        }
    }
}