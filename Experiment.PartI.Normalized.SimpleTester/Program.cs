using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.PartI.Normalized.App.DatabaseManager;
using Experiment.PartI.Normalized.App.DataModel;
using Experiment.PartI.Normalized.App.PerformanceMonitor;
using Experiment.PartI.Normalized.App.Shared;

namespace Experiment.PartI.Normalized.SimpleTester
{
   class Program
   {
      static void Main(string[] args)
      {        
         Stopwatch sw = new Stopwatch();
         SqlCommandParamQueryPerformanceMonitor monitor = new SqlCommandParamQueryPerformanceMonitor(Configuration.SqlConnectionString);

         var dbManager = new SqlDatabaseManager(Configuration.SqlConnectionString);          
         dbManager.DeleteAll();
         int id = 0;
         foreach (var count in new[] { 4, 4, 16, 128, 256, 512, 1000 }) //new[] { 10, 128, 1000, 10000 }
         {
           // int count = 1000;
            for (int i = 0; i < count; i++)
               monitor.Insert(new Department { Id = ++id, DateAdded = DateTime.Now, Name = "dep " + i }, sw);
            sw.Stop();
            Console.WriteLine(((double)sw.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency) * 1000);
         }
         Console.ReadLine();
      }
   }
}
