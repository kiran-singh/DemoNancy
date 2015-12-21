using System;
using System.IO;
using System.Xml.Linq;
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
        private Random _random;
        private string _filePath;
        private Mock<TodoDataStore> _todoDataStore;

        [SetUp]
        public void SetUp()
        {
            _random = new Random();
            _filePath = _random.Next().ToString();
            new XDocument(new XElement("todos")).Save(_filePath);

            var filePathProvider = new Mock<IFilePathProvider>();
            filePathProvider.Setup(x => x.GetPath()).Returns(_filePath);

            _todoDataStore = new Mock<TodoDataStore>(filePathProvider.Object) { CallBase = true, };
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        [Test]
        public void Update_RemoveReturnsFalse_AddAndSaveNotCalled_ReturnsFalse()
        {
            // Arrange
            var id = _random.Next();
            _todoDataStore.Setup(x => x.Remove(id)).Returns(false);

            // Act
            var actual = _todoDataStore.Object.Update(new Todo { Id = id });

            // Assert
            _todoDataStore.Verify(x => x.Remove(id));
            _todoDataStore.Verify(x => x.Add(It.IsAny<Todo>()), Times.Never);
            _todoDataStore.Verify(x => x.Save(), Times.Never);
            actual.Should().BeFalse();
        }

        [Test]
        public void Update_AddReturnsFalse_SaveNotCalled_ReturnsFalse()
        {
            // Arrange
            var id = this._random.Next();
            _todoDataStore.Setup(x => x.Remove(id)).Returns(true);
            _todoDataStore.Setup(x => x.Add(It.IsAny<Todo>())).Returns(false);

            // Act
            var actual = _todoDataStore.Object.Update(new Todo { Id = id });

            // Assert
            _todoDataStore.Verify(x => x.Remove(id));
            _todoDataStore.Verify(x => x.Add(It.IsAny<Todo>()));
            _todoDataStore.Verify(x => x.Save(), Times.Never);
            actual.Should().BeFalse();
        }

        [Test]
        public void Update_UpdateAndAddReturnTrue_CallsSaveAndReturnsTrue()
        {
            // Arrange
            var id = this._random.Next();
            _todoDataStore.Setup(x => x.Remove(id)).Returns(true);
            _todoDataStore.Setup(x => x.Add(It.IsAny<Todo>())).Returns(true);

            // Act
            var actual = _todoDataStore.Object.Update(new Todo { Id = id });

            // Assert
            _todoDataStore.Verify(x => x.Remove(id));
            _todoDataStore.Verify(x => x.Add(It.IsAny<Todo>()));
            _todoDataStore.Verify(x => x.Save());
            actual.Should().BeTrue();
        }
    }
}