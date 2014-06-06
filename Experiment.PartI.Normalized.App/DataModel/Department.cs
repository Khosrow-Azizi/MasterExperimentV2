using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.PartI.Normalized.App.DataModel
{
   public class Department : PartIBaseClass, IHasName
   {
      public string Name { get; set; }
   }
}
