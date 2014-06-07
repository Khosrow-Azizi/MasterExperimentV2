using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.PartI.Normalized.App.DataModel;

namespace Experiment.PartI.Normalized.App.PerformanceMonitor
{
   public class SqlCommandParamQueryPerformanceMonitor : IDatabasePerformanceMonitor
   {
      public SqlCommandParamQueryPerformanceMonitor(string connectionString)
      {
         this.connectionString = connectionString;
      }

      public double Insert(Department department, Stopwatch stopwatch)
      {
         double execTime;
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;
               command.CommandText = "INSERT INTO [dbo].[Department] (Id, Name, DateAdded) VALUES (@id, @name, @dateAdd)";
               command.Parameters.AddWithValue("@id", department.Id);
               command.Parameters.AddWithValue("@name", department.Name);
               command.Parameters.AddWithValue("@dateAdd", department.DateAdded);

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               command.ExecuteNonQuery();
               stopwatch.Stop();
               execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      public double Insert(Project project, Stopwatch stopwatch)
      {
         double execTime;
         int[] userIds = project.Users.ToArray();
         List<string> paramTuples = new List<string>();
         StringBuilder sb = new StringBuilder();
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;
               // build up insert query for project
               sb.Append("INSERT INTO [dbo].[Project] (Id, Name, DateAdded, ManagerId, DepartmentId) VALUES (@id, @name, @dateAdd, @manId, @depId) ");
               command.Parameters.AddWithValue("@id", project.Id);
               command.Parameters.AddWithValue("@name", project.Name);
               command.Parameters.AddWithValue("@dateAdd", project.DateAdded);
               command.Parameters.AddWithValue("@manId", project.ManagerId);
               command.Parameters.AddWithValue("@depId", project.DepartmentId);
               // build up insert query for ProjectUser
               sb.Append("INSERT INTO [dbo].[ProjectUser] (ProjectId, UserId) VALUES ");
               for (int i = 0; i < userIds.Length; i++)
               {
                  string userIdParam = "@uId" + i;
                  paramTuples.Add(string.Format("(@id, {0})", userIdParam));
                  command.Parameters.AddWithValue(userIdParam, userIds[i]);
               }
               sb.Append(string.Join(", ", paramTuples));
               command.CommandText = sb.ToString();

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               command.ExecuteNonQuery();
               stopwatch.Stop();
               execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      public double Insert(User user, Stopwatch stopwatch)
      {
         double execTime;
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;
               command.CommandText = "INSERT INTO [dbo].[User] (Id, FirstName, DateAdded, LastName, Age, DepartmentId) VALUES (@id, @firstN, @dateAdd, @lastN, @ag, @depId)";
               command.Parameters.AddWithValue("@id", user.Id);
               command.Parameters.AddWithValue("@firstN", user.FirstName);
               command.Parameters.AddWithValue("@dateAdd", user.DateAdded);
               command.Parameters.AddWithValue("@lastN", user.LastName);
               command.Parameters.AddWithValue("@ag", user.Age);
               command.Parameters.AddWithValue("@depId", user.DepartmentId);

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               command.ExecuteNonQuery();
               stopwatch.Stop();
               execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      public double UpdateDepartmentNameByKeys(int[] keys, string newName, Stopwatch stopwatch)
      {
         if (!keys.Any())
            throw new ArgumentException("Keys array should contain at least one item.");
         double execTime;
         List<string> paramIndexes = new List<string>();
         int keysCount = keys.Length;
         StringBuilder sb = new StringBuilder();
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;
               sb.Append("UPDATE [dbo].[Department] SET Name = @name WHERE Id IN (");
               command.Parameters.AddWithValue("@name", newName);
               for (int i = 0; i < keysCount; i++)
               {
                  string paramIndex = "@p" + i;
                  paramIndexes.Add(paramIndex);
                  command.Parameters.AddWithValue(paramIndex, keys[i]);
               }
               sb.Append(string.Join(", ", paramIndexes) + ")");
               command.CommandText = sb.ToString();

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               command.ExecuteNonQuery();
               stopwatch.Stop();
               execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      public double UpdateProjectNameByKeys(int[] keys, string newName, Stopwatch stopwatch)
      {
         double execTime;
         List<string> paramNames = new List<string>();
         int keysCount = keys.Length;
         StringBuilder sb = new StringBuilder();
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;
               sb.Append("UPDATE [dbo].[Project] SET Name = @name WHERE Id IN (");
               command.Parameters.AddWithValue("@name", newName);
               for (int i = 0; i < keysCount; i++)
               {
                  string paramIndex = "@p" + i;
                  paramNames.Add(paramIndex);
                  command.Parameters.AddWithValue(paramIndex, keys[i]);
               }
               sb.Append(string.Join(", ", paramNames) + ")");
               command.CommandText = sb.ToString();

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               command.ExecuteNonQuery();
               stopwatch.Stop();
               execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      public double UpdateUserLastName(string firstNamePattern, string newLastName, Stopwatch stopwatch)
      {
         double execTime;
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;
               command.CommandText = "UPDATE [dbo].[User] SET Lastname = @newLastName WHERE FirstName LIKE @firstNamePattern";
               command.Parameters.AddWithValue("@newLastName", newLastName);
               command.Parameters.AddWithValue("@firstNamePattern", firstNamePattern);

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               command.ExecuteNonQuery();
               stopwatch.Stop();
               execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      public double SelectDepartmentByKey(int key, Stopwatch stopwatch)
      {
         double execTime;
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;
               command.CommandText = "SELECT Id, Name, DateAdded FROM [dbo].[Department] WHERE Id = @id";
               command.Parameters.AddWithValue("@id", key);

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               using (SqlDataReader rdr = command.ExecuteReader())
               {
                  if (rdr.HasRows)
                  {
                     while (rdr.Read())
                     {
                        Department department = new Department
                        {
                           Id = rdr.GetInt32(0),
                           Name = rdr.GetString(1),
                           DateAdded = rdr.GetDateTime(2),
                        };
                     }
                  }
                  stopwatch.Stop();
                  execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
                  rdr.Close();
               }
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      public double SelectUserByKey(int key, Stopwatch stopwatch)
      {
         double execTime;
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;
               command.CommandText = "SELECT Id, FirstName, LastName, Age, DepartmentId, DateAdded FROM [dbo].[User] WHERE Id = @id";
               command.Parameters.AddWithValue("@id", key);

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               using (SqlDataReader rdr = command.ExecuteReader())
               {
                  if (rdr.HasRows)
                  {
                     while (rdr.Read())
                     {
                        User user = new User
                        {
                           Id = rdr.GetInt32(0),
                           FirstName = rdr.GetString(1),
                           LastName = rdr.GetString(2),
                           Age = rdr.GetInt32(3),
                           DepartmentId = rdr.GetInt32(4),
                           DateAdded = rdr.GetDateTime(5),
                        };
                     }
                  }
                  stopwatch.Stop();
                  execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
                  rdr.Close();
               }
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      public double SelectDepartmentByName(string name, Stopwatch stopwatch)
      {
         double execTime;
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;
               command.CommandText = "SELECT Id, Name, DateAdded FROM [dbo].[Department] WHERE Name LIKE @name";
               command.Parameters.AddWithValue("@name", name);

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               using (SqlDataReader rdr = command.ExecuteReader())
               {
                  if (rdr.HasRows)
                  {
                     while (rdr.Read())
                     {
                        Department department = new Department
                        {
                           Id = rdr.GetInt32(0),
                           Name = rdr.GetString(1),
                           DateAdded = rdr.GetDateTime(2),
                        };
                     }
                  }
                  stopwatch.Stop();
                  execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
                  rdr.Close();
               }
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      public double SelectUsersByFirstName(string firstName, Stopwatch stopwatch)
      {
         double execTime;
         List<User> users = new List<User>();
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;
               command.CommandText = "SELECT Id, FirstName, LastName, Age, DepartmentId, DateAdded FROM [dbo].[User] WHERE FirstName LIKE @firstName";
               command.Parameters.AddWithValue("@firstName", firstName);

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               using (SqlDataReader rdr = command.ExecuteReader())
               {
                  if (rdr.HasRows)
                  {
                     while (rdr.Read())
                     {
                        users.Add(new User
                        {
                           Id = rdr.GetInt32(0),
                           FirstName = rdr.GetString(1),
                           LastName = rdr.GetString(2),
                           Age = rdr.GetInt32(3),
                           DepartmentId = rdr.GetInt32(4),
                           DateAdded = rdr.GetDateTime(5),
                        });
                     }
                  }
                  stopwatch.Stop();
                  execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
                  rdr.Close();
               }
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      public double SelectDepartmentsByUser(string userFirstName, Stopwatch stopwatch)
      {
         double execTime;
         StringBuilder sb = new StringBuilder();
         List<Department> departments = new List<Department>();
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;
               sb.Append("SELECT [dbo].[Department].Id, [dbo].[Department].Name, [dbo].[Department].DateAdded FROM [dbo].[Department] ");
               sb.Append("INNER JOIN [dbo].[User] ON [dbo].[Department].[Id] = [dbo].[User].[DepartmentId] ");
               sb.Append("WHERE [dbo].[User].[FirstName] LIKE @userFirstName");
               command.CommandText = sb.ToString();
               command.Parameters.AddWithValue("@userFirstName", userFirstName);

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               using (SqlDataReader rdr = command.ExecuteReader())
               {
                  if (rdr.HasRows)
                  {
                     while (rdr.Read())
                     {
                        departments.Add(new Department
                        {
                           Id = rdr.GetInt32(0),
                           Name = rdr.GetString(1),
                           DateAdded = rdr.GetDateTime(2),
                        });
                     }
                  }
                  stopwatch.Stop();
                  execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
                  rdr.Close();
               }
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      public double SelectUsersByProjects(int[] projectKeys, Stopwatch stopwatch)
      {
         if (!projectKeys.Any())
            throw new ArgumentException("ProjectKeys array should contain at least one item.");
         double execTime;
         StringBuilder sb = new StringBuilder();
         List<User> users = new List<User>();
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;
               sb.Append("SELECT DISTINCT [dbo].[User].Id, [dbo].[User].FirstName, [dbo].[User].LastName, ");
               sb.Append("[dbo].[User].Age, [dbo].[User].DepartmentId, [dbo].[User].DateAdded FROM [dbo].[User] ");
               sb.Append("INNER JOIN [dbo].[ProjectUser] ON [dbo].[User].[Id] = [dbo].[ProjectUser].[UserId] ");
               sb.Append("WHERE [dbo].[ProjectUser].[ProjectId] IN (");
               List<string> paramNames = new List<string>();
               int keysCount = projectKeys.Length;
               for (int i = 0; i < keysCount; i++)
               {
                  string paramIndex = "@p" + i;
                  paramNames.Add(paramIndex);
                  command.Parameters.AddWithValue(paramIndex, projectKeys[i]);
               }
               sb.Append(string.Join(", ", paramNames) + ")");
               command.CommandText = sb.ToString();

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               using (SqlDataReader rdr = command.ExecuteReader())
               {
                  if (rdr.HasRows)
                  {
                     while (rdr.Read())
                     {
                        users.Add(new User
                        {
                           Id = rdr.GetInt32(0),
                           FirstName = rdr.GetString(1),
                           LastName = rdr.GetString(2),
                           Age = rdr.GetInt32(3),
                           DepartmentId = rdr.GetInt32(4),
                           DateAdded = rdr.GetDateTime(5),
                        });
                     }
                  }
                  stopwatch.Stop();
                  execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
                  rdr.Close();
               }
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      public double SelectAverageAgeByProjects(int[] projectKeys, Stopwatch stopwatch)
      {
         if (!projectKeys.Any())
            throw new ArgumentException("ProjectKeys array should contain at least one item.");
         double execTime;
         double averageAge = 0;
         StringBuilder sb = new StringBuilder();
         using (SqlConnection sqlConnection = new SqlConnection(connectionString))
         {
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand())
            {
               command.Connection = sqlConnection;
               command.CommandType = CommandType.Text;

               sb.Append("SELECT AVG([dbo].[User].Age) AS AverageAge FROM [dbo].[User] ");
               sb.Append("INNER JOIN [dbo].[ProjectUser] ON [dbo].[User].[Id] = [dbo].[ProjectUser].[UserId] ");
               sb.Append("WHERE [dbo].[ProjectUser].[ProjectId] IN (");
               List<string> paramNames = new List<string>();
               int keysCount = projectKeys.Length;
               for (int i = 0; i < keysCount; i++)
               {
                  string paramIndex = "@p" + i;
                  paramNames.Add(paramIndex);
                  command.Parameters.AddWithValue(paramIndex, projectKeys[i]);
               }
               sb.Append(string.Join(", ", paramNames) + ")");
               command.CommandText = sb.ToString();

               var initialTime = stopwatch.Elapsed.TotalMilliseconds;
               stopwatch.Start();
               using (SqlDataReader rdr = command.ExecuteReader())
               {
                  if (rdr.HasRows)
                  {
                     while (rdr.Read())
                     {
                        averageAge = rdr.GetDouble(0);
                     }
                  }
                  stopwatch.Stop();
                  execTime = stopwatch.Elapsed.TotalMilliseconds - initialTime;
                  rdr.Close();
               }
            }
            sqlConnection.Close();
         }
         return execTime;
      }

      private readonly string connectionString;
   }
}
