using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoDBSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine("Hello World!");
            MongoCRUD db = new MongoCRUD("MicroServiceLearningPath");
           
            //Insert Record
            db.InsertRecord("Staff", new StaffDetail { FirstName = "Chijioke", LastName = "FCMB", Age=120 });

            //Insert Record
            db.InsertRecord("AnotherStaff", new NameModel { FirstName = "Chijioke", LastName = "FCMB" });


            //Read Record
            var record = db.LoadRecords<StaffDetail>("Staff");
            foreach (var rec in record)
            {
                var oneRecord = db.LoadRecordById<StaffDetail>("Staff", rec.Id);
                Console.WriteLine($"{oneRecord}");
                Console.WriteLine($"{rec.Id}: {rec.FirstName} {rec.LastName} {rec.Age}");
                Console.ReadLine();
             }

           // Update Record
             var recordToUpdate = db.LoadRecords<StaffDetail>("Staff");
            foreach (var rec in recordToUpdate)
            {
                var oneRecordToUpdate = db.LoadRecordById<StaffDetail>("Staff", rec.Id);
                oneRecordToUpdate.Age = 200;
                db.UpdateRecord("Staff", oneRecordToUpdate.Id, oneRecordToUpdate);
            }

            //Delete Record
            var recordToDelete = db.LoadRecords<StaffDetail>("Staff");
            foreach (var rec in recordToDelete)
            {
                if (rec.LastName.ToLower().Equals("fcmb"))
                {
                    db.DeleteRecord<StaffDetail>("Staff", rec.Id);
                }
               
            }

            Console.ReadLine();
        }
    }
    public class StaffDetail
    {
        [BsonId]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public class NameModel
    {
        [BsonId]//stores as id field-unique identifier
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        [BsonElement("MyLastName")]
        public string LastName { get; set; }
    }
    //Helper Class
    public class MongoCRUD
        {
        // Connection to Database
        private IMongoDatabase db;

        //constructor
        public MongoCRUD(string database)
        {
            // create a new client to be used with MongoDB
            var client = new MongoClient();
            db = client.GetDatabase(database);
        }
        //Insert Record
        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }

        //Read All Record
        public List<T> LoadRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return collection.Find(new BsonDocument()).ToList();
        }

      

        // Read One Record
        public T LoadRecordById<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            return collection.Find(filter).First();
        }

        //Update Record
        public void UpdateRecord<T>(string table, Guid id, T record)
        {
            var collection = db.GetCollection<T>(table);

            var result = collection.ReplaceOne(
                new BsonDocument("_id", id),
                record,
                new UpdateOptions { IsUpsert = true }
                );
        }

        //Delete record
        public void DeleteRecord<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }
    }
}
