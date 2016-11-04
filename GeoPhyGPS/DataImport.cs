
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Geophy
{
    [Serializable]
    public class DataImport : IComparable<DataImport>
    {
        private string name;
        public string Name
        {
            get
            {
                if (name != null)
                {
                    name.ToUpper();
                }

                return name;
            }
            set
            {
                if (value != null)
                {
                    name = value.ToUpper();
                }

            }
        }


        public double Easting { get; set; }
        public double Northing { get; set; }

        private double elevation;
        public double Elevation
        {
            get
            {
                if (elevation == default(double))
                {
                    elevation = -1;
                }

                return elevation;
            }
            set
            {
                if (value == default(double))
                {
                    elevation = -1;
                }
                else
                {
                    elevation = value;
                }
            }
        }
        public string Code { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }



        public DataImport(string name, double east,
            double north, double elev, string code,
            string descrip1, string descript2)
        {
            Name = name;
            Easting = east;
            Northing = north;
            Elevation = elev;
            Code = code;
            Description1 = descrip1;
            Description2 = descript2;
        }

        // Get a string array splitting a name property by underscore or dot
        // Summary:
        //      Take a string name property and split first by underscore and then by dot
        //      if it cointains both. Return a string array that cointains as first element the
        //      string before the dot and second element the string after dot. 
        // Parameters:
        //      string represented by name property string
        // Returns:
        //      array of strings
        public string[] EditNameProperty()
        {

            string[] newName = new string[3];
            if (this.Name.Contains("_"))
            {
                string tempName = this.Name.Substring(this.Name.LastIndexOf('_') + 1);
                newName = tempName.Split('.');

            }
            else if (!this.Name.Contains("_") && this.Name.Contains("."))
            {
                newName = this.Name.Split('.');
            }
            else
            {
                newName = new string[] { this.Name };
            }

            return newName;
        }
        int IComparable<DataImport>.CompareTo(DataImport dat)
        {

            return String.Compare(this.Name, dat.Name);
        }

        public static IComparer<DataImport> SortNameAscending()
        {
            return new SortNameAscendingHelper();
        }
        public static IComparer<DataImport> SortNameDescending()
        {
            return new SortNameDescendingHelper();
        }
        //public static IComparer CompareElements()
        //{
        //    return new DataImportComparer();
        //}


        private class SortNameAscendingHelper : IComparer<DataImport>
        {
            // Compare first for lenght of first index in the array, if the lenghts are different
            // they can't be equal and then compare only the letter to see which is bigger
            // If the lenghts are equal then compare any char in the first string. 
            // If they are equal compare the second string converting to int
            int IComparer<DataImport>.Compare(DataImport x, DataImport y)
            {


                string[] numbNameForX = x.EditNameProperty();
                string[] numbNameForY = y.EditNameProperty();

                if (numbNameForX.Length > 1 && numbNameForY.Length > 1)
                {


                    // First Case: the first strings in the arrays have same lenght
                    if (numbNameForX.First().Length == numbNameForY.First().Length)
                    {
                        for (int i = 0; i < numbNameForX.First().Length; i++)
                        {
                            // Compare each digit in the first strings of objects 
                            // and get if an object is greater or less than other one
                            if (numbNameForX.First()[i] > numbNameForY.First()[i])
                            {
                                return 1;
                            }
                            else if (numbNameForX.First()[i] < numbNameForY.First()[i])
                            {
                                return -1;
                            }

                        }
                        // if the first strings are equal in all char elements then it compares
                        // the second strings of the objects. As the second strings has to be numeric
                        // it converts string to int and then comparing.
                        int numbForX;
                        int numbForY;
                        bool checkFirstObj = int.TryParse(numbNameForX[1], out numbForX);
                        bool checkSecondObj = int.TryParse(numbNameForY[1], out numbForY);
                        if (numbForX > numbForY)
                        {
                            return 1;
                        }
                        else if (numbForX < numbForY)
                        {
                            return -1;
                        }


                    }

                    // Second Case: Lenght of first strings are different
                    // Creates a variable len to use in the for loop depending on
                    // which object has the longer first strings. So if the object x has 
                    // longer lenght then it uses the lenght of object y in the for loop to 
                    // avoid index out of range exception
                    else if (numbNameForX.First().Length != numbNameForY.First().Length)
                    {

                        int len = 0;
                        if (numbNameForX.First().Length > numbNameForY.First().Length)
                        {
                            len = numbNameForY.First().Length;
                        }
                        else
                        {
                            len = numbNameForX.First().Length;
                        }
                        for (int i = 0; i < len; i++)
                        {
                            // If the item i is a letter in both objects then it compares the object
                            // based on the order of letter
                            if (char.IsLetter(numbNameForX.First()[i]) &&
                                char.IsLetter(numbNameForY.First()[i]))
                            {
                                if (numbNameForX.First()[i] > numbNameForY.First()[i] && len == numbNameForY.First().Length
                                    || numbNameForY.First()[i] < numbNameForX.First()[i] && len == numbNameForX.First().Length)
                                {
                                    return 1;
                                }
                                else if (numbNameForX.First()[i] < numbNameForY.First()[i] && len == numbNameForY.First().Length
                                    || numbNameForY.First()[i] > numbNameForX.First()[i] && len == numbNameForX.First().Length)
                                {
                                    return -1;
                                }
                            }
                            // If the letters are equal or there are no more letters in the string
                            // then the longer strings has to be also the greater one and viceversa
                            else
                            {
                                if (len == numbNameForX.First().Length)
                                {
                                    return -1;
                                }
                                if (len == numbNameForY.First().Length)
                                {
                                    return 1;
                                }
                            }
                        }

                    }
                    return 0;
                }
                else
                {
                    return String.Compare(x.Name, y.Name);
                }



            }
        }

        private class SortNameDescendingHelper : IComparer<DataImport>
        {
            int IComparer<DataImport>.Compare(DataImport x, DataImport y)
            {

                string[] numbNameForX = x.EditNameProperty();
                string[] numbNameForY = y.EditNameProperty();
                if (numbNameForX.Length > 1 && numbNameForY.Length > 1)
                {

                    if (numbNameForX.First().Length == numbNameForY.First().Length)
                    {
                        for (int i = 0; i < numbNameForX.First().Length; i++)
                        {
                            if (numbNameForX.First()[i] < numbNameForY.First()[i])
                            {
                                return 1;
                            }
                            else if (numbNameForX.First()[i] > numbNameForY.First()[i])
                            {
                                return -1;
                            }

                        }

                        int numbForX;
                        int numbForY;
                        bool checkFirstObj = int.TryParse(numbNameForX[1], out numbForX);
                        bool checkSecondObj = int.TryParse(numbNameForY[1], out numbForY);
                        if (numbForX < numbForY)
                        {
                            return 1;
                        }
                        else if (numbForX > numbForY)
                        {
                            return -1;
                        }

                        
                    }
                    else if (numbNameForX.First().Length != numbNameForY.First().Length)
                    {
                        int len = 0;
                        if (numbNameForX.First().Length > numbNameForY.First().Length)
                        {
                            len = numbNameForY.First().Length;
                        }
                        else
                        {
                            len = numbNameForX.First().Length;
                        }
                        for (int i = 0; i < len; i++)
                        {
                            if (char.IsLetter(numbNameForX.First()[i]) &&
                                char.IsLetter(numbNameForY.First()[i]))
                            {
                                if (numbNameForX.First()[i] > numbNameForY.First()[i] && len == numbNameForY.First().Length
                                    || numbNameForY.First()[i] < numbNameForX.First()[i] && len == numbNameForX.First().Length)
                                {
                                    return -1;
                                }
                                else if (numbNameForX.First()[i] < numbNameForY.First()[i] && len == numbNameForY.First().Length
                                    || numbNameForY.First()[i] > numbNameForX.First()[i] && len == numbNameForX.First().Length)
                                {
                                    return 1;
                                }
                            }
                            else
                            {
                                if (len == numbNameForX.First().Length)
                                {
                                    return 1;
                                }
                                if (len == numbNameForY.First().Length)
                                {
                                    return -1;
                                }
                            }
                        }
                        
                    }
                    return 0;
                }
                else
                {
                    return String.Compare(y.Name, x.Name);
                }


            }



        }
    }
}

        //public class DataImportComparer : IComparer
        //{
        //    public int Compare (object a, object b)
        //    {
        //        return CompareElements((DataImport)a, (DataImport)b);
        //    }
        //    public static int CompareElements(DataImport x , DataImport y)
        //    {
        //        if (String.Compare(x.Name, y.Name) != 0)
        //        {
        //            return String.Compare(x.Name, y.Name);
        //        }
        //        else if (x.Northing.CompareTo(y.Northing) != 0 && x.Northing != default(double) && y.Northing != default(double))
        //        {
        //            return x.Northing.CompareTo(y.Northing);
        //        }
        //        else if (x.Easting.CompareTo(y.Easting) != 0 && x.Easting != default(double) && y.Easting != default(double))
        //        {
        //            return x.Easting.CompareTo(y.Easting);
        //        }
        //        else if (x.Elevation.CompareTo(y.Elevation) != 0 && x.Elevation != -1 && y.Elevation != -1)
        //        {
        //            return x.Elevation.CompareTo(y.Elevation);
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }

        //}

    

    


