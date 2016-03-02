using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Registrar
{
  public class Student
  {
    private int _id;
    private string _name;
    private DateTime _enrollment;

    public Student(string Name, DateTime Enrollment, int Id = 0)
    {
      _id = Id;
      _name = Name;
      _enrollment = Enrollment;
    }

    public override bool Equals(System.Object otherStudent)
  {
    if (!(otherStudent is Student))
    {
      return false;
    }
    else {
      Student newStudent = (Student) otherStudent;
      bool idEquality = this.GetId() == newStudent.GetId();
      bool nameEquality = this.GetName() == newStudent.GetName();
      bool enrollmentEquality = this.GetEnrollment() == newStudent.GetEnrollment();
      return (idEquality && nameEquality && enrollmentEquality);
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
  public DateTime GetEnrollment()
  {
    return _enrollment;
  }

  public static List<Student> GetAll()
{
  List<Student> AllStudents = new List<Student>{};

  SqlConnection conn = DB.Connection();
  SqlDataReader rdr = null;
  conn.Open();

  SqlCommand cmd = new SqlCommand("SELECT * FROM students", conn);
  rdr = cmd.ExecuteReader();

  while(rdr.Read())
  {
    int studentId = rdr.GetInt32(0);
    string studentName = rdr.GetString(1);
    DateTime  studentEnrollment =rdr.GetDateTime(2);
    Student newStudent = new Student(studentName, studentEnrollment, studentId);
    AllStudents.Add(newStudent);
  }
  if (rdr != null)
  {
    rdr.Close();
  }
  if (conn != null)
  {
    conn.Close();
  }
  return AllStudents;
}


public void Save()
{
  SqlConnection conn = DB.Connection();
  SqlDataReader rdr;
  conn.Open();

  SqlCommand cmd = new SqlCommand("INSERT INTO students (name, enrollment) OUTPUT INSERTED.id VALUES (@StudentName, @StudentEnrollment)", conn);

  SqlParameter nameParam = new SqlParameter();
  nameParam.ParameterName = "@StudentName";
  nameParam.Value = this.GetName();

  SqlParameter enrollmentParam = new SqlParameter();
  enrollmentParam.ParameterName = "@StudentEnrollment";
  enrollmentParam.Value = this.GetEnrollment();

  cmd.Parameters.Add(nameParam);
  cmd.Parameters.Add(enrollmentParam);


  rdr = cmd.ExecuteReader();

  while(rdr.Read())
  {
    this._id = rdr.GetInt32(0);
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

public List<Class> GetClasses()
{
  SqlConnection conn = DB.Connection();
  SqlDataReader rdr = null;
  conn.Open();

  SqlCommand cmd = new SqlCommand("SELECT class_id FROM students_classes WHERE student_id = @StudentId;", conn);

  SqlParameter studentIdParameter = new SqlParameter();
  studentIdParameter.ParameterName = "@StudentId";
  studentIdParameter.Value = this.GetId();
  cmd.Parameters.Add(studentIdParameter);

  rdr = cmd.ExecuteReader();

  List<int> classIds = new List<int> {};

  while (rdr.Read())
  {
    int ClassId = rdr.GetInt32(0);
    classIds.Add(ClassId);
  }
  if (rdr != null)
  {
    rdr.Close();
  }

  List<Class> classes = new List<Class> {};

  foreach (int class_id in classIds)
  {
    SqlDataReader queryReader = null;
    SqlCommand classQuery = new SqlCommand("SELECT * FROM class WHERE Id = @ClassId;", conn);

    SqlParameter classIdParameter = new SqlParameter();
    classIdParameter.ParameterName = "@ClassId";
    classIdParameter.Value = class_id;
    classQuery.Parameters.Add(classIdParameter);

    queryReader = classQuery.ExecuteReader();
    while (queryReader.Read())
    {
      int thisclassId = queryReader.GetInt32(0);
      string className = queryReader.GetString(1);
      string courseId = queryReader.GetString(2);
      Class foundClass = new Class(className, courseId, thisclassId);
      classes.Add(foundClass);
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
  return classes;
}

public static void DeleteAll()
  {
    SqlConnection conn = DB.Connection();
    conn.Open();
    SqlCommand cmd = new SqlCommand("DELETE FROM students;", conn);
    cmd.ExecuteNonQuery();
  }

  public static Student Find(int id)
  {
    SqlConnection conn = DB.Connection();
    SqlDataReader rdr = null;
    conn.Open();

    SqlCommand cmd = new SqlCommand("SELECT * FROM students WHERE Id = @StudentId", conn);
    SqlParameter studentIdParameter = new SqlParameter();
    studentIdParameter.ParameterName = "@StudentId";
    studentIdParameter.Value = id.ToString();
    cmd.Parameters.Add(studentIdParameter);
    rdr = cmd.ExecuteReader();

    int foundStudentId = 0;
    string foundStudentName = null;
    DateTime foundDate = new DateTime (2016-02-23);

    while(rdr.Read())
    {
      foundStudentId = rdr.GetInt32(0);
      foundStudentName = rdr.GetString(1);
      foundDate = rdr.GetDateTime(2);
    }
    Student foundStudent = new Student(foundStudentName, foundDate, foundStudentId);

    if (rdr != null)
    {
      rdr.Close();
    }
    if (conn != null)
    {
      conn.Close();
    }
    return foundStudent;
  }
  public void AddClass(Class newClass)
  {
    SqlConnection conn = DB.Connection();
    conn.Open();

    SqlCommand cmd = new SqlCommand("INSERT INTO students_classes (student_id, class_id) VALUES (@StudentId, @ClassId);", conn);

    SqlParameter classIdParameter = new SqlParameter();
    classIdParameter.ParameterName = "@ClassId";
    classIdParameter.Value = newClass.GetId();
    cmd.Parameters.Add(classIdParameter);

    SqlParameter studentIdParameter = new SqlParameter();
    studentIdParameter.ParameterName = "@StudentId";
    studentIdParameter.Value = this.GetId();
    cmd.Parameters.Add(studentIdParameter);

    cmd.ExecuteNonQuery();

    if (conn != null)
    {
      conn.Close();
    }
  }

  // public void UpdateChecked(int newChecked)
  // {
  //   SqlConnection conn = DB.Connection();
  //   SqlDataReader rdr;
  //   conn.Open();
  //
  //   SqlCommand cmd = new SqlCommand("UPDATE tasks SET complete = @newchecked OUTPUT INSERTED.complete WHERE id = @TID;", conn);
  //
  //   SqlParameter newCheckedParam = new SqlParameter();
  //   newCheckedParam.ParameterName = "@newChecked";
  //   newCheckedParam.Value = newChecked;
  //   cmd.Parameters.Add(newCheckedParam);
  //
  //
  //   SqlParameter taskIDParam = new SqlParameter();
  //   taskIDParam.ParameterName = "@TID";
  //   taskIDParam.Value = this.GetId();
  //   cmd.Parameters.Add(taskIDParam);
  //   rdr = cmd.ExecuteReader();
  //
  //   while(rdr.Read())
  //   {
  //     this._completed = rdr.GetInt32(0);
  //   }
  //
  //   if (rdr != null)
  //   {
  //     rdr.Close();
  //   }
  //
  //   if (conn != null)
  //   {
  //     conn.Close();
  //   }
  // }

  public void Delete()
  {
    SqlConnection conn = DB.Connection();
    conn.Open();

    SqlCommand cmd = new SqlCommand("DELETE FROM students WHERE id = @StudentId; DELETE FROM categories_tasks WHERE task_id = @TaskId;", conn);
    SqlParameter studentIdParameter = new SqlParameter();
    studentIdParameter.ParameterName = "@StudentId";
    studentIdParameter.Value = this.GetId();

    cmd.Parameters.Add(studentIdParameter);
    cmd.ExecuteNonQuery();

    if (conn != null)
    {
      conn.Close();
    }
  }
}
}
