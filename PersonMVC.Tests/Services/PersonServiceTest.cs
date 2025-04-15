using Assignment5.Interfaces;
using Assignment5.Models;
using Assignment5.Service;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace NUnitTest.Services;

public class PersonServiceTest
{
    private Mock<IPersonData> _mockPersonData;
    private Mock<IMemoryCache> _mockMemoryCache;
    private PersonService _personService;
    private List<Person> _people;
    private Mock<ICacheEntry> _mockCacheEntry;
    private List<Person> _cachedPeople;

    [SetUp]
    public void Setup()
    {
        _mockPersonData = new Mock<IPersonData>();
        _mockMemoryCache = new Mock<IMemoryCache>();
        _mockCacheEntry = new Mock<ICacheEntry>();
        _personService = new PersonService(_mockPersonData.Object, _mockMemoryCache.Object);

        _people = new List<Person>
        {
            new Person(1, "Tai", "Tue", GenderType.Male, new DateTime(1990, 1, 1), "1234567890", "Ba Vi", true),
            new Person(2, "Thanh", "Tu", GenderType.FeMale, new DateTime(1995, 2, 2), "0987654321", "Ba Vi", false)
        };
        _cachedPeople = null;

        _mockMemoryCache
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(_mockCacheEntry.Object)
            .Callback<object>(key =>
            {
                _mockCacheEntry.SetupSet(e => e.Value = It.IsAny<object>())
                    .Callback<object>(value => _cachedPeople = value as List<Person>);
            });

        _mockMemoryCache
            .Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
            .Returns((object key, out object value) =>
            {
                value = _cachedPeople;
                return _cachedPeople != null;
            });
    }

    [Test]
    public void ListAll_ReturnsPeopleAndCachesResult()
    {
        // Arrange
        _mockPersonData.Setup(d => d.GetAllPeople()).Returns(_people);

        // Act
        var result = _personService.ListAll();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(_people));
        _mockMemoryCache.Verify(m => m.CreateEntry("HelloWorld"), Times.Once());
        _mockPersonData.Verify(d => d.GetAllPeople(), Times.Once());
        Assert.That(_cachedPeople, Is.EqualTo(_people));
    }
    
    [Test]
    public void AddPerson_CacheMiss_AddsPersonWithNewId()
    {
        // Arrange
        _mockPersonData.Setup(d => d.GetAllPeople()).Returns(_people);
        var newPerson = new Person
        {
            FirstName = "Hai",
            LastName = "Dang",
            Gender = GenderType.FeMale,
            DateOfBirth = new DateTime(2000, 1, 1),
            PhoneNumber = "1112223333",
            BirthPlace = "Ba Vi",
            IsGraduated = false
        };

        // Act
        _personService.AddPerson(newPerson);

        // Assert
        Assert.That(newPerson.Id, Is.EqualTo(3));
        _mockPersonData.Verify(d => d.GetAllPeople(), Times.Once());
        _mockMemoryCache.Verify(m => m.CreateEntry("HelloWorld"), Times.Once());
        Assert.That(_cachedPeople, Does.Contain(newPerson));
    }
    
    [Test]
    public void GetPersonById_CacheMiss_ReturnsPerson()
    {
        // Arrange
        _mockPersonData.Setup(d => d.GetAllPeople()).Returns(_people);

        // Act
        var result = _personService.GetPersonById(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(_people[0]));
        _mockPersonData.Verify(d => d.GetAllPeople(), Times.Once());
        _mockMemoryCache.Verify(m => m.CreateEntry("HelloWorld"), Times.Once());
        Assert.That(_cachedPeople, Is.EqualTo(_people));
    }
    
    [Test]
    public void GetPersonById_PersonNotFound_ReturnsNull()
    {
        // Arrange
        _mockPersonData.Setup(d => d.GetAllPeople()).Returns(_people);

        // Act
        var result = _personService.GetPersonById(999);

        // Assert
        Assert.That(result, Is.Null);
        _mockPersonData.Verify(d => d.GetAllPeople(), Times.Once());
        _mockMemoryCache.Verify(m => m.CreateEntry("HelloWorld"), Times.Once());
    }
    
    [Test]
    public void UpdatePerson_CacheHit_PersonExists_UpdatesAndReturnsTrue()
    {
        // Arrange
        _cachedPeople = new List<Person>(_people);
        var updatedPerson = new Person(1, "Van", "A", GenderType.Male, new DateTime(1990, 1, 1), "1112223333", "Ba Vi", false);

        // Act
        var result = _personService.UpdatePerson(1, updatedPerson);

        // Assert
        Assert.That(result, Is.True);
        var person = _cachedPeople.First(p => p.Id == 1);
        Assert.Multiple(() =>
        {
            Assert.That(person.LastName, Is.EqualTo("A"));
            Assert.That(person.BirthPlace, Is.EqualTo("Ba Vi"));
            Assert.That(person.IsGraduated, Is.EqualTo(false));
        });
        _mockMemoryCache.Verify(m => m.CreateEntry("HelloWorld"), Times.Once());
    }

    [Test]
    public void UpdatePerson_CacheMiss_ReturnsFalse()
    {
        // Arrange
        _cachedPeople = null;
        var updatedPerson = new Person(1, "John", "Updated", GenderType.Male, new DateTime(1990, 1, 1), "1112223333", "CityC", false);

        // Act
        var result = _personService.UpdatePerson(1, updatedPerson);

        // Assert
        Assert.That(result, Is.False);
        _mockMemoryCache.Verify(m => m.CreateEntry("HelloWorld"), Times.Never());
    }

    [Test]
    public void DeletePerson_CacheHit_PersonExists_DeletesAndReturnsTrue()
    {
        // Arrange
        _cachedPeople = new List<Person>(_people);

        // Act
        var result = _personService.DeletePerson(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_cachedPeople.Any(p => p.Id == 1), Is.False);
        });
        _mockMemoryCache.Verify(m => m.CreateEntry("HelloWorld"), Times.Once());
    }

    [Test]
    public void DeletePerson_CacheHit_PersonNotFound_ReturnsFalse()
    {
        // Arrange
        _cachedPeople = _people;

        // Act
        var result = _personService.DeletePerson(999);

        // Assert
        Assert.That(result, Is.False);
        _mockMemoryCache.Verify(m => m.CreateEntry("HelloWorld"), Times.Never());
    }
}