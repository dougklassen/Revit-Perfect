﻿using System;
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
            //the standards column heading formats used to label units. If headings don't match these, columns will be relabelled.
            //Regex unitsLabelRegex = new Regex(@"\([a-zA-Z]+\)$");
            Regex lengthRegex = new Regex(@"\(ft\)$|\(lf\)$");
            Regex volumeRegex = new Regex(@"\(cy\)$");
            Regex areaRegex = new Regex(@"\(sf\)$|\(ssf\)$|\(sfca\)$");

            //filter the selection for schedules only
            var schedules = uiDoc.Selection
                .GetElementIds()
                .Select(id => dbDoc.GetElement(id) as ViewSchedule)
                .Where(v => null != v);
                
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

                        FormatOptions formatOptions = field.GetFormatOptions();
                        switch (field.UnitType)
                        {
                            case UnitType.UT_Undefined:
                                break;
                            case UnitType.UT_Custom:
                                break;
                            case UnitType.UT_Length: //set length to display in decimal feet
                                if (!lengthRegex.IsMatch(field.ColumnHeading))
                                {
                                    field.ColumnHeading += " (lf)";
                                }
                                formatOptions.UseDefault = false;
                                formatOptions.DisplayUnits = DisplayUnitType.DUT_DECIMAL_FEET;
                                formatOptions.Accuracy = 0.001;
                                break;
                            case UnitType.UT_Area: //set area to display in square feet
                                if (!areaRegex.IsMatch(field.ColumnHeading))
                                {
                                    field.ColumnHeading += " (sf)";
                                }
                                formatOptions.UseDefault = false;
                                formatOptions.DisplayUnits = DisplayUnitType.DUT_SQUARE_FEET;
                                formatOptions.Accuracy = 0.1;
                                break;
                            case UnitType.UT_Volume: //set volume to display in cubic yards
                                if (!volumeRegex.IsMatch(field.ColumnHeading))
                                {
                                    field.ColumnHeading += " (cy)";
                                }
                                formatOptions.UseDefault = false;
                                formatOptions.DisplayUnits = DisplayUnitType.DUT_CUBIC_YARDS;
                                formatOptions.Accuracy = 0.1;
                                break;
                            case UnitType.UT_Number:
                                break;
                            case UnitType.UT_Currency:
                                formatOptions.UseDefault = false;
                                formatOptions.DisplayUnits = DisplayUnitType.DUT_CURRENCY;
                                formatOptions.Accuracy = 0.01;
                                break;
                            default:
                                break;
                        }

                        //standardize field format
                        if (!formatOptions.UseDefault)
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
                            field.SetFormatOptions(formatOptions); 
                        }
                    }
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
    }

    //run the command only if at least one schedule is selected
    class StandardizeSchedulesCommandAvailability : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            var uiDoc = applicationData.ActiveUIDocument;

            if (null != uiDoc)
            {
                var dbDoc = uiDoc.Document;
                if (0 != uiDoc.Selection.GetElementIds().Count)
                {
                    if (selectedCategories.Contains(Category.GetCategory(dbDoc, BuiltInCategory.OST_Schedules)))
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
}
