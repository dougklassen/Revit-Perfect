using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

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

            //Set up tab and panel
            String tabName = "DK";
            try //an exception will be thrown if the tab already exists
            {
                application.CreateRibbonTab(tabName);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException)
            {
                //ignore
            }
            RibbonPanel PerfectRibbonPanel = application.CreateRibbonPanel(tabName, "Perfect Standards");

#region Create Column One-Naming, Clean up, Export
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
            addButtonToPulldown(
                pulldown: exportPullDownButton,
                commandClass: "ExportParametersCommand",
                buttonText: "Export Parameter Data",
                buttonToolTip: "Export a catalog of parameters in the project in JSON format");
            addButtonToPulldown(
                pulldown: exportPullDownButton,
                commandClass: "ExportSchedulesCommand",
                buttonText: "Export Schedule Data",
                buttonToolTip: "Export a catalog of schedules in the project in JSON format");
            addButtonToPulldown(
                pulldown: exportPullDownButton,
                commandClass: "ExportCategoriesCommand",
                buttonText: "Export Category Data",
                buttonToolTip: "Export a catalog of categories in the project in JSON format");
            addButtonToPulldown(
                pulldown: exportPullDownButton,
                commandClass: "ExportProjectDataCommand",
                buttonText: "Export Project Data",
                buttonToolTip: "Export a catalog of data about the current project");
    #endregion Export Pulldown

#endregion Create Column One-Naming, Clean up, Export

#region Create Column Two-Geometry, Elements, Schedules
            PulldownButtonData geometryPullDownButtonData = new PulldownButtonData(
                name: "GeometryPullDownButton",
                text: "Fix Geometry");
            PulldownButtonData elementPullDownButtonData = new PulldownButtonData(
                name: "ElementPullDownButton",
                text: "Element Properties");
            PulldownButtonData schedulesPullDownButtonData = new PulldownButtonData(
                name: "SchdeulesPullDownButton",
                text: "Schedules");
            IList<RibbonItem> stackTwo = PerfectRibbonPanel.AddStackedItems(
				geometryPullDownButtonData,
				elementPullDownButtonData,
                schedulesPullDownButtonData);

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
            addButtonToPulldown(
                pulldown: geometryPullDownButton,
                commandClass: "SplitWallByLevelCommand",
                buttonText: "Split Wall by Level",
                buttonToolTip: "Split wall by intervening levels");
    #endregion Geometry Pulldown

    #region Elements Pulldown
            PulldownButton elementPullDownButton = (PulldownButton)stackTwo[1];
            addButtonToPulldown(
                pulldown: elementPullDownButton,
                commandClass: "FlagUnitElementsCommand",
                buttonText: "Flag Unit Elements",
                buttonToolTip: "Flag elements that are part of a unit group");
            addButtonToPulldown(
                pulldown: elementPullDownButton,
                commandClass: "CommentAddCommand",
                buttonText: "Add Comment",
                buttonToolTip: "Add space delimited tags to the commment parameter of selected elements");
            addButtonToPulldown(
                pulldown: elementPullDownButton,
                commandClass: "CommentRemoveCommand",
                buttonText: "Remove Comment",
                buttonToolTip: "Remove space delimited tags from the commment parameter of selected elements");
    #endregion Elements Pulldown

    #region Schedules Pulldown
            PulldownButton schedulesPullDOwnButton = (PulldownButton)stackTwo[2];
            addButtonToPulldown(
                pulldown: schedulesPullDOwnButton,
                commandClass: "StandardizeSchedulesCommand",
                buttonText: "Standardize Schedule Formating",
                buttonToolTip: "Implement schedule formatting standards for export to Excel",
                commandAvailability: "StandardizeSchedulesCommandAvailability");
            addButtonToPulldown(
                pulldown: schedulesPullDOwnButton,
                commandClass: "CreateQuantityScheduleCommand",
                buttonText: "Create a Quantity Schedule",
                buttonToolTip: "Create a standardized quantity schedule using a configuration template");
    #endregion Schedules Pulldown

#endregion Create Column Two-Geometry, Elements, Schedules

#region Create slide out panel-About
            PerfectRibbonPanel.AddSlideOut();
            PushButtonData aboutCommandPushButtonData = new PushButtonData(
                name: "AboutCommandButton",
                text: "About",
                assemblyName: FileLocations.AssemblyPath,
                className: "DougKlassen.Revit.Perfect.Commands.AboutCommand")
            {
                LargeImage = largeIcon,
                Image = smallIcon,
                AvailabilityClassName = "DougKlassen.Revit.Perfect.Commands.AlwaysAvailableCommandAvailability"
            };
            PerfectRibbonPanel.AddItem(aboutCommandPushButtonData);
#endregion Create slide out panel-About

#region Viz Reset Panel
            RibbonPanel ResetRibbonPanel = application.CreateRibbonPanel(tabName, "Reset View Overrides");

            PushButtonData resetHiddenCommandPushButtonData = new PushButtonData(
                 "resetHiddenCommandButton", //name of the button
                 "Reset Hidden", //text on the button
                 FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                 "DougKlassen.Revit.Perfect.Commands.ResetHiddenCommand");
            resetHiddenCommandPushButtonData.LargeImage = largeIcon;
            resetHiddenCommandPushButtonData.ToolTip = "Unhide all elements hidden in the current view";

            PushButtonData resetGraphicsCommandPushButtonData = new PushButtonData(
                "resetGraphicsCommandButton", //name of the button
                "Reset Graphics", //text on the button
                FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                "DougKlassen.Revit.Perfect.Commands.ResetGraphicsCommand");
            resetGraphicsCommandPushButtonData.LargeImage = largeIcon;
            resetGraphicsCommandPushButtonData.ToolTip = "Reset all visibility graphics overrides in the current view";

            ResetRibbonPanel.AddItem(resetHiddenCommandPushButtonData);
            ResetRibbonPanel.AddItem(resetGraphicsCommandPushButtonData);
#endregion Viz Reset Panel

#region Viz Apply Styles Panel
            RibbonPanel ApplyStylesPanel = application.CreateRibbonPanel(tabName, "Apply Override Styles");

            PushButtonData pickupStyleCommandPushButtonData = new PushButtonData(
                 "pickupStyleCommandButton", //name of the button
                 "Style Eyedropper", //text on the button
                 FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                 "DougKlassen.Revit.Perfect.Commands.PickupStyleCommand");
            pickupStyleCommandPushButtonData.LargeImage = largeIcon;
            pickupStyleCommandPushButtonData.ToolTip = "Choose an override style to apply to other elements";
            pickupStyleCommandPushButtonData.AvailabilityClassName = "DougKlassen.Revit.Perfect.Commands.OverrideableViewCommandAvailability";
            ApplyStylesPanel.AddItem(pickupStyleCommandPushButtonData);

            PushButtonData applyStyleCommandPushButtonData = new PushButtonData(
                 "applyStyleCommandButton", //name of the button
                 "Apply Style", //text on the button
                 FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                 "DougKlassen.Revit.Perfect.Commands.ApplyStyleCommand");
            applyStyleCommandPushButtonData.LargeImage = largeIcon;
            applyStyleCommandPushButtonData.ToolTip = "Apply an override style to selected elements";
            applyStyleCommandPushButtonData.AvailabilityClassName = "DougKlassen.Revit.Perfect.Commands.OverrideableViewCommandAvailability";
            ApplyStylesPanel.AddItem(applyStyleCommandPushButtonData);
#endregion Viz Apply Styles Panel

#region Viz Manage Callouts Panel
            RibbonPanel ManageCalloutsPanel = application.CreateRibbonPanel(tabName, "Manage View Callouts");

            PushButtonData filterBugsCommandPushButtonData = new PushButtonData(
                 "filterCalloutsCommandButton", //name of the button
                 "Filter View Callouts", //text on the button
                 FileLocations.AddInDirectory + FileLocations.AssemblyName + ".dll",
                 "DougKlassen.Revit.Perfect.Commands.FilterCalloutsCommand");
            filterBugsCommandPushButtonData.LargeImage = largeIcon;
            filterBugsCommandPushButtonData.ToolTip = "Filter Callouts for the current view";
            filterBugsCommandPushButtonData.AvailabilityClassName = "DougKlassen.Revit.Perfect.Commands.SheetCommandAvailability";
            ManageCalloutsPanel.AddItem(filterBugsCommandPushButtonData);
#endregion Viz Manage Callouts Panel

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

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Always make a command available, including when no project is open
    /// </summary>
    class AlwaysAvailableCommandAvailability : IExternalCommandAvailability
    {
        public Boolean IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return true;
        }
    }

    /// <summary>
    /// Make command available if a view that allows graphic overrides is active
    /// </summary>
    public class OverrideableViewCommandAvailability : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            //check if a document is open
            if (applicationData.ActiveUIDocument != null)
            {
                //check if there's an active view
                if (applicationData.ActiveUIDocument.ActiveView != null)
                {
                    //check if graphic overrides are allowed
                    if (applicationData.ActiveUIDocument.ActiveView.AreGraphicsOverridesAllowed())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Make command available if the current view is a sheet or the current selection contains at least one sheet
    /// </summary>
    public class SheetCommandAvailability : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            UIDocument uiDoc = applicationData.ActiveUIDocument;
            Document dbDoc = applicationData.ActiveUIDocument.Document;

            if(
                selectedCategories.Contains(Category.GetCategory(dbDoc, BuiltInCategory.OST_Sheets)) ||
                uiDoc.ActiveView is ViewSheet)
            {
                return true;
            }
            else
            {
                return false;
            }    
        }
    }
}