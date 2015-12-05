using System.Collections.Generic;
using DemoNancy.Model;
using Nancy;
using Nancy.ModelBinding;

namespace DemoNancy
{
    public class TodosModule : NancyModule
    {
        private readonly Dictionary<long, Todo> _store;

        public TodosModule(Dictionary<long, Todo> store) : base("todos")
        {
            _store = store;

            Delete["/{id}"] = d =>
            {
                if (!_store.ContainsKey(d.id))
                    return HttpStatusCode.NotFound;
                _store.Remove(d.id);
                return HttpStatusCode.OK;
            };

            Get["/"] = _ => Response.AsJson(_store.Values);

            Post["/"] = _ =>
            {
                var todo = this.Bind<Todo>();
                if (todo.Id == 0)
                    todo.Id = _store.Count + 1;

                if (_store.ContainsKey(todo.Id))
                    return HttpStatusCode.NotAcceptable;

                _store.Add(todo.Id, todo);
                return Response.AsJson(todo).WithStatusCode(HttpStatusCode.Created);
            };

            Put["/{id}"] = p =>
            {
                if (!_store.ContainsKey(p.id))
                    return HttpStatusCode.NotFound;

                var updatedTodo = this.Bind<Todo>();
                store[p.id] = updatedTodo;
                return Response.AsJson(updatedTodo);
            };
        }
    }
}