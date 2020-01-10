using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CreateQuantityScheduleCommand : IExternalCommand
    {
        private String filterParamColumnName = "TPhase";
        private String labelColumnName = "Description";
        private String unitsColumnName = "Units";
        private String elementCategoryColumnName = "Category";
        private String projectParamColumnName = "Project Parameter";
        private String builtInParamColumnName = "BuiltIn Parameter";
        private String calculationColumnName = "Calculated Field";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            OpenFileDialog openDialog = new OpenFileDialog()
            {
                Filter = "Excel file|*.xlsx",
                Title = "Schedule Template File"
            };
            DialogResult result = openDialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return Result.Cancelled;
            }
            String templateFilePath = openDialog.FileName;
            IQuantityScheduleTemplateRepo repo = new QuantityScheduleTemplateExcelRepo(
                templateFilePath,
                filterParamColumnName,
                labelColumnName,
                unitsColumnName,
                elementCategoryColumnName,
                projectParamColumnName,
                builtInParamColumnName,
                calculationColumnName);

            Dictionary<String, QuantityScheduleTemplate> templates;
            try
            {
                templates = repo.LoadTemplates();
            }
            catch (Exception e)
            {
                String errorFilePath = FileLocations.AddInDirectory + "error" + Helpers.GetTimeStamp() + ".txt";
                String errorMessage = String.Empty;

                errorMessage += "Data:\n";
                foreach (DictionaryEntry de in e.Data)
                {
                    errorMessage += "  " + de.Key.ToString() + ": " + de.Value + "\n";
                }
                errorMessage += "Message:\n" + e.Message + "\n";
                errorMessage += "Stack Trace:\n" + e.StackTrace + "\n";
                File.WriteAllText(errorFilePath, errorMessage);

                TaskDialog.Show(
                    "Create Quantity Schedule Error",
                    "Error reading template file " + templateFilePath + "\nSee error file " + errorFilePath);

                return Result.Failed;
            }

            ChooseScheduleWindow window = new ChooseScheduleWindow(templates);
            String msg = String.Empty;
            msg += String.Format("{0} templates found in file\n", templates.Count);
            foreach (QuantityScheduleTemplate template in templates.Values)
            {
                msg += String.Format("  {0}-{1}: {2} fields\n", template.FilterParameterValue, template.FilterParameterValueLabel, template.Fields.Count);
            }
            window.messageTextBlock.Text = msg;
            Boolean? chooseScheduleResult = window.ShowDialog();
            if (!chooseScheduleResult.Value)
            {
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }
    }
}
