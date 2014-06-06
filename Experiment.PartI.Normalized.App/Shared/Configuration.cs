using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.PartI.Normalized.App.Properties;

namespace Experiment.PartI.Normalized.App.Shared
{
   public class Configuration
   {
      public static string SqlConnectionString
      {
         get
         {
            return Settings.Default.RunLocal ?
                @Settings.Default.LocalSqlConnectionString :
                @Settings.Default.RemoteSqlConnectionString;
         }
      }

      public static string MongoConnectionString
      {
         get
         {
            return Settings.Default.RunLocal ?
                @Settings.Default.LocalMongoConnectionString :
                @Settings.Default.RemoteMongoConnectionString;
         }
      }

      public static string DatabaseName
      {
         get
         {
            return Settings.Default.DatabaseName;
         }
      }

      public static string ResultDbSqlConnectionString
      {
         get
         {
            return Settings.Default.RunLocal ?
                @Settings.Default.LocalResultDbConnectionString :
                @Settings.Default.RemoteResultDbConnectionString;
         }
      }
   }
}
