using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.PartI.Normalized.App.DataModel;

namespace Experiment.PartI.Normalized.App.PerformanceMonitor
{
   public interface IDatabasePerformanceMonitor
   {
      double Insert(Department department, Stopwatch stopwatch);
      double Insert(Project project, Stopwatch stopwatch);
      double Insert(User user, Stopwatch stopwatch);
      double UpdateDepartmentNameByKeys(int[] keys, string newName, Stopwatch stopwatch);
      double UpdateProjectNameByKeys(int[] keys, string newName, Stopwatch stopwatch);
      double UpdateUserLastName(string firstNamePattern, string newLastName, Stopwatch stopwatch);
      double SelectDepartmentByKey(int key, Stopwatch stopwatch);
      double SelectUserByKey(int key, Stopwatch stopwatch);
      double SelectDepartmentByName(string name, Stopwatch stopwatch);
      double SelectUsersByFirstName(string firstName, Stopwatch stopwatch);
      double SelectDepartmentsByUser(string userFirstName, Stopwatch stopwatch);
      double SelectUsersByProjects(int[] projectKeys, Stopwatch stopwatch);
      double SelectAverageAgeByProjects(int[] projectKeys, Stopwatch stopwatch);
   }
}
