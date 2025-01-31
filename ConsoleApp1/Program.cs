using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }  // Тривалість курсу в годинах
}

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }  // Дата народження
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
            Console.WriteLine("1. Додати курс\n2. Показати курси\n3. Оновити курс\n4. Видалити курс\n5. Додати студента\n6. Показати студентів\n7. Оновити студента\n8. Видалити студента\n9. Записати студента на курс\n10. Вийти");
            Console.Write("Оберіть опцію: ");
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
                    Console.WriteLine("Невірна опція. Спробуйте ще раз.");
                    break;
            }
        }
    }

    static void ShowAddCourseMenu(AppDbContext db)
    {
        Console.Clear();
        Console.WriteLine("Додати курс:");
        AddCourse(db);
        BackToMainMenu();
    }

    static void AddCourse(AppDbContext db)
    {
        Console.Write("Назва курсу: ");
        string name = Console.ReadLine();
        Console.Write("Опис курсу: ");
        string description = Console.ReadLine();
        Console.Write("Тривалість курсу (в годинах): ");
        int duration = int.Parse(Console.ReadLine());

        db.Courses.Add(new Course { Name = name, Description = description, Duration = duration });
        db.SaveChanges();
        Console.WriteLine("Курс додано!\n");
    }

    static void ShowCourses(AppDbContext db)
    {
        Console.Clear();
        var courses = db.Courses.ToList();
        if (courses.Any())
        {
            Console.WriteLine("Список курсів:");
            foreach (var course in courses)
            {
                Console.WriteLine($"{course.Id}: {course.Name} - {course.Description}, Тривалість: {course.Duration} годин");
            }
        }
        else
        {
            Console.WriteLine("Курси не знайдені.");
        }
        BackToMainMenu();
    }

    static void UpdateCourse(AppDbContext db)
    {
        Console.Clear();
        ShowCourses(db); // Спочатку показуємо список курсів

        Console.Write("Введіть ID курсу для редагування: ");
        int courseId = int.Parse(Console.ReadLine());
        var course = db.Courses.Find(courseId);

        if (course != null)
        {
            Console.Write("Нова назва курсу: ");
            course.Name = Console.ReadLine();
            Console.Write("Новий опис курсу: ");
            course.Description = Console.ReadLine();
            Console.Write("Нова тривалість курсу (в годинах): ");
            course.Duration = int.Parse(Console.ReadLine());

            db.SaveChanges();
            Console.WriteLine("Курс оновлено!\n");
        }
        else
        {
            Console.WriteLine("Курс не знайдений.");
        }
        BackToMainMenu();
    }

    static void DeleteCourse(AppDbContext db)
    {
        Console.Clear();
        ShowCourses(db); // Спочатку показуємо список курсів

        Console.Write("Введіть ID курсу для видалення: ");
        int courseId = int.Parse(Console.ReadLine());
        var course = db.Courses.Find(courseId);

        if (course != null)
        {
            db.Courses.Remove(course);
            db.SaveChanges();
            Console.WriteLine("Курс видалено!\n");
        }
        else
        {
            Console.WriteLine("Курс не знайдений.");
        }
        BackToMainMenu();
    }

    static void ShowAddStudentMenu(AppDbContext db)
    {
        Console.Clear();
        Console.WriteLine("Додати студента:");
        AddStudent(db);
        BackToMainMenu();
    }

    static void AddStudent(AppDbContext db)
    {
        Console.Write("Ім'я студента: ");
        string name = Console.ReadLine();
        Console.Write("Email студента: ");
        string email = Console.ReadLine();
        Console.Write("Дата народження (yyyy-mm-dd): ");
        DateTime birthDate = DateTime.Parse(Console.ReadLine());

        db.Students.Add(new Student { Name = name, Email = email, BirthDate = birthDate });
        db.SaveChanges();
        Console.WriteLine("Студента додано!\n");
    }

    static void ShowStudents(AppDbContext db)
    {
        Console.Clear();
        var students = db.Students.Include(s => s.Courses).ToList();
        if (students.Any())
        {
            Console.WriteLine("Список студентів:");
            foreach (var student in students)
            {
                Console.WriteLine($"{student.Id}: {student.Name} - {student.Email}, Дата народження: {student.BirthDate.ToShortDateString()}");

                // Виведення курсів, на які записаний студент
                if (student.Courses.Any())
                {
                    Console.WriteLine("Записані курси:");
                    foreach (var course in student.Courses)
                    {
                        Console.WriteLine($"- {course.Name}: {course.Description}, Тривалість: {course.Duration} годин");
                    }
                }
                else
                {
                    Console.WriteLine("Студент не записаний на жоден курс.");
                }

                Console.WriteLine(); // Розрив між студентами
            }
        }
        else
        {
            Console.WriteLine("Студенти не знайдені.");
        }
        BackToMainMenu();
    }

    static void UpdateStudent(AppDbContext db)
    {
        Console.Clear();
        ShowStudents(db); // Спочатку показуємо список студентів

        Console.Write("Введіть ID студента для редагування: ");
        int studentId = int.Parse(Console.ReadLine());
        var student = db.Students.Find(studentId);

        if (student != null)
        {
            Console.Write("Нове ім'я студента: ");
            student.Name = Console.ReadLine();
            Console.Write("Новий Email студента: ");
            student.Email = Console.ReadLine();
            Console.Write("Нова дата народження (yyyy-mm-dd): ");
            student.BirthDate = DateTime.Parse(Console.ReadLine());

            db.SaveChanges();
            Console.WriteLine("Дані студента оновлено!\n");
        }
        else
        {
            Console.WriteLine("Студент не знайдений.");
        }
        BackToMainMenu();
    }

    static void DeleteStudent(AppDbContext db)
    {
        Console.Clear();
        ShowStudents(db); // Спочатку показуємо список студентів

        Console.Write("Введіть ID студента для видалення: ");
        int studentId = int.Parse(Console.ReadLine());
        var student = db.Students.Find(studentId);

        if (student != null)
        {
            db.Students.Remove(student);
            db.SaveChanges();
            Console.WriteLine("Студента видалено!\n");
        }
        else
        {
            Console.WriteLine("Студент не знайдений.");
        }
        BackToMainMenu();
    }

    static void EnrollStudent(AppDbContext db)
    {
        Console.Clear();
        ShowCourses(db); // Спочатку показуємо список курсів
        Console.Write("Введіть ID курсу: ");
        int courseId = int.Parse(Console.ReadLine());

        Console.Write("Введіть Email студента: ");
        string email = Console.ReadLine();
        var student = db.Students.Include(s => s.Courses).FirstOrDefault(s => s.Email == email);
        var course = db.Courses.Find(courseId);

        if (student == null)
        {
            Console.WriteLine($"Студент з Email {email} не знайдений!\n");
        }

        if (course == null)
        {
            Console.WriteLine($"Курс з ID {courseId} не знайдений!\n");
        }

        if (student != null && course != null)
        {
            if (!student.Courses.Contains(course))
            {
                student.Courses.Add(course);
                db.SaveChanges();
                Console.WriteLine("Студент записаний на курс!\n");
            }
            else
            {
                Console.WriteLine("Студент вже записаний на цей курс!\n");
            }
        }
        BackToMainMenu();
    }

    // Метод для повернення до головного меню
    static void BackToMainMenu()
    {
        Console.WriteLine("\nНатисніть 'Enter' для повернення до головного меню.");
        Console.ReadLine(); // Очікуємо натискання клавіші для повернення до основного меню
    }
}
