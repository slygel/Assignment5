using Assignment5.Interfaces;
using Assignment5.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Assignment5.Service;

public class PersonService : IPersonService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IPersonData _personData;
    private const string CacheKey = "HelloWorld";

    public PersonService(IPersonData personData, IMemoryCache memoryCache)
    {
        _personData = personData;
        _memoryCache = memoryCache;
    }
    
    public List<Person> ListAll()
    {
        var people = _personData.GetAllPeople();
        _memoryCache.Set(CacheKey, people, TimeSpan.FromMinutes(30));
        return people;
    }
    
    public void AddPerson(Person person)
    {
        if (!_memoryCache.TryGetValue(CacheKey, out List<Person> ?people))
        {
            people = _personData.GetAllPeople();
        }
        person.Id = people != null && people.Any() ? people.Max(p => p.Id) + 1 : 1;
        people?.Add(person);
        _memoryCache.Set(CacheKey, people, TimeSpan.FromMinutes(30));
    }
    
    public Person? GetPersonById(int id)
    {
        if (!_memoryCache.TryGetValue(CacheKey, out List<Person> ?people) || people == null)
        {
            people = _personData.GetAllPeople();
            _memoryCache.Set(CacheKey, people, TimeSpan.FromMinutes(30));
        }
        return people.FirstOrDefault(p => p.Id == id);;
    }

    public bool UpdatePerson(int id, Person person)
    {
        if (_memoryCache.TryGetValue(CacheKey, out List<Person>? people) && people != null)
        {
            var existingPerson = people.FirstOrDefault(p => p.Id == id);

            if (existingPerson == null)
            {
                return false;
            }
            existingPerson.FirstName = person.FirstName;
            existingPerson.LastName = person.LastName;
            existingPerson.Gender = person.Gender;
            existingPerson.DateOfBirth = person.DateOfBirth;
            existingPerson.PhoneNumber = person.PhoneNumber;
            existingPerson.BirthPlace = person.BirthPlace;
            existingPerson.IsGraduated = person.IsGraduated;
                
            _memoryCache.Set(CacheKey, people, TimeSpan.FromMinutes(30));
            return true;
        }
        return false;
    }

    public bool DeletePerson(int id)
    {
        if (_memoryCache.TryGetValue(CacheKey, out List<Person> ?people))
        {
            var personToDelete = people.FirstOrDefault(p => p.Id == id);
            
            if (personToDelete != null)
            {
                people.Remove(personToDelete);
                _memoryCache.Set(CacheKey, people, TimeSpan.FromMinutes(30));
                return true;
            }
        }
        return false;
    }
}