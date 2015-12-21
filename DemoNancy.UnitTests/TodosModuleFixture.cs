using System;
using System.Collections.Generic;
using DemoNancy.Data;
using DemoNancy.Model;
using FluentAssertions;
using Moq;
using Nancy;
using Nancy.Testing;
using NUnit.Framework;

namespace DemoNancy.UnitTests
{
    [TestFixture]
    public class TodosModuleFixture
    {
        private const string PathTodos = "/todos/";
        public const string KeyApplicationXml = "application/xml";
        public const string KeyApplicationJson = "application/json";
        public const string KeyTextHtml = "text/html";

        private Browser _browser;

        private Todo _aTodo;

        private Mock<IDataStore<Todo>> _store;

        private Todo _editedTodo;
        private Random _random;

        [SetUp]
        public void SetUp()
        {
            _random = new Random();
            _store = new Mock<IDataStore<Todo>>();

            _aTodo = new Todo { Completed = false, Order = 0, Title = "Task 1" };
            _editedTodo = new Todo { Completed = false, Id = 42, Order = 0, Title = "Edited name" };

            _browser = new Browser(new ConfigurableBootstrapper(with =>
            {
                with.Dependency(_store.Object);
                with.Module<TodosModule>();
                with.RootPathProvider<TestRootPathProvider>();
            }));
        }

        [Test]
        public void Delete_IdOfExistingTodo_TodoDeleted()
        {
            // Arrange
            var id = _random.Next();
            _store.Setup(x => x.Remove(id)).Returns(true);

            // Act
            var actual = _browser.Delete(PathTodos + id);

            // Assert
            _store.Verify(x => x.Remove(id));
            actual.StatusCode.Should().Be(HttpStatusCode.OK);
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
            _store.Setup(x => x.GetAll()).Returns(new List<Todo>());

            // Act
            var actual = _browser.Get(PathTodos, with =>
            {
                with.HttpRequest();
                with.Accept(KeyApplicationJson);
            });

            // Assert
            _store.Verify(x => x.GetAll());
            actual.StatusCode.Should().Be(HttpStatusCode.OK);
            actual.Body.DeserializeJson<Todo[]>().Should().BeEmpty();
        }

        [Test]
        public void Get_TextHtmlRequested_ReturnsView()
        {
            // Arrange
            _aTodo.Id = 1;
            _store.Setup(x => x.GetAll()).Returns(new[] { _aTodo });

            // Act
            var actual = _browser.Get(PathTodos,
                            with => with.Accept(KeyTextHtml))
                            .Body;

            // Assert
            _store.Verify(x => x.GetAll());
            actual["form"].ShouldExistOnce();
            actual["title"].AllShouldContain("Todos");
            actual["tr td:first-child"]
                .ShouldExistOnce()
                .And
                .ShouldContain(_aTodo.Title);
        }

        [Test]
        public void Post_TodoPosted_Returns201()
        {
            // Arrange
            _store.Setup(x => x.Add(_aTodo)).Returns(true);

            // Act
            var actual = _browser.Post(PathTodos, with =>
            {
                with.JsonBody(_aTodo);
                with.Accept(KeyApplicationXml);
            });

            // Assert
            _store.Verify(x => x.Add(_aTodo));
            actual.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public void Post_TodoPostedForHtml_ReturnsViewCreated()
        {
            // Arrange
            _store.Setup(x => x.Add(_aTodo)).Returns(true);

            // Act
            var actual = _browser.Post(PathTodos, with =>
            {
                with.JsonBody(_aTodo);
                with.Accept(KeyTextHtml);
            });

            // Assert
            _store.Verify(x => x.Add(_aTodo));
            actual.StatusCode.Should().Be(HttpStatusCode.Created);
            actual.Body["title"].AllShouldContain("Todo");
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
        public void Put_EditedTodo_OriginalTodoChanged()
        {
            // Arrange
            _store.Setup(x => x.Update(_editedTodo)).Returns(true);
            
            // Act
            var actual = _browser.Put(PathTodos + _editedTodo.Id, with => with.JsonBody(_editedTodo));

            // Assert
            _store.Verify(x => x.Update(_editedTodo));
            actual.StatusCode.Should().Be(HttpStatusCode.OK);
            actual.Body.DeserializeJson<Todo>().Should().Be(_editedTodo);
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