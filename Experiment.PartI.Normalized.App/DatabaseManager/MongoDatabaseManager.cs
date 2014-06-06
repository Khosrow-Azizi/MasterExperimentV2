using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.PartI.Normalized.App.DataModel;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using Experiment.PartI.Normalized.App.Shared;

namespace Experiment.PartI.Normalized.App.DatabaseManager
{
   public class MongoDatabaseManager : DatabaseManagerBase
   {
      public override Dictionary<int, string> GetKeyAndNames<T>(float portion)
      {
         return GetCollection<T>().FindAll()
            .SetLimit((int)ConvertToCount<T>(portion))
            .Select(e => new KeyValuePair<int, string>(e.Id, e.Name)).ToDictionary(k => k.Key, v => v.Value);
      }

      public override long GetTotalCount<T>()
      {
         return GetCollection<T>().FindAll().Count();
      }

      public override int GetRandomKey<T>()
      {
         int key = -1;
         var collection = GetCollection<T>();
         if (collection.FindOne() != null)
         {
            Random randGen = new Random();
            key = randGen.Next(collection.FindAll().Min(e => e.Id), collection.FindAll().Max(e => e.Id));
         }
         return key;
      }

      public override string GetRandomName<T>()
      {
         string name = string.Empty;
         var collection = GetCollection<T>();
         T entity = collection.FindOne(Query<T>.EQ(e => e.Id, GetRandomKey<T>()));
         if (entity != null)
            name = entity.Name;
         return name;
      }

      public override string GetRandomUserFirstName()
      {
         string name = string.Empty;
         var collection = GetCollection<User>();
         User entity = collection.FindOne(Query<User>.EQ(e => e.Id, GetRandomKey<User>()));
         if (entity != null)
            name = entity.FirstName;
         return name;
      }

      public override int GetNewId<T>()
      {
         var collection = GetCollection<T>().AsQueryable<T>();
         if (collection.Any())
            return collection.Max(e => e.Id) + 1;
         return 1;
      }

      public override string GetLatestName<T>()
      {
         var entity = GetCollection<T>().FindAll()
            .SetSortOrder(SortBy.Descending("_id")).FirstOrDefault();
         return entity == null ? string.Empty : entity.Name;
      }

      private MongoCollection<T> GetCollection<T>() where T : PartIRootClass
      {
         MongoServer server = new MongoClient(Configuration.MongoConnectionString).GetServer();
         MongoDatabase database = server.GetDatabase(Configuration.DatabaseName);
         return database.GetCollection<T>(typeof(T).Name);
      }

      public override void RestoreDatabase<T>(Dictionary<int, string> keyAndNames)
      {
         var collection = GetCollection<T>();
         foreach (var item in keyAndNames)
         {
            IMongoQuery query = Query<T>.EQ(e => e.Id, item.Key);
            IMongoUpdate command = Update<T>.Set(e => e.Name, item.Value);
            collection.Update(query, command);
         }
      }

      public override void FlushDatabase()
      {
         GetCollection<Project>().Drop();
         GetCollection<User>().Drop();
         GetCollection<Department>().Drop();
      }

      public override int[] GetAllKeys<T>()
      {
         return GetCollection<T>().FindAll().Select(e => e.Id).ToArray();
      }
   }
}
