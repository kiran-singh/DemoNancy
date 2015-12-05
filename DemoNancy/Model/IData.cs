using System.Xml.Linq;

namespace DemoNancy.Model
{
    public interface IData
    {
        int Id { get; set; }
        XElement AsXElement();
        Todo FromXElement(XElement xElement);
    }
}