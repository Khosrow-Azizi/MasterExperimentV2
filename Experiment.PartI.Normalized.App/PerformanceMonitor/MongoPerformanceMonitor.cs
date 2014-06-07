using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.PartI.Normalized.App.DataModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Experiment.PartI.Normalized.App.PerformanceMonitor
{
   public class MongoPerformanceMonitor : IDatabasePerformanceMonitor
   {
      public MongoPerformanceMonitor(string connectionString, string databaseName)
      {
         if (!localDateTimeRegistered)
         {
            DateTimeSerializationOptions options = DateTimeSerializationOptions.LocalInstance;
            var serializer = new DateTimeSerializer(options);
            BsonSerializer.RegisterSerializer(typeof(DateTime), serializer);
            localDateTimeRegistered = true;
         }
         this.connectionString = connectionString;
         this.databaseName = databaseName;
         this.insertOptions = new MongoInsertOptions { WriteConcern = WriteConcern.Acknowledged };
         this.updateOptions = new MongoUpdateOptions { Flags = UpdateFlags.Multi, WriteConcern = WriteConcern.Acknowledged };
      }

      public double Insert(Department department, Stopwatch stopwatch)
      {
         var collection = GetCollection<Department>();
         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         collection.Insert<Department>(department, insertOptions);
         stopwatch.Stop();
         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      public double Insert(Project project, Stopwatch stopwatch)
      {
         var collection = GetCollection<Project>();
         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         collection.Insert<Project>(project, insertOptions);
         stopwatch.Stop();
         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      public double Insert(User user, Stopwatch stopwatch)
      {
         var collection = GetCollection<User>();
         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         collection.Insert<User>(user, insertOptions);
         stopwatch.Stop();
         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      public double UpdateDepartmentNameByKeys(int[] keys, string newName, Stopwatch stopwatch)
      {
         var collection = GetCollection<Department>();
         IMongoQuery query = Query<Department>.In(e => e.Id, keys);
         IMongoUpdate command = Update<Department>.Set(e => e.Name, newName);

         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         collection.Update(query, command, updateOptions);
         stopwatch.Stop();
         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      public double UpdateProjectNameByKeys(int[] keys, string newName, Stopwatch stopwatch)
      {
         var collection = GetCollection<Project>();
         IMongoQuery query = Query<Project>.In(e => e.Id, keys);
         IMongoUpdate command = Update<Project>.Set(e => e.Name, newName);

         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         collection.Update(query, command, updateOptions);
         stopwatch.Stop();
         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      public double UpdateUserLastName(string firstNamePattern, string newLastName, Stopwatch stopwatch)
      {
         var collection = GetCollection<User>();
         IMongoQuery query = Query<User>.EQ(e => e.FirstName, firstNamePattern);
         IMongoUpdate command = Update<User>.Set(e => e.LastName, newLastName);

         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         collection.Update(query, command, updateOptions);
         stopwatch.Stop();
         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      public double SelectDepartmentByKey(int key, Stopwatch stopwatch)
      {
         var collection = GetCollection<Department>();
         IMongoQuery query = Query<Department>.EQ(e => e.Id, key);

         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         Department department = collection.Find(query).FirstOrDefault();
         stopwatch.Stop();
         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      public double SelectUserByKey(int key, Stopwatch stopwatch)
      {
         var collection = GetCollection<User>();
         IMongoQuery query = Query<User>.EQ(e => e.Id, key);

         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         User user = collection.Find(query).FirstOrDefault();
         stopwatch.Stop();
         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      public double SelectDepartmentByName(string name, Stopwatch stopwatch)
      {
         var collection = GetCollection<Department>();
         IMongoQuery query = Query<Department>.EQ(e => e.Name, name);

         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         Department department = collection.Find(query).FirstOrDefault();
         stopwatch.Stop();
         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      public double SelectUsersByFirstName(string firstName, Stopwatch stopwatch)
      {
         var collection = GetCollection<User>();
         IMongoQuery query = Query<User>.EQ(e => e.FirstName, firstName);

         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         IEnumerable<User> users = collection.Find(query).ToArray();
         stopwatch.Stop();
         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      public double SelectDepartmentsByUser(string userFirstName, Stopwatch stopwatch)
      {
         var depCollection = GetCollection<Department>();
         var userCollection = GetCollection<User>();

         IMongoQuery userQuery = Query<User>.EQ(u => u.FirstName, userFirstName);
         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         var departmentIds = userCollection.Find(userQuery).Select(u => u.DepartmentId);
         stopwatch.Stop();

         IMongoQuery depQuery = Query<Department>.In(d => d.Id, departmentIds);
         stopwatch.Start();
         IEnumerable<Department> departments = depCollection.Find(depQuery).ToArray();
         stopwatch.Stop();

         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      public double SelectUsersByProjects(int[] projectKeys, Stopwatch stopwatch)
      {
         var userCollection = GetCollection<User>();
         var projectCollection = GetCollection<Project>();

         IMongoQuery projectQuery = Query<Project>.In(p => p.Id, projectKeys);
         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         var projectUserIds = projectCollection.Find(projectQuery).SelectMany(p => p.Users);
         stopwatch.Stop();

         IMongoQuery userQuery = Query<User>.In(u => u.Id, projectUserIds);
         stopwatch.Start();
         IEnumerable<User> users = userCollection.Find(userQuery).ToArray();
         stopwatch.Stop();

         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      public double SelectAverageAgeByProjects(int[] projectKeys, Stopwatch stopwatch)
      {
         var userCollection = GetCollection<User>();
         var projectCollection = GetCollection<Project>();
         IMongoQuery projectQuery = Query<Project>.In(p => p.Id, projectKeys);

         var initialTime = stopwatch.Elapsed.TotalMilliseconds;
         stopwatch.Start();
         var aggResult = userCollection.Aggregate(new AggregateArgs
         {
            Pipeline = new BsonDocument[]
               {
                  new BsonDocument //match stage 
                  { 
                     {
                        "$match", new BsonDocument 
                           { { "_id", new BsonDocument 
                              { { "$in", new BsonArray(projectCollection.Find(projectQuery).SelectMany(p => p.Users)) } } } }
                     } 
                  },
                  new BsonDocument //aggregation stage
                  { 
                     { 
                        "$group", new BsonDocument 
                                 { 
                                    { "_id", 0 }, 
                                    { "AverageAge", new BsonDocument { {"$avg", "$Age"} } } 
                                 }
                     }  
                  }
               },
            OutputMode = AggregateOutputMode.Inline,
         });
         stopwatch.Stop();
         var executionTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
         return executionTime;
      }

      private MongoCollection<T> GetCollection<T>() where T : PartIRootClass
      {
         MongoServer server = new MongoClient(connectionString).GetServer();
         MongoDatabase database = server.GetDatabase(databaseName);
         return database.GetCollection<T>(typeof(T).Name);
      }

      private readonly string connectionString;
      private readonly string databaseName;
      private readonly MongoInsertOptions insertOptions;
      private readonly MongoUpdateOptions updateOptions;
      private static bool localDateTimeRegistered;
   }
}
