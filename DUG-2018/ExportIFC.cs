﻿using System;
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
    class ExportIFC : IExternalCommand
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

            // Get 3D views which name ends with "IFC"
            List<Autodesk.Revit.DB.View> IFC3DViews = new FilteredElementCollector(doc).
                OfClass(typeof(Autodesk.Revit.DB.View)).ToElements().
                Cast<Autodesk.Revit.DB.View>().
                // Select only 3D views which name ends with "IFC"
                Where(x => x.ViewType == ViewType.ThreeD &&
                !x.IsTemplate &&
                x.ViewName.ToLower().EndsWith("ifc")).
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
            IFCExportOptions options = new IFCExportOptions();
            // You can leave it as it is if you don't want to customise any option

            // Initialise counter
            int count = 0;

            // Whenever you need to modify anything in the document you need to
            // do it inside a transaction so that it is reversible
            using (Transaction t = new Transaction(doc))
            {
                // Give a name to the transaction. This is the name that will be displayed in Revit too.
                t.Start("Delete All Views Not on Sheets");

                // Try-Catch = try to do this, but if you catch an error, so that
                try
                {
                    // Loop through all views in list above (do something for each of them)
                    foreach (Autodesk.Revit.DB.View v in IFC3DViews)
                    {
                        // Export view. The method requires a path where to save,
                        // the name of the file to save and any option
                        doc.Export(folder, v.Name + ".ifc", options);

                        // Add one to the counter
                        count++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ooops, something happened");
                }

                // Then "end the transaction
                t.Commit();

                // Recap to user telling the process is done
                Autodesk.Revit.UI.TaskDialog.Show("Recap", $"I successfully exported {count} views!");
            }
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
