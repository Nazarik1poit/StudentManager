using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StudentManager.Modules
{
    public class Student
    {
        private string name;
        private string group;
        private int course;
        private int age;
        private int id;

        public string Name { get { return name; } set { name = value; } }
        public int Id { get { return id; } set { id = value; } }
        public int Course { get { return course; } set { course = value; } }

        public string Group { get { return group; } set { group = value; } }
        public int Age { get { return age; } set { age = value; } }

        public Program Program
        {
            get => default;
            set
            {
            }
        }

        public Student(string name, int course, string group, int age, int id)
        {
            this.name = name;
            this.course = course;
            this.group = group;
            this.age = age;
            this.id = id;
        }

        public void Print()
        {
            Console.Write($"|ID: {this.Id} | Имя: {this.name} | Возраст: {this.Age} | Группа: {this.group} | Курс: {this.Course}|");
            Console.WriteLine();
        }
    }
}
