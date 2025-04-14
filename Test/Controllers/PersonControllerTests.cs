using Assignment5.Controllers;
using Assignment5.Interfaces;
using Assignment5.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Test.Controllers;

public class PersonControllerTests
{
    private Mock<IPersonService> _mockPersonService;
    private PersonController _controller;
    
    [SetUp]
    public void Setup()
    {
        _mockPersonService = new Mock<IPersonService>();
        _controller = new PersonController(_mockPersonService.Object);
    }
    
    [Test]
    public void Index_ReturnsViewWithPeopleList()
    {
        // Arrange
        var people = new List<Person>
        {
            new Person(1, "Nguyễn Tài", "Tuệ", GenderType.Male, new DateTime(1990, 5, 15), "1234567890", "Hà Nội", true),
            new Person(2, "Nguyễn Thùy", "Linh", GenderType.FeMale, new DateTime(1992, 8, 22), "9876543210", "Hà Nội", false)
        };
        _mockPersonService.Setup(s => s.ListAll()).Returns(people);

        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.That(result?.Model, Is.Not.Null);
        Assert.That(result?.Model, Is.EqualTo(people));
    }
    
    [Test]
    public void CreatePerson_Get_ReturnsView()
    {
        // Act
        var result = _controller.CreatePerson() as ViewResult;

        // Assert
        Assert.That(result?.Model,Is.Null);
    }
    
    [Test]
    public void CreatePerson_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var person = new Person
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Gender = GenderType.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            PhoneNumber = "1234567890",
            BirthPlace = "Hà Nội",
            IsGraduated = true
        };
        _controller.ModelState.Clear(); // Simulate valid ModelState
        
        // Act
        var result = _controller.CreatePerson(person) as RedirectToActionResult;

        // Assert
        Assert.That(result?.ActionName, Is.EqualTo("Index"));
        Assert.That(result?.ActionName, Is.Not.Null);
        _mockPersonService.Verify(s => s.AddPerson(person), Times.Once());
    }
    
    [Test]
    public void CreatePerson_Post_InvalidModel_ReturnsView()
    {
        // Arrange
        var person = new Person();
        _controller.ModelState.AddModelError("FirstName", "First name cannot be empty.");

        // Act
        var result = _controller.CreatePerson(person) as ViewResult;

        // Assert
        Assert.That(result?.Model, Is.Not.Null);
        Assert.That(result?.Model, Is.EqualTo(person));
    }
    
    [Test]
    public void PersonDetails_ValidId_ReturnsViewWithPerson()
    {
        // Arrange
        var person = new Person(1, "Nguyễn Tài", "Tuệ", GenderType.Male, new DateTime(1990, 5, 15), "1234567890", "Hà Nội", true);
        _mockPersonService.Setup(s => s.GetPersonById(1)).Returns(person);

        // Act
        var result = _controller.PersonDetails(1) as ViewResult;

        // Assert
        Assert.That(result?.Model, Is.Not.Null);
        Assert.That(result?.Model, Is.EqualTo(person));
    }
    
    [Test]
    public void PersonDetails_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockPersonService.Setup(s => s.GetPersonById(1)).Returns((Person)null);
    
        // Act
        var result = _controller.PersonDetails(1);
    
        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
    
    [Test]
    public void DeletePerson_ValidId_RedirectsToDeleteConfirmation()
    {
        // Arrange
        var person = new Person(1, "Nguyễn Tài", "Tuệ", GenderType.Male, new DateTime(1990, 5, 15), "1234567890", "Hà Nội", true);
        _mockPersonService.Setup(s => s.GetPersonById(1)).Returns(person);
        _mockPersonService.Setup(s => s.DeletePerson(1)).Returns(true);

        // Act
        var result = _controller.DeletePerson(1) as RedirectToActionResult;

        // Assert
        Assert.That(result?.ActionName, Is.Not.Null);
        Assert.That(result?.ActionName, Is.EqualTo("DeleteConfirmation"));
        Assert.That(result?.RouteValues?["deletedName"], Is.EqualTo("Nguyễn Tài Tuệ"));
    }
    
    [Test]
    public void DeletePerson_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockPersonService.Setup(s => s.GetPersonById(1)).Returns((Person)null);

        // Act
        var result = _controller.DeletePerson(1);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
    
    [Test]
    public void EditPerson_Get_ValidId_ReturnsViewWithPerson()
    {
        // Arrange
        var person = new Person(1, "Nguyễn Tài", "Tuệ", GenderType.Male, new DateTime(1990, 5, 15), "1234567890", "Hà Nội", true);
        _mockPersonService.Setup(s => s.GetPersonById(1)).Returns(person);

        // Act
        var result = _controller.EditPerson(1) as ViewResult;

        // Assert
        Assert.That(result?.Model, Is.Not.Null);
        Assert.That(result?.Model, Is.EqualTo(person));
    }
    
    [Test]
    public void EditPerson_Get_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockPersonService.Setup(s => s.GetPersonById(1)).Returns((Person)null);

        // Act
        var result = _controller.EditPerson(1);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
    
    [Test]
    public void EditPerson_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var person = new Person(1, "Nguyễn Tài", "Tuệ", GenderType.Male, new DateTime(1990, 5, 15), "1234567890", "Hà Nội", true);
        _mockPersonService.Setup(s => s.UpdatePerson(1, person)).Returns(true);
        _controller.ModelState.Clear();

        // Act
        var result = _controller.EditPerson(1, person) as RedirectToActionResult;

        // Assert
        Assert.That(result?.ActionName, Is.Not.Null);
        Assert.That(result?.ActionName, Is.EqualTo("Index"));
    }
    
    [Test]
    public void EditPerson_Post_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var person = new Person(2, "Nguyễn Tài", "Tuệ", GenderType.Male, new DateTime(1990, 5, 15), "1234567890", "Hà Nội", true);

        // Act
        var result = _controller.EditPerson(1, person);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    
    [Test]
    public void EditPerson_Post_InvalidModel_ReturnsView()
    {
        // Arrange
        var person = new Person(1, "Nguyễn Tài", "Tuệ", GenderType.Male, new DateTime(1990, 5, 15), "1234567890", "Hà Nội", true);
        _controller.ModelState.AddModelError("FirstName", "First name cannot be empty.");

        // Act
        var result = _controller.EditPerson(1, person) as ViewResult;

        // Assert
        Assert.That(result?.Model, Is.Not.Null);
        Assert.That(result?.Model, Is.EqualTo(person));
    }
}