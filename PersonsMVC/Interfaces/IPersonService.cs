using Assignment5.Models;

namespace Assignment5.Interfaces;

public interface IPersonService
{
    Person? GetPersonById(int id);
    
    void AddPerson(Person person);

    bool UpdatePerson(int id, Person person);

    bool DeletePerson(int id);

    List<Person> ListAll();
}