using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Reflection;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.StartUp
{
    public static class FileLocations
    {
        //AddInDirectory is initialized at runtime
        public static String AddInDirectory;
        public static String AssemblyName;
        public static readonly String imperialTemplateDirectory = @"C:\ProgramData\Autodesk\RVT 2014\Family Templates\English_I\";
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

            //load image resources
            BitmapImage largeIcon = GetEmbeddedImageResource("iconLarge.png");
            BitmapImage smallIcon = GetEmbeddedImageResource("iconSmall.png");

            PushButtonData renameFamiliesCommandPushButtonData = new PushButtonData(
                name: "RenameFamiliesCommandButton",
                text: "Family Names",
                assemblyName: FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                className: "DougKlassen.Revit.Perfect.Commands.RenameFamiliesCommand")
                {
                    LargeImage = largeIcon,
                    Image = smallIcon
                };
            renameFamiliesCommandPushButtonData.LargeImage = largeIcon;

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

            RibbonPanel PerfectRibbonPanel = application.CreateRibbonPanel("Perfect Standards");
            PerfectRibbonPanel.AddItem(renameFamiliesCommandPushButtonData);

            SplitButtonData stylesSplitButtonData = new SplitButtonData(
                name: "StylesSplitButton",
                text: "Manage Import Styles");
            SplitButton sb = PerfectRibbonPanel.AddItem(stylesSplitButtonData) as SplitButton;
            sb.AddPushButton(exportImportStylesCommandPushButtonData);
            sb.AddPushButton(loadImportStylesCommandPushButtonData);

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
