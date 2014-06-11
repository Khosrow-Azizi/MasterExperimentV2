using System;
using System.Collections.Generic;
using Experiment.PartI.Normalized.App.DataRecorder;
using Experiment.PartI.Normalized.App.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Experiment.PartI.Normalized.Test
{
   [TestClass]
   public class UnitTest1
   {
      [TestMethod]
      public void TestMethod1()
      {
         Recorder recorder = new Recorder(DataBaseTypeEnums.UnitTest);
         List<PerformanceResult> results = new List<PerformanceResult>();

         for (int i = 0; i < 1000; i++)
            results.Add(new PerformanceResult
            {
               ExecutionTime = -1,
               TestCase = TestCaseEnums.UnitTest,
               TestScenario = TestScenarioEnums.UnitTest,
            });

         recorder.Record(results);
         Assert.AreNotEqual(recorder.GetLatestResult(TestCaseEnums.UnitTest, TestScenarioEnums.UnitTest), null);
         recorder.DeleteAllResults();

      }
   }
}
