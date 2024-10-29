using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using StudentManager;

namespace StudentManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Путь к SQL файлу с mock данными
            string filePath = "D:/MOCK_DATA.sql";

            // Инициализация MongoDB
            var connectionString = "mongodb+srv://burdykonazar:MYgpe21hePQFxp3w@students.7lfps.mongodb.net/?retryWrites=true&w=majority&appName=Students";
            var databaseName = "College";
            var context = new MongoDbContext(connectionString, databaseName);

            // Парсим студентов и добавляем их в MongoDB
            var students = ParseStudentsFromFile(filePath);
            AddStudentsToMongoDb(context, students);

            Console.WriteLine("Студенты успешно добавлены в MongoDB.");
        }

        public static List<Student> ParseStudentsFromFile(string filePath)
        {
            var students = new List<Student>();
            var insertRegex = new Regex(@"insert into MOCK_DATA .* values \((\d+), '([^']+)', (\d+), '([^']+)', (\d+)\);", RegexOptions.IgnoreCase);

            foreach (var line in File.ReadLines(filePath))
            {
                var match = insertRegex.Match(line);
                if (match.Success)
                {
                    var student = new Student
                    {
                        Id = int.Parse(match.Groups[1].Value),
                        Name = match.Groups[2].Value,
                        Course = int.Parse(match.Groups[3].Value),
                        Group = match.Groups[4].Value,
                        Age = int.Parse(match.Groups[5].Value)
                    };
                    students.Add(student);
                }
            }

            return students;
        }

        public static void AddStudentsToMongoDb(MongoDbContext context, List<Student> students)
        {
            var studentCollection = context.Students;
            studentCollection.InsertMany(students);
        }
    }

    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Student> Students => _database.GetCollection<Student>("students");
    }
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Course { get; set; }
        public string Group { get; set; }
        public int Age { get; set; }
    }
}

