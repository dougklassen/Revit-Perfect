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

        /// <summary>
        /// Run on startup. Set up the ribbon UI
        /// </summary>
        /// <param name="application">A Reference to the Revit UI</param>
        /// <returns>Whether the application sucessfully started up</returns>
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

            #region Naming Standards Pulldown
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
            #endregion Naming Standards Pulldown

            #region Clean Up Pull Down
            PulldownButton cleanUpPullDownButton = (PulldownButton)stackOne[1];
            addButtonToPulldown(
                pulldown: cleanUpPullDownButton,
                commandClass: "PurgeLinePatternsCommand",
                buttonText: "Purge Line Patterns",
                buttonToolTip: "Purge line patterns using regular expression matches");
            addButtonToPulldown(
                pulldown: cleanUpPullDownButton,
                commandClass: "PurgeRefPlanesCommand",
                buttonText: "Purge Reference Planes",
                buttonToolTip: "Purge unlabelled reference planes");
            addButtonToPulldown(
                pulldown: cleanUpPullDownButton,
                commandClass: "PurgeViewsCommand",
                buttonText: "Purge Views",
                buttonToolTip: "Purge unnamed views");

            #endregion Clean Up Pull Down

            #region Export Pulldown
            PulldownButton exportPullDownButton = (PulldownButton)stackOne[2];
            addButtonToPulldown(
                pulldown: exportPullDownButton,
                commandClass: "ExportImportStylesCommand",
                buttonText: "Export CAD Import Styles",
                buttonToolTip: "Export CAD import styles");
            addButtonToPulldown(
                pulldown: exportPullDownButton,
                commandClass: "LoadImportStylesCommand",
                buttonText: "Load CAD Import Styles",
                buttonToolTip: "Load CAD import styles");
            addButtonToPulldown(
                pulldown: exportPullDownButton,
                commandClass: "ExportDetailTextCommand",
                buttonText: "Export Callout Text for Review",
                buttonToolTip: "Export callout text to a CSV file for review");
            #endregion Export Pulldown

            #endregion Create Column One

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

            #region Geometry Pulldown
            PulldownButton geometryPullDownButton = (PulldownButton)stackTwo[0];
            addButtonToPulldown(
                pulldown: geometryPullDownButton,
                commandClass: "UnflipCommand",
                buttonText: "Unflip Windows",
                buttonToolTip: "Unflip windows that have been flipped on both axes",
                commandAvailability: "UnflipCommandAvailability");
            addButtonToPulldown(
                pulldown: geometryPullDownButton,
                commandClass: "DisallowWallJoinsCommand",
                buttonText: "Dissallow Wall Joins",
                buttonToolTip: "Turn off wall joins for all selected walls",
                commandAvailability: "DisallowWallJoinsCommandAvailability");
            #endregion Geometry Pulldown

            #region Elements Pulldown
            PulldownButton elementPullDownButton = (PulldownButton)stackTwo[1];
            addButtonToPulldown(
                pulldown: elementPullDownButton,
                commandClass: "FlagUnitElementsCommand",
                buttonText: "Flag Unit Elements",
                buttonToolTip: "Flag elements that are part of a unit group");
            #endregion Elements Pulldown

            #endregion Create Column Two

            #region Create slide out panel
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
        /// Helper method to add a button to a pulldown
        /// </summary>
        /// <param name="pulldown"></param>
        /// <param name="commandClass"></param>
        /// <param name="buttonText"></param>
        /// <param name="buttonToolTip"></param>
        /// <param name="largeImage"></param>
        /// <param name="smallImage"></param>
        /// <param name="commandAvailability"></param>
        /// <returns>A reference to the button that was created</returns>
        PushButton addButtonToPulldown(
            PulldownButton pulldown,
            String buttonText,
            String buttonToolTip,
            String commandClass,
            String commandAvailability=null,
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

            if (null != commandAvailability)
            {
                button.AvailabilityClassName = FileLocations.CommandNameSpace + commandAvailability;
            }

            return button;
        }

        /// <summary>
        /// Run on shutdown
        /// </summary>
        /// <param name="application">A reference to the Revit UI</param>
        /// <returns></returns>
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
