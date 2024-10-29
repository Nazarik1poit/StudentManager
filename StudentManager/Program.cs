using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManager.Modules;

namespace StudentManager
{
    public class Program
    {
        private static StudentRepository _repository;
        static string sortField = "Id"; // Поле для сортировки по умолчанию
        static int itemsPerPage = 20;     // Количество студентов на странице
        static int currentPage = 1;      // Текущая страница

        static async Task Main()
        {
            string connectionString = "mongodb+srv://burdykonazar:MYgpe21hePQFxp3w@students.7lfps.mongodb.net/?retryWrites=true&w=majority&appName=Students";
            string databaseName = "College";

            var dbContext = new MongoDbContext(connectionString, databaseName);
            _repository = new StudentRepository(dbContext);

            string[] menuItems = { "Список студентов", "Сортировать", "Фильтр", "Редактировать", "Добавить", "Выход" };
            int selectedIndex = 0;
            ConsoleKey key;
            List<Student> students = await _repository.GetAllStudentsAsync();

            do
            {
                Console.Clear();

                // Отображаем меню в одну строку
                for (int i = 0; i < menuItems.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[{0}] ", menuItems[i]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(" {0}  ", menuItems[i]);
                    }
                }

                if (menuItems[selectedIndex] == "Список студентов")
                {
                    var sortedStudents = SortStudents(students, sortField);

                    // Расчёт количества страниц
                    int totalPages = (int)Math.Ceiling((double)sortedStudents.Count / itemsPerPage);
                    currentPage = Math.Max(1, Math.Min(currentPage, totalPages)); // Ограничение на диапазон страниц

                    // Получение студентов для текущей страницы
                    var paginatedStudents = sortedStudents
                        .Skip((currentPage - 1) * itemsPerPage)
                        .Take(itemsPerPage)
                        .ToList();

                    Console.WriteLine($"\nСписок студентов (Страница {currentPage}/{totalPages}):");

                    foreach (var student in paginatedStudents)
                    {
                        student.Print();
                    }

                    Console.WriteLine("\nИспользуйте стрелки вверх/вниз для навигации по страницам.");
                }

                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.RightArrow)
                {
                    selectedIndex = (selectedIndex + 1) % menuItems.Length;
                }
                else if (key == ConsoleKey.LeftArrow)
                {
                    selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;
                }
                else if (menuItems[selectedIndex] == "Список студентов")
                {
                    // Обработка перехода по страницам
                    if (key == ConsoleKey.UpArrow && currentPage > 1)
                    {
                        currentPage--;
                    }
                    else if (key == ConsoleKey.DownArrow && currentPage < (int)Math.Ceiling((double)students.Count / itemsPerPage))
                    {
                        currentPage++;
                    }
                    else if (key == ConsoleKey.Enter)
                    {
                        continue;
                    }
                }
                else if (key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    switch (menuItems[selectedIndex])
                    {
                        case "Список студентов":
                            students = await _repository.GetAllStudentsAsync();
                            currentPage = 1; // Сброс на первую страницу
                            Console.WriteLine("Отображение списка студентов...");
                            break;

                        case "Редактировать":
                            Console.Write("Введите ID студента для редактирования: ");
                            if (int.TryParse(Console.ReadLine(), out int id))
                            {
                                var student = students.FirstOrDefault(s => s.Id == id);
                                if (student != null)
                                {
                                    Console.Write("Новое имя (текущ.: {0}): ", student.Name);
                                    student.Name = Console.ReadLine();

                                    Console.Write("Новый курс (текущ.: {0}): ", student.Course);
                                    student.Course = int.Parse(Console.ReadLine());

                                    Console.Write("Новая группа (текущ.: {0}): ", student.Group);
                                    student.Group = Console.ReadLine();

                                    Console.Write("Новый возраст (текущ.: {0}): ", student.Age);
                                    student.Age = int.Parse(Console.ReadLine());

                                    await _repository.UpdateStudentAsync(student);
                                    Console.WriteLine("Студент успешно обновлен!");
                                }
                                else
                                {
                                    Console.WriteLine("Студент с таким ID не найден.");
                                }
                            }
                            break;

                        case "Добавить":
                            Console.Write("Введите имя: ");
                            string name = Console.ReadLine();
                            Console.Write("Введите курс: ");
                            int course = int.Parse(Console.ReadLine());
                            Console.Write("Введите группу: ");
                            string group = Console.ReadLine();
                            Console.Write("Введите возраст: ");
                            int age = int.Parse(Console.ReadLine());

                            int newid = students.Count > 0 ? students.Max(s => s.Id) + 1 : 1;

                            var newStudent = new Student(name, course, group, age, newid);
                            await _repository.AddStudentAsync(newStudent);
                            Console.WriteLine("Студент успешно добавлен!");
                            students = await _repository.GetAllStudentsAsync();
                            break;

                        case "Сортировать":
                            Console.WriteLine("Выберите поле для сортировки:");
                            Console.WriteLine("1. Имя");
                            Console.WriteLine("2. Курс");
                            Console.WriteLine("3. Группа");
                            Console.WriteLine("4. Возраст");
                            Console.WriteLine("5. ID");

                            ConsoleKey sortKey = Console.ReadKey().Key;
                            switch (sortKey)
                            {
                                case ConsoleKey.D1: sortField = "Name"; break;
                                case ConsoleKey.D2: sortField = "Course"; break;
                                case ConsoleKey.D3: sortField = "Group"; break;
                                case ConsoleKey.D4: sortField = "Age"; break;
                                case ConsoleKey.D5: sortField = "Id"; break;
                                default:
                                    Console.WriteLine("\nНеверный выбор. Сортировка отменена.");
                                    break;
                            }
                            break;

                        case "Фильтр":
                            Console.WriteLine("Выберите поле для фильтрации:");
                            Console.WriteLine("1. Имя");
                            Console.WriteLine("2. Курс");
                            Console.WriteLine("3. Группа");
                            Console.WriteLine("4. Возраст");
                            Console.WriteLine("5. ID");

                            ConsoleKey filterKey = Console.ReadKey().Key;
                            string filterField = filterKey switch
                            {
                                ConsoleKey.D1 => "Имя",
                                ConsoleKey.D2 => "Курс",
                                ConsoleKey.D3 => "Группа",
                                ConsoleKey.D4 => "Возраст",
                                ConsoleKey.D5 => "ID",
                                _ => null
                            };

                            if (filterField != null)
                            {
                                Console.Write($"\nВведите значение для фильтрации по {filterField}: ");
                                string filterValue = Console.ReadLine();

                                var filteredStudents = await _repository.FilterStudentsAsync(filterField, filterValue);
                                if (filteredStudents.Any())
                                {
                                    Console.WriteLine("\nРезультаты фильтрации:");
                                    foreach (var student in filteredStudents)
                                    {
                                        student.Print();
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Нет студентов, соответствующих критериям фильтрации.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Неверный выбор. Фильтрация отменена.");
                            }
                            break;

                        case "Выход":
                            Environment.Exit(0);
                            break;
                    }

                    Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
                    Console.ReadKey();
                }
            }
            while (true);
        }

        static List<Student> SortStudents(List<Student> students, string sortField)
        {
            return sortField switch
            {
                "Name" => students.OrderBy(s => s.Name).ToList(),
                "Course" => students.OrderBy(s => s.Course).ToList(),
                "Group" => students.OrderBy(s => s.Group).ToList(),
                "Age" => students.OrderBy(s => s.Age).ToList(),
                "Id" => students.OrderBy(s => s.Id).ToList(),
                _ => students
            };
        }
    }
}
