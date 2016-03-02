using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Registrar
{
  public class ClassTest : IDisposable
  {
    public ClassTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=registrar_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_ClassEmptyAtFirst()
    {
      int result = Class.GetAll().Count;
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_ClassReturnTrueForSameName()
    {
      Class firstClass = new Class("Philosophy", "PHIL101");
      Class secondClass = new Class("Philosophy", "PHIL101");
      Assert.Equal(firstClass, secondClass);
    }


        [Fact]
    public void Test_Save_SavesClassToDatabase()
    {
      //Arrange
      Class testClass = new Class("Fiction", "CRWT001");
      testClass.Save();

      //Act
      List<Class> result = Class.GetAll();
      List<Class> testList = new List<Class>{testClass};

      //Assert
      Assert.Equal(testList, result);
    }


    [Fact]
    public void Test_Save_AssignsIdToClassObject()
    {
      //Arrange
      Class testClass = new Class("Fiction", "CRWT001");
      testClass.Save();

      //Act
      Class savedClass = Class.GetAll()[0];
      int result = savedClass.GetId();
      int testId = testClass.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_Find_FindsClassInDatabase()
    {
      //Arrange
      Class testClass = new Class("Fiction", "CRWT001");
      testClass.Save();

      //Act
      Class foundClass = Class.Find(testClass.GetId());

      //Assert
      Assert.Equal(testClass, foundClass);
    }

        [Fact]
        public void Test_AddStudent_AddsStudentToClass()
        {
          //Arrange
          Class testClass = new Class("Fiction", "CRWT001");
          testClass.Save();

          Student testStudent = new Student("Veronica Alley", new DateTime (2009,10,01));
          testStudent.Save();

          Student testStudent2 = new Student("Sean Peerenboom", new DateTime (2009,10,01));
          testStudent2.Save();

          //Act
          testClass.AddStudent(testStudent);
          testClass.AddStudent(testStudent2);

          List<Student> result = testClass.GetStudents();
          List<Student> testList = new List<Student>{testStudent, testStudent2};

          //Assert
          Assert.Equal(testList, result);
        }

        [Fact]
        public void Test_GetStudents_ReturnsAllClassStudents()
        {
          //Arrange
          Class testClass = new Class("Fiction", "CRWT001");
          testClass.Save();

          Student testStudent1 = new Student("Veronica Alley", new DateTime (2009,10,01));
          testStudent1.Save();

          // Student testStudent2 = new Student("Sean Peerenboom", new DateTime (2009,10,01));
          // testStudent2.Save();

          //Act
          testClass.AddStudent(testStudent1);
          List<Student> savedStudents = testClass.GetStudents();
          List<Student> testList = new List<Student> {testStudent1};

          //Assert
          Assert.Equal(testList, savedStudents);
        }
        public void Test_GetClasses_ReturnsAllStudentClasses()
        {
      //Arrange
      Student testStudent = new Student("Veronica Alley", new DateTime (2009,10,01));
      testStudent.Save();

      Class testClass1 = new Class("Fiction", "CRWT001");
      testClass1.Save();

      Class testClass2 = new Class("Philosophy", "PHIL002");
      testClass2.Save();

      //Act
      testStudent.AddClass(testClass1);
      List<Class> result = testStudent.GetClasses();
      List<Class> testList = new List<Class> {testClass1};

      //Assert
      Assert.Equal(testList, result);
    }
    [Fact]
        public void Test_Delete_DeletesClassAssociationsFromDatabase()
        {
          //Arrange
          Student testStudent = new Student("Ted Mosley", new DateTime (2009,10,01));
          testStudent.Save();

          string testName = "Ted Mosley";
          Class testClass = new Class(testName,"Gred1001");
          testClass.Save();

          //Act
          testClass.AddStudent(testStudent);
          testClass.Delete();

          List<Class> resultStudentClasses = testStudent.GetClasses();
          List<Class> testStudentClasses = new List<Class> {};

          //Assert
          Assert.Equal(testStudentClasses, resultStudentClasses);
        }
    [Fact]
    public void Test_Delete_DeletesClassFromDatabase()
    {
      //Arrange
      string name1 = "Buddhism";
      Class testClass1 = new Class(name1, "Gred1001");
      testClass1.Save();

      string name2 = "Basket Weaving";
      Class testClass2 = new Class(name2,"Gred1001");
      testClass2.Save();

      //Act
      testClass1.Delete();
      List<Class> resultClasses = Class.GetAll();
      List<Class> testClassList = new List<Class> {testClass2};

      //Assert
      Assert.Equal(testClassList, resultClasses);
    }

      [Fact]
        public void Dispose()
        {
          Student.DeleteAll();
          Class.DeleteAll();
        }
      }
    }
