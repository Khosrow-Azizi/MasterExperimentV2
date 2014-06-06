using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.PartI.Normalized.App.DataRecorder.Db;
using Experiment.PartI.Normalized.App.Shared;

namespace Experiment.PartI.Normalized.App.DataRecorder
{
   public interface IRecorder
   {
      void Record(PerformanceResult performanceResult);
      void Delete(PartIResult partIResult);
      PartIResult GetLatestResult(TestCaseEnums testCase, TestScenarioEnums scenario);
      void DeleteAllResults();
   }
}
