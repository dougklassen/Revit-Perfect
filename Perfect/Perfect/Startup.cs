using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Collections.Generic;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect
{
    public static class FileLocations
    {
        //AddInDirectory is initialized at runtime
        public static String AddInDirectory;
        public static String AssemblyName;
        public static String AssemblyPath;
        public static readonly String imperialTemplateDirectory = @"C:\ProgramData\Autodesk\RVT 2014\Family Templates\English_I\";
        public static readonly String addInResourcesDirectory = @"C:\2014-BCRA-RVT\addins\Resources\";
        public static readonly String ResourceNameSpace = @"DougKlassen.Revit.Perfect.Resources";
    }

    public class StartUpApp : IExternalApplication
    {
        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            //initialize AssemblyName using reflection
            FileLocations.AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            //initialize AddInDirectory. The addin should be stored in a directory named after the assembly
            FileLocations.AddInDirectory = application.ControlledApplication.AllUsersAddinsLocation + "\\" + FileLocations.AssemblyName + "\\";
            FileLocations.AssemblyPath = FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll";

            //load image resources
            BitmapImage largeIcon = GetEmbeddedImageResource("iconLarge.png");
            BitmapImage smallIcon = GetEmbeddedImageResource("iconSmall.png");

            PushButtonData aboutCommandPushButtonData = new PushButtonData(
                name: "AboutCommandButton",
                text: "About Perfect",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.AboutCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon,
                AvailabilityClassName = "DougKlassen.Revit.Perfect.Commands.AboutCommandAvailability"
            };

            PushButtonData auditViewNamesCommandPushButtonData = new PushButtonData(
                name: "AuditViewNamesPushButton",
                text: "Audit View Naming",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.AuditViewNamesCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };

            PushButtonData exportImportStylesCommandPushButtonData = new PushButtonData(
                name: "ExportImportStylesCommandButton",
                text: "Export Styles",
                assemblyName: FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                className: "DougKlassen.Revit.Perfect.Commands.ExportImportStylesCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };

            PushButtonData loadImportStylesCommandPushButtonData = new PushButtonData(
                name: "LoadImportStylesCommandButton",
                text: "Import Styles",
                assemblyName: FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                className: "DougKlassen.Revit.Perfect.Commands.LoadImportStylesCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };

            PushButtonData purgeLinePatternsCommandPushButtonData = new PushButtonData(
                name: "PurgeLinePatternsCommandPushButtonData",
                text: "Purge Line Patterns",
                assemblyName: FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                className: "DougKlassen.Revit.Perfect.Commands.PurgeLinePatternsCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            
            PushButtonData purgeParametersCommandPushButtonData = new PushButtonData(
                name: "PurgeParametersCommandButton",
                text: "Purge Parameters",
                assemblyName: FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                className: "DougKlassen.Revit.Perfect.Commands.PurgeParametersCommand")
                {
                    LargeImage = largeIcon,
                    Image = smallIcon
                };

            PushButtonData purgeRefPlanesCommandPushButtonData = new PushButtonData(
                name: "PurgeRefPlanesCommandButton",
                text: "Purge Unnamed Reference Planes",
                assemblyName: FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                className: "DougKlassen.Revit.Perfect.Commands.PurgeRefPlanesCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };

            PushButtonData renameFamiliesCommandPushButtonData = new PushButtonData(
                name: "RenameFamiliesCommandButton",
                text: "Family Names",
                assemblyName: FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                className: "DougKlassen.Revit.Perfect.Commands.RenameFamiliesCommand")
                {
                    LargeImage = largeIcon,
                    Image = smallIcon
                };

            PushButtonData setViewTitleCommandPushButtonData = new PushButtonData(
                name: "SetViewTitleCommandButton",
                text: "Set Empty View Titles",
                assemblyName: FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                className: "DougKlassen.Revit.Perfect.Commands.SetViewTitleCommand")
                {
                    LargeImage = largeIcon,
                    Image = smallIcon
                };

            PushButtonData UnflipCommandPushButtonData = new PushButtonData(
                name: "UnflipCommandPushButtonData",
                text: "Unflip Windows",
                assemblyName: FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                className: "DougKlassen.Revit.Perfect.Commands.UnflipCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon,
                AvailabilityClassName = "DougKlassen.Revit.Perfect.Commands.UnflipCommandAvailability"
            };

            RibbonPanel PerfectRibbonPanel = application.CreateRibbonPanel("Perfect Standards");

            PulldownButtonData auditNamesToolsPulldownButtonData = new PulldownButtonData(
                name: "AuditNamesToolsPulldown",
                text: "Name Auditing Tools");
            
            PulldownButtonData cleanUpToolsPullDownButtonData = new PulldownButtonData(
                name: "CleanUpToolsPulldown",
                text: "Clean Up Tools");

            PulldownButtonData stylesPullDownButttonData = new PulldownButtonData(
                name: "StylesPullDownButton",
                text: "Import Styles Management");

            PulldownButtonData geometryPullDownButtonData = new PulldownButtonData(
                name: "GeometryPullDownButton",
                text: "Geometry Tools");

            IList<RibbonItem> stackOne = PerfectRibbonPanel.AddStackedItems(
                auditNamesToolsPulldownButtonData,
                cleanUpToolsPullDownButtonData,
                stylesPullDownButttonData);

            PulldownButton auditNamesToolsPullDownButton = (PulldownButton)stackOne[0];
            auditNamesToolsPullDownButton.AddPushButton(auditViewNamesCommandPushButtonData);
            auditNamesToolsPullDownButton.AddPushButton(renameFamiliesCommandPushButtonData);
            auditNamesToolsPullDownButton.AddPushButton(setViewTitleCommandPushButtonData);
            
            PulldownButton cleanUpToolsPullDownButton = (PulldownButton) stackOne[1];
            cleanUpToolsPullDownButton.AddPushButton(purgeLinePatternsCommandPushButtonData);
            cleanUpToolsPullDownButton.AddPushButton(purgeParametersCommandPushButtonData);
            cleanUpToolsPullDownButton.AddPushButton(purgeRefPlanesCommandPushButtonData);

            PulldownButton stylesPullDownButton = (PulldownButton) stackOne[2];
            stylesPullDownButton.AddPushButton(exportImportStylesCommandPushButtonData);
            stylesPullDownButton.AddPushButton(loadImportStylesCommandPushButtonData);

            PulldownButton geometryPulldownButton = (PulldownButton)PerfectRibbonPanel.AddItem(
                geometryPullDownButtonData);
            geometryPulldownButton.AddPushButton(UnflipCommandPushButtonData);

            PerfectRibbonPanel.AddSlideOut();

            PerfectRibbonPanel.AddItem(aboutCommandPushButtonData);

            return Result.Succeeded;
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        /// Utility method to retrieve an embedded image resource from the assembly
        /// </summary>
        /// <param name="resourceName">The name of the image, corresponding to the filename of the embedded resouce added to the solution</param>
        /// <returns>The loaded image represented as a BitmapImage</returns>
        BitmapImage GetEmbeddedImageResource(String resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream str = asm.GetManifestResourceStream(FileLocations.ResourceNameSpace + "." + resourceName);

            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = str;
            bmp.EndInit();

            return bmp;
        }
    }
}
