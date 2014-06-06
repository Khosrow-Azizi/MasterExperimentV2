using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.PartI.Normalized.App.DataModel;

namespace Experiment.PartI.Normalized.App.DatabaseManager
{
   public interface IDatabaseManager
   {
      long GetTotalCount<T>() where T : PartIBaseClass;
      int GetNewId<T>() where T : PartIBaseClass;
      int[] GetAllKeys<T>() where T : PartIBaseClass;
      Dictionary<int, string> GetKeyAndNames<T>(float portion) where T : PartIBaseClass, IHasName;
      void RestoreDatabase<T>(Dictionary<int, string> keyAndNames) where T : PartIBaseClass, IHasName;
      int GetRandomKey<T>() where T : PartIBaseClass;
      string GetRandomName<T>() where T : PartIBaseClass, IHasName;
      string GetRandomUserFirstName();
      string GetLatestName<T>() where T : PartIBaseClass, IHasName;
      void FlushDatabase();
   }
}
