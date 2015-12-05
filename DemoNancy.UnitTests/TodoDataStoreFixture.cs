using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DemoNancy.Data;
using DemoNancy.Model;
using Moq;

namespace DemoNancy.UnitTests
{
    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class TodoDataStoreFixture
    {
        private TodoDataStore _todoDataStore;
        private Random _random;
        private List<int> _ids;
        private string _filePath;

        [SetUp]
        public void SetUp()
        {
            _random = new Random();
            _filePath = Path.Combine(Path.GetTempPath(), "todos.xml");
            _todoDataStore = new TodoDataStore(_filePath);
            _todoDataStore.Clear();

            _ids = new List<int>();
            Enumerable.Range(1, _random.Next(5, 20)).ToList().ForEach(x =>
            {
                var id = _random.Next();
                _ids.Add(id);
                _todoDataStore.Add(new Todo
                {
                    Id = id,
                    Completed = _random.Next() % 2 == 0,
                    Order = _random.Next(5, 10),
                    Title = _random.Next().ToString(),
                }).Should().BeTrue();
            });
        }

        [Test]
        public void Add_TodoProvided_AddedToStore()
        {
            // Arrange
            _todoDataStore.Count.Should().Be(_ids.Count);
            var id = _random.Next(500, 600);

            // Act
            var actual = _todoDataStore.Add(new Todo
            {
                Id = id,
                Completed = _random.Next()%2 == 0,
                Order = _random.Next(5, 10),
                Title = _random.Next().ToString(),
            });

            // Assert
            actual.Should().BeTrue();
            var todoDataStore = new TodoDataStore(_filePath);
            todoDataStore.Get(id).Should().NotBeNull();
            todoDataStore.Count.Should().Be(_ids.Count + 1);
        }

        [Test]
        public void Add_TodoNotProper_NotAddedToStore()
        {
            // Arrange
            var expected = _ids.Count;
            _todoDataStore.Count.Should().Be(expected);

            // Act
            var actual = _todoDataStore.Add(null);

            // Assert
            actual.Should().BeFalse();
            var todoDataStore = new TodoDataStore(_filePath);
            todoDataStore.Count.Should().Be(expected);
        }

        [Test]
        public void Count_TodosInStore_ReturnsCount()
        {
            // Arrange
            var expected = _ids.Count;

            // Act
            var actual = _todoDataStore.Count;

            // Assert
            actual.Should().Be(expected);
        }

        [Test]
        public void GetAll_TodosInStore_ReturnsCount()
        {
            // Arrange
            var expected = _ids.Count;

            // Act
            var actual = _todoDataStore.GetAll().Count();

            // Assert
            actual.Should().Be(expected);
        }

        [Test]
        public void Remove_IdOfAvailableTodo_TodoRemovedAndReturnsTrue()
        {
            // Arrange
            var toRemove = _ids[_random.Next(0, _ids.Count)];
            _todoDataStore.Get(toRemove).Should().NotBeNull();

            // Act
            var actual = _todoDataStore.Remove(toRemove);

            // Assert
            actual.Should().BeTrue();
            var todoDataStore = new TodoDataStore(_filePath);
            todoDataStore.Get(toRemove).Should().BeNull();
        }

        [Test]
        public void Remove_IdNotInDataStore_ReturnsFalse()
        {
            // Arrange
            var toRemove = _random.Next(500, 1000);
            _ids.Contains(toRemove).Should().BeFalse();
            _todoDataStore.Count.Should().Be(_ids.Count);

            // Act
            var actual = _todoDataStore.Remove(toRemove);

            // Assert
            actual.Should().BeFalse();
            var todoDataStore = new TodoDataStore(_filePath);
            todoDataStore.Count.Should().Be(_ids.Count);
        }

        [Test]
        public void Update_AlteredTodoProvided_TodoInStoreUpdated()
        {
            // Arrange
            var count = _ids.Count + 1;
            var toEdit = new Todo
            {
                Id = _random.Next(500, 1000),
                Completed = _random.Next()%2 == 0,
                Order = _random.Next(5, 10),
                Title = _random.Next().ToString(),
            };
            _todoDataStore.Add(toEdit);
            _todoDataStore.Count.Should().Be(count);
            var editedTodo = new Todo
            {
                Id = toEdit.Id,
                Completed = !toEdit.Completed,
                Order = _random.Next(15, 20),
                Title = toEdit.Title + _random.Next(),
            };

            // Act
            var actual = _todoDataStore.Update(editedTodo);

            // Assert
            actual.Should().BeTrue();
            var todoDataStore = new TodoDataStore(_filePath);
            todoDataStore.Count.Should().Be(count);
            todoDataStore.Get(toEdit.Id).Should().Be(editedTodo);
        }

        [Test]
        public void Update_RemoveReturnsFalse_AddAndSaveNotCalled_ReturnsFalse()
        {
            // Arrange
            var id = this._random.Next();
            var todoDataStore = new Mock<TodoDataStore>(_filePath) {CallBase = true,};
            todoDataStore.Setup(x => x.Remove(id)).Returns(false);

            // Act
            var actual = todoDataStore.Object.Update(new Todo { Id = id });

            // Assert
            todoDataStore.Verify(x => x.Remove(id));
            todoDataStore.Verify(x => x.Add(It.IsAny<Todo>()), Times.Never);
            todoDataStore.Verify(x => x.Save(), Times.Never);
            actual.Should().BeFalse();
        }

        [Test]
        public void Update_AddReturnsFalse_SaveNotCalled_ReturnsFalse()
        {
            // Arrange
            var id = this._random.Next();
            var todoDataStore = new Mock<TodoDataStore>(_filePath) {CallBase = true,};
            todoDataStore.Setup(x => x.Remove(id)).Returns(true);
            todoDataStore.Setup(x => x.Add(It.IsAny<Todo>())).Returns(false);

            // Act
            var actual = todoDataStore.Object.Update(new Todo { Id = id });

            // Assert
            todoDataStore.Verify(x => x.Remove(id));
            todoDataStore.Verify(x => x.Add(It.IsAny<Todo>()));
            todoDataStore.Verify(x => x.Save(), Times.Never);
            actual.Should().BeFalse();
        }

        [Test]
        public void Update_UpdateAndAddReturnTrue_CallsSaveAndReturnsTrue()
        {
            // Arrange
            var id = this._random.Next();
            var todoDataStore = new Mock<TodoDataStore>(_filePath) {CallBase = true,};
            todoDataStore.Setup(x => x.Remove(id)).Returns(true);
            todoDataStore.Setup(x => x.Add(It.IsAny<Todo>())).Returns(true);

            // Act
            var actual = todoDataStore.Object.Update(new Todo { Id = id });

            // Assert
            todoDataStore.Verify(x => x.Remove(id));
            todoDataStore.Verify(x => x.Add(It.IsAny<Todo>()));
            todoDataStore.Verify(x => x.Save());
            actual.Should().BeTrue();
        }
    }
}