using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace DougKlassen.Revit.Perfect.Commands
{
    class QuantityScheduleTemplateExcelRepo : IQuantityScheduleTemplateRepo
    {
        private String filePath;
        private String filterParamColumnName;
        private String labelColumnName;
        private String unitsColumnName;
        private String elementCategoryColumnName;
        private String projectParamColumnName;
        private String builtInParamColumnName;
        private String calculationColumnName;

        private Int32 filterParamColumn = -1;
        private Int32 labelColumn = -1;
        private Int32 unitsColumn = -1;
        private Int32 elementCategoryColumn = -1;
        private Int32 projectParamColumn = -1;
        private Int32 builtInParamColumn = -1;
        private Int32 calculationColumn = -1;

        public QuantityScheduleTemplateExcelRepo(
            String repoFilePath,
            String repoFilterParamColumn,
            String repoLabelColumn,
            String repoUnitsColumn,
            String repoElementCategoryColumn,
            String repoProjectParamColumn,
            String repoBuiltInParamColumn,
            String repoCalculationColumn)
        {
            filePath = repoFilePath;
            filterParamColumnName = repoFilterParamColumn;
            labelColumnName = repoLabelColumn;
            unitsColumnName = repoUnitsColumn;
            elementCategoryColumnName = repoElementCategoryColumn;
            projectParamColumnName = repoProjectParamColumn;
            builtInParamColumnName = repoBuiltInParamColumn;
            calculationColumnName = repoCalculationColumn;
        }

        public Dictionary<String, QuantityScheduleTemplate> LoadTemplates()
        {
            Dictionary<String, QuantityScheduleTemplate> templates = new Dictionary<String, QuantityScheduleTemplate>();
            Dictionary<Int32, String> parseErrors = new Dictionary<Int32, String>();

            IWorkbook workBook;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                workBook = new XSSFWorkbook(fs);
            }

            ISheet sheet = workBook.GetSheetAt(0);
            //Find the columns specified by column names. Columns are identified by their first row header titles
            IRow headerRow = sheet.GetRow(0);
            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                String val;
                try
                {
                    val = headerRow.GetCell(i).StringCellValue.Trim();
                }
                catch (Exception e)
                {
                    throw new Exception("At header row, cell " + i, e);
                }
                if (val == filterParamColumnName)
                {
                    filterParamColumn = i;
                }
                else if (val == labelColumnName)
                {
                    labelColumn = i;
                }
                else if (val == unitsColumnName)
                {
                    unitsColumn = i;
                }
                else if (val == elementCategoryColumnName)
                {
                    elementCategoryColumn = i;
                }
                else if (val == projectParamColumnName)
                {
                    projectParamColumn = i;
                }
                else if (val == builtInParamColumnName)
                {
                    builtInParamColumn = i;
                }
                else if (val == calculationColumnName)
                {
                    calculationColumn = i;
                }
            }
            if (
                filterParamColumn == -1 ||
                labelColumn == -1 ||
                unitsColumn == -1 ||
                elementCategoryColumn == -1 ||
                projectParamColumn == -1 ||
                builtInParamColumn == -1 ||
                calculationColumn == -1)
            {
                throw new QuantityScheduleParseException("A required column was not found in the spreadsheet");
            }
            //The filterParamName is defined in the header of the filterParamColumn
            String filterParamName = headerRow.GetCell(filterParamColumn).StringCellValue.Trim();

            //iterate through all rows after the heading row, starting a new template each time a new filter parameter value is encountered
            IRow row;
            QuantityScheduleTemplate currentTemplate = null;
            for (int r = 1; r < sheet.LastRowNum; r++)
            {
                try
                {
                    row = sheet.GetRow(r);
                    String filterParamVal = row.GetCell(filterParamColumn).StringCellValue.Trim();
                    //if an entry is found in this cell start a new template
                    if (filterParamVal != String.Empty)
                    {
                        //add the last template found to the dictionary. currentTemplate is null when no template has been parsed yet.
                        if (currentTemplate != null)
                        {
                            //skip instances where ElementCategory is undefined
                            if (currentTemplate.ElementCategory != null)
                            {
                                //skip instances where no fields are defined
                                if (currentTemplate.Fields.Count > 0)
                                {
                                    templates.Add(currentTemplate.FilterParameterValue, currentTemplate);  
                                }
                            }
                        }
                        currentTemplate = new QuantityScheduleTemplate();
                        currentTemplate.FilterParameterName = filterParamName;
                        currentTemplate.FilterParameterValue = filterParamVal;
                        String elementCategoryValue = row.GetCell(elementCategoryColumn).StringCellValue.Trim();
                        if (elementCategoryValue != String.Empty)
                        {
                            currentTemplate.ElementCategory = elementCategoryValue;
                        }
                        else
                        {
                            currentTemplate.ElementCategory = null;
                        }
                        String filterParamValueLabel = row.GetCell(labelColumn).StringCellValue.Trim();
                        currentTemplate.FilterParameterValueLabel = filterParamValueLabel;
                    }
                    //if value if found, parse the row into a new schedule field to be added to the template
                    else
                    {
                        QuantityScheduleField currentField = new QuantityScheduleField();
                        //parse the label
                        String labelValue = row.GetCell(labelColumn).StringCellValue.Trim();
                        if (labelValue != String.Empty)
                        {
                            currentField.Label = labelValue;
                            String unitsValue = row.GetCell(unitsColumn).StringCellValue.Trim();
                            //only parse units if the field has a label
                            if (unitsValue != String.Empty)
                            {
                                currentField.Units = unitsValue;
                            }
                        }
                        //parse the field 
                        String calculationValue = row.GetCell(calculationColumn).StringCellValue.Trim();
                        String builtInParamValue = row.GetCell(builtInParamColumn).StringCellValue.Trim();
                        String projectParamValue = row.GetCell(projectParamColumn).StringCellValue.Trim();
                        Int32 sourcesInRow = 0; //track how many values are present
                        if (projectParamValue != String.Empty)
                        {
                            sourcesInRow++;
                            currentField.FieldValue = projectParamValue;
                            currentField.Type = FieldType.ProjectParameter;
                        }
                        if (builtInParamValue != String.Empty)
                        {
                            sourcesInRow++;
                            currentField.FieldValue = builtInParamValue;
                            currentField.Type = FieldType.BuiltInParameter;
                        }
                        if (calculationValue != String.Empty)
                        {
                            sourcesInRow++;
                            currentField.FieldValue = calculationValue;
                            currentField.Type = FieldType.Calculation;
                        }

                        //check how many values are present. There should be no more than one
                        if (sourcesInRow == 1)
                        {
                            currentTemplate.Fields.Add(currentField);
                        }
                        else if (sourcesInRow > 1)
                        {
                            parseErrors.Add(r, "Only one source can be specified for a field");
                        }
                        //if sourcesInRow == 0, don't add the row
                    }
                }
                catch (Exception e)
                {
                    e.Data.Add("Current row", r);
                    e.Data.Add("Stack Trace", e.StackTrace.Split('\n')[0]);
                    throw e;
                }
            }

            //if no parse errors occured, return the list of templates
            if (parseErrors.Count == 0)
            {
                return templates;
            }
            //if parse errors occured, generate an error file and throw an exception
            else
            {
                String msg = String.Empty;
                foreach (Int32 rowNum in parseErrors.Keys)
                {
                    msg += String.Format("  Row {0}: {1}\n", rowNum, parseErrors[rowNum]);
                }

                throw new QuantityScheduleParseException(msg);
            }
        }

        public void WriteTemplates(Dictionary<String, QuantityScheduleTemplate> templates)
        {
            throw new NotImplementedException();
        }
    }

    public class QuantityScheduleParseException : FileFormatException
    {
        public QuantityScheduleParseException(String message)
            :base(message)
        {

        }
    }
}
