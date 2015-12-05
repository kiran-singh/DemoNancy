using System.Collections.Generic;
using System.Linq;
using DemoNancy.Model;
using Nancy;
using Nancy.Testing;

namespace DemoNancy.UnitTests
{
    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class TodosModuleFixture
    {
        private const string PathTodos = "/todos/";
        private Browser _browser;
        private Todo _aTodo;
        private Dictionary<long, Todo> _store;
        private Todo _editedTodo;

        [SetUp]
        public void SetUp()
        {
            _store = new Dictionary<long, Todo>();

            _aTodo = new Todo { Completed = false, Order = 0, Title = "Task 1" };
            _editedTodo = new Todo { Completed = false, Id = 42, Order = 0, Title = "Edited name" };

            _browser = new Browser(new ConfigurableBootstrapper(with =>
            {
                with.Dependency(_store);
                with.Module<TodosModule>();
            }));
        }

        [Test]
        public void Get_NoTodosPosted_ReturnsEmptyList()
        {
            // Arrange
            // Act
            var actual = _browser.Get(PathTodos);

            // Assert
            actual.StatusCode.Should().Be(HttpStatusCode.OK);
            actual.Body.DeserializeJson<Todo[]>().Should().BeEmpty();
        }

        [Test]
        public void Post_TodoPosted_Returns201()
        {
            // Arrange

            // Act
            var actual = _browser.Post(PathTodos, with => with.JsonBody(_aTodo));

            // Assert
            actual.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public void Post_TodoWithDuplicateId_ReturnsNotAcceptableStatusCode()
        {
            // Arrange
            var editedTodo = new Todo { Completed = false, Id = 42, Order = 0, Title = "edited title"};

            // Act
            var actual = _browser.Post(PathTodos, with => with.JsonBody(editedTodo))
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
                _browser.Post(PathTodos, with => with.JsonBody(_aTodo))
                    .Then.Get(PathTodos)
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
            var actual = _browser.Post(PathTodos, with => with.JsonBody(_aTodo)).Then.Get(PathTodos);
            var actualBody = actual.Body.DeserializeJson<Todo[]>();

            // Assert
            actualBody.Length.Should().Be(1);
            actualBody[0].Should().Be(_aTodo);
        }

        [Test]
        public void Put_EditedTodo_OriginalTodoChanged()
        {
            // Arrange
            // Act
            var actual =
                _browser.Post(PathTodos, with => with.JsonBody(_aTodo))
                    .Then.Put(PathTodos + "1", with => with.JsonBody(_editedTodo))
                    .Then.Get(PathTodos)
                    .Body.DeserializeJson<Todo[]>();

            // Assert
            actual.Length.Should().Be(1);
            actual[0].Should().Be(_editedTodo);
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

        [Test]
        public void Delete_IdOfExistingTodo_TodoDeleted()
        {
            // Arrange
            // Act
            var actual =
                _browser.Post(PathTodos, with => with.JsonBody(_aTodo))
                    .Then.Delete(PathTodos + "1")
                    .Then.Get(PathTodos);

            // Assert
            actual.StatusCode.Should().Be(HttpStatusCode.OK);
            actual.Body.DeserializeJson<Todo[]>().Should().BeEmpty();
        }

        [Test]
        public void Delete_IdDoesnotExistInStore_ReturnsNotFound()
        {
            // Arrange
            var expected = HttpStatusCode.NotFound;

            // Act
            var actual = _browser.Delete(PathTodos + "89");

            // Assert
            actual.StatusCode.Should().Be(expected);
        }
    }
}