using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using Excel = Microsoft.Office.Interop.Excel;
using CsvHelper;

namespace Geophy
{
    public class Methods
    {
        // List keep tracking any change in the original file and it's not binding to bindingsource
   
        public static Dictionary<int, List<DataImport>> LinesCollection = new Dictionary<int, List<DataImport>>();

        #region File    
        

        // Add data to an existing list
        // Summary:
        //      import a new file and attach it to an existing list of data 
        // Parameters:
        //      list of data import elements that represent the existing data
        //      string of file path of a new file
        // Returns:
        //      collection of data import class elements
        public static List<DataImport> AddDataToList(List<DataImport> existData, List<DataImport> newData)
        {
         
            List<DataImport> diffList = GetDifferenceBetweenTwoLists(newData, existData);
            existData.AddRange(diffList);
         
            return existData;

        }

        // Open file dialog to get a file path
        // Summary:
        //      Open file dialog, allow user to choose only csv file and then 
        //      get the file path of a chosen file
        // Parameters:
        //      None
        // Returns:
        //      string of file path
        public static string OpenFile()
        {
            string filePath = null;
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Title = "Open File";
                openFile.Filter = "CSV (Comma delimited) (*.csv) | *.csv";
                openFile.RestoreDirectory = true;
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFile.FileName;
                }

            }

            return filePath;
        }

        #endregion

        #region Edit
        // Filter list of Data Import by name property
        // Summary:
        //      From a collection of data import get a code in the name property
        //      that can distinguish the elements in the list based on the different acquisition line
        //      and then create new list by all elements belong to the same line.
        // Parameters:
        //      integer index that represents the index of original list from where the 
        //      method has to start filtering
        //      array of data import elements. Array instead list to keep a fixed lenght in the for loop
        // Returns:
        //      collection of data import elements
        public static List<DataImport> FilterByName(int index, List<DataImport> originalList)
        {
           
            List<DataImport> outputList = new List<DataImport>();
            string codeName = originalList[index].EditNameProperty()[0];
            int i = index;
            for (i = index; i < originalList.Count; i++)
            {
                if (originalList[i].EditNameProperty()[0].Equals(codeName))
                {
                    outputList.Add(originalList[i]);
                }
            }

            return outputList;

        }

        

        public static Dictionary<int, List<DataImport>> GetCollectionOfLines(List<DataImport> originList)
        {
            int trackIndex = 0;
            Dictionary<int, List<DataImport>> collectionList = new Dictionary<int, List<DataImport>>();
            originList.Sort(DataImport.SortNameAscending());
            List<DataImport> tempList = new List<DataImport>();
            int index = 1;
            while (trackIndex < originList.Count)
            {
                tempList = FilterByName(trackIndex, originList);
                collectionList.Add(index, tempList);
                trackIndex += tempList.Count;
                index++;
            }
            return collectionList;
        }

        // Check out all the missing data inside a single already filtered line based on the name property
        // Summary:
        //      In the single line visualized on dataGridView, check first if the first element of line
        //      is equal to 1 (the side on the right of dot in the name property), 
        //      if it's not it asks to user if wants to add the first element. Then asks to the user 
        //      if the last element is the correct one. And the finally adds all missing elements 
        //      inside the line based on succesive numbers from first element to the last.
        // Parameters:
        //      dataGridView that represents the current visualization on the application
        //      boolean variable that allows to switch the operation whether the line is in ascending order
        //      or in descending order
        // Returns:
        //      None
        public static void CheckMissingDataInSingleLine(DataGridView dgv, int indexOfLine)
        {
            DialogResult result;
            BindingSource bs = new BindingSource();
            bs.DataSource = dgv.DataSource;
            dgv.DataSource = bs;
            LinesCollection[indexOfLine].Sort(DataImport.SortNameAscending());
            List<DataImport> viewLine = LinesCollection[indexOfLine];
            
            bs.DataSource = viewLine;

        
            
            int firstElement = int.Parse(viewLine.First().EditNameProperty()[1]);
            int lastElement = int.Parse(viewLine.Last().EditNameProperty()[1]);
            int countMissingPoints = 0;
            if (firstElement != 1)
            {
                result = CustomMessageForm.CustomMessageBox.Show("Data Check", "Missing the first data point of line. Would you like to add it?", "Yes", "No");
                if (result == DialogResult.Yes)
                {
                    for (int i = 1; i < firstElement; i++)
                    {
                        bs.Insert(i - 1, new DataImport(viewLine.First().EditNameProperty()[0] + "." + i.ToString(), 0, 0, 0, null, null, null));
                        
                        countMissingPoints++;
                        dgv.Rows[i - 1].DefaultCellStyle.BackColor = System.Drawing.Color.Aquamarine;
                    }
                }
            }
            result = CustomMessageForm.CustomMessageBox.Show("Data Check",
                    string.Format("The last data point is {0}. Would you like to add more data points?",
                    (viewLine.Last().EditNameProperty()[0] + "." +
                    lastElement.ToString())), "Yes", "No");

            if (result == DialogResult.Yes)
            {
                DialogResult resText;
                int lastPoint;
                bool checkLastPoint;
                do
                {
                    InsertTextForm insForm = new InsertTextForm("Insert Data", "Insert the actual last point of line");
                    resText = insForm.Show();
                    checkLastPoint = int.TryParse(insForm.InputData, out lastPoint);
                }
                while (checkLastPoint == false);

                if (resText == DialogResult.OK)
                {

                    for (int i = lastElement + 1; i <= lastPoint; i++)
                    {
                        DataImport tempData = new DataImport((viewLine.First().EditNameProperty()[0] + "." + i.ToString()), 0, 0, 0, null, null, null);
                        bs.Add(tempData);
                        
                        countMissingPoints++;
                        dgv.Rows[viewLine.Count - 1].DefaultCellStyle.BackColor = System.Drawing.Color.Aquamarine;
                    }
                }

            }


            for (int i = 0; i < viewLine.Count - 1; i++)
            {

                if ((int.Parse(viewLine[i + 1].EditNameProperty()[1]) -
                    int.Parse(viewLine[i].EditNameProperty()[1])) != 1)
                {
                    bs.Insert(i + 1, new DataImport((viewLine[i].EditNameProperty()[0] +
                        "." + (int.Parse(viewLine[i].EditNameProperty()[1]) + 1).ToString()),
                        0, 0, 0, null, null, null));
                    
                    countMissingPoints++;
                    dgv.Rows[i + 1].DefaultCellStyle.BackColor = System.Drawing.Color.Aquamarine;
                }
            }

            MessageBox.Show(string.Format("Added {0} data points", countMissingPoints), "Data Check",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            bs.DataSource = null;
            dgv.DataSource = viewLine;
            LinesCollection[indexOfLine] = viewLine;
            
        }
            
       



    

        // Insert Data in the single line automatically based on linear series formula
        // Summary:
        //      First check if the first and last element do have values on their properties
        //      then it counts how many points are with no value between two points with values.
        //      Calculate value for the zero-value point interpolating the values of 
        //      points above it and below it on the line
        // Parameters:
        //      collection fo data import that represents the acquisition line
        //      data grid view to visualize the data instantaneously.
        // Returns:
        //      None 
        public static void InsertDataAutomatically(Dictionary<int, List<DataImport>> collLines, int index)
        {
            // Checks first if the first and last point have data.
            // Then start to traverse from second element until it finds some element without data (store the previous element with data).
            // Then from the first element without data, traverse again from this point up to the next point with data (and store this point with data).
            // After data insert data in the empty elements that have been found.
            List<DataImport> line = collLines[index];
            if (line.First().Easting != 0 && line.First().Northing != 0 && line.First().Elevation != -1 && line.Last().Easting != 0
                && line.Last().Northing != 0 && line.Last().Elevation != -1)
            {
                int countMissingValue = 0;
                int i;
                for (i = 1; i < line.Count - 1; i++)
                {
                    if (line[i].Easting == 0 && line[i].Northing == 0 && line[i].Elevation == -1)
                    {
                        DataImport previousPoint = line[i - 1];
                        countMissingValue++;
                        for (int j = i + 1; j < line.Count; j++)
                        {

                            if (line[j].Easting != 0 && line[j].Northing != 0 && line[j].Elevation != -1)
                            {
                                DataImport nextPoint = line[j];
                                for (int k = i; k < j; k++)
                                {
                                    // TEMPORARY FORMULA. WILL BE CHANGED WITH A PROPER ONE
                                    line[k].Easting = (previousPoint.Easting - nextPoint.Easting) / countMissingValue;
                                    line[k].Northing = (previousPoint.Northing - nextPoint.Northing) / countMissingValue;
                                    line[k].Elevation = (previousPoint.Elevation - nextPoint.Elevation) / countMissingValue;

                                }
                                countMissingValue = 0;
                                i = j;
                                break;
                            }
                            countMissingValue++;
                        }


                    }
                }
            }
            else
            {
                MessageBox.Show(string.Format("Enter first data manually for {0} and/or {1}!", line.First().Name, line.Last().Name), "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            
            collLines[index] = line;
        }



        #endregion

        #region Save
        public static void ExportToCsv(List<DataImport> listToExport, string filePath)
        {
            using (StreamWriter text = new StreamWriter(filePath))
            {
                using (var saveFile = new CsvWriter(text))
                {
                    saveFile.WriteRecords(listToExport);
                }

            }
        }

        // Export file to excel using Microsoft Office Interop...bit slow operation
        // Summary:
        //      Create an excel application, create an empty workbook and a worksheet
        //      then first creates an header and then add all files in the list to the worksheet.
        //      Save the file and release objects
        // Parameters:
        //      collection of data import records to save
        //      string that represents the file path that user chooses for saving the file
        public static void ExportToExcel(List<DataImport> listToExport, string fileName)
        {
            // Load Excel application
            Excel.Application excel = new Excel.Application();

            // Create empty workbook
            excel.Workbooks.Add();

            // Create Worksheet from active sheet
            Excel._Worksheet workSheet = excel.ActiveSheet;

            // Created Application and Worksheet objects before try/catch,
            // so that they can be closed in finally block.

            try
            {
                // ------------------------------------------------
                // Creation of header cells
                // ------------------------------------------------
                workSheet.Cells[1, "A"] = "Name";
                workSheet.Cells[1, "B"] = "Easting";
                workSheet.Cells[1, "C"] = "Northing";
                workSheet.Cells[1, "D"] = "Elevation";
                workSheet.Cells[1, "E"] = "Code";
                workSheet.Cells[1, "F"] = "Description1";
                workSheet.Cells[1, "G"] = "Description2";
                // ------------------------------------------------
                // Populate sheet with some real data from "cars" list
                // ------------------------------------------------
                int row = 2; // start row (in row 1 are header cells)
                foreach (DataImport record in listToExport)
                {
                    workSheet.Cells[row, "A"] = record.Name;
                    workSheet.Cells[row, "B"] = record.Easting;
                    workSheet.Cells[row, "C"] = record.Northing;
                    workSheet.Cells[row, "D"] = record.Elevation;
                    workSheet.Cells[row, "E"] = record.Code;
                    workSheet.Cells[row, "F"] = record.Description1;
                    workSheet.Cells[row, "G"] = record.Description2;
                    row++;

                }


                workSheet.SaveAs(fileName);


            }
            catch (Exception exception)
            {
                MessageBox.Show("Error", "There was a problem saving Excel file!" +
                     Environment.NewLine + exception.Message,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Quit Excel application
                excel.Quit();

                // Release COM objects 
                if (excel != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);

                if (workSheet != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet);

                // Empty variables
                excel = null;
                workSheet = null;

                // Force garbage collector cleaning
                GC.Collect();
            }
        }

        // Import Csv file using Csv helpers library
        // Summary:
        //      create a stream reader variable to instantiate a csv reader
        //      while the csv reader is reading the file it extracts the fields
        //      and validating the fields and adding the good records to the list to export
        // Parameters: 
        //      string of path of file to import in the application
        // Returns:
        //      collection of records of data import type
        public static List<DataImport> ImportCSVFile(string filePath)
        {
            List<DataImport> output = new List<DataImport>();
            string message = "Cannot open file. File doesn't seem to be in the correct format.";
            if (filePath != null)
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    using (var csv = new CsvReader(reader))
                    {
                        try
                        {

                            csv.Configuration.HasHeaderRecord = false;

                            while (csv.Read())
                            {
                                csv.Configuration.SkipEmptyRecords = true;
                                double easting;
                                double northing;
                                double elevation;
                                string code = string.Empty;
                                string description1 = string.Empty;
                                string description2 = string.Empty;

                                //if the file doesn't not contain easting, northing and elevation
                                // throw an exception
                                if (csv.CurrentRecord.Length < 4)
                                {
                                    message = "Cannot Open File. Important Data is missing.";
                                    throw new FormatException();
                                   
                                }
                                string name = csv.GetField(0);
                                bool checkEasting = csv.TryGetField(1, out easting);
                                bool checkNorthing = csv.TryGetField(2, out northing);
                                bool checkElevation = csv.TryGetField(3, out elevation);


                                if (csv.CurrentRecord.Length >= 7)
                                {
                                    code = csv.GetField(4);
                                    description1 = csv.GetField(5);
                                    description2 = csv.GetField(6);
                                }
                                else if (csv.CurrentRecord.Length >= 6)
                                {
                                    code = csv.GetField(4);
                                    description1 = csv.GetField(5);
                                }

                                else if (csv.CurrentRecord.Length >= 5)
                                {
                                    code = csv.GetField(4);
                                }

                                // Check if there is header and do not add it to the list
                                if (csv.Row == 1)
                                {
                                    if (!string.IsNullOrEmpty(name) && checkEasting == false &&
                                    checkElevation == false && checkNorthing == false &&
                                    !string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(description1) &&
                                    !string.IsNullOrEmpty(description2))
                                    {
                                        name = null;
                                    }
                                }

                                // if the conversion of easting, northing and elevation fails throw an exception
                                // It means something wrong in the origin file or not the proper file
                                else if (checkEasting == false || checkElevation == false || checkNorthing == false)
                                {
                                    message = "Cannot Open File. File not formatted correctly or corrupted.";
                                    throw new FormatException();
                                    
                                }

                                // Skip eventually record that contains PRS. It's not data
                                if (!string.IsNullOrEmpty(name) && !name.Contains("PRS") && name.Length <= 10 && checkEasting && checkNorthing && checkElevation)
                                {
                                    DataImport data = new DataImport(name, easting, northing, elevation, code, description1, description2);

                                    output.Add(data);
                                }

                            }
                        }
                        catch(FormatException)
                        {
                            MessageBox.Show(message, "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        
                        catch(Exception)
                        {
                            MessageBox.Show("Cannot Open file. Something went wrong!", "Error", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
           
            return output;
        }

      

        public static List<DataImport> FlattenLists(Dictionary<int, List<DataImport>> collecLines)
        {
            List<DataImport> flatList = new List<DataImport>();
            foreach (var value in collecLines.Values)
            {
                List<DataImport> diffList = GetDifferenceBetweenTwoLists(value, flatList);
                flatList.AddRange(diffList);
            }

            return flatList;
        }

        public static List<DataImport> InsertRowWithDefaultData(List<DataImport> tempList, int index)
        {
            if(!tempList[index].EditNameProperty()[1].Equals(string.Empty))
            {
                int pointNumber = int.Parse(tempList[index].EditNameProperty()[1]) - 1;

                if (pointNumber > 0)
                {
                    DataImport tempData = new DataImport(tempList[0].EditNameProperty()[0] + "." + pointNumber.ToString(), 0, 0, 0, null, null, null);

                    if (index == tempList.IndexOf(tempList.First()))
                    {
                        tempList.Insert(index, tempData);
                    }
                    else if (int.Parse(tempList[index - 1].EditNameProperty()[1]) == pointNumber)
                    {
                        DialogResult result = MessageBox.Show(string.Format("Data point {0} already existed! Do you want to use the same name for this point?", tempList[index - 1].Name),
                                     "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            tempList.Insert(index, tempData);
                        }
                        else if (result == DialogResult.No)
                        {
                            tempData.Name = tempList[0].EditNameProperty()[0] + ".";
                            tempList.Insert(index, tempData);
                        }
                    }
                    else
                    {
                        tempList.Insert(index, tempData);
                    }
                }

                else
                {
                    MessageBox.Show("No possible insert element. First data point must be greater than zero", "Information", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            

            return tempList;
        }
        #endregion

        public static bool AreListsContainSameElements(List<DataImport> firstList, List<DataImport> secondList)
        {
            if(firstList != null && secondList != null && firstList.Count == secondList.Count)
            {
          
                List<DataImport> differenceList = GetDifferenceBetweenTwoLists(firstList, secondList);

                if(differenceList.Count == 0)
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

        public static bool IsTheListInDescendingOrder(List<DataImport> inputList)
        {
            if(String.Compare(inputList.Last().Name, inputList.First().Name) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
           
        }

        private static List<DataImport> GetDifferenceBetweenTwoLists(List<DataImport> inputList, List<DataImport> containerList)
        {
            List<DataImport> differenceList = new List<DataImport>();
            if (inputList != null && containerList != null)
            {

                differenceList = ((IEnumerable)(inputList.Where(x => !containerList.Any(y =>
                     y.Name == x.Name && y.Northing == x.Northing && y.Easting == x.Easting &&
                     y.Elevation == x.Elevation)))).Cast<DataImport>().ToList(); 

           
            }

            return differenceList;

        }
    }
}
