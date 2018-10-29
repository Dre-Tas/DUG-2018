#region Namespaces
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace DUG_2018
{
    class App : IExternalApplication
    {
        // define a method that will create our tab and button
        static void AddRibbonPanel(UIControlledApplication application)
        {
            // How do you want to call the toolbox
            string tabName = "DSUG 2018";
            // Create a custom ribbon tab
            application.CreateRibbonTab(tabName);

            // Get dll assembly path
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            // Add new ribbon panels
            RibbonPanel ribbonPanelHello =
                application.CreateRibbonPanel(tabName, "Just a test");

            // Create button object
            PushButtonData buttonData = new PushButtonData(
                "cmdHello",
                "Hello!",
                thisAssemblyPath,
                "DUG_2018.Hello");

            // Add the created button object to the panel
            PushButton pushButton = ribbonPanelHello.AddItem(buttonData) as PushButton;
            // Define tooltip that will appear when hovering on the button
            pushButton.ToolTip = "Say hi!";
            // Create an image object that will become the icon of the button
            BitmapImage pbUImage = new BitmapImage(new Uri
                ("pack://application:,,,/DUG-2018;component/Resources/hello.png"));
            // Assign image to the button as a LargeImage
            pushButton.LargeImage = pbUImage;

            // Add new ribbon panels
            RibbonPanel ribbonPanelExports =
                application.CreateRibbonPanel(tabName, "Exports");

            // Create button object
            PushButtonData expButtonData = new PushButtonData(
                "cmdIFC",
                "Export to IFC",
                thisAssemblyPath,
                "DUG_2018.ExportIFC");

            // Add the created button object to the panel
            PushButton expPushButton = ribbonPanelExports.AddItem(expButtonData) as PushButton;
            // Define tooltip that will appear when hovering on the button
            expPushButton.ToolTip = "Export views to IFC";
            // Create an image object that will become the icon of the button
            BitmapImage pbExpImage = new BitmapImage(new Uri
                ("pack://application:,,,/DUG-2018;component/Resources/share.png"));
            // Assign image to the button as a LargeImage
            expPushButton.LargeImage = pbExpImage;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // Load toolbox at startup
            AddRibbonPanel(application);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}