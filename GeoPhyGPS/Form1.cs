using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Diagnostics;

namespace Geophy
{
    public partial class GeophyData : Form
    {
        // List of data file import from CSV
        List<DataImport> WholeDataFile = new List<DataImport>();       

        DataGridViewCell clickedCell;
        private string filePath;
       
     
        Stack<Dictionary<int, List<DataImport>>> Undo = new Stack<Dictionary<int, List<DataImport>>>();
        Stack<Dictionary<int, List<DataImport>>> Redo = new Stack<Dictionary<int, List<DataImport>>>();

        Dictionary<int, List<DataImport>> supportList = new Dictionary<int, List<DataImport>>();
        
        

        public GeophyData()
        {
            InitializeComponent();                
            
        }

        #region File
        // Import data event: First open file dialog, import file chosen and binding to binding source
        // and binding dataGridView to the same binding source
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            
            filePath = Methods.OpenFile();
           
            if(!string.IsNullOrEmpty(filePath))
            {
                WholeDataFile = Methods.ImportCSVFile(filePath);
                defaultDgv.DataSource = null;
                defaultDgv.DataSource = WholeDataFile;
                lblCountPoints.Text = WholeDataFile.Count.ToString() + " data points";
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                filterByLineToolStripMenuItem.Enabled = true;
                sortToolStripMenuItem.Enabled = true;
                ascendingToolStripMenuItem.Enabled = true;
                descendingToolStripMenuItem.Enabled = true;
                addDataToolStripMenuItem.Enabled = true;
            }
           
            
        }

        // Add Data Event
        private void addDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filePath = Methods.OpenFile();
            if(!string.IsNullOrEmpty(filePath))
            {
                int countDataPointsBefore = WholeDataFile.Count;
                List<DataImport> dataToAdd = Methods.ImportCSVFile(filePath);
                if(dataToAdd.Count != 0)
                {
                    WholeDataFile = Methods.AddDataToList(WholeDataFile, dataToAdd);
                    WholeDataFile.Sort(DataImport.SortNameAscending());
                    defaultDgv.DataSource = null;
                    defaultDgv.DataSource = WholeDataFile;
                    lblCountPoints.Text = WholeDataFile.Count.ToString() + " data points";
                    string message = string.Format("{0} data points have been added", dataToAdd.Count);
                    if (WholeDataFile.Count != countDataPointsBefore + dataToAdd.Count)
                    {
                        int countDataPointsAdded = WholeDataFile.Count - countDataPointsBefore;
                        int countConflicts = (countDataPointsBefore + dataToAdd.Count) - WholeDataFile.Count;
                        message = string.Format("{0} data points have been added. {1} data points in conflict have been removed.", countDataPointsAdded.ToString(), countConflicts.ToString());
                    }

                    MessageBox.Show(message, "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
      



        }

        // Save File Event
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WholeDataFile.Sort(DataImport.SortNameAscending());
            Methods.ExportToCsv(WholeDataFile, filePath);

        }

        // Save As method
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePathName = null;
            using (SaveFileDialog saveFile = new SaveFileDialog())
            {
                saveFile.Title = "Save File As";
                saveFile.Filter = "CSV (Comma delimited) (*.csv) | *.csv; *,csv | Excel Workbook (*.xlsx) | *.xlsx; *,xlsx";
                saveFile.RestoreDirectory = true;

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    filePathName = saveFile.FileName;
                    WholeDataFile.Sort(DataImport.SortNameAscending());

                    if (saveFile.FilterIndex == 1)
                    {
                        Methods.ExportToCsv(WholeDataFile, filePathName);

                    }

                    else if (saveFile.FilterIndex == 2)
                    {
                        Methods.ExportToExcel(WholeDataFile/*((IEnumerable)defaultDgv.DataSource).Cast<DataImport>().ToList()*/, filePathName);
                    }

                }

            }


        }

        // Exit Application Event
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Sort
        // Ascending Order Sorting
        private void ascendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (defaultDgv.Visible)
            {
                defaultDgv.DataSource = null;
                WholeDataFile.Sort(DataImport.SortNameAscending());
                defaultDgv.DataSource = WholeDataFile;
                
            }
            else
            {                
                //List<DataImport> tempList = //((IEnumerable)editingDgv.DataSource).Cast<DataImport>().ToList();
                editingDgv.DataSource = null;
                Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)].Sort(DataImport.SortNameAscending());
              
                editingDgv.DataSource = Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)];
            }

        
            
        }

        // Descending Order Sorting
        private void descendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (defaultDgv.Visible)
            {
                defaultDgv.DataSource = null;
                WholeDataFile.Sort(DataImport.SortNameDescending());
                defaultDgv.DataSource = WholeDataFile;
                
            }
            else
            {
                //List<DataImport> tempList = ((IEnumerable)editingDgv.DataSource).Cast<DataImport>().ToList(); 
                editingDgv.DataSource = null;
                Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)].Sort(DataImport.SortNameDescending());
                editingDgv.DataSource = Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)];
              
            }

        
            
        }

        #endregion

        #region View
        // View Whole file event
        private void viewAllFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

            IsEditMode(false);
            WholeDataFile = Methods.FlattenLists(Methods.LinesCollection);
            lblCountPoints.Visible = true;
            lblCountPoints.Text = WholeDataFile.Count.ToString();
            
            defaultDgv.DataSource = null;
            defaultDgv.DataSource = WholeDataFile;
            
          
        }

        #endregion

        #region Filter

       
        private void filterByLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            IsEditMode(true);
            Methods.LinesCollection = Methods.GetCollectionOfLines(WholeDataFile);
            lblCountPoints.Visible = false;
            //supportList = Methods.GetCollectionOfLines(WholeDataFile);
          
            bindNavig.BindingSource.DataSource = Methods.LinesCollection;
   
            DisplayFilteredList(Methods.LinesCollection);


        }

        // Method to display single line in dataGridView
        // Summary:
        //      Take a List of all file(updated) and then get from it a list of sublists
        //      where each sublist is a single filtered line, and binds a single list to 
        //      dataGridView based on the current position of binding navigator
        // Parameters: 
        //      List of Data Import class
        // Return: 
        //      None

        private void DisplayFilteredList(Dictionary<int, List<DataImport>> collectList)
        {
             editingDgv.DataSource = collectList[int.Parse(bindNavig.PositionItem.Text)];
            
        }
       
        #endregion

        #region Binding Navigator
        // Move Next Item Event
        private void bindingNavigatorMoveNextItem_Click(object sender, EventArgs e)
        {
           
            DisplayFilteredList(Methods.LinesCollection);
            if(int.Parse(bindNavig.PositionItem.Text) == Methods.LinesCollection.Keys.Count)
            {
                bindNavig.MoveNextItem.Enabled = false;
            }
            insertDataAutomaticallyToolStripMenuItem.Enabled = false;
            
            
        }

       
        // Move to Previous Item Event
        private void bindingNavigatorMovePreviousItem_Click(object sender, EventArgs e)
        {
            DisplayFilteredList(Methods.LinesCollection);
            if (int.Parse(bindNavig.PositionItem.Text) == Methods.LinesCollection.Keys.First())
            {
                bindNavig.MovePreviousItem.Enabled = false;
            }
            insertDataAutomaticallyToolStripMenuItem.Enabled = false;
        }

        // Move to Last Item Event (Couldn't use Last() method as in first item event because it 
        //                          didn't work correctly)
        private void bindingNavigatorMoveLastItem_Click(object sender, EventArgs e)
        {
            DisplayFilteredList(Methods.LinesCollection);
            
            insertDataAutomaticallyToolStripMenuItem.Enabled = false;
        }

        // Move to First Item Event
        private void bindingNavigatorMoveFirstItem_Click(object sender, EventArgs e)
        {
            DisplayFilteredList(Methods.LinesCollection);//editingDgv.DataSource = Methods.LinesCollection[1];
          
            insertDataAutomaticallyToolStripMenuItem.Enabled = false;
        }

        // Key Press Event: to move position only typing the position number and press enter
        private void bindingNavigatorPositionItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                DisplayFilteredList(Methods.LinesCollection);
            }
        }

        private void addRowButton_Click(object sender, EventArgs e)
        {
            int keyLine = int.Parse(bindNavig.PositionItem.Text);
            List<DataImport> tempList = Methods.LinesCollection[keyLine];

            DataImport tempData = new DataImport(tempList[0].EditNameProperty()[0] + 
                "." + (int.Parse(tempList[tempList.IndexOf(tempList.Last())].EditNameProperty()[1]) 
                + 1).ToString(), 0, 0, 0, null, null, null);
            
            tempList.Add(tempData);

            editingDgv.DataSource = null; 
            editingDgv.DataSource = tempList;
            Methods.LinesCollection[keyLine] = tempList;


        }

        private void editingDgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            List<DataImport> tempList = Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)];//((IEnumerable)editingDgv.DataSource).Cast<DataImport>().ToList();
            string strEdit = editingDgv.CurrentCell.Value.ToString();
            int counter = 0;
            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i].Name == strEdit)
                {
                    counter++;
                    if (counter >= 2)
                    {
                        DialogResult result = MessageBox.Show("Data point name already existed! Would you like to add it anyway?",
                            "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.No)
                        {
                            editingDgv.CurrentCell.Value = tempList[0].EditNameProperty()[0] + ".";
                        }
                    }

                }

            }
           
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            redoButton.Enabled = true;
            Redo.Push(Undo.Peek());
            Undo.Pop();


            Dictionary<int, List<DataImport>> latestLine = Undo.Peek();
            if(latestLine.Count == 1)
            {
                List<DataImport> retrieveList = latestLine.Values.First();

                
               bindNavig.PositionItem.Text = latestLine.Keys.First().ToString();
            
                Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)] = retrieveList;

                
                DisplayFilteredList(Methods.LinesCollection);
            }

            if(Undo.Count == 1)
            {
                undoButton.Enabled = false;
            }
            
        }

        private void insertButton_Click(object sender, EventArgs e)
        {

           
            int index = editingDgv.CurrentCell.RowIndex;

            List<DataImport> modifyList = Methods.InsertRowWithDefaultData(Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)], index);
           
            editingDgv.DataSource = null;
            editingDgv.DataSource = modifyList;
            Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)] = modifyList;
           
           

        }

        private void deleteRowButton_Click(object sender, EventArgs e)
        {
            
            List<DataImport> tempList = Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)];//((IEnumerable)editingDgv.DataSource).Cast<DataImport>().ToList();

            int index = editingDgv.CurrentCell.RowIndex;
            DataImport tempData = tempList[index];

            if (tempList.Count <= 1)
            {
                DialogResult answer = MessageBox.Show("This is the last data point of the line. Do you want to delete the entire line?",
                   "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (answer == DialogResult.Yes)
                {

                    tempList.RemoveAt(index);
                    editingDgv.DataSource = null;
                    editingDgv.DataSource = tempList;
                    Methods.LinesCollection.Remove(int.Parse(bindNavig.PositionItem.Text));
                    bindNavig.BindingSource.DataSource = null;
                    bindNavig.BindingSource.DataSource = Methods.LinesCollection;

                }
                
            }
            else
            {
                
                tempList.RemoveAt(index);
                editingDgv.DataSource = null;
                editingDgv.DataSource = tempList;
                Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)] = tempList;
            }
            



    

        }

        private void editindDgv_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
            switch (e.Button)
            {
                case MouseButtons.Left:
                    break;
                case MouseButtons.Right:
                    var relativePosition = editingDgv.PointToClient(Cursor.Position);
                    editingDgv.CurrentCell = clickedCell;
                    editingDgv.CurrentRow.Selected = true;
                    rowSideBarMenuStrip.Show(relativePosition);
                    break;

            }

        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertButton_Click(sender, e);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteRowButton_Click(sender, e);
        }

        private void editingDgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                clickedCell = editingDgv.Rows[e.RowIndex].Cells[0];
            }


        }
        #endregion

        #region Edit
        // Check Missing data inside a singlew filtered line
        private void checkDataSingleLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(bindNavig.Enabled)
            {
               Methods.CheckMissingDataInSingleLine(editingDgv, int.Parse(bindNavig.PositionItem.Text));
                
            }
            insertDataAutomaticallyToolStripMenuItem.Enabled = true;
        }


        // Insert Data automatically in a single filtered line
        private void insertDataAutomaticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = CustomMessageForm.CustomMessageBox.Show("Insert Data", "Are you sure you want to insert data automatically?", "Yes", "No");
            
            if (result == DialogResult.Yes)
            {
                editingDgv.DataSource = Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)];
                
                Methods.InsertDataAutomatically(Methods.LinesCollection, int.Parse(bindNavig.PositionItem.Text));
                editingDgv.DataSource = null;
                editingDgv.DataSource = Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)];
            }
                        
            
            
        }

        private void calculate2D3DLengthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            defaultDgv.Columns.Add("2D Length", "2D Length");
            defaultDgv.Columns.Add("3D Length", "3D Length");
            editingDgv.Columns.Add("2D Length", "2D Length");
            editingDgv.Columns.Add("3D Length", "3D Length");
            editingDgv.Columns["2D Length"].ReadOnly = true;
            editingDgv.Columns["3D Length"].ReadOnly = true;
            
        }




        #endregion

        private void editingDgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("An Error happened: input data is not in the correct format", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            
        }



        private void editingDgv_DataSourceChanged(object sender, EventArgs e)
        {
            if (editingDgv.DataSource != null)
            {
                
                List<DataImport> currentDisplayList = (((IEnumerable)editingDgv.DataSource).Cast<DataImport>().ToList()); //Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)];
                Dictionary<int, List<DataImport>> currentDict = new Dictionary<int, List<DataImport>>
                {
                    {int.Parse(bindNavig.PositionItem.Text), currentDisplayList }
                };

                if(Undo.Count > 0)
                {
                    Dictionary<int, List<DataImport>> lastDictOnStack = Undo.Peek();

                    List<DataImport> storedList = lastDictOnStack.Values.First();
                    
                    if (!Methods.AreListsContainSameElements(storedList, currentDisplayList))                        
                    {
                        Undo.Push(currentDict);
                    }
                    else if (Methods.IsTheListInDescendingOrder(currentDisplayList) && !Methods.IsTheListInDescendingOrder(storedList)
                        || Methods.IsTheListInDescendingOrder(storedList) && !Methods.IsTheListInDescendingOrder(currentDisplayList))
                    {
                        Undo.Push(currentDict);
                    }
                }
                else
                {
                    Undo.Push(currentDict);
                }
               
               if(Undo.Count > 1)
               {
                    undoButton.Enabled = true;
               }
            }


        }

        private void GPSData_Load(object sender, EventArgs e)
        {
            editingDgv.Visible = false;
        }

        private void IsEditMode(bool isEdit)
        {
            if (isEdit)
            {
                bindNavig.Visible = true;
                bindNavig.Enabled = true;
                editingDgv.Visible = true;
                defaultDgv.Visible = false;
                checkDataSingleLineToolStripMenuItem.Enabled = true;
                rowSideBarMenuStrip.Enabled = true;
                insertToolStripMenuItem.Enabled = true;
                deleteToolStripMenuItem.Enabled = true;
                calculate2D3DLengthToolStripMenuItem.Enabled = true;
                viewAllFileToolStripMenuItem.Enabled = true;
                addDataToolStripMenuItem.Enabled = false;
                importToolStripMenuItem.Enabled = false;
            }
            else
            {
                bindNavig.Enabled = false;
                editingDgv.Visible = false;
                defaultDgv.Visible = true;
                checkDataSingleLineToolStripMenuItem.Enabled = false;
                insertDataAutomaticallyToolStripMenuItem.Enabled = false;
                insertToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = false;
                rowSideBarMenuStrip.Enabled = false;
                calculate2D3DLengthToolStripMenuItem.Enabled = false;
                addDataToolStripMenuItem.Enabled = true;
                importToolStripMenuItem.Enabled = true;
            }
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            Dictionary<int, List<DataImport>> retrieveDict = Redo.Peek();
            if(retrieveDict.Count == 1)
            {
                List<DataImport> retrieveList = retrieveDict.Values.First();

                bindNavig.PositionItem.Text = retrieveDict.Keys.First().ToString();

                Methods.LinesCollection[int.Parse(bindNavig.PositionItem.Text)] = retrieveList;


                DisplayFilteredList(Methods.LinesCollection);

            }
         
         
            Redo.Pop();
            if(Redo.Count == 0)
            {
                redoButton.Enabled = false;
            }
        
       
        }

        private void bindingNavigatorPositionItem_TextChanged(object sender, EventArgs e)
        {
            bindNavig.PositionItem.Text = bindNavig.PositionItem.Text;
        }
    }
}
