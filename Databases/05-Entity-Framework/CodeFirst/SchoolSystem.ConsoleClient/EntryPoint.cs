using SchoolSystem.Data;
using SchoolSystem.Models;
using SchoolSystem.Models.Enums;

namespace SchoolSystem.ConsoleClient
{
    class EntryPoint
    {
        static void Main()
        {
            using (var db = new SchoolDbContext())
            {
                var s1 = new Student()
                {
                    Name = "Ivan Ivanov",
                    FacultyNumber = "F12312",
                    Gender = GenderType.Male
                };

                db.Students.Add(s1);
                db.SaveChanges();
            }
        }
    }
}
