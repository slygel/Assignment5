using Assignment5.Interfaces;
using Assignment5.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment5.Controllers;

public class PersonController : Controller
{
    private readonly IPersonService _personService;

    public PersonController(IPersonService personService)
    {
        _personService = personService;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        var people = _personService.ListAll();
        return View(people);
    }

    [HttpGet]
    public IActionResult CreatePerson()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreatePerson(Person person)
    {
        if (!ModelState.IsValid)
        {
            return View(person);
        }
        _personService.AddPerson(person);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public IActionResult PersonDetails(int id)
    {
        var person = _personService.GetPersonById(id);
        if (person == null)
        {
            return NotFound();
        }
        return View(person);
    }
    
    [HttpPost]
    public IActionResult DeletePerson(int id)
    {
        var person = _personService.GetPersonById(id);
        if (person == null)
        {
            return NotFound();
        }

        if (_personService.DeletePerson(id))
        {
            return RedirectToAction(nameof(DeleteConfirmation), new { 
                deletedName = $"{person.FirstName} {person.LastName}" 
            });
        }
        
        return BadRequest("Failed to delete person");
    }

    [HttpGet]
    public IActionResult DeleteConfirmation(string deletedName)
    {
        ViewBag.DeletedName = deletedName;
        return View();
    }
    
    [HttpGet]
    public IActionResult EditPerson(int id)
    {
        var person = _personService.GetPersonById(id);
        if (person == null)
        {
            return NotFound();
        }
        return View(person);
    }

    [HttpPost]
    public IActionResult EditPerson(int id, Person person)
    {
        if (id != person.Id)
        {
            return BadRequest("Person ID mismatch");
        }
        if (!ModelState.IsValid)
        {
            return View(person);
        }

        if (_personService.UpdatePerson(id, person))
        {
            return RedirectToAction(nameof(Index));
        }
        return View(person);
    }
}