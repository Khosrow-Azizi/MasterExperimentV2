using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.PartI.Normalized.App.DataRecorder.Db
{
   public partial class Entities :  DbContext
   {
      public Entities()
         : base("name=" + Experiment.PartI.Normalized.App.Shared.Configuration.ResultDbSqlConnectionString)
      {

      }
   }
}
