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
        public static readonly String CommandNameSpace = "DougKlassen.Revit.Perfect.Commands.";
	}

	public class StartUpApp : IExternalApplication
	{
        BitmapImage largeIcon;
        BitmapImage smallIcon;

		Result IExternalApplication.OnStartup(UIControlledApplication application)
		{
			//initialize AssemblyName using reflection
			FileLocations.AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
			//initialize AddInDirectory. The addin should be stored in a directory named after the assembly
			FileLocations.AddInDirectory = application.ControlledApplication.AllUsersAddinsLocation + "\\" + FileLocations.AssemblyName + "\\";
			FileLocations.AssemblyPath = FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll";

			//load image resources
			largeIcon = GetEmbeddedImageResource("iconLarge.png");
			smallIcon = GetEmbeddedImageResource("iconSmall.png");

			RibbonPanel PerfectRibbonPanel = application.CreateRibbonPanel("Perfect Standards");

            #region Create Column One
            PulldownButtonData namingStandardsPulldownButtonData = new PulldownButtonData(
                name: "AuditNamesToolsPulldown",
                text: "Name Auditing");
            PulldownButtonData cleanUpToolsPullDownButtonData = new PulldownButtonData(
                name: "CleanUpToolsPulldown",
                text: "Clean Up");
            PulldownButtonData exportDataPullDownButttonData = new PulldownButtonData(
                name: "ExportDataDownButton",
                text: "Export/Import Data");
            IList<RibbonItem> stackOne = PerfectRibbonPanel.AddStackedItems(
                namingStandardsPulldownButtonData,
                cleanUpToolsPullDownButtonData,
                exportDataPullDownButttonData);
            //set up Naming Standards pull down button
            PulldownButton nameStandardsPullDownButton = (PulldownButton)stackOne[0];
            addButtonToPulldown(
                pulldown: nameStandardsPullDownButton,
                commandClass: "AuditViewNamesCommand",
                buttonText: "Audit View Naming",
                buttonToolTip: "Check view names against the view naming standard");
            addButtonToPulldown(
                pulldown: nameStandardsPullDownButton,
                commandClass: "RenameFamiliesCommand",
                buttonText: "Family Names",
                buttonToolTip: "Rename families according to family naming standards");
            addButtonToPulldown(
                pulldown: nameStandardsPullDownButton,
                commandClass: "SetViewTitleCommand",
                buttonText: "Set Empty View Titles",
                buttonToolTip: "Set Title on Sheet view parameter for views that don't have it set yet");
            //set up Clean Up pull down button
            PulldownButton cleanUpPullDownButton = (PulldownButton)stackOne[1];
            PushButtonData purgeLinePatternsCommandPushButtonData = new PushButtonData(
                name: "PurgeLinePatternsCommandPushButtonData",
                text: "Purge Line Patterns",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.PurgeLinePatternsCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            var purgeLinePatternsButton = cleanUpPullDownButton.AddPushButton(purgeLinePatternsCommandPushButtonData);
            purgeLinePatternsButton.ToolTip = "Purge line patterns using regular expression matches";
            PushButtonData purgeRefPlanesCommandPushButtonData = new PushButtonData(
                name: "PurgeRefPlanesCommandButton",
                text: "Purge Reference Planes",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.PurgeRefPlanesCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            cleanUpPullDownButton.AddPushButton(purgeRefPlanesCommandPushButtonData);
            PushButtonData purgeViewsCommandPushButtonData = new PushButtonData(
                name: "PurgeViewsCommandButton",
                text: "Purge Views",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.PurgeViewsCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            cleanUpPullDownButton.AddPushButton(purgeViewsCommandPushButtonData);
            //set up Export pull down button
            PulldownButton exportPullDownButton = (PulldownButton)stackOne[2];
            PushButtonData exportImportStylesCommandPushButtonData = new PushButtonData(
                name: "ExportImportStylesCommandButton",
                text: "Export CAD Import Styles",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.ExportImportStylesCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            exportPullDownButton.AddPushButton(exportImportStylesCommandPushButtonData);
            PushButtonData loadImportStylesCommandPushButtonData = new PushButtonData(
                name: "LoadImportStylesCommandButton",
                text: "Load CAD Import Styles",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.LoadImportStylesCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            exportPullDownButton.AddPushButton(loadImportStylesCommandPushButtonData);
            PushButtonData exportDetailTextCommandPushButtonData = new PushButtonData(
                name: "ExportDetailTextCommandPushButtonData",
                text: "Export Callout Text for Review",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.ExportDetailTextCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon
            };
            exportPullDownButton.AddPushButton(exportDetailTextCommandPushButtonData);
            #endregion

            #region Create Column Two
            PulldownButtonData geometryPullDownButtonData = new PulldownButtonData(
                name: "GeometryPullDownButton",
                text: "Fix Geometry");
            PulldownButtonData elementPullDownButtonData = new PulldownButtonData(
                name: "ElementPullDownButton",
                text: "Element Properties");
			IList<RibbonItem> stackTwo = PerfectRibbonPanel.AddStackedItems(
				geometryPullDownButtonData,
				elementPullDownButtonData);
            //set up Geometry pulldown button
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
            //set up Elements pull down button
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
            #endregion

            #region add slide out panel
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
            #endregion

            return Result.Succeeded;
		}

        /// <summary>
        /// helper method to add a button to a pulldown
        /// </summary>
        /// <param name="pulldown"></param>
        /// <param name="commandClass"></param>
        /// <param name="buttonText"></param>
        /// <param name="buttonToolTip"></param>
        /// <param name="largeImage"></param>
        /// <param name="smallImage"></param>
        /// <returns></returns>
        PushButton addButtonToPulldown(
            PulldownButton pulldown,
            String buttonText,
            String buttonToolTip,
            String commandClass,
            BitmapImage largeImage=null,
            BitmapImage smallImage=null)
        {
            //set icons to default if not provided
            if(null == largeImage)
            {
                largeImage = largeIcon;
            }
            if(null == smallImage)
            {
                smallImage = smallIcon;
            }

            PushButtonData buttonData = new PushButtonData(
                name: commandClass + "Button",
                text: buttonText,
                assemblyName: FileLocations.AssemblyPath,
                className: FileLocations.CommandNameSpace + commandClass)
            {
                LargeImage = largeImage,
                Image = smallImage
            };
            var button = pulldown.AddPushButton(buttonData);
            button.ToolTip = buttonToolTip;

            return button;
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
