using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class StandardizeSchedulesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var dbDoc = uiDoc.Document;
            Regex lengthRegex = new Regex(@"\(ft\)$");
            Regex volumeRegex = new Regex(@"\(CY\)$");
            Regex areaRegex = new Regex(@"\(SF\)$");

            var schedules = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>();

            //var msg = String.Empty;
            //msg += String.Format("Schedules Found: {0}\n", schedules.Count());
            //foreach (var s in schedules)
            //{
            //    msg += String.Format("{0}\n", s.Name);
            //}
            //TaskDialog.Show("Schedules Found", msg);

            using (Transaction t = new Transaction(dbDoc, "Standardize schedules"))
            {
                t.Start();

                foreach (var schedule in schedules)
                {
                    ScheduleDefinition definition = schedule.Definition;
                    //step through each field in the schedule
                    ScheduleField field;
                    for (int f = 0; f < definition.GetFieldCount(); f++)
                    {
                        field = definition.GetField(f);
                        var formatOptions = field.GetFormatOptions();
                        switch (field.UnitType)
                        {
                            case UnitType.UT_Undefined:
                                break;
                            case UnitType.UT_Custom:
                                break;
                            case UnitType.UT_Length:
                                if (!lengthRegex.IsMatch(field.ColumnHeading))
                                {
                                    field.ColumnHeading = field.ColumnHeading + " (ft)";
                                }
                                formatOptions.UseDefault = false;
                                formatOptions.DisplayUnits = DisplayUnitType.DUT_DECIMAL_FEET;
                                normalizeFieldFormat();
                                break;
                            case UnitType.UT_Area:
                                if (!areaRegex.IsMatch(field.ColumnHeading))
                                {
                                    field.ColumnHeading = field.ColumnHeading + " (SF)";
                                }
                                formatOptions.UseDefault = false;
                                formatOptions.DisplayUnits = DisplayUnitType.DUT_SQUARE_FEET;
                                normalizeFieldFormat();
                                break;
                            case UnitType.UT_Volume:
                                if (!volumeRegex.IsMatch(field.ColumnHeading))
                                {
                                    field.ColumnHeading = field.ColumnHeading + " (CY)";
                                }
                                formatOptions.UseDefault = false;
                                formatOptions.DisplayUnits = DisplayUnitType.DUT_CUBIC_YARDS;
                                normalizeFieldFormat();
                                break;
                            case UnitType.UT_Number:
                                break;
                            case UnitType.UT_Currency:
                                break;
                            default:
                                break;
                        }
                        field.SetFormatOptions(formatOptions);

                        void normalizeFieldFormat()
                        {
                            if (FormatOptions.CanHaveUnitSymbol(formatOptions.DisplayUnits))
                            {
                                formatOptions.UnitSymbol = UnitSymbolType.UST_NONE;
                            }
                            if (formatOptions.CanSuppressTrailingZeros())
                            {
                                formatOptions.SuppressTrailingZeros = false;
                            }
                            if (formatOptions.CanSuppressLeadingZeros())
                            {
                                formatOptions.SuppressLeadingZeros = false;
                            }
                            if (formatOptions.CanUsePlusPrefix())
                            {
                                formatOptions.UsePlusPrefix = false;
                            }
                            formatOptions.UseDigitGrouping = false;
                            if (formatOptions.CanSuppressSpaces())
                            {
                                formatOptions.SuppressSpaces = true;
                            }
                            return;
                        }
                    }
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
