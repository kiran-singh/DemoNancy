using System;
using System.IO;
using System.Linq;
using DemoNancy.Data;
using DemoNancy.Model;
using Nancy;
using Nancy.Testing;

namespace DemoNancy.IntegrationTests
{
    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class TodosModuleFixture
    {
        private const string PathTodos = "/todos/";
        public const string KeyApplicationXml = "application/xml";
        public const string KeyApplicationJson = "application/json";

        private Browser _browser;

        private Todo _aTodo;

        private IDataStore<Todo> _store;

        private Todo _editedTodo;
        private Random _random;

        [SetUp]
        public void SetUp()
        {
            _random = new Random();
            _store = new TodoDataStore(Path.Combine(Path.GetTempPath(), "todos.xml"));

            _aTodo = new Todo { Completed = false, Order = 0, Title = "Task 1" };
            _editedTodo = new Todo { Completed = false, Id = 42, Order = 0, Title = "Edited name" };

            _browser = new Browser(new ConfigurableBootstrapper(with =>
            {
                with.Dependency(_store);
                with.Module<TodosModule>();
            }));

            _store.Clear();
        }

        [Test]
        public void Delete_IdOfExistingTodo_TodoDeleted()
        {
            // Arrange
            var id = _random.Next();
            _store.Add(new Todo
            {
                Completed = _random.Next() % 2 == 0,
                Id = id,
                Order = _random.Next(4, 10),
                Title = _random.Next().ToString()
            });

            // Act
            var actual = _browser.Delete(PathTodos + id);

            // Assert
            actual.StatusCode.Should().Be(Nancy.HttpStatusCode.OK);
        }

        [Test]
        public void Delete_IdDoesnotExistInStore_ReturnsNotFound()
        {
            // Arrange
            var id = _random.Next();

            // Act
            var actual = _browser.Delete(PathTodos + id);

            // Assert
            actual.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public void Get_NoTodosPosted_ReturnsEmptyList()
        {
            // Arrange
            // Act
            var actual = _browser.Get(PathTodos, with => with.Accept(KeyApplicationJson));

            // Assert
            actual.StatusCode.Should().Be(HttpStatusCode.OK);
            actual.Body.DeserializeJson<Todo[]>().Should().BeEmpty();
        }

        [Test]
        public void Post_TodoPosted_Returns201()
        {
            // Arrange


            // Act
            var actual = _browser.Post(PathTodos, with =>
            {
                with.JsonBody(_aTodo);
                with.Accept(KeyApplicationXml);
            });

            // Assert
            actual.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public void Post_TodoWithDuplicateId_ReturnsNotAcceptableStatusCode()
        {
            // Arrange
            var editedTodo = new Todo { Completed = false, Id = 42, Order = 0, Title = "edited title" };

            // Act
            var actual = _browser.Post(PathTodos, with =>
            {
                with.JsonBody(editedTodo);
                with.Accept(KeyApplicationXml);
            })
                .Then.Post(PathTodos, with => with.JsonBody(editedTodo));

            // Assert
            actual.StatusCode.Should().Be(HttpStatusCode.NotAcceptable);
        }

        [Test]
        public void Post_TodoAdded_PostedTodoCanBeRetrieved()
        {
            // Arrange
            // Act
            var actual =
                _browser.Post(PathTodos, with =>
                {
                    with.JsonBody(_aTodo);
                    with.Accept(KeyApplicationXml);
                })
                    .Then.Get(PathTodos, with => with.Accept(KeyApplicationJson))
                    .Body.DeserializeJson<Todo[]>();

            // Assert
            actual.Count().Should().Be(1);
            actual[0].Should().Be(_aTodo);
        }

        [Test]
        public void PostGet_TodoInput_TodoReturned()
        {
            // Arrange
            // Act
            var actual = _browser.Post(PathTodos, with =>
            {
                with.JsonBody(_aTodo);
                with.Accept(KeyApplicationXml);
            })
                .Then.Get(PathTodos, with => with.Accept(KeyApplicationJson));
            var actualBody = actual.Body.DeserializeJson<Todo[]>();

            // Assert
            actualBody.Length.Should().Be(1);
            actualBody[0].Should().Be(_aTodo);
        }

        [Test]
        public void PostGet_XmlTodo_TodoReturnedAsJson()
        {
            // Arrange
            // Act
            var actual = _browser.Post(PathTodos, with =>
            {
                with.XMLBody(_aTodo);
                with.Accept(KeyApplicationXml);
            }).Then.Get(PathTodos, with => with.Accept(KeyApplicationJson));
            var actualBody = actual.Body.DeserializeJson<Todo[]>();

            // Assert
            actualBody.Length.Should().Be(1);
            actualBody[0].Should().Be(_aTodo);
        }

        [Test]
        public void PostGet_XmlTodo_TodoReturnedAsXml()
        {
            // Arrange
            // Act
            var actual = _browser.Post(PathTodos, with =>
            {
                with.XMLBody(_aTodo);
                with.Accept(KeyApplicationXml);
            });
            var actualBody = actual.Body.DeserializeXml<Todo>();

            // Assert
            actualBody.Should().Be(_aTodo);
        }

        [Test]
        public void Put_EditedTodo_OriginalTodoChanged()
        {
            // Arrange // Act
            var actual = _browser.Post(PathTodos, with =>
            {
                with.JsonBody(_aTodo);
                with.Accept(KeyApplicationJson);
            })
                .Then.Put(PathTodos + "1", with => with.JsonBody(_editedTodo));
            actual.Body.DeserializeJson<Todo>().Should().Be(_editedTodo);

            // Assert
            var allTodos = _browser.Get(PathTodos, with => with.Accept(KeyApplicationJson));
            allTodos.StatusCode.Should().Be(HttpStatusCode.OK);
            allTodos.Body.DeserializeJson<Todo[]>().First().Should().Be(_editedTodo);
        }

        [Test]
        public void Put_IdDoesnotExistInStore_ReturnsNotFound()
        {
            // Arrange
            var expected = HttpStatusCode.NotFound;

            // Act
            var actual = _browser.Put(PathTodos + "89", with => with.JsonBody(_editedTodo));

            // Assert
            actual.StatusCode.Should().Be(expected);
        }
    }
}