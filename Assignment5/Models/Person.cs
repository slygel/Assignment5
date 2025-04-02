using System.ComponentModel.DataAnnotations;
using Assignment5.Validation;

namespace Assignment5.Models;

public class Person
{
    
    public int Id { get; set; }
    
    [Required(ErrorMessage = "First name cannot be empty.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name cannot be empty.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Gender cannot be empty.")]
    public GenderType Gender { get; set; }

    [Required(ErrorMessage = "Date of birth cannot be empty.")]
    [DateRange]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Phone number cannot be empty.")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Birth place cannot be empty.")]
    public string BirthPlace { get; set; }
    
    public bool IsGraduated { get; set; }

    public Person()
    {
        
    }
    
    public Person(int id, string firstName, string lastName, GenderType gender, DateTime dateOfBirth, string phoneNumber, string birthPlace, bool isGraduated)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        BirthPlace = birthPlace;
        IsGraduated = isGraduated;
    }
}