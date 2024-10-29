using MongoDB.Driver;
using StudentManager.Modules;

namespace StudentManager
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Student> Students
        {
            get { return _database.GetCollection<Student>("students"); }
        }

        internal StudentRepository StudentRepository
        {
            get => default;
            set
            {
            }
        }

        public Program Program
        {
            get => default;
            set
            {
            }
        }
    }
}
