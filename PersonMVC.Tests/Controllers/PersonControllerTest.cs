using Assignment5.Controllers;
using Assignment5.Interfaces;
using Assignment5.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace NUnitTest.Controllers;

public class PersonControllerTest
{
    private Mock<IPersonService> _mockPersonService;
    private PersonController _controller;
    private List<Person> _people;
    
    [SetUp]
    public void Setup()
    {
        _mockPersonService = new Mock<IPersonService>();
        _controller = new PersonController(_mockPersonService.Object);
        _people = new List<Person>
        {
            new Person { Id = 1, FirstName = "Tai", LastName = "Tue" },
            new Person { Id = 2, FirstName = "Thanh", LastName = "Tu" }
        };
    }
    
    [TearDown]
    public void TearDown()
    {
        _controller?.Dispose();
    }
    
    [Test]
    public void Index_ReturnsViewWithPeople()
    {
        // Arrange
        _mockPersonService.Setup(s => s.ListAll()).Returns(_people);

        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Model, Is.EqualTo(_people));
    }
    
    [Test]
    public void CreatePerson_Get_ReturnsView()
    {
        // Act
        var result = _controller.CreatePerson() as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }
    
    [Test]
    public void CreatePerson_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var person = new Person { Id = 3, FirstName = "Tai", LastName = "Tue" };
        _mockPersonService.Setup(s => s.AddPerson(It.IsAny<Person>()));
        _controller.ModelState.Clear();

        // Act
        var result = _controller.CreatePerson(person) as RedirectToActionResult;

        // Assert
        _mockPersonService.Verify(s => s.AddPerson(person), Times.Once());
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo(nameof(PersonController.Index)));
    }
    
    [Test]
    public void CreatePerson_Post_InvalidModel_ReturnsView()
    {
        // Arrange
        var person = new Person();
        _controller.ModelState.AddModelError("FirstName", "Required");
    
        // Act
        var result = _controller.CreatePerson(person) as ViewResult;
    
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Model, Is.EqualTo(person));
    }
    
    [Test]
    public void PersonDetails_PersonExists_ReturnsView()
    {
        // Arrange
        var person = _people[0];
        _mockPersonService.Setup(s => s.GetPersonById(1)).Returns(person);

        // Act
        var result = _controller.PersonDetails(1) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Model, Is.EqualTo(person));
    }
    
    [Test]
    public void PersonDetails_PersonNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockPersonService.Setup(s => s.GetPersonById(999)).Returns((Person)null);

        // Act
        var result = _controller.PersonDetails(999);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
    
    [Test]
    public void DeletePerson_Post_PersonExists_RedirectsToDeleteConfirmation()
    {
        // Arrange
        var person = _people[0];
        _mockPersonService.Setup(s => s.GetPersonById(1)).Returns(person);
        _mockPersonService.Setup(s => s.DeletePerson(1)).Returns(true);

        // Act
        var result = _controller.DeletePerson(1) as RedirectToActionResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo(nameof(PersonController.DeleteConfirmation)));
        Assert.That(result.RouteValues?["deletedName"], Is.EqualTo($"{person.FirstName} {person.LastName}"));
    }
    
    [Test]
    public void DeleteConfirmation_ReturnsView_WithDeletedName()
    {
        // Arrange
        string deletedName = "Tai Tue";
        
        // Act
        var result = _controller.DeleteConfirmation(deletedName) as ViewResult;
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ViewData["DeletedName"], Is.EqualTo(deletedName));
    }
    
    [Test]
    public void DeletePerson_Post_PersonNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockPersonService.Setup(s => s.GetPersonById(999)).Returns((Person)null);

        // Act
        var result = _controller.DeletePerson(999);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void EditPerson_Get_PersonExists_ReturnsView()
    {
        // Arrange
        var person = _people[0];
        _mockPersonService.Setup(s => s.GetPersonById(1)).Returns(person);

        // Act
        var result = _controller.EditPerson(1) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Model, Is.EqualTo(person));
    }
    
    [Test]
    public void EditPerson_Get_PersonNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockPersonService.Setup(s => s.GetPersonById(999)).Returns((Person)null);

        // Act
        var result = _controller.EditPerson(999);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
    
    [Test]
    public void EditPerson_Post_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var person = new Person { Id = 2 };

        // Act
        var result = _controller.EditPerson(1, person) as BadRequestObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.EqualTo("Person ID mismatch"));
    }
    
    [Test]
    public void EditPerson_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var person = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
        _mockPersonService.Setup(s => s.UpdatePerson(1, person)).Returns(true);
        _controller.ModelState.Clear();

        // Act
        var result = _controller.EditPerson(1, person) as RedirectToActionResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo(nameof(PersonController.Index)));
    }
    
    [Test]
    public void EditPerson_Post_InvalidModel_ReturnsView()
    {
        // Arrange
        var person = new Person { Id = 1 };
        _controller.ModelState.AddModelError("FirstName", "Required");

        // Act
        var result = _controller.EditPerson(1, person) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Model, Is.EqualTo(person));
    }
}