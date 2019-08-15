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
		public static readonly String imperialTemplateDirectory = @"C:\ProgramData\Autodesk\RVT 2020\Family Templates\English_I\";
		public static readonly String addInResourcesDirectory = @"C:\ProgramData\Autodesk\Revit\Addins\2020\Perfect";
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

			RibbonPanel PerfectRibbonPanel = application.CreateRibbonPanel("Perfect Standards");

            PulldownButtonData cleanUpToolsPullDownButtonData = new PulldownButtonData(
                name: "CleanUpToolsPulldown",
                text: "Clean Up");
            PulldownButtonData stylesPullDownButttonData = new PulldownButtonData(
                name: "StylesPullDownButton",
                text: "Import Styles");
            PulldownButtonData auditNamesToolsPulldownButtonData = new PulldownButtonData(
                name: "AuditNamesToolsPulldown",
                text: "Name Auditing");
            IList<RibbonItem> stackOne = PerfectRibbonPanel.AddStackedItems(
				auditNamesToolsPulldownButtonData,
				cleanUpToolsPullDownButtonData,
				stylesPullDownButttonData);

            PulldownButton auditNamesToolsPullDownButton = (PulldownButton)stackOne[0];
            PushButtonData auditViewNamesCommandPushButtonData = new PushButtonData(
                name: "AuditViewNamesPushButton",
                text: "Audit View Naming",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.AuditViewNamesCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            auditNamesToolsPullDownButton.AddPushButton(auditViewNamesCommandPushButtonData);
            PushButtonData renameFamiliesCommandPushButtonData = new PushButtonData(
                name: "RenameFamiliesCommandButton",
                text: "Family Names",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.RenameFamiliesCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            auditNamesToolsPullDownButton.AddPushButton(renameFamiliesCommandPushButtonData);
            PushButtonData setViewTitleCommandPushButtonData = new PushButtonData(
                name: "SetViewTitleCommandButton",
                text: "Set Empty View Titles",
                assemblyName: FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                className: "DougKlassen.Revit.Perfect.Commands.SetViewTitleCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            auditNamesToolsPullDownButton.AddPushButton(setViewTitleCommandPushButtonData);

            PulldownButton cleanUpToolsPullDownButton = (PulldownButton)stackOne[1];
            PushButtonData purgeLinePatternsCommandPushButtonData = new PushButtonData(
                name: "PurgeLinePatternsCommandPushButtonData",
                text: "Purge Line Patterns",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.PurgeLinePatternsCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            cleanUpToolsPullDownButton.AddPushButton(purgeLinePatternsCommandPushButtonData);
            PushButtonData purgeRefPlanesCommandPushButtonData = new PushButtonData(
                name: "PurgeRefPlanesCommandButton",
                text: "Purge Reference Planes",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.PurgeRefPlanesCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            cleanUpToolsPullDownButton.AddPushButton(purgeRefPlanesCommandPushButtonData);
            PushButtonData purgeViewsCommandPushButtonData = new PushButtonData(
                name: "PurgeViewsCommandButton",
                text: "Purge Views",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.PurgeViewsCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            cleanUpToolsPullDownButton.AddPushButton(purgeViewsCommandPushButtonData);

            PulldownButton stylesPullDownButton = (PulldownButton)stackOne[2];
            PushButtonData exportImportStylesCommandPushButtonData = new PushButtonData(
                name: "ExportImportStylesCommandButton",
                text: "Export CAD Import Styles",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.ExportImportStylesCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            stylesPullDownButton.AddPushButton(exportImportStylesCommandPushButtonData);
            PushButtonData loadImportStylesCommandPushButtonData = new PushButtonData(
                name: "LoadImportStylesCommandButton",
                text: "Load CAD Import Styles",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.LoadImportStylesCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            stylesPullDownButton.AddPushButton(loadImportStylesCommandPushButtonData);

            PulldownButtonData geometryPullDownButtonData = new PulldownButtonData(
                name: "GeometryPullDownButton",
                text: "Geometry");
            PulldownButtonData elementPullDownButtonData = new PulldownButtonData(
                name: "ElementPullDownButton",
                text: "Element Properties");
            PulldownButtonData wordListPullDownButtonData = new PulldownButtonData(
                name: "WordListPullDownButton",
                text: "Word List");
			IList<RibbonItem> stackTwo = PerfectRibbonPanel.AddStackedItems(
				geometryPullDownButtonData,
				elementPullDownButtonData,
                wordListPullDownButtonData);

			PulldownButton geometryPullDownButton = (PulldownButton)stackTwo[0];
            PushButtonData unflipCommandPushButtonData = new PushButtonData(
                name: "UnflipCommandPushButtonData",
                text: "Unflip Windows",
                assemblyName: FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                className: "DougKlassen.Revit.Perfect.Commands.UnflipCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon,
                AvailabilityClassName = "DougKlassen.Revit.Perfect.Commands.UnflipCommandAvailability"
            };
            geometryPullDownButton.AddPushButton(unflipCommandPushButtonData);

            PulldownButton elementPullDownButton = (PulldownButton)stackTwo[1];
            PushButtonData flagUnitElementsCommandPushButtonData = new PushButtonData(
                name: "FlagUnitElementsCommand",
                text: "Flag Unit Elements",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.FlagUnitElementsCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            elementPullDownButton.AddPushButton(flagUnitElementsCommandPushButtonData);

            PulldownButton wordListPullDownButton = (PulldownButton)stackTwo[2];
            PushButtonData exportDetailTextCommandPushButtonData = new PushButtonData(
                name: "ExportDetailTextCommandPushButtonData",
                text: "Export Callout Text for Review",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.ExportDetailTextCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            wordListPullDownButton.AddPushButton(exportDetailTextCommandPushButtonData);

			PerfectRibbonPanel.AddSlideOut();
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
