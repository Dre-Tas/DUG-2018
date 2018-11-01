using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DUG_2018
{
    // A transaction is a self-contained Revit action that can be rolled back if needed
    [Transaction(TransactionMode.Manual)]
    // Class definition. Classes contain methods
    class Hello : IExternalCommand
    {
        // Method definition. This kind of Revit methods has to return a Result
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                // TaskDialogs are pop-up messages that can also be customised
                // with buttons that do things
                TaskDialog.Show("Greetings", "Hello, DSUG!");

            }
            catch (Exception)
            {
                return Result.Failed;
            }
            // If everything's fine, tell Revit it succeeded.
            return Result.Succeeded;
        }
    }
}
