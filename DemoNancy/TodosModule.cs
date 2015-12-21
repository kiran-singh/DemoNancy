using System.Collections.Generic;
using System.Linq;
using DemoNancy.Data;
using DemoNancy.Model;
using Nancy;
using Nancy.ModelBinding;

namespace DemoNancy
{
    public class TodosModule : NancyModule
    {
        private readonly IDataStore<Todo> _store;

        public TodosModule(IDataStore<Todo> store) : base("todos")
        {
            _store = store;

            Delete["/{id}"] = d => _store.Remove(d.id) ? HttpStatusCode.OK : HttpStatusCode.NotFound;

            Get["/"] = _ => Negotiate
                .WithModel(_store.GetAll().ToArray())
                .WithView("Todos");

            Post["/"] = _ =>
            {
                var todo = this.Bind<Todo>();
                if (todo.Id == 0)
                    todo.Id = _store.Count + 1;

                if (_store.Add(todo))
                    return Negotiate.WithModel(todo)
                        .WithStatusCode(HttpStatusCode.Created)
                        .WithView("Created");

                return HttpStatusCode.NotAcceptable;
            };

            Put["/{id}"] = p =>
            {
                var updatedTodo = this.Bind<Todo>();
                return _store.Update(updatedTodo) ? Response.AsJson(updatedTodo) : HttpStatusCode.NotFound;
            };
        }
    }
}