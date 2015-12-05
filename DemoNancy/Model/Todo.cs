using System;
using System.Linq;
using System.Xml.Linq;

namespace DemoNancy.Model
{
    public class Todo : IData
    {
        public const string KeyId = "id";
        public const string KeyTodo = "todo";

        public int Id { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public bool Completed { get; set; }

        protected bool Equals(Todo other)
        {
            return String.Equals(Title, other.Title) && Order == other.Order && Completed == other.Completed;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Todo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Title.GetHashCode();
                hashCode = (hashCode*397) ^ Order;
                hashCode = (hashCode*397) ^ Completed.GetHashCode();
                return hashCode;
            }
        }

        public XElement AsXElement()
        {
            var enumerable =
                GetType()
                    .GetProperties()
                    .Select(propertyInfo => new XElement(propertyInfo.Name.ToLower(), propertyInfo.GetValue(this)));
            return new XElement(GetType().Name.ToLower(), enumerable);
        }

        public Todo FromXElement(XElement xElement)
        {
            Id = xElement.Element(KeyId).Value.ToInt();
            Completed = xElement.Element("completed").Value.ToBool();
            Order = xElement.Element("order").Value.ToInt();    
            Title = xElement.Element("title").Value;
            return this;
        }
    }
}