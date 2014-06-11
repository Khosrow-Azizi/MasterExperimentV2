using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Experiment.PartI.Normalized.App.DatabaseManager;
using Experiment.PartI.Normalized.App.DataModel;
using Experiment.PartI.Normalized.App.DataRecorder;
using Experiment.PartI.Normalized.App.DataRecorder.Db;
using Experiment.PartI.Normalized.App.PerformanceMonitor;
using Experiment.PartI.Normalized.App.Shared;

namespace Experiment.PartI.Normalized.App
{
   public class ExperimentRunner
   {
      public static void Main(string[] args)
      {
         Process proc = Process.GetCurrentProcess();
         ProcessThread thread = proc.Threads[0];
         thread.IdealProcessor = 1;
         thread.ProcessorAffinity = (IntPtr)2;

         ExperimentRunner expRunner = null;
         bool waitForUser = true;
         while (waitForUser)
         {
            Console.WriteLine("Which part of the Experiment would you like to run?");
            Console.WriteLine("     Press 1 for SQL Server with COMMAND query");
            Console.WriteLine("     Press 2 for MongoDB with Acknowledged writes option");
            Console.WriteLine("     Press 6 to stop the program");

            int answer;
            int.TryParse(Console.ReadLine(), out answer);
            switch (answer)
            {
               case -1:
                  {
                     expRunner = new ExperimentRunner(DataBaseTypeEnums.UnitTest);
                     waitForUser = false;
                  }
                  break;
               case 1:
                  {
                     expRunner = new ExperimentRunner(DataBaseTypeEnums.SqlServerWithCommandQuery);
                     waitForUser = false;
                  }
                  break;
               case 2:
                  {
                     expRunner = new ExperimentRunner(DataBaseTypeEnums.MongoDbNormalizedWriteAck);
                     waitForUser = false;
                  }
                  break;
               case 6:
                  waitForUser = false;
                  break;
               default:
                  Console.Beep();
                  Console.WriteLine("Please select a number from the options list..");
                  break;
            }
         }

         if (expRunner != null)
            expRunner.RunAll();

         Console.Write("\nStopping the program...");
         int count = 4;
         while (count > 0)
         {
            Thread.Sleep(1000);
            Console.Write("...");
            count--;
         }
         Console.WriteLine("\nPress any key to close the program.");
         Console.ReadLine();
      }

      public ExperimentRunner(DataBaseTypeEnums databaseType)
      {
         switch (databaseType)
         {
            case DataBaseTypeEnums.UnitTest:
            case DataBaseTypeEnums.SqlServerWithCommandQuery:
               {
                  this.dbPerformanceMonitor = new SqlCommandParamQueryPerformanceMonitor(Configuration.SqlConnectionString);
                  this.dbManager = new SqlDatabaseManager(Configuration.SqlConnectionString);
               }
               break;
            case DataBaseTypeEnums.MongoDbNormalizedWriteAck:
               {
                  this.dbPerformanceMonitor = new MongoPerformanceMonitor(Configuration.MongoConnectionString, Configuration.DatabaseName);
                  this.dbManager = new MongoDatabaseManager(Configuration.MongoConnectionString, Configuration.DatabaseName);
               }
               break;
            default:
               Console.WriteLine("There is no performance monitor for the given database type.");
               return;
         }

         this.databaseType = databaseType;
         this.recorder = new Recorder(databaseType);
         this.stopwatch = new Stopwatch();
         this.results = new List<PerformanceResult>();
      }

      public void RunAll()
      {
         Console.WriteLine(string.Format("Running the experiment with database type '{0}'..", databaseType.ToString()));
         Console.WriteLine("Start time: " + DateTime.Now);
         if (!FlushDatabases())
            return;
         Run(TestCaseEnums.Initialize);
         Run(TestCaseEnums.TestCase1);
         Run(TestCaseEnums.TestCase2);
         Run(TestCaseEnums.TestCase3);
         Run(TestCaseEnums.TestCase4);
         Run(TestCaseEnums.TestCase5);
         Run(TestCaseEnums.TestCase6);
         Run(TestCaseEnums.TestCase7);
         Run(TestCaseEnums.TestCase8);
         SaveRandomData();
         Console.WriteLine("The experiment finished successfully.");
         Console.WriteLine("End time: " + DateTime.Now);
      }

      public void Run(TestCaseEnums testCase)
      {
         Console.WriteLine(string.Format("Test case '{0}' has started..", testCase.ToString()));
         RunConfiguration config = RunConfiguration.GetConfigurations(testCase);

         stopwatch.Reset();
         results.Clear();
         if (!RunInserts(config, stopwatch, results))
            return;        

         if (!RunUpdateDepartmentName(config, stopwatch, results))
            return;        

         if (!RunUpdateUserLastName(config, stopwatch, results))
            return;

         if (!RunUpdateProjectName(config, stopwatch, results))
            return;

         if (!RunSelectDepartmentByKey(config, stopwatch, results))
            return;

         if (!RunSelectDepartmentByRandomName(config, stopwatch, results))
            return;

         if (!RunSelectUserByKey(config, stopwatch, results))
            return;

         if (!RunSelectUserByRandomFirstName(config, stopwatch, results))
            return;

         if (!RunSelectDepartmentByRandomUser(config, stopwatch, results))
            return;

         if (!RunSelectUserByRandomProject(config, stopwatch, results))
            return;

         if (!RunSelectAvgUserAgeByProjects(config, stopwatch, results))
            return;
         Console.WriteLine("Saving result data in the database..");
         recorder.Record(results);
         Console.WriteLine("Saving result data succeeded.");

         results.Clear();
         stopwatch.Stop();
         Console.WriteLine(string.Format("Test case '{0}' successfully completed.", testCase.ToString()));
         Console.WriteLine("--------------------------------------------------------------------------");
         Thread.Sleep(5000);
      }

      private bool RunInserts(RunConfiguration config, Stopwatch sw, List<PerformanceResult> resultsTobeRecorded)
      {
         Console.WriteLine("Inserts tests has started..");
         int namePostfix = GetLatestNamePostfix<Department>();
         HashSet<int> departemtIds = new HashSet<int>();
         HashSet<int> userIds = new HashSet<int>();
         Random randomGeneratore = new Random();
         long dbCount = 0;
         long numberOfInserts = 0;
         try
         {
            // insert departments
            Console.WriteLine("Inserting departments..");
            dbCount = dbManager.GetTotalCount<Department>();
            numberOfInserts = config.TotalNumberOfDepartments - dbCount;

            dbPerformanceMonitor.Insert(new Department
               {
                  Id = dbManager.GetNewId<Department>(),
                  Name = DepartmentNamePrefix + (++namePostfix),
                  DateAdded = DateTime.Now,
               }, sw);
            for (int i = 0; i < numberOfInserts; i++)
            {
               Department department = new Department
               {
                  Id = dbManager.GetNewId<Department>(),
                  Name = DepartmentNamePrefix + (++namePostfix),
                  DateAdded = DateTime.Now,
               };

               resultsTobeRecorded.Add(new PerformanceResult
               {
                  ExecutionTime = dbPerformanceMonitor.Insert(department, sw),
                  TestCase = config.TestCase,
                  TestScenario = TestScenarioEnums.InsertDepartment,
               });
               departemtIds.Add(department.Id);
            }

            if (!departemtIds.Any())
            {
               // just in case there are no new department inserts
               // get all department ids from db to be used by insertion 
               // of new users with random department id
               foreach (int id in dbManager.GetAllKeys<Department>())
                  departemtIds.Add(id);
            }
            Console.WriteLine("Inserting departments succeeded.");

            // insert users with random departments
            Console.WriteLine("Inserting users..");
            dbCount = dbManager.GetTotalCount<User>();
            numberOfInserts = config.TotalNumberOfUsers - dbCount;
            for (int i = 0; i < numberOfInserts; i++)
            {
               namePostfix = randomGeneratore.Next(i, MaxNamePostfix);
               User user = new User
               {
                  Id = dbManager.GetNewId<User>(),
                  FirstName = FirstNamePrefix + namePostfix,
                  LastName = LastNamePrefix + namePostfix,
                  Age = randomGeneratore.Next(20, 50),
                  DepartmentId = departemtIds.ElementAt(randomGeneratore.Next(0, departemtIds.Count - 1)),
                  DateAdded = DateTime.Now,
               };

               resultsTobeRecorded.Add(new PerformanceResult
               {
                  ExecutionTime = dbPerformanceMonitor.Insert(user, sw),
                  TestCase = config.TestCase,
                  TestScenario = TestScenarioEnums.InsertUser,
               });
               userIds.Add(user.Id);
            }
            if (!userIds.Any())
            {
               // just in case there are no new user inserts
               // get all user ids from db to be used by insertion 
               // of new projects with random user id
               foreach (int id in dbManager.GetAllKeys<User>())
                  userIds.Add(id);
            }
            Console.WriteLine("Inserting users succeeded.");

            // insert projects
            Console.WriteLine("Inserting projects..");
            dbCount = dbManager.GetTotalCount<Project>();
            numberOfInserts = config.TotalNumberOfProjects - dbCount;
            for (int i = 0; i < numberOfInserts; i++)
            {
               namePostfix = GetLatestNamePostfix<Project>();
               Project project = new Project
               {
                  Id = dbManager.GetNewId<Project>(),
                  Name = ProjectNamePrefix + (++namePostfix),
                  DepartmentId = departemtIds.ElementAt(randomGeneratore.Next(0, departemtIds.Count - 1)),
                  ManagerId = userIds.ElementAt(randomGeneratore.Next(0, userIds.Count - 1)),
                  DateAdded = DateTime.Now,
               };
               // add random users to project
               int maxUsers = randomGeneratore.Next(1, MaxUserPerProject);
               for (int j = 0; j < maxUsers; j++)
                  project.Users.Add(userIds.ElementAt(randomGeneratore.Next(0, userIds.Count - 1)));

               resultsTobeRecorded.Add(new PerformanceResult
               {
                  ExecutionTime = dbPerformanceMonitor.Insert(project, sw),
                  TestCase = config.TestCase,
                  TestScenario = TestScenarioEnums.InsertProject,
               });
            }
            Console.WriteLine("Inserting projects succeeded.");
         }
         catch (Exception ex)
         {
            Console.WriteLine("Failed to complete Insert tests. Error: " + ex.Message);
            return false;
         }
         Console.WriteLine("Insert tests completed.");
         return true;
      }

      private bool RunUpdateDepartmentName(RunConfiguration config, Stopwatch sw, List<PerformanceResult> resultsTobeRecorded)
      {
         Console.WriteLine(string.Format("Update tests with scenario '{0}' has started..",
           TestScenarioEnums.UpdateDepartmentNameByKeys.ToString()));
         Dictionary<int, string> originalKeyAndNames = new Dictionary<int, string>();
         string newName = string.Empty;
         float portionToUpdate = (float)0.1;
         try
         {
            for (int i = 0; i < config.NumberOfUpdates; i++)
            {
               originalKeyAndNames = dbManager.GetKeyAndNames<Department>(portionToUpdate);
               newName = NewDepartmentNamePrefix + i;
               resultsTobeRecorded.Add(new PerformanceResult
               {
                  ExecutionTime = dbPerformanceMonitor.UpdateDepartmentNameByKeys(originalKeyAndNames.Keys.ToArray(), newName, sw),
                  TestCase = config.TestCase,
                  TestScenario = TestScenarioEnums.UpdateDepartmentNameByKeys,
               });
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Failed to complete Update tests. Error: ", ex.Message);
            return false;
         }
         Console.WriteLine("Upadte test completed.");
         return true;
      }

      private bool RunUpdateUserLastName(RunConfiguration config, Stopwatch sw, List<PerformanceResult> resultsTobeRecorded)
      {
         Console.WriteLine(string.Format("Update tests with scenario '{0}' has started..",
          TestScenarioEnums.UpdateUserLastNameByFirstName.ToString()));
         try
         {
            for (int i = 0; i < config.NumberOfUpdates; i++)
            {
               if (RandomUserFirsftNames.Count <= i)
                  RandomUserFirsftNames.Add(FirstNamePrefix + new Random().Next(i, MaxNamePostfix));
               resultsTobeRecorded.Add(new PerformanceResult
               {
                  ExecutionTime = dbPerformanceMonitor.UpdateUserLastName(RandomUserFirsftNames[i], NewUserLastNamePrefix, sw),
                  TestCase = config.TestCase,
                  TestScenario = TestScenarioEnums.UpdateUserLastNameByFirstName,
               });
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Failed to complete Update tests. Error: ", ex.Message);
            return false;
         }
         Console.WriteLine("Upadte test completed.");
         return true;
      }

      private bool RunUpdateProjectName(RunConfiguration config, Stopwatch sw, List<PerformanceResult> resultsTobeRecorded)
      {
         Console.WriteLine(string.Format("Update tests with scenario '{0}' has started..",
          TestScenarioEnums.UpdateProjectNameByKeys.ToString()));
         Dictionary<int, string> originalKeyAndNames = new Dictionary<int, string>();
         string newName = string.Empty;
         float portionToUpdate = (float)0.25;
         try
         {
            originalKeyAndNames = dbManager.GetKeyAndNames<Project>(portionToUpdate);
            var projectKeysBundles = GetKeyBundles(originalKeyAndNames.Keys.ToArray());
            for (int i = 0; i < config.NumberOfUpdates; i++)
            {
               newName = NewProjectNamePrefix + i;
               foreach (var bundle in projectKeysBundles)
               {
                  resultsTobeRecorded.Add(new PerformanceResult
                  {
                     ExecutionTime = dbPerformanceMonitor.UpdateProjectNameByKeys(bundle, newName, sw),
                     TestCase = config.TestCase,
                     TestScenario = TestScenarioEnums.UpdateProjectNameByKeys,
                  });
               }
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Failed to complete Update tests. Error: ", ex.Message);
            return false;
         }
         Console.WriteLine("Upadte test completed.");
         return true;
      }

      private bool RunSelectDepartmentByKey(RunConfiguration config, Stopwatch sw, List<PerformanceResult> resultsTobeRecorded)
      {
         Console.WriteLine(string.Format("Select tests with scenario '{0}' has started..",
          TestScenarioEnums.SelectDepartmentByKey.ToString()));
         try
         {
            for (int i = 0; i < config.NumberOfSelects; i++)
            {
               if (RandomDepartmentIds.Count <= i)
                  RandomDepartmentIds.Add(dbManager.GetRandomKey<Department>().ToString());
               resultsTobeRecorded.Add(new PerformanceResult
            {
               ExecutionTime = dbPerformanceMonitor.SelectDepartmentByKey(int.Parse(RandomDepartmentIds[i]), sw),
               TestCase = config.TestCase,
               TestScenario = TestScenarioEnums.SelectDepartmentByKey,
            });
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Failed to complete Select tests. Error: ", ex.Message);
            return false;
         }
         Console.WriteLine("Select test completed.");
         return true;
      }

      private bool RunSelectDepartmentByRandomName(RunConfiguration config, Stopwatch sw, List<PerformanceResult> resultsTobeRecorded)
      {
         Console.WriteLine(string.Format("Select tests with scenario '{0}' has started..",
          TestScenarioEnums.SelectDepartmentByRandomName.ToString()));
         try
         {
            for (int i = 0; i < config.NumberOfSelects; i++)
            {
               if (RandomDepartmentNames.Count <= i)
                  RandomDepartmentNames.Add(dbManager.GetRandomName<Department>());
               resultsTobeRecorded.Add(new PerformanceResult
               {
                  ExecutionTime = dbPerformanceMonitor.SelectDepartmentByName(RandomDepartmentNames[i], sw),
                  TestCase = config.TestCase,
                  TestScenario = TestScenarioEnums.SelectDepartmentByRandomName,
               });
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Failed to complete Select tests. " + ex.Message);
            return false;
         }
         Console.WriteLine("Select test completed.");
         return true;
      }

      private bool RunSelectUserByKey(RunConfiguration config, Stopwatch sw, List<PerformanceResult> resultsTobeRecorded)
      {
         Console.WriteLine(string.Format("Select tests with scenario '{0}' has started..",
         TestScenarioEnums.SelectUserByKey.ToString()));
         try
         {
            for (int i = 0; i < config.NumberOfSelects; i++)
            {
               if (RandomUserIds.Count <= i)
                  RandomUserIds.Add(dbManager.GetRandomKey<User>().ToString());
               resultsTobeRecorded.Add(new PerformanceResult
               {
                  ExecutionTime = dbPerformanceMonitor.SelectUserByKey(int.Parse(RandomUserIds[i]), sw),
                  TestCase = config.TestCase,
                  TestScenario = TestScenarioEnums.SelectUserByKey,
               });
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Failed to complete Select tests. Error: " + ex.Message);
            return false;
         }
         Console.WriteLine("Select test completed.");
         return true;
      }

      private bool RunSelectUserByRandomFirstName(RunConfiguration config, Stopwatch sw, List<PerformanceResult> resultsTobeRecorded)
      {
         Console.WriteLine(string.Format("Select tests with scenario '{0}' has started..",
            TestScenarioEnums.SelectUsersByRandomFirstName.ToString()));
         try
         {
            for (int i = 0; i < config.NumberOfSelects; i++)
            {
               if (RandomUserFirsftNames.Count <= i)
                  RandomUserFirsftNames.Add(dbManager.GetRandomUserFirstName());
               resultsTobeRecorded.Add(new PerformanceResult
               {
                  ExecutionTime = dbPerformanceMonitor.SelectUsersByFirstName(RandomUserFirsftNames[i], sw),
                  TestCase = config.TestCase,
                  TestScenario = TestScenarioEnums.SelectUsersByRandomFirstName,
               });
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Failed to complete Select tests. Error: " + ex.Message);
            return false;
         }
         Console.WriteLine("Select test completed.");
         return true;
      }

      private bool RunSelectDepartmentByRandomUser(RunConfiguration config, Stopwatch sw, List<PerformanceResult> resultsTobeRecorded)
      {
         Console.WriteLine(string.Format("Select tests with scenario '{0}' has started..",
           TestScenarioEnums.SelectDepartmentByRandomUserFirstName.ToString()));
         try
         {
            for (int i = 0; i < config.NumberOfSelects; i++)
            {
               if (RandomUserFirsftNames.Count <= i)
                  RandomUserFirsftNames.Add(dbManager.GetRandomUserFirstName());
               resultsTobeRecorded.Add(new PerformanceResult
               {
                  ExecutionTime = dbPerformanceMonitor.SelectDepartmentsByUser(RandomUserFirsftNames[i], sw),
                  TestCase = config.TestCase,
                  TestScenario = TestScenarioEnums.SelectDepartmentByRandomUserFirstName,
               });
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Failed to complete Select tests. Error: ", ex.Message);
            return false;
         }
         Console.WriteLine("Select test completed.");
         return true;
      }

      private bool RunSelectUserByRandomProject(RunConfiguration config, Stopwatch sw, List<PerformanceResult> resultsTobeRecorded)
      {
         Console.WriteLine(string.Format("Select tests with scenario '{0}' has started..",
           TestScenarioEnums.SelectUsersByRandomProjectKeys.ToString()));
         HashSet<int> projectKeys = null;
         try
         {
            for (int i = 0; i < config.NumberOfSelects; i++)
            {
               projectKeys = new HashSet<int>();
               if (RandomProjectIds.Count <= i)
               {
                  if (config.TestCase == TestCaseEnums.Initialize)
                     projectKeys.Add(dbManager.GetRandomKey<Project>());
                  else
                  {
                     // get three random projects
                     while (projectKeys.Count < 3)
                        projectKeys.Add(dbManager.GetRandomKey<Project>());
                  }
                  RandomProjectIds.Add(i, projectKeys.ToArray());
               }
               resultsTobeRecorded.Add(new PerformanceResult
               {
                  ExecutionTime = dbPerformanceMonitor.SelectUsersByProjects(RandomProjectIds[i], sw),
                  TestCase = config.TestCase,
                  TestScenario = TestScenarioEnums.SelectUsersByRandomProjectKeys,
               });
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Failed to complete Select tests. Error: ", ex.Message);
            return false;
         }
         Console.WriteLine("Select test completed.");
         return true;
      }

      private bool RunSelectAvgUserAgeByProjects(RunConfiguration config, Stopwatch sw, List<PerformanceResult> resultsTobeRecorded)
      {
         Console.WriteLine(string.Format("Select tests with scenario '{0}' has started..",
           TestScenarioEnums.SelectAverageAgeByRandomProjectKeys.ToString()));
         HashSet<int> projectKeys = null;
         try
         {
            for (int i = 0; i < config.NumberOfSelects; i++)
            {
               projectKeys = new HashSet<int>();
               if (RandomProjectIds.Count <= i)
               {
                  // get three random projects
                  while (projectKeys.Count < 3)
                     projectKeys.Add(dbManager.GetRandomKey<Project>());
                  randomProjectIds.Add(i, projectKeys.ToArray());
               }
               resultsTobeRecorded.Add(new PerformanceResult
               {
                  ExecutionTime = dbPerformanceMonitor.SelectAverageAgeByProjects(RandomProjectIds[i], sw),
                  TestCase = config.TestCase,
                  TestScenario = TestScenarioEnums.SelectAverageAgeByRandomProjectKeys,
               });
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Failed to complete Select tests. Error: " + ex.Message);
            return false;
         }
         Console.WriteLine("Select test completed.");
         return true;
      }

      private void SaveRandomData()
      {
         Console.WriteLine("Saving the random data..");

         using (SqlConnection sqlConnection = new SqlConnection(Configuration.ResultDbSqlConnectionString))
         {
            var query = "Delete [dbo].[PartIRandomData]";
            sqlConnection.Open();
            using (var command = new SqlCommand(query, sqlConnection))
            {
               command.ExecuteNonQuery();
            }
            sqlConnection.Close();
         }

         using (Entities ctx = new Entities())
         {
            foreach (string id in RandomDepartmentIds)
               ctx.PartIRandomData.Add(new PartIRandomData { Type = (int)RandomDataTypeEnums.DepartmentId, Value = id });

            foreach (string id in RandomUserIds)
               ctx.PartIRandomData.Add(new PartIRandomData { Type = (int)RandomDataTypeEnums.UserId, Value = id });

            foreach (var bundle in RandomProjectIds)
            {
               foreach (int id in bundle.Value)
                  ctx.PartIRandomData.Add(new PartIRandomData { Type = (int)RandomDataTypeEnums.ProjectId, Value = id.ToString() });
            }

            foreach (string name in RandomDepartmentNames)
               ctx.PartIRandomData.Add(new PartIRandomData { Type = (int)RandomDataTypeEnums.DepartmentName, Value = name });

            foreach (string name in RandomUserFirsftNames)
               ctx.PartIRandomData.Add(new PartIRandomData { Type = (int)RandomDataTypeEnums.UserFirstName, Value = name });

            ctx.SaveChanges();
         }
         Console.WriteLine("Saving the random data succeeded.");
      }

      private int GetLatestNamePostfix<T>() where T : PartIBaseClass, IHasName
      {
         string lastNameInDb = dbManager.GetLatestName<T>();
         if (string.IsNullOrEmpty(lastNameInDb))
            return 0;
         string number = "";
         for (int i = 0; i < lastNameInDb.Length; i++)
         {
            if (char.IsNumber(lastNameInDb[i]))
               number += lastNameInDb[i];
         }
         return string.IsNullOrEmpty(number) ? 0 : int.Parse(number);
      }

      private List<int[]> GetKeyBundles(int[] allProjectKeys)
      {
         long totalKeys = allProjectKeys.LongCount();
         List<int[]> projectKeysBundles = new List<int[]>();
         if (totalKeys > MaxAllowedSqlParameters)
         {
            long numberOfBundles = totalKeys / MaxAllowedSqlParameters;
            int numberOfKeysBundled = 0;
            for (int i = 0; i < numberOfBundles; i++)
            {
               projectKeysBundles.Add(allProjectKeys.Skip(i * MaxAllowedSqlParameters).Take(MaxAllowedSqlParameters).ToArray());
               numberOfKeysBundled += MaxAllowedSqlParameters;
            }
            if (numberOfKeysBundled < totalKeys)
               projectKeysBundles.Add(allProjectKeys.Skip(numberOfKeysBundled).Take(MaxAllowedSqlParameters).ToArray());
         }
         else
         {
            projectKeysBundles.Add(allProjectKeys);
         }
         return projectKeysBundles;
      }

      public bool FlushDatabases()
      {
         Console.WriteLine("Flushing the database..");
         try
         {
            dbManager.DeleteAll();
            recorder.DeleteAllResults();
            Console.WriteLine("Flushing the database succeeded.");
            return true;
         }
         catch (Exception ex)
         {
            Console.WriteLine("Failed to flush the database. Error: " + ex.Message);
            return false;
         }
      }

      private static List<string> randomUserFirstNames;
      protected static List<string> RandomUserFirsftNames
      {
         get
         {
            if (randomUserFirstNames == null)
            {
               using (Entities ctx = new Entities())
               {
                  randomUserFirstNames = ctx.PartIRandomData
                        .Where(d => d.Type == (int)RandomDataTypeEnums.UserFirstName)
                        .Select(dv => dv.Value).ToList();
               }
            }
            return randomUserFirstNames;
         }
      }

      private static List<string> randomDepartmentNames;
      protected static List<string> RandomDepartmentNames
      {
         get
         {
            if (randomDepartmentNames == null)
            {
               using (Entities ctx = new Entities())
               {
                  randomDepartmentNames = ctx.PartIRandomData
                     .Where(d => d.Type == (int)RandomDataTypeEnums.DepartmentName)
                     .Select(dv => dv.Value).ToList();
               }
            }
            return randomDepartmentNames;
         }
      }

      private static List<string> randomDepartmentIds;
      protected static List<string> RandomDepartmentIds
      {
         get
         {
            if (randomDepartmentIds == null)
            {
               using (Entities ctx = new Entities())
               {
                  randomDepartmentIds = ctx.PartIRandomData
                           .Where(d => d.Type == (int)RandomDataTypeEnums.DepartmentId)
                           .Select(dv => dv.Value).ToList();
               }
            }
            return randomDepartmentIds;
         }
      }

      private static List<string> randomUserIds;
      protected static List<string> RandomUserIds
      {
         get
         {
            if (randomUserIds == null)
            {
               using (Entities ctx = new Entities())
               {
                  randomUserIds = ctx.PartIRandomData
                              .Where(d => d.Type == (int)RandomDataTypeEnums.UserId)
                              .Select(dv => dv.Value).ToList();
               }
            }
            return randomUserIds;
         }
      }

      private static Dictionary<int, int[]> randomProjectIds;
      protected static Dictionary<int, int[]> RandomProjectIds
      {
         get
         {
            if (randomProjectIds == null)
            {
               using (Entities ctx = new Entities())
               {
                  randomProjectIds = new Dictionary<int, int[]>();
                  Queue<int> projectIds = new Queue<int>();
                  foreach (var id in ctx.PartIRandomData.Where(d => d.Type == (int)RandomDataTypeEnums.ProjectId).ToArray())
                  {
                     projectIds.Enqueue(int.Parse(id.Value));
                  }
                  int index = 0;
                  List<int> keyTriples = null;
                  while (projectIds.Count > 0)
                  {
                     keyTriples = new List<int>();
                     for (int i = 0; i < 3; i++)
                     {
                        if (projectIds.Count > 0)
                           keyTriples.Add(projectIds.Dequeue());
                     }
                     randomProjectIds.Add(index, keyTriples.ToArray());
                     index++;
                  }
               }
            }
            return randomProjectIds;
         }
      }

      private readonly DataBaseTypeEnums databaseType;
      private readonly IDatabasePerformanceMonitor dbPerformanceMonitor;
      private readonly IDatabaseManager dbManager;
      private readonly IRecorder recorder;
      private readonly List<PerformanceResult> results;
      private readonly Stopwatch stopwatch;
      private const string DepartmentNamePrefix = "Department ";
      private const string FirstNamePrefix = "First name ";
      private const string LastNamePrefix = "Last name ";
      private const string ProjectNamePrefix = "Project ";
      private const string NewDepartmentNamePrefix = "New Dapartment Name ";
      private const string NewUserLastNamePrefix = "New Last Name ";
      private const string NewProjectNamePrefix = "New Project Name ";
      private const int MaxUserPerProject = 15;
      private const int MaxNamePostfix = 1000000;
      private const int MaxAllowedSqlParameters = 2000;
   }
}
