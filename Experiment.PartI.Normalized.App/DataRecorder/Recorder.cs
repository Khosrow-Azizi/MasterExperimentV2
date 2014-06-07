using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.PartI.Normalized.App.DataRecorder.Db;
using Experiment.PartI.Normalized.App.Shared;

namespace Experiment.PartI.Normalized.App.DataRecorder
{
   public class Recorder : IRecorder
   {
      public Recorder(DataBaseTypeEnums dataBaseType)
      {
         this.dataBaseType = dataBaseType;
      }

      public void Record(PerformanceResult performanceResult)
      {
         using (var ctx = new Entities())
         {
            ctx.PartIResult.Add(new PartIResult
               {
                  TestNumber = GenerateNewTestNumber(ctx, performanceResult),
                  DataBaseType = (int)dataBaseType,
                  DateTimeAdded = DateTime.Now,
                  TestCase = (int)performanceResult.TestCase,
                  TestScenario = (int)performanceResult.TestScenario,
                  ExecutionTime = performanceResult.ExecutionTime,
               });
            ctx.SaveChanges();
         }
      }

      public void Record(IEnumerable<PerformanceResult> performanceResults)
      {
         using (var ctx = new Entities())
         {
            int testNumber = GenerateNewTestNumber(ctx, performanceResults.First());
            foreach (var result in performanceResults)
            {
               ctx.PartIResult.Add(new PartIResult
               {
                  TestNumber = testNumber,
                  DataBaseType = (int)dataBaseType,
                  DateTimeAdded = DateTime.Now,
                  TestCase = (int)result.TestCase,
                  TestScenario = (int)result.TestScenario,
                  ExecutionTime = result.ExecutionTime,
               });
               testNumber++;
            }
            ctx.SaveChanges();
         }
      }

      public PartIResult GetLatestResult(TestCaseEnums testCase, TestScenarioEnums scenario)
      {
         using (Entities ctx = new Entities())
         {
            return ctx.PartIResult.OrderByDescending(tr => tr.TestNumber).FirstOrDefault(r => r.DataBaseType == (int)dataBaseType
               && r.TestCase == (int)testCase && r.TestScenario == (int)scenario);
         }
      }

      public void Delete(PartIResult partIResult)
      {
         using (Entities ctx = new Entities())
         {
            PartIResult resultInDb = ctx.PartIResult.FirstOrDefault(r => r.DataBaseType == (int)dataBaseType &&
               r.TestNumber == partIResult.TestNumber &&
                 r.TestCase == (int)partIResult.TestCase && r.TestScenario == (int)partIResult.TestScenario);
            if (resultInDb == null)
               return;
            ctx.PartIResult.Remove(resultInDb);
            ctx.SaveChanges();
         }
      }

      public void DeleteAllResults()
      {
         using (SqlConnection sqlConnection = new SqlConnection(Configuration.ResultDbSqlConnectionString))
         {
            var query = "Delete [dbo].[PartIResult] WHERE DataBaseType = " + (int)dataBaseType;
            sqlConnection.Open();
            using (var command = new SqlCommand(query, sqlConnection))
            {
               command.ExecuteNonQuery();
            }
            sqlConnection.Close();
         }
      }

      private int GenerateNewTestNumber(Entities ctx, PerformanceResult performanceResult)
      {
         var testResults = ctx.PartIResult
              .Where(r => r.DataBaseType == (int)dataBaseType &&
                 r.TestCase == (int)performanceResult.TestCase && r.TestScenario == (int)performanceResult.TestScenario);
         if (testResults.Any())
            return testResults.Max(r => r.TestNumber) + 1;
         return 1;
      }

      private readonly DataBaseTypeEnums dataBaseType;
   }
}
