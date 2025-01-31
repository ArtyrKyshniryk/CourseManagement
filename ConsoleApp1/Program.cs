using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }  
}

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }  
    public List<Course> Courses { get; set; } = new();
}

public class AppDbContext : DbContext
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=app.db");
    }
}

class Program
{
    static void Main()
    {
        using var db = new AppDbContext();
        db.Database.Migrate();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Dodaj kurs\n2. Pokaż kursy\n3. Aktualizuj kurs\n4. Usuń kurs\n5. Dodaj ucznia\n6. Pokaż uczniom\n7. Zaktualizuj ucznia\n8. Usuń ucznia\n9. Zapisz ucznia na kurs\n10. Wychodzić");
            Console.Write("Wybierz opcję: ");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ShowAddCourseMenu(db);
                    break;
                case "2":
                    ShowCourses(db);
                    break;
                case "3":
                    UpdateCourse(db);
                    break;
                case "4":
                    DeleteCourse(db);
                    break;
                case "5":
                    ShowAddStudentMenu(db);
                    break;
                case "6":
                    ShowStudents(db);
                    break;
                case "7":
                    UpdateStudent(db);
                    break;
                case "8":
                    DeleteStudent(db);
                    break;
                case "9":
                    EnrollStudent(db);
                    break;
                case "10":
                    return;
                default:
                    Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.");
                    break;
            }
        }
    }

    static void ShowAddCourseMenu(AppDbContext db)
    {
        Console.Clear();
        Console.WriteLine("Dodaj kurs:");
        AddCourse(db);
        BackToMainMenu();
    }

    static void AddCourse(AppDbContext db)
    {
        Console.Write("Nazwa kursu: ");
        string name = Console.ReadLine();
        Console.Write("Opis kursu: ");
        string description = Console.ReadLine();
        Console.Write("Czas trwania kursu (w godzinach): ");
        int duration = int.Parse(Console.ReadLine());

        db.Courses.Add(new Course { Name = name, Description = description, Duration = duration });
        db.SaveChanges();
        Console.WriteLine("Kurs dodany!\n");
    }

    static void ShowCourses(AppDbContext db)
    {
        Console.Clear();
        var courses = db.Courses.ToList();
        if (courses.Any())
        {
            Console.WriteLine("Lista kursów:");
            foreach (var course in courses)
            {
                Console.WriteLine($"{course.Id}: {course.Name} - {course.Description}, Czas trwania: {course.Duration} godziny");
            }
        }
        else
        {
            Console.WriteLine("Nie znaleziono kursów.");
        }
        BackToMainMenu();
    }

    static void UpdateCourse(AppDbContext db)
    {
        Console.Clear();
        ShowCourses(db); 

        Console.Write("Wprowadź identyfikator kursu, który chcesz edytować: ");
        int courseId = int.Parse(Console.ReadLine());
        var course = db.Courses.Find(courseId);

        if (course != null)
        {
            Console.Write("Nowa nazwa kursu: ");
            course.Name = Console.ReadLine();
            Console.Write("Nowy opis kursu: ");
            course.Description = Console.ReadLine();
            Console.Write("Nowy czas trwania kursu (w godzinach): ");
            course.Duration = int.Parse(Console.ReadLine());

            db.SaveChanges();
            Console.WriteLine("Kurs został zaktualizowany!\n");
        }
        else
        {
            Console.WriteLine("Nie znaleziono kursu.");
        }
        BackToMainMenu();
    }

    static void DeleteCourse(AppDbContext db)
    {
        Console.Clear();
        ShowCourses(db); 

        Console.Write("Wprowadź identyfikator kursu, który chcesz usunąć: ");
        int courseId = int.Parse(Console.ReadLine());
        var course = db.Courses.Find(courseId);

        if (course != null)
        {
            db.Courses.Remove(course);
            db.SaveChanges();
            Console.WriteLine("Kurs został usunięty!\n");
        }
        else
        {
            Console.WriteLine("Nie znaleziono kursu.");
        }
        BackToMainMenu();
    }

    static void ShowAddStudentMenu(AppDbContext db)
    {
        Console.Clear();
        Console.WriteLine("Dodaj ucznia:");
        AddStudent(db);
        BackToMainMenu();
    }

    static void AddStudent(AppDbContext db)
    {
        Console.Write("Imię i nazwisko ucznia: ");
        string name = Console.ReadLine();
        Console.Write("E-mail studencki: ");
        string email = Console.ReadLine();
        Console.Write("Data urodzenia (yyyy-mm-dd): ");
        DateTime birthDate = DateTime.Parse(Console.ReadLine());

        db.Students.Add(new Student { Name = name, Email = email, BirthDate = birthDate });
        db.SaveChanges();
        Console.WriteLine("Dodano ucznia!\n");
    }

    static void ShowStudents(AppDbContext db)
    {
        Console.Clear();
        var students = db.Students.Include(s => s.Courses).ToList();
        if (students.Any())
        {
            Console.WriteLine("Lista studentów:");
            foreach (var student in students)
            {
                Console.WriteLine($"{student.Id}: {student.Name} - {student.Email}, Data urodzenia: {student.BirthDate.ToShortDateString()}");

                if (student.Courses.Any())
                {
                    Console.WriteLine("Nagrane kursy:");
                    foreach (var course in student.Courses)
                    {
                        Console.WriteLine($"- {course.Name}: {course.Description}, Czas trwania: {course.Duration} godziny");
                    }
                }
                else
                {
                    Console.WriteLine("Student nie jest zapisany na żaden kurs.");
                }

                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("Nie znaleziono żadnych uczniów.");
        }
        BackToMainMenu();
    }

    static void UpdateStudent(AppDbContext db)
    {
        Console.Clear();
        ShowStudents(db); 

        Console.Write("Wprowadź identyfikator studenta, aby go edytować: ");
        int studentId = int.Parse(Console.ReadLine());
        var student = db.Students.Find(studentId);

        if (student != null)
        {
            Console.Write("Nowe imię i nazwisko studenta: ");
            student.Name = Console.ReadLine();
            Console.Write("Nowy e-mail studenta: ");
            student.Email = Console.ReadLine();
            Console.Write("Nowa data urodzenia (yyyy-mm-dd): ");
            student.BirthDate = DateTime.Parse(Console.ReadLine());

            db.SaveChanges();
            Console.WriteLine("Dane ucznia zostały zaktualizowane!\n");
        }
        else
        {
            Console.WriteLine("Nie znaleziono ucznia.");
        }
        BackToMainMenu();
    }

    static void DeleteStudent(AppDbContext db)
    {
        Console.Clear();
        ShowStudents(db);

        Console.Write("Wprowadź identyfikator studenta, który chcesz usunąć: ");
        int studentId = int.Parse(Console.ReadLine());
        var student = db.Students.Find(studentId);

        if (student != null)
        {
            db.Students.Remove(student);
            db.SaveChanges();
            Console.WriteLine("Uczeń został usunięty!\n");
        }
        else
        {
            Console.WriteLine("Nie znaleziono ucznia.");
        }
        BackToMainMenu();
    }

    static void EnrollStudent(AppDbContext db)
    {
        Console.Clear();
        ShowCourses(db);
        Console.Write("Wprowadź identyfikator kursu: ");
        int courseId = int.Parse(Console.ReadLine());

        Console.Write("Wpisz adres e-mail ucznia: ");
        string email = Console.ReadLine();
        var student = db.Students.Include(s => s.Courses).FirstOrDefault(s => s.Email == email);
        var course = db.Courses.Find(courseId);

        if (student == null)
        {
            Console.WriteLine($"Student z Email {email} nie znaleziono!\n");
        }

        if (course == null)
        {
            Console.WriteLine($"Kurs z ID {courseId} nie znaleziono!\n");
        }

        if (student != null && course != null)
        {
            if (!student.Courses.Contains(course))
            {
                student.Courses.Add(course);
                db.SaveChanges();
                Console.WriteLine("Student zostaje zapisany na kurs!\n");
            }
            else
            {
                Console.WriteLine("Student jest już zapisany na ten kurs!\n");
            }
        }
        BackToMainMenu();
    }
    static void BackToMainMenu()
    {
        Console.WriteLine("\nNaciśnij „Enter”, aby powrócić do menu głównego.");
        Console.ReadLine(); 
    }
}
