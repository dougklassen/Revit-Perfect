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

        public List<QuantityScheduleTemplate> LoadTemplates()
        {
            List<QuantityScheduleTemplate> templates = new List<QuantityScheduleTemplate>();
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
                String val = ParseCellToString(headerRow.GetCell(i));

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
            Int32 rowLength = sheet.GetRow(1).Cells.Count;
            QuantityScheduleTemplate currentTemplate = null;
            for (int r = 1; r < sheet.LastRowNum; r++)
            {
                row = sheet.GetRow(r);

                //skip empty rows
                if (row == null)
                {
                    continue;
                }

                #region row analysis
                //if (row.Cells == null || row.Cells.Count < rowLength)
                ////if (true)
                //{
                //    //show a diagnostic message the row
                //    String msg = "Row: " + (r + 1);
                //    msg += "\n physical number of cells: " + row.PhysicalNumberOfCells;
                //    if (row.Collapsed.HasValue)
                //    {
                //        msg += "\n collapsed?: " + row.Collapsed.Value;
                //    }
                //    else
                //    {
                //        msg += "\n collapsed?: no value";
                //    }
                //    msg += "\n first cell num: " + row.FirstCellNum;
                //    msg += "\n last cell num: " + row.LastCellNum;
                //    if (row.Hidden.HasValue)
                //    {
                //        msg += "\n hidden?: " + row.Hidden.Value;
                //    }
                //    else
                //    {
                //        msg += "\n hidden?: no value";
                //    }
                //    msg += "\n outline level: " + row.OutlineLevel;
                //    msg += "\n";
                //    if (row != null)
                //    {
                //        for (int c = 0; c < row.Cells.Count; c++)
                //        {
                //            ICell cell = row.GetCell(c);
                //            String cellVal;
                //            if (cell != null)
                //            {
                //                if (cell.CellType == CellType.String)
                //                {
                //                    cellVal = cell.StringCellValue.Trim();
                //                    if (cellVal == String.Empty)
                //                    {
                //                        cellVal = "Empty String";
                //                    }
                //                }
                //                else if (cell.CellType == CellType.Numeric)
                //                {
                //                    cellVal = cell.NumericCellValue.ToString();
                //                }
                //                else
                //                {
                //                    cellVal = "*" + Enum.GetName(typeof(CellType), cell.CellType) + "*";
                //                }
                //            }
                //            else
                //            {
                //                cellVal = "*NULL*";
                //            }
                //            msg += String.Format("\n   Cell {0}: {1}", c, cellVal);
                //        }
                //    }
                //    else
                //    {
                //        msg += "\n   *NULL ROW*";
                //    }
                //    Autodesk.Revit.UI.TaskDialog.Show("Corrupt Row", msg);
                //    continue;
                //}
                #endregion row analysis

                String filterParamVal = ParseCellToString(row.GetCell(filterParamColumn));
                //if an entry is found in the filterParamColumn start a new template
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
                                templates.Add(currentTemplate);
                            }
                        }
                    }
                    currentTemplate = new QuantityScheduleTemplate();
                    currentTemplate.FilterParameterName = filterParamName;
                    currentTemplate.FilterParameterValue = filterParamVal;
                    String elementCategoryValue = ParseCellToString(row.GetCell(elementCategoryColumn));
                    if (elementCategoryValue != String.Empty)
                    {
                        currentTemplate.ElementCategory = elementCategoryValue;
                    }
                    else
                    {
                        currentTemplate.ElementCategory = null;
                    }
                    String filterParamValueLabel = ParseCellToString(row.GetCell(labelColumn));
                    currentTemplate.FilterParameterValueLabel = filterParamValueLabel;
                }
                //if no new filterParam value is found, parse the row into a new schedule field to be added to the template
                else
                {
                    QuantityScheduleField currentField = new QuantityScheduleField();
                    //parse the label
                    String labelValue = ParseCellToString(row.GetCell(labelColumn));
                    if (labelValue != String.Empty)
                    {
                        currentField.Label = labelValue;
                        String unitsValue = ParseCellToString(row.GetCell(unitsColumn));
                        //only parse units if the field has a label
                        if (unitsValue != String.Empty)
                        {
                            currentField.Units = unitsValue;
                        }
                    }
                    //parse the field 
                    String calculationValue = ParseCellToString(row.GetCell(calculationColumn));
                    String builtInParamValue = ParseCellToString(row.GetCell(builtInParamColumn));
                    String projectParamValue = ParseCellToString(row.GetCell(projectParamColumn));
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

        public static String ParseCellToString(ICell cell)
        {
            String val;

            if (cell == null)
            {
                val = String.Empty;
            }
            else if (cell.CellType == CellType.String)
            {
                val = cell.StringCellValue.Trim();
            }
            else if (cell.CellType == CellType.Numeric)
            {
                val = cell.NumericCellValue.ToString();
            }
            else
            {
                val = String.Empty;
            }

            return val;
        }

        public void WriteTemplates(List<QuantityScheduleTemplate> templates)
        {
            throw new NotImplementedException();
        }
    }

    public class QuantityScheduleParseException : FileFormatException
    {
        public QuantityScheduleParseException(String message)
            : base(message)
        {

        }
    }
}
