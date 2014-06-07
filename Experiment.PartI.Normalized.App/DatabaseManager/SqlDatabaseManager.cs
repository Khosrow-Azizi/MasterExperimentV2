using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.PartI.Normalized.App.DatabaseManager
{
   public class SqlDatabaseManager : DatabaseManagerBase
   {
      public SqlDatabaseManager(string connectionString)
      {
         this.connectionString = connectionString;
      }

      public override Dictionary<int, string> GetKeyAndNames<T>(float portion)
      {
         long amount = ConvertToCount<T>(portion);
         Dictionary<int, string> keyAndNames = new Dictionary<int, string>();
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            string query = string.Format(
               "SELECT TOP {0} Id, Name FROM [dbo].[{1}]",
               amount, typeof(T).Name);
            sqlConnection.Open();
            using (var rdr = new SqlCommand(query, sqlConnection).ExecuteReader())
            {
               if (rdr.HasRows)
               {
                  while (rdr.Read())
                  {
                     keyAndNames.Add(Convert.ToInt32(rdr["Id"]), Convert.ToString(rdr["Name"]));
                  }
               }
               rdr.Close();
            }
            sqlConnection.Close();
         }
         return keyAndNames;
      }

      public override int GetRandomKey<T>()
      {
         int key = -1;
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            string query = string.Format(
               "SELECT TOP 1 Id FROM [dbo].[{0}] ORDER BY NEWID()", typeof(T).Name);
            sqlConnection.Open();
            using (var rdr = new SqlCommand(query, sqlConnection).ExecuteReader())
            {
               if (rdr.HasRows)
               {
                  while (rdr.Read())
                  {
                     key = Convert.ToInt32(rdr["Id"]);
                  }
               }
               rdr.Close();
            }
            sqlConnection.Close();
         }
         return key;
      }

      public override string GetRandomName<T>()
      {
         string name = string.Empty;
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            string query = string.Format(
               "SELECT TOP 1 Name FROM [dbo].[{0}] ORDER BY NEWID()", typeof(T).Name);
            sqlConnection.Open();
            using (var rdr = new SqlCommand(query, sqlConnection).ExecuteReader())
            {
               if (rdr.HasRows)
               {
                  while (rdr.Read())
                  {
                     name = Convert.ToString(rdr["Name"]);
                  }
               }
               rdr.Close();
            }
            sqlConnection.Close();
         }
         return name;
      }

      public override string GetRandomUserFirstName()
      {
         string firstName = string.Empty;
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            string query = "SELECT TOP 1 FirstName FROM [dbo].[User] ORDER BY NEWID()";
            sqlConnection.Open();
            using (var rdr = new SqlCommand(query, sqlConnection).ExecuteReader())
            {
               if (rdr.HasRows)
               {
                  while (rdr.Read())
                  {
                     firstName = Convert.ToString(rdr["FirstName"]);
                  }
               }
               rdr.Close();
            }
            sqlConnection.Close();
         }
         return firstName;
      }

      public override int GetNewId<T>()
      {
         int id = 1;
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            string query = string.Format("SELECT Max(Id) AS MaxId FROM [dbo].[{0}]", typeof(T).Name);
            using (var rdr = new SqlCommand(query, sqlConnection).ExecuteReader())
            {
               if (rdr.HasRows)
               {
                  while (rdr.Read())
                  {
                     id = DBNull.Value.Equals(rdr["MaxId"]) ? id : Convert.ToInt32(rdr["MaxId"]) + 1;
                  }
               }
               rdr.Close();
            }
            sqlConnection.Close();
         }
         return id;
      }

      public override string GetLatestName<T>()
      {
         string name = string.Empty;
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            string query = string.Format("SELECT TOP 1 Name FROM [dbo].[{0}] ORDER BY Id DESC", typeof(T).Name);
            using (var rdr = new SqlCommand(query, sqlConnection).ExecuteReader())
            {
               if (rdr.HasRows)
               {
                  while (rdr.Read())
                  {
                     name = Convert.ToString(rdr["Name"]);
                  }
               }
               rdr.Close();
            }
            sqlConnection.Close();
         }
         return name;
      }

      public override long GetTotalCount<T>()
      {
         long count = 0;
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            string query = string.Format(
               "SELECT COUNT(*) AS Total From [dbo].[{0}]",
               typeof(T).Name);
            sqlConnection.Open();
            using (var rdr = new SqlCommand(query, sqlConnection).ExecuteReader())
            {
               if (rdr.HasRows)
               {
                  while (rdr.Read())
                  {
                     count = Convert.ToInt32(rdr["Total"]);
                  }
               }
               rdr.Close();
            }
            sqlConnection.Close();
         }
         return count;
      }

      public override void RestoreDatabase<T>(Dictionary<int, string> keyAndNames)
      {
         string query = "";
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            foreach (var item in keyAndNames)
            {
               query = string.Format("UPDATE [dbo].[{0}] SET Name = '{1}' WHERE Id = {2}",
               typeof(T).Name, item.Value, item.Key);
               var reader = new SqlCommand(query, sqlConnection).ExecuteReader();
               reader.Close();
               reader.Dispose();
            }
         }
      }

      public override void DeleteAll()
      {
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            var query = string.Format("{0} {1} {2} {3}",
               "Delete [dbo].[ProjectUser]",
               "Delete [dbo].[Project]",
               "Delete [dbo].[User]",
               "Delete [dbo].[Department]");
            new SqlCommand(query, sqlConnection).ExecuteNonQuery();
            sqlConnection.Close();
         }
      }

      public override int[] GetAllKeys<T>()
      {
         List<int> ids = new List<int>();
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            string query = string.Format("SELECT Id FROM [dbo].[{0}]", typeof(T).Name);
            using (var rdr = new SqlCommand(query, sqlConnection).ExecuteReader())
            {
               if (rdr.HasRows)
               {
                  while (rdr.Read())
                  {
                     ids.Add(Convert.ToInt32(rdr["Id"]));
                  }
               }
               rdr.Close();
            }
            sqlConnection.Close();
         }
         return ids.ToArray();
      }

      private readonly string connectionString;
   }
}
