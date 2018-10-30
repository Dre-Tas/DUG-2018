using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Dialogs;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DUG_2018
{
    // A transaction is a self-contained Revit action that can be rolled back if needed
    [Transaction(TransactionMode.Manual)]
    // Class definition. Classes contain methods
    class ExportNWC : IExternalCommand
    {
        // Method definition. This kind of Revit methods has to return a Result
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            // Get application and document objects
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Autodesk.Revit.DB.Document doc = uidoc.Document;

            // Get 3D views which name ends with "NWC"
            List<Autodesk.Revit.DB.View> NWC3DViews = new FilteredElementCollector(doc).
                OfClass(typeof(Autodesk.Revit.DB.View)).ToElements().
                Cast<Autodesk.Revit.DB.View>().
                // Select only 3D views which name ends with "IFC"
                Where(x => x.ViewType == ViewType.ThreeD &&
                !x.IsTemplate &&
                x.ViewName.ToLower().EndsWith("nwc")).
                ToList();

            // Initialize the file browser and its settings
            CommonOpenFileDialog foldBrowser = FolderBrowserInitializer();
            // Overwrite some settings
            foldBrowser.Title = "Select Export Folder";
            foldBrowser.Multiselect = false;

            // Initialise variable to store folder path
            string folder = null;

            // Check if user's input
            if (foldBrowser.ShowDialog() == CommonFileDialogResult.Ok)
            {
                // Store folder destination from folder browser
                folder = foldBrowser.FileName;
            }
            else
            {
                // Create pop up message to interact with user
                MessageBox.Show("Ok, no worries. I'll be here when you need me.",
                    "Abort",
                    // Set which buttons to show in window
                    MessageBoxButtons.OK,
                    // Set symbol to show in window
                    MessageBoxIcon.Information);
                return Result.Failed;
            }

            // Create options object
            NavisworksExportOptions options = new NavisworksExportOptions();
            // You can leave it as it is if you don't want to customise any option
            options.DivideFileIntoLevels = false;
            options.ExportRoomGeometry = false;
            options.ExportRoomAsAttribute = false;
            options.ExportUrls = false;
            options.FindMissingMaterials = false;

            // Initialise counter
            int count = 0;

            // Loop through all views in list above (do something for each of them)
            foreach (Autodesk.Revit.DB.View v in NWC3DViews)
            {
                // Set remaining options for each view
                options.ExportScope = NavisworksExportScope.View;
                options.ViewId = v.Id;
                options.ExportLinks = true;

                // Try-Catch = try to do this, but if you catch an error, do that
                try
                {
                    // Export view. The method requires a path where to save,
                    // the name of the file to save and any option
                    doc.Export(folder, v.Name + ".nwc", options);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ooops, something happened");
                }

                // Add one to the counter
                count++;
            }

            // Recap to user telling the process is done
            Autodesk.Revit.UI.TaskDialog.Show("Recap", $"I successfully exported {count} views!");

            // If everything's fine, tell Revit it succeeded.
            return Result.Succeeded;
        }

        // Method to summon a folder browser
        public static CommonOpenFileDialog FolderBrowserInitializer()
        {
            // Create folder browser object
            CommonOpenFileDialog openFolderDialog1 = new CommonOpenFileDialog();
            // Set up properties
            openFolderDialog1.Title = "Select folders...";
            openFolderDialog1.InitialDirectory = (@"C:\");
            openFolderDialog1.IsFolderPicker = true;
            openFolderDialog1.Multiselect = true;
            openFolderDialog1.RestoreDirectory = true;

            // Return folder browser object
            return openFolderDialog1;
        }
    }
}
