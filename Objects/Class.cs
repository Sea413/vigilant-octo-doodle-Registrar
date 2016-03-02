using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Registrar
{
  public class Class
  {
    private int _id;
    private string _name;
    private string _courseId;


    public Class(string Name, string CourseId, int Id = 0)
    {
      _id = Id;
      _name = Name;
      _courseId = CourseId;
    }

    public override bool Equals(System.Object otherCourses)
    {
        if (!(otherCourses is Class))
        {
          return false;
        }
        else
        {
          Class newClass = (Class) otherCourses;
          bool idEquality = this.GetId() == newClass.GetId();
          bool nameEquality = this.GetName() == newClass.GetName();
          bool courseidEquality = this.GetCourseId() == newClass.GetCourseId();
          return (idEquality && nameEquality && courseidEquality);
        }
    }
    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }
    public string GetCourseId()
    {
      return _courseId;
    }

    public void AddStudent(Student newStudent)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO students_classes (class_id, student_id) VALUES (@ClassId, @StudentId)", conn);
      SqlParameter classIdParameter = new SqlParameter();
      classIdParameter.ParameterName = "@ClassId";
      classIdParameter.Value = this.GetId();
      cmd.Parameters.Add(classIdParameter);

      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = newStudent.GetId();
      cmd.Parameters.Add(studentIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
    public static List<Class> GetAll()
    {
      List<Class> allClass = new List<Class>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM class;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int classId = rdr.GetInt32(0);
        string classname = rdr.GetString(1);
        string courseId= rdr.GetString(2);
        Class newClass = new Class(classname, courseId, classId);
        allClass.Add(newClass);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allClass;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO class (name, course_id) OUTPUT INSERTED.id VALUES (@ClassName, @ClassCourseId)", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@ClassName";
      nameParameter.Value = this.GetName();
      cmd.Parameters.Add(nameParameter);

      SqlParameter courseIdparameter = new SqlParameter();
      courseIdparameter.ParameterName = "@ClassCourseId";
      courseIdparameter.Value = this.GetCourseId();
      cmd.Parameters.Add(courseIdparameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM class;", conn);
      cmd.ExecuteNonQuery();
    }

    public static Class Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM class WHERE id = @ClassId;", conn);
      SqlParameter ClassIdParameter = new SqlParameter();
      ClassIdParameter.ParameterName = "@ClassId";
      ClassIdParameter.Value = id.ToString();
      cmd.Parameters.Add(ClassIdParameter);
      rdr = cmd.ExecuteReader();

      int foundClassId = 0;
      string foundCourseId = null;
      string foundClassName = null;

      while(rdr.Read())
      {
        foundClassId = rdr.GetInt32(0);
        foundClassName = rdr.GetString(1);
        foundCourseId = rdr.GetString(2);
      }
      Class foundClass = new Class(foundClassName, foundCourseId, foundClassId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundClass;
    }
    public List<Student> GetStudents()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT student_id FROM students_classes WHERE class_id = @ClassId;", conn);
      SqlParameter ClassIdParameter = new SqlParameter();
      ClassIdParameter.ParameterName = "@ClassId";
      ClassIdParameter.Value = this.GetId();
      cmd.Parameters.Add(ClassIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> studentIds = new List<int> {};
      while(rdr.Read())
      {
        int studentId = rdr.GetInt32(0);
        studentIds.Add(studentId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<Student> students = new List<Student> {};
      foreach (int studentId in studentIds)
      {
        SqlDataReader queryReader = null;
        SqlCommand studentQuery = new SqlCommand("SELECT * FROM students WHERE id = @StudentId;", conn);

        SqlParameter studentIdParameter = new SqlParameter();
        studentIdParameter.ParameterName = "@StudentId";
        studentIdParameter.Value = studentId;
        studentQuery.Parameters.Add(studentIdParameter);

        queryReader = studentQuery.ExecuteReader();
        while(queryReader.Read())
        {
              int thisStudentId = queryReader.GetInt32(0);
              string studentName = queryReader.GetString(1);
              DateTime enrollment = queryReader.GetDateTime(2);
              Student foundStudent = new Student(studentName, enrollment, thisStudentId);
              students.Add(foundStudent);
        }
        if (queryReader != null)
        {
          queryReader.Close();
        }
      }
      if (conn != null)
      {
        conn.Close();
      }
      return students;
    }
    public void Update(string newName)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE class SET name = @NewName OUTPUT INSERTED.name WHERE id = @ClassId;", conn);

      SqlParameter newNameParameter = new SqlParameter();
      newNameParameter.ParameterName = "@NewName";
      newNameParameter.Value = newName;
      cmd.Parameters.Add(newNameParameter);


      SqlParameter ClassIdParameter = new SqlParameter();
      ClassIdParameter.ParameterName = "@ClassId";
      ClassIdParameter.Value = this.GetId();
      cmd.Parameters.Add(ClassIdParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._name = rdr.GetString(0);
      }

      if (rdr != null)
      {
        rdr.Close();
      }

      if (conn != null)
      {
        conn.Close();
      }
    }
    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM class WHERE id = @ClassId; DELETE FROM students_classes WHERE Class_id = @ClassId;", conn);
      SqlParameter ClassIdParameter = new SqlParameter();
      ClassIdParameter.ParameterName = "@ClassId";
      ClassIdParameter.Value = this.GetId();

      cmd.Parameters.Add(ClassIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
