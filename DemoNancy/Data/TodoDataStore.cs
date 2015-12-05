using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DemoNancy.Model;

namespace DemoNancy.Data
{
    public class TodoDataStore : IDataStore<Todo>
    {
        private readonly string _filePath;
        //private static readonly string TodoFilePath = HttpContext.Current.Server.MapPath("~/App_Data/Todo.xml");
        private readonly XDocument _todosXmlDoc;

        public TodoDataStore(string filePath)
        {
            _filePath = filePath;
            if (!File.Exists(_filePath))
            {
                _todosXmlDoc = new XDocument(new XElement("todos"));
                Save();
            }
            else
            {
                _todosXmlDoc = XDocument.Load(_filePath);
            }
        }

        public virtual bool Add(Todo todo)
        {
            try
            {
                _todosXmlDoc.Root.Add(todo.AsXElement());
                Save();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }

            return true;
        }

        public void Clear()
        {
            _todosXmlDoc.Root.Elements(Todo.KeyTodo).Remove();
            Save();
        }

        public int Count => _todosXmlDoc.Root?.Elements().Count() ?? 0;

        public Todo Get(int id)
        {
            return _todosXmlDoc.Root.Elements(Todo.KeyTodo)
                .Where(x => x.Element(Todo.KeyId).Value == id.ToString())
                .Select(y => new Todo().FromXElement(y)).SingleOrDefault();
        }

        public IEnumerable<Todo> GetAll()
        {
            return from xElement in _todosXmlDoc.Root.Elements(Todo.KeyTodo)
                select new Todo().FromXElement(xElement);
        }

        public virtual bool Remove(int id)
        {
            try
            {
                _todosXmlDoc.Root.Elements(Todo.KeyTodo)
                        .First(x => x.Element(Todo.KeyId).Value == id.ToString())
                        .Remove();
                Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool Update(Todo todo)
        {
            if (!Remove(todo.Id) || !Add(todo)) return false;

            Save();
            return true;
        }

        public virtual void Save()
        {
            _todosXmlDoc.Save(_filePath);
        }
    }
}