using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.PartI.Normalized.App.DataModel
{
   public class ProjectUser : PartIRootClass
   {
      public int ProjectId { get; set; }
      public int UserId { get; set; }
   }
}
