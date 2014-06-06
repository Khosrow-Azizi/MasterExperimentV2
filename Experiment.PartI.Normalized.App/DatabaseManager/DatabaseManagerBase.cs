using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.PartI.Normalized.App.DataModel;

namespace Experiment.PartI.Normalized.App.DatabaseManager
{
   public abstract class DatabaseManagerBase : IDatabaseManager
   {
      public abstract long GetTotalCount<T>() where T : PartIBaseClass;
      public abstract int[] GetAllKeys<T>() where T : PartIBaseClass;
      public abstract Dictionary<int, string> GetKeyAndNames<T>(float portion) where T : PartIBaseClass, IHasName;
      public abstract void RestoreDatabase<T>(Dictionary<int, string> keyAndNames) where T : PartIBaseClass, IHasName;
      public abstract int GetRandomKey<T>() where T : PartIBaseClass;
      public abstract string GetRandomName<T>() where T : PartIBaseClass, IHasName;
      public abstract string GetRandomUserFirstName();
      public abstract int GetNewId<T>() where T : PartIBaseClass;
      public abstract string GetLatestName<T>() where T : PartIBaseClass, IHasName;
      public abstract void FlushDatabase();
      public virtual long ConvertToCount<T>(float portion) where T : PartIBaseClass
      {
         long count = GetTotalCount<T>();
         float ratio = portion;
         if (ratio < 0)
            ratio = 0;
         if (portion > 1)
            ratio = 1;
         var result = (ratio * count);
         if (result > 0 && result < 1)
            result = 1;
         return (long)result;
      }
   }
}
