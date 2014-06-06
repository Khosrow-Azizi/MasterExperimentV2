using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.PartI.Normalized.App.DataModel
{
   public class Project : PartIBaseClass, IHasName
   {
      public Project()
      {
         this.Users = new HashSet<int>();
      }

      public string Name { get; set; }
      public int ManagerId { get; set; }
      public int DepartmentId { get; set; }
      public ICollection<int> Users { get; set; }
   }
}
