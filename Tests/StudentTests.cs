using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Registrar
{
  public class StudentTest : IDisposable
  {
    public StudentTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=registrar_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_EmptyAtFirst()
    {
      //Arrange, Act
      int result = Student.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameDescription()
    {
      //Arrange, Act
      Student firstStudent = new Student("David Turley",new DateTime (2009,10,01));
      Student secondStudent = new Student("David Turley", new DateTime (2009,10,01));

      //Assert
      Assert.Equal(firstStudent, secondStudent);
    }

    [Fact]
    public void Test_Save()
    {
      //Arrange
      Student testStudent = new Student("David Turley",new DateTime (2009,10,01));
      testStudent.Save();

      //Act
      List<Student> result = Student.GetAll();
      List<Student> testList = new List<Student>{testStudent};

      //Assert
      Assert.Equal(testList, result);
    }



    [Fact]
    public void Test_SaveAssignsIdToObject()
    {
      //Arrange
      Student testStudent = new Student("Hai Lam",new DateTime (2009,10,01));
      testStudent.Save();

      //Act
      Student savedStudent = Student.GetAll()[0];

      int result = savedStudent.GetId();
      int testId = testStudent.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_FindFindsStudentInDatabase()
    {
      //Arrange
      Student testStudent = new Student("Zach Black",new DateTime (2009,10,01));
      testStudent.Save();

      //Act
      Student result = Student.Find(testStudent.GetId());

      //Assert
      Assert.Equal(testStudent, result);
    }
    [Fact]
public void Test_AddClass_AddsClassToStudent()
{
  //Arrange
  Student testStudent = new Student("Adin Moon",new DateTime (2009,10,01));
  testStudent.Save();

  Class testClass = new Class("Seahorse Riding", "SEA2008");
  testClass.Save();

  //Act
  testStudent.AddClass(testClass);

  foreach (var course in Class.GetAll()) {
        Console.WriteLine(course.GetName());
  }

  List<Class> result = testStudent.GetClasses();
  List<Class> testList = new List<Class>{testClass};

  //Assert
  Assert.Equal(testList, result);
}

  [Fact]
  public void Test_GetCategories_ReturnsAllStudentCategories()
  {
    //Arrange
    Student testStudent = new Student("Zach Quinto",new DateTime (2009,10,01));
    testStudent.Save();

    Class testClass1 = new Class("William James", "Phil008");
    testClass1.Save();

    Class testClass2 = new Class("Zach Quinto", "Phil1009");
    testClass2.Save();

    //Act
    testStudent.AddClass(testClass1);
    List<Class> result = testStudent.GetClasses();
    List<Class> testList = new List<Class> {testClass1};

    //Assert
    Assert.Equal(testList, result);
  }

  public void Test_Delete_DeletesStudentAssociationsFromDatabase()
  {
    //Arrange
    Class testClass = new Class("Barrel Slinging", "Gears2001");
    testClass.Save();

    string testDescription = "David Phil";
    DateTime testcompletion = new DateTime (2009,10,01);
    Student testStudent = new Student(testDescription, testcompletion);
    testStudent.Save();

    //Act
    testStudent.AddClass(testClass);
    testStudent.Delete();

    List<Student> resultClassStudents = testClass.GetStudents();
    List<Student> testClassStudents = new List<Student> {};

    //Assert
    Assert.Equal(testClassStudents, resultClassStudents);
  }


    public void Dispose()
    {
      Student.DeleteAll();
      Class.DeleteAll();
    }
  }
}
