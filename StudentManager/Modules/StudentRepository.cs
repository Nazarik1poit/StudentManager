using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using StudentManager;

namespace StudentManager.Modules
{
    internal class StudentRepository
    {
        private readonly IMongoCollection<Student> _students;

        public StudentRepository(MongoDbContext dbContext)
        {
            _students = dbContext.Students;
        }

        public Program Program
        {
            get => default;
            set
            {
            }
        }

        // Метод для загрузки всех студентов из базы данных
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _students.Find(student => true).ToListAsync();
        }

        // Метод для добавления нового студента
        public async Task AddStudentAsync(Student student)
        {
            await _students.InsertOneAsync(student);
        }

        // Метод для обновления информации о студенте
        public async Task UpdateStudentAsync(Student student)
        {
            var filter = Builders<Student>.Filter.Eq(s => s.Id, student.Id);
            await _students.ReplaceOneAsync(filter, student);
        }

        // Метод для получения студента по ID
        public async Task<Student> GetStudentByIdAsync(int id)
        {
            return await _students.Find(student => student.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Student>> FilterStudentsAsync(string field, string value)
        {
            var filterBuilder = Builders<Student>.Filter;
            FilterDefinition<Student> filter = field switch
            {
                "Имя" => filterBuilder.Eq(s => s.Name, value),
                "Курс" => filterBuilder.Eq(s => s.Course, int.Parse(value)),
                "Группа" => filterBuilder.Eq(s => s.Group, value),
                "Возраст" => filterBuilder.Eq(s => s.Age, int.Parse(value)),
                "ID" => filterBuilder.Eq(s => s.Id, int.Parse(value)),
                _ => FilterDefinition<Student>.Empty
            };

            return await _students.Find(filter).ToListAsync();
        }

        public async Task<List<Student>> SortStudentsAsync(string field)
        {
            SortDefinition<Student> sort = field switch
            {
                "Имя" => Builders<Student>.Sort.Ascending(s => s.Name),
                "Курс" => Builders<Student>.Sort.Ascending(s => s.Course),
                "Группа" => Builders<Student>.Sort.Ascending(s => s.Group),
                "Возраст" => Builders<Student>.Sort.Ascending(s => s.Age),
                "ID" => Builders<Student>.Sort.Ascending(s => s.Id),
                _ => Builders<Student>.Sort.Ascending(s => s.Id)
            };

            return await _students.Find(FilterDefinition<Student>.Empty).Sort(sort).ToListAsync();
        }
    }
}
