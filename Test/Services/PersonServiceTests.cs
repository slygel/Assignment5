using Assignment5.Interfaces;
using Assignment5.Models;
using Assignment5.Service;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;

namespace Test.Services;

public class PersonServiceTests
{
    private Mock<IPersonData> _mockPersonData;
    private Mock<IMemoryCache> _mockMemoryCache;
    private PersonService _service;
    private List<Person> _people;
    
    [SetUp]
    public void Setup()
    {
        _mockPersonData = new Mock<IPersonData>();
        _mockMemoryCache = new Mock<IMemoryCache>();
        _people = new List<Person>
        {
            new Person(1, "Nguyễn Tài", "Tuệ", GenderType.Male, new DateTime(1990, 5, 15), "1234567890", "Hà Nội", true),
            new Person(2, "Nguyễn Thùy", "Linh", GenderType.FeMale, new DateTime(1992, 8, 22), "9876543210", "Hà Nội", false)
        };
        _mockPersonData.Setup(d => d.GetAllPeople()).Returns(_people);

        // Mock memory cache
        object cacheValue = _people;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);
        var cacheEntry = new Mock<ICacheEntry>();
        _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntry.Object);

        _service = new PersonService(_mockPersonData.Object, _mockMemoryCache.Object);
    }
    
    [Test]
    public void ListAll_ReturnsAllPeople()
    {
        // Arrange
        var capturedPeople = new List<Person>();
        var cacheEntryMock = new Mock<ICacheEntry>();
        cacheEntryMock.SetupSet(m => m.Value = It.IsAny<object>()).Callback<object>(value => capturedPeople.AddRange((List<Person>)value));
        _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

        // Act
        var result = _service.ListAll();

        // Assert
        Assert.That(result, Is.EqualTo(_people));
        Assert.That(capturedPeople, Has.Count.EqualTo(2));
        Assert.That(capturedPeople, Is.EquivalentTo(_people));
    }
    
    [Test]
    public void AddPerson_AddsPersonAndUpdatesCache()
    {
        var newPerson = new Person
        {
            FirstName = "Test",
            LastName = "User",
            Gender = GenderType.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            PhoneNumber = "1234567890",
            BirthPlace = "Hà Nội",
            IsGraduated = true
        };
        object cacheValue = null;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);

        // Capture the cache entry to verify the set operation
        var capturedPeople = new List<Person>();
        var cacheEntryMock = new Mock<ICacheEntry>();
        cacheEntryMock.SetupSet(m => m.Value = It.IsAny<object>()).Callback<object>(value => capturedPeople.AddRange((List<Person>)value));
        _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

        // Act
        _service.AddPerson(newPerson);

        // Assert
        Assert.That(newPerson.Id, Is.EqualTo(3));
        Assert.That(capturedPeople, Has.Count.EqualTo(3)); // Original 2 + new person
        Assert.That(capturedPeople.Any(p => p.FirstName == "Test" && p.LastName == "User"), Is.True);
    }
    
    [Test]
    public void GetPersonById_ValidId_ReturnsPerson()
    {
        // Arrange
        object cacheValue = null;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);

        // Act
        var result = _service.GetPersonById(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Id, Is.EqualTo(1));
    }
    
    [Test]
    public void GetPersonById_InvalidId_ReturnsNull()
    {
        // Arrange
        object cacheValue = null;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);

        // Act
        var result = _service.GetPersonById(999);

        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public void UpdatePerson_ValidId_ReturnsTrue()
    {
        // Arrange
        var updatedPerson = new Person
        {
            Id = 1,
            FirstName = "Updated",
            LastName = "User",
            Gender = GenderType.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            PhoneNumber = "1234567890",
            BirthPlace = "Hà Nội",
            IsGraduated = true
        };
        var cachedPeople = new List<Person>(_people); // Create a copy to simulate cache
        object cacheValue = cachedPeople;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(true);

        var capturedPeople = new List<Person>();
        var cacheEntryMock = new Mock<ICacheEntry>();
        cacheEntryMock.SetupSet(m => m.Value = It.IsAny<object>()).Callback<object>(value => capturedPeople.AddRange((List<Person>)value));
        _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

        // Act
        var result = _service.UpdatePerson(1, updatedPerson);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(capturedPeople.First(p => p.Id == 1).FirstName, Is.EqualTo("Updated"));
        Assert.That(capturedPeople, Has.Count.EqualTo(2));
    }
    
    [Test]
    public void UpdatePerson_InvalidId_ReturnsFalse()
    {
        // Arrange
        var updatedPerson = new Person { Id = 999 };
        object cacheValue = _people;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(true);

        // Act
        var result = _service.UpdatePerson(999, updatedPerson);

        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void DeletePerson_ValidId_ReturnsTrue()
    {
        // Arrange
        var cachedPeople = new List<Person>(_people); // Create a copy to simulate cache
        object cacheValue = cachedPeople;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(true);

        var capturedPeople = new List<Person>();
        var cacheEntryMock = new Mock<ICacheEntry>();
        cacheEntryMock.SetupSet(m => m.Value = It.IsAny<object>()).Callback<object>(value => capturedPeople.AddRange((List<Person>)value));
        _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

        // Act
        var result = _service.DeletePerson(1);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(capturedPeople, Has.Count.EqualTo(1));
        Assert.That(capturedPeople.Any(p => p.Id == 1), Is.False); // Verify person with Id=1 is removed
    }
    
    [Test]
    public void DeletePerson_InvalidId_ReturnsFalse()
    {
        // Arrange
        object cacheValue = _people;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(true);

        // Act
        var result = _service.DeletePerson(999);

        // Assert
        Assert.That(result, Is.False);
    }
}