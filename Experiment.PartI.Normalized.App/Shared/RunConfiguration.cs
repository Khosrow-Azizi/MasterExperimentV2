using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.PartI.Normalized.App.Shared
{
   public class RunConfiguration
   {
      public int NumberOfSelects { get; private set; }
      public int NumberOfUpdates { get; private set; }
      public int TotalNumberOfDepartments { get; private set; }
      public int TotalNumberOfUsers { get; private set; }
      public int TotalNumberOfProjects { get; private set; }
      public TestCaseEnums TestCase { get; private set; }

      private RunConfiguration () 	{	}

      public static RunConfiguration GetConfigurations(TestCaseEnums testCase)
      {
         switch (testCase)
         {
            case TestCaseEnums.Initialize:
               return new RunConfiguration
               {
                  NumberOfSelects = 1,
                  NumberOfUpdates = 1,
                  TotalNumberOfDepartments = 1,
                  TotalNumberOfUsers = 1,
                  TotalNumberOfProjects = 1,
                  TestCase = testCase,
               };
            case TestCaseEnums.TestCase1:
               return new RunConfiguration
               {
                  NumberOfSelects = 100,
                  NumberOfUpdates = 100,
                  TotalNumberOfDepartments = 4,
                  TotalNumberOfUsers = 128,
                  TotalNumberOfProjects = 16,
                  TestCase = testCase,
               };
            case TestCaseEnums.TestCase2:
               return new RunConfiguration
               {
                  NumberOfSelects = 100,
                  NumberOfUpdates = 100,
                  TotalNumberOfDepartments = 8,
                  TotalNumberOfUsers = 256,
                  TotalNumberOfProjects = 64,
                  TestCase = testCase,
               };
            case TestCaseEnums.TestCase3:
               return new RunConfiguration
               {
                  NumberOfSelects = 100,
                  NumberOfUpdates = 100,
                  TotalNumberOfDepartments = 16,
                  TotalNumberOfUsers = 1024,
                  TotalNumberOfProjects = 512,
                  TestCase = testCase,
               };
            case TestCaseEnums.TestCase4:
               return new RunConfiguration
               {
                  NumberOfSelects = 100,
                  NumberOfUpdates = 100,
                  TotalNumberOfDepartments = 128,
                  TotalNumberOfUsers = 4096,
                  TotalNumberOfProjects = 8192,
                  TestCase = testCase,
               };
            case TestCaseEnums.TestCase5:
               return new RunConfiguration
               {
                  NumberOfSelects = 100,
                  NumberOfUpdates = 100,
                  TotalNumberOfDepartments = 256,
                  TotalNumberOfUsers = 8192,
                  TotalNumberOfProjects = 16384,
                  TestCase = testCase,
               };
            case TestCaseEnums.TestCase6:
               return new RunConfiguration
               {
                  NumberOfSelects = 100,
                  NumberOfUpdates = 100,
                  TotalNumberOfDepartments = 512,
                  TotalNumberOfUsers = 16384,
                  TotalNumberOfProjects = 32768,
                  TestCase = testCase,
               };
            case TestCaseEnums.TestCase7:
               return new RunConfiguration
               {
                  NumberOfSelects = 100,
                  NumberOfUpdates = 100,
                  TotalNumberOfDepartments = 1024,
                  TotalNumberOfUsers = 32768,
                  TotalNumberOfProjects = 65536,
                  TestCase = testCase,
               };
            case TestCaseEnums.TestCase8:
               return new RunConfiguration
               {
                  NumberOfSelects = 100,
                  NumberOfUpdates = 100,
                  TotalNumberOfDepartments = 2048,
                  TotalNumberOfUsers = 65536,
                  TotalNumberOfProjects = 131072,
                  TestCase = testCase,
               };
            default:
               return new RunConfiguration
               {
                  NumberOfSelects = 0,
                  NumberOfUpdates = 0,
                  TotalNumberOfDepartments = 0,
                  TotalNumberOfUsers = 0,
                  TotalNumberOfProjects = 0,
                  TestCase = testCase,
               };
         }
      }
   }
}
