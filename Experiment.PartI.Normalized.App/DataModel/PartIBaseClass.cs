using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.PartI.Normalized.App.DataModel
{
   public interface IHasName
   {
      string Name { get; set; }
   }

   public abstract class PartIRootClass
   {
   }

   public abstract class PartIBaseClass : PartIRootClass
   {
      public virtual int Id { get; set; }
      public virtual DateTime DateAdded { get; set; }
   }
}
