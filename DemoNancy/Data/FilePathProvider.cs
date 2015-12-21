using System.Web;

namespace DemoNancy.Data
{
    public class FilePathProvider : IFilePathProvider
    {
        public string GetPath()
        {
            return HttpContext.Current.Server.MapPath("~/App_Data/todos.xml");
        }
    }
}