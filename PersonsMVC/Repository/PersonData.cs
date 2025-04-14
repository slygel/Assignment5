using Assignment5.Interfaces;
using Assignment5.Models;

namespace Assignment5.Repository;

public class PersonData : IPersonData
{
    private readonly List<Person> _people =
    [
        new (1, "Nguyễn Tài", "Tuệ", GenderType.Male, new DateTime(1990, 5, 15), "1234567890", "Hà Nội", true),
        new (2, "Nguyễn Thùy", "Linh", GenderType.FeMale, new DateTime(1992, 8, 22), "9876543210", "Hà Nội", false),
        new (3, "Đào Ngọc", "Văn", GenderType.Male, new DateTime(1988, 12, 1), "5551234567", "Hưng Yên", true),
        new (4, "Trần Ngọc", "Bích", GenderType.FeMale, new DateTime(1995, 3, 10), "4445556666", "Hà Nội", true),
        new (5, "Nguyễn Văn", "An", GenderType.Male, new DateTime(1993, 7, 25), "7778889999", "Nghệ An", false),
        new (6, "Trần Thị", "Ngọc", GenderType.FeMale, new DateTime(2000, 1, 1), "1112223333", "Ninh Bình", false),
        new (7, "Trần Văn", "Mạnh", GenderType.Male, new DateTime(2005, 6, 15), "2223334444", "Hà Nội", false)
    ];

    public List<Person> GetAllPeople()
    {
        return _people;
    }
}