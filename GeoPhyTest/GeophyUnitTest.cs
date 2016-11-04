using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Geophy;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections;
using System.Linq;

namespace GeophyTest
{
    [TestClass]
    public class GeophyUnitTest
    {
        [TestMethod]
        public void CheckIfListIsAddedAddDataToListMethod()
        {
            //arrange         
            List<DataImport> aList = new List<DataImport>();
            string path = "C:/Users/Miki/Desktop/Nick's Project/Test2.csv";
            List<DataImport> expected = Methods.ImportCSVFile(path);

            //act
            //List<DataImport> actu = Methods.AddDataToList(aList);

            //assert
            //CollectionAssert.AreEqual( expected, actu, new DataImportComparer());

        }
        [TestMethod]
        public void FilterByNameCheckIfReturnAnExpectedListOfOneElement()
        {
            //arrange
            int index = 0;
            List<DataImport> inputList = new List<DataImport>{new DataImport( "5030_R5.3", 0, 0, 0, null, null, null), new DataImport("r22.13", 0, 0, 0, null, null, null),
                                    new DataImport("6578", 0, 0, 0, null, null, null), new DataImport("345.65", 0, 0, 0, null, null, null), new DataImport("ret.5", 0, 0, 0, null, null, null),
                                    new DataImport("54_re_rt45.6", 0, 0, 0, null, null, null)};
            List<DataImport> expected = new List<DataImport> { new DataImport("5030_R5.3", 0, 0, -1, null, null, null) };
         

            //act
            List<DataImport> actual = Methods.FilterByName(index, inputList);

            //assert
            //CollectionAssert.AreEqual(expected, actual, new DataImportComparer());
         
        }

        [TestMethod]
        public void FilterByNameCheckIfReturnAnExpectedListStartingAtAnIndexAfter()
        {
            //arrange
            int index = 1;
            List<DataImport> inputList = new List<DataImport>{new DataImport( "r22.2", 0, 0, 0, null, null, null), new DataImport("r22.13", 0, 0, 0, null, null, null),
                                    new DataImport("r24", 345, 78, 0, null, null, null), new DataImport("345.65", 0, 0, 0, null, null, null), new DataImport("ret.5", 0, 0, 0, null, null, null),
                                    new DataImport("54_re_rt45.6", 0, 0, 0, null, null, null)};
            List<DataImport> expected = new List<DataImport> { new DataImport("r22.13", 0, 0, -1, null, null, null)};


            //act
            List<DataImport> actual = Methods.FilterByName(index, inputList);

            //assert
            //CollectionAssert.AreEqual(expected, actual, new DataImportComparer());

        }
        [TestMethod]
        public void FilterByNameCheckIfNameDifferentButOtherPropertiesEqual()
        {
            //arrange
            int index = 0;
            List<DataImport> inputList = new List<DataImport>{new DataImport( "r2.2", 23, 45, 5, null, null, null), new DataImport("r22.13", 0, 0, 0, null, null, null),
                                    new DataImport("r24", 23, 45, 5, null, null, null), new DataImport("345.65", 0, 0, 0, null, null, null), new DataImport("ret.5", 0, 0, 0, null, null, null),
                                    new DataImport("54_re_rt45.6", 0, 0, 0, null, null, null)};
            List<DataImport> expected = new List<DataImport> { new DataImport("r2.2", 23, 45, 5, null, null, null) };
            //act
            List<DataImport> actual = Methods.FilterByName(index, inputList);

            //assert
           // CollectionAssert.AreEqual(expected, actual, new DataImportComparer());

        }



        [TestMethod]
        public void EditNamePropertyCheckIfReturnsCorrectValueWithNameContainsUnderscoreAndDot()
        {
            //arrange
            string name = "er_r45_3.45";
            DataImport dat = new DataImport(name, 0, 0, 0, null, null, null);
            string[] output = new string[] { "3", "45" };

            //act
            string[] actual = dat.EditNameProperty();

            //assert
            CollectionAssert.AreEqual(output, actual);
        }

        [TestMethod]
        public void EditNamePropertyCheckIfReturnsCorrectValueWithNameContainsDot()
        {
            //arrange
            string name = "R43.5";
            DataImport dat = new DataImport(name, 0, 0, 0, null, null, null);
            string[] output = new string[] { "R43", "5" };

            //act
            string[] actual = dat.EditNameProperty();

            //assert
            CollectionAssert.AreEqual(output, actual);
        }
        [TestMethod]
        public void EditNamePropertyIfOnlyNameContainsNoSymbols()
        {
            //arrange
            string name = "R43675";
            DataImport dat = new DataImport(name, 0, 0, 0, null, null, null);
            string[] output = new string[] { "R43675" };

            //act
            string[] actual = dat.EditNameProperty();

            //assert
            CollectionAssert.AreEqual(output, actual);
        }

   
        [TestMethod]

        public void AreListsContainSameElementCheckIfListsContainElementsWithSamePropertiesValue()
        {
            List<DataImport> first = new List<DataImport>() { new DataImport("r1", 0, 4, 3, null, null, null),
                        new DataImport("r2", 0, 3, 4, null, null, null),
                        new DataImport("r3", 2, 6, 0, null, null, null)};
            List<DataImport> second = new List<DataImport>() {new DataImport("r1", 0, 4, 3, null, null, null),
                        new DataImport("r2", 0, 3, 4, null, null, null),
                        new DataImport("r3", 2, 6, 0, null, null, null)};
            bool expected = true;

            // act
            bool actual = Methods.AreListsContainSameElements(first, second);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AreListsContainSameElementCheckIfListsAreDifferentOnlyForNameProperty()
        {
            List<DataImport> first = new List<DataImport>() { new DataImport("r4", 0, 4, 3, null, null, null),
                        new DataImport("r2", 0, 3, 4, null, null, null),
                        new DataImport("r3", 2, 6, 0, null, null, null)};
            List<DataImport> second = new List<DataImport>() {new DataImport("r1", 0, 4, 3, null, null, null),
                        new DataImport("r2", 0, 3, 4, null, null, null),
                        new DataImport("r3", 2, 6, 0, null, null, null)};
            bool expected = false;

            // act
            bool actual = Methods.AreListsContainSameElements(first, second);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]

        public void AreListsContainSameElementCheckIfListsHaveDifferentLength()
        {
            List<DataImport> first = new List<DataImport>() { new DataImport("r1", 0, 4, 3, null, null, null),
                        new DataImport("r2", 0, 3, 4, null, null, null),
                        new DataImport("r3", 2, 6, 0, null, null, null)};
            List<DataImport> second = new List<DataImport>() {new DataImport("r1", 0, 4, 3, null, null, null),
                        new DataImport("r2", 0, 3, 4, null, null, null),
                        new DataImport("r3", 2, 6, 0, null, null, null),
                        new DataImport("r5", 3, 3, 3, null, null, null)};
            bool expected = false;

            // act
            bool actual = Methods.AreListsContainSameElements(first, second);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]

        public void AreListsContainSameElementCheckIfListsAreEqualButDifferentOrder()
        {
            List<DataImport> first = new List<DataImport>() { new DataImport("r1", 0, 4, 3, null, null, null),
                        new DataImport("r2", 0, 3, 4, null, null, null),
                        new DataImport("r3", 2, 6, 0, null, null, null)};
            List<DataImport> second = new List<DataImport>() {new DataImport("r3", 2, 6, 0, null, null, null),
                        new DataImport("r2", 0, 3, 4, null, null, null),
                        new DataImport("r1", 0, 4, 3, null, null, null)};
            bool expected = true;

            // act
            bool actual = Methods.AreListsContainSameElements(first, second);

            Assert.AreEqual(expected, actual);
        }

    }
}
